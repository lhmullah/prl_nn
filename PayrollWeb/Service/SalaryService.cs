using AutoMapper;
using com.linde.DataContext;
using com.linde.Model;
using MySql.Data.MySqlClient;
using PayrollWeb.Models;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PayrollWeb.Service
{
    public class SalaryService
    {
        private payroll_systemContext dataContext;
        private List<prl_salary_hold> thisMonthsHold;

        List<prl_employee_discontinue> thisMonthsDiscontinued;
        private List<prl_upload_deduction> thisMonthUploadDeduction;

        private List<prl_salary_process_detail> lstSalaryProcessDetails;
        private IProcessResult result;
        private IProcessResult worker;

        public SalaryService(payroll_systemContext dataContext)
        {
            this.dataContext = dataContext;
            lstSalaryProcessDetails = new List<prl_salary_process_detail>();
            result = new SalaryProcessResult(ProcessType.SALARY);
            worker = new SalaryProcessResult(ProcessType.SALARY);
        }

        public IProcessResult ProcessSalary(bool allEmployee, List<int> selectedEmployees, int deptId, DateTime salaryMonth, DateTime processDate, DateTime paymentDate)
        {
            var proStartDate = new DateTime(salaryMonth.Year, salaryMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(salaryMonth.Year, salaryMonth.Month);
            DateTime proEndDate = new DateTime(salaryMonth.Year, salaryMonth.Month, daysInMonth);
            int salaryProcessId = 0;

            ISalaryProcess salAlw;
            ISalaryProcess salDed;

            //1. create salary process object

            var sp = new prl_salary_process();
            sp.batch_no = BatchNumberGenerator.generateSalaryBatchNumber("SALARY", salaryMonth);
            sp.process_date = processDate.Date;
            sp.payment_date = paymentDate.Date;
            sp.salary_month = salaryMonth.Date;
            sp.is_disbursed = "N";

            var spBatchNo = dataContext.prl_salary_process.AsEnumerable().SingleOrDefault(x => x.batch_no == sp.batch_no);

            if (spBatchNo == null || selectedEmployees != null)
            {
                if (allEmployee && deptId == 0)
                {
                    if (IsSalaryAlreadyProcessed(paymentDate.Date, "all", null, 0, null))
                    {
                        return result;
                    }
                    var emps = GetEligibleEmployeeForSalaryProcess (salaryMonth.Date);
                    salAlw = new SalaryAllowanceProcess(salaryMonth.Date, proStartDate, proEndDate, emps);
                    salDed = new SalaryDeductionProcess(salaryMonth.Date, proStartDate, proEndDate, emps);

                    sp.department_id = 0;

                }
                else if (selectedEmployees != null && selectedEmployees.Count > 0)
                {
                    if (IsSalaryAlreadyProcessed(paymentDate.Date, "selected employee", null, null, selectedEmployees))
                    {
                        return result;
                    }
                    if (spBatchNo!=null)
                    {
                        salaryProcessId = dataContext.prl_salary_process.FirstOrDefault(x => x.batch_no == spBatchNo.batch_no).id; 
                    }
                    
                    var empDs = GetEligibleEmployeeForSalaryProcess(salaryMonth.Date);
                    empDs = empDs.Where(x => selectedEmployees.Contains(x.emp_id)).ToList();
                    salAlw = new SalaryAllowanceProcess(salaryMonth.Date, proStartDate, proEndDate, empDs);
                    salDed = new SalaryDeductionProcess(salaryMonth.Date, proStartDate, proEndDate, empDs);

                    sp.department_id = 0;

                }
                else if (deptId > 0)
                {
                    if (IsSalaryAlreadyProcessed(paymentDate.Date, "department", null, deptId, null))
                    {
                        return result;
                    }
                    var emps = GetEligibleEmployeeForSalaryProcess(salaryMonth.Date);
                    emps = emps.Where(x => x.department_id == deptId).ToList();

                    if (emps.Count <= 0)
                    {
                        result.ErrorOccured = true;
                        result.AddToErrorList("Salary already processed for this department.");
                        return result;
                    }
                    salAlw = new SalaryAllowanceProcess(salaryMonth.Date, proStartDate, proEndDate, emps);
                    salDed = new SalaryDeductionProcess(salaryMonth.Date, proStartDate, proEndDate, emps);

                    sp.department_id = deptId;

                }
                else
                {
                    return null;
                }

                var userIdentity = Thread.CurrentPrincipal.Identity;
                sp.created_date = DateTime.Now;
                sp.created_by = userIdentity.Name;

                //sp.updated_by = userIdentity.Name;

                //1. calculate allowance 
                var salAlwProAll = salAlw.Process();
                var lstAlwNotProcessedIds = salAlw.GetEmployeeList().Select(x => x.emp_id).ToList().Except((salAlwProAll.GetCompletedResultObjects() as List<prl_salary_allowances>).Select(x => x.emp_id).ToList());
                var lstAlwProcessedObjects = salAlwProAll.GetCompletedResultObjects() as List<prl_salary_allowances>;
                salAlwProAll.GetErrors.ForEach(x => {result.AddToErrorList(x); });

                //2. calculate deduction 
                var salDedProAll = salDed.Process();
                var lstDedNotProcessedIds = salDed.GetEmployeeList().Select(x => x.emp_id).ToList().Except((salDedProAll.GetCompletedResultObjects() as List<prl_salary_deductions>).Select(x => x.emp_id).ToList());
                var lstDedProcessedObjects = salDedProAll.GetCompletedResultObjects() as List<prl_salary_deductions>;
                salDedProAll.GetErrors.ForEach(x => { result.AddToErrorList(x); });

                //3. sync allowance and deduction lists
                if (salAlwProAll.ErrorOccured)
                {
                    if (lstDedProcessedObjects != null)
                        lstDedProcessedObjects.RemoveAll(x => lstAlwNotProcessedIds.Contains(x.emp_id));
                }
                if (salDedProAll.ErrorOccured)
                {
                    if (lstAlwProcessedObjects != null)
                        lstAlwProcessedObjects.RemoveAll(x => lstDedNotProcessedIds.Contains(x.emp_id));
                }

                //4. apply leave without pay 
                var employeeIds = lstAlwProcessedObjects.Select(x => x.emp_id).Distinct().ToList(); //after sync ids for which salary to be processed
                var extEmpDetails = dataContext.prl_employee_details.Include("prl_employee").AsEnumerable().Where(x => employeeIds.Contains(x.emp_id)).GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();
                var salaryReviewList = GetPrlSalaryReviews(salaryMonth.Date, extEmpDetails);

                //5.generate salary process details object for each employee whose allowance and deduction have been calculated
                lstSalaryProcessDetails = CreateSalaryDetails(salaryMonth, extEmpDetails, lstAlwProcessedObjects, lstDedProcessedObjects, salaryReviewList);

                //var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;

                //calculate salary for left over employees of previous month
                var prevMon = salaryMonth.AddMonths(-1);
                var prevSalaryDetails = new List<prl_salary_process_detail>();

                if (selectedEmployees.Count == 0)
                {
                    //CalculateSalaryPreviousMonthLeftOver(prevMon, ref lstAlwProcessedObjects, ref lstDedProcessedObjects, ref prevSalaryDetails);
                }

                MySqlConnection connection = null;
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                

                //string _batch = BatchNumberGenerator.generateSalaryBatchNumber("SALARY", paymentDate);
                //sp.batch_no = _batch;

                try
                {
                    connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
                    connection.Open();
                    command.Connection = connection;

                    if (spBatchNo == null)
                    {
                        MySqlTransaction tran = null;
                        tran = connection.BeginTransaction();

                        //1. insert salary process table and get the process id
                        var salprocessText = @"INSERT INTO prl_salary_process (batch_no, salary_month, process_date, payment_date, 
	                                    division_id, department_id, grade_id, gender, is_disbursed, created_by, 
	                                    created_date, updated_by, updated_date)
                                        VALUES	(?batch_no, ?salary_month, ?process_date, ?payment_date,
	                                    ?division_id, ?department_id, ?grade_id, ?gender, ?is_disbursed, ?created_by, 
	                                    ?created_date, ?updated_by, ?updated_date);";
                        command.CommandText = salprocessText;
                        command.Parameters.AddWithValue("?batch_no", sp.batch_no);
                        command.Parameters.AddWithValue("?salary_month", sp.salary_month);
                        command.Parameters.AddWithValue("?process_date", sp.process_date);
                        command.Parameters.AddWithValue("?payment_date", sp.payment_date);
                        //command.Parameters.AddWithValue("?company_id", sp.company_id);
                        command.Parameters.AddWithValue("?division_id", sp.division_id);
                        command.Parameters.AddWithValue("?department_id", sp.department_id);
                        command.Parameters.AddWithValue("?grade_id", sp.grade_id);
                        command.Parameters.AddWithValue("?gender", sp.gender);
                        command.Parameters.AddWithValue("?is_disbursed", "N");
                        command.Parameters.AddWithValue("?created_by", sp.created_by);
                        command.Parameters.AddWithValue("?created_date", sp.created_date);
                        command.Parameters.AddWithValue("?updated_by", sp.updated_by);
                        command.Parameters.AddWithValue("?updated_date", sp.updated_date);
                        command.ExecuteNonQuery();
                        salaryProcessId = (int)command.LastInsertedId;
                        tran.Commit();  
                    }

                    MySqlTransaction tran2 = null;
                    foreach (var spd in lstSalaryProcessDetails)
                    {
                        try
                        {
                            tran2 = connection.BeginTransaction();
                            //insert individual salary allowances
                            foreach (var sa in lstAlwProcessedObjects.Where(x => x.emp_id == spd.emp_id))
                            {
                                command.Parameters.Clear();
                                command.CommandText = @"INSERT INTO prl_salary_allowances (salary_process_id, salary_month, 
	                                            calculation_for_days, emp_id, allowance_name_id, amount, arrear_amount, remarks)
                                                VALUES	(?salary_process_id, ?salary_month, ?calculation_for_days, 
	                                            ?emp_id, ?allowance_name_id, ?amount, ?arrear_amount, ?remarks);";
                                command.Parameters.AddWithValue("?salary_process_id", salaryProcessId);
                                command.Parameters.AddWithValue("?salary_month", sa.salary_month.Date);
                                command.Parameters.AddWithValue("?calculation_for_days", spd.calculation_for_days);
                                command.Parameters.AddWithValue("?emp_id", sa.emp_id);
                                command.Parameters.AddWithValue("?allowance_name_id", sa.allowance_name_id);
                                command.Parameters.AddWithValue("?amount", sa.amount);
                                command.Parameters.AddWithValue("?arrear_amount", sa.arrear_amount);
                                command.Parameters.AddWithValue("?remarks", sa.remarks);
                                command.ExecuteNonQuery();
                                Trace.WriteLine("---individual allowance save emp id = " + sa.emp_id);
                            }

                            //insert individual salary deductions
                            foreach (var de in lstDedProcessedObjects.Where(x => x.emp_id == spd.emp_id))
                            {
                                command.Parameters.Clear();
                                command.CommandText = @"INSERT INTO prl_salary_deductions (salary_process_id, 
	                                        salary_month, calculation_for_days, emp_id, deduction_name_id, 	amount, arrear_amount, remarks)
	                                        VALUES
	                                        (?salary_process_id, ?salary_month, ?calculation_for_days,  ?emp_id, 
	                                        ?deduction_name_id, ?amount,  ?arrear_amount, ?remarks);";
                                command.Parameters.AddWithValue("?salary_process_id", salaryProcessId);
                                command.Parameters.AddWithValue("?salary_month", de.salary_month.Date);
                                command.Parameters.AddWithValue("?calculation_for_days", spd.calculation_for_days);
                                command.Parameters.AddWithValue("?emp_id", de.emp_id);
                                command.Parameters.AddWithValue("?deduction_name_id", de.deduction_name_id);
                                command.Parameters.AddWithValue("?amount", de.amount);
                                command.Parameters.AddWithValue("?arrear_amount", de.arrear_amount);
                                command.Parameters.AddWithValue("?remarks", de.remarks);
                                command.ExecuteNonQuery();
                                Trace.WriteLine("---individual deduction save emp id = " + de.emp_id);
                            }

                            //update salary review table for this employee
                            var salReview = salaryReviewList.SingleOrDefault(x => x.emp_id == spd.emp_id);
                            if (salReview != null)
                            {
                                command.Parameters.Clear();
                                command.CommandText = @"UPDATE prl_salary_review SET is_arrear_calculated='Yes', arrear_calculated_date=?arrear_calculated_date WHERE id=?id;";
                                command.Parameters.AddWithValue("?id", salReview.id);
                                command.Parameters.AddWithValue("?arrear_calculated_date", salaryMonth.Date);
                                command.ExecuteNonQuery();

                                /// Update Employee Details Basic
                                command.Parameters.Clear();
                                command.CommandText = @"UPDATE prl_employee_details 
                                                SET basic_salary = ?_amount, updated_by = ?_user, updated_date = ?dt
                                            WHERE emp_id = ?emp_id";
                                command.Parameters.AddWithValue("?_amount", salReview.new_basic);
                                command.Parameters.AddWithValue("?emp_id", salReview.emp_id);
                                command.Parameters.AddWithValue("?_user", userIdentity.Name);
                                command.Parameters.AddWithValue("?dt", DateTime.Now);
                                command.ExecuteNonQuery();

                                Trace.WriteLine("---Salary Review Updated for employee id " + spd.emp_id);
                            }

                            if (prevSalaryDetails.Count > 0)
                            {
                                var prevDtl = prevSalaryDetails.Where(x => x.emp_id == spd.emp_id).FirstOrDefault();
                                if (prevDtl != null)
                                {
                                    spd.this_month_basic += prevDtl.this_month_basic;
                                    spd.total_allowance += prevDtl.total_allowance;
                                    spd.totla_arrear_allowance += prevDtl.totla_arrear_allowance;
                                    spd.pf_amount += prevDtl.pf_amount;
                                    spd.pf_arrear += prevDtl.pf_arrear;
                                    spd.total_deduction += prevDtl.total_deduction;
                                    spd.total_arrear_deduction += prevDtl.total_arrear_deduction;
                                    spd.total_monthly_tax += prevDtl.total_monthly_tax;
                                    spd.total_bonus += prevDtl.total_bonus;
                                    //spd.current_basic += prevDtl.current_basic;

                                    prevSalaryDetails.Remove(prevDtl);
                                }
                            }


                            decimal thisMonthPF = 0;

                            thisMonthPF = Math.Round((spd.this_month_basic.Value * (decimal)0.10 ), 0, MidpointRounding.AwayFromZero);

                            //insert individual salary process detail 
                            command.Parameters.Clear();
                            command.CommandText = @"INSERT INTO prl_salary_process_detail (	salary_process_id, salary_month, emp_id, 
                                        calculation_for_days, current_basic, this_month_basic, total_allowance, 
                                        totla_arrear_allowance, pf_amount, pf_arrear, total_deduction, total_arrear_deduction, 
                                        total_monthly_tax, total_bonus, net_pay)
                                        VALUES
                                        ( ?salary_process_id, ?salary_month, ?emp_id, ?calculation_for_days, ?current_basic, 
                                        ?this_month_basic, ?total_allowance, ?totla_arrear_allowance, ?pf_amount, ?pf_arrear,
                                        ?total_deduction, ?total_arrear_deduction, ?total_monthly_tax, 
                                        ?total_bonus, ?net_pay);";

                            command.Parameters.AddWithValue("?salary_process_id", salaryProcessId);
                            command.Parameters.AddWithValue("?salary_month", spd.salary_month.Date);
                            command.Parameters.AddWithValue("?emp_id", spd.emp_id);
                            command.Parameters.AddWithValue("?calculation_for_days", spd.calculation_for_days);
                            command.Parameters.AddWithValue("?current_basic", spd.current_basic);
                            command.Parameters.AddWithValue("?this_month_basic", spd.this_month_basic);
                            command.Parameters.AddWithValue("?total_allowance", spd.total_allowance);
                            command.Parameters.AddWithValue("?totla_arrear_allowance", spd.totla_arrear_allowance);

                            command.Parameters.AddWithValue("?pf_amount", thisMonthPF);
                            command.Parameters.AddWithValue("?pf_arrear", spd.pf_arrear);

                            command.Parameters.AddWithValue("?total_deduction", spd.total_deduction);
                            command.Parameters.AddWithValue("?total_arrear_deduction", spd.total_arrear_deduction);
                            command.Parameters.AddWithValue("?total_monthly_tax", 0);
                            command.Parameters.AddWithValue("?total_bonus", 0);
                            command.Parameters.AddWithValue("?net_pay", 0);

                            command.ExecuteNonQuery();
                            tran2.Commit();
                            Trace.WriteLine("---individual salary process details save successful emp id = " + spd.emp_id);
                        }
                        catch (Exception ex)
                        {
                            result.ErrorOccured = true;
                            result.AddToErrorList("Could not process salary for employee " + spd.prl_employee.emp_no);
                            tran2.Rollback();
                        }
                    }

                    int taxResult = 0;
                    IncomeTaxService ins = new IncomeTaxService(dataContext);
                    var empIds = new List<int>();
                    var extEmpDetailList = new List<prl_employee_details>();

                    if (selectedEmployees != null && selectedEmployees.Count > 0)
                    {
                        extEmpDetailList = dataContext.prl_employee_details.Include("prl_employee").AsEnumerable().Where(x => selectedEmployees.Contains(x.emp_id)).GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();
                    }
                    else
                    {
                        empIds = lstSalaryProcessDetails.Where(x => x.salary_month.Month == salaryMonth.Month && x.salary_month.Year == salaryMonth.Year).Select(x => x.emp_id).Distinct().ToList();
                        extEmpDetailList = dataContext.prl_employee_details.Include("prl_employee").AsEnumerable().Where(x => empIds.Contains(x.emp_id)).GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();
                    }

                    var empList = extEmpDetailList.Select(x => x.prl_employee).ToList();
                    int fiscalYear = FindFiscalYear(salaryMonth);
                    string processUser = userIdentity.Name.ToString();

                   // var _res = ins.process_incomeTax(empList, sp.batch_no, salaryMonth, sp.process_date, fiscalYear, processUser);
                }
                catch (Exception ex)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList(ex.Message);
                    Trace.WriteLine("---exception ----");
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                result.ErrorOccured = true;
                result.AddToErrorList("The Payment Month is similiar to a another Salary Process that already done.");
                return result;
            }

            return result;
        }

        private List<prl_salary_process_detail> CreateSalaryDetails(DateTime processDate, List<prl_employee_details> extEmpDetails, List<prl_salary_allowances> lstAllowances, List<prl_salary_deductions> lstDeductions, List<prl_salary_review> salaryReviewList)
        {
            var salDetailResult = new List<prl_salary_process_detail>();

            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            //// Discontinued_list

            thisMonthsDiscontinued = dataContext.prl_employee_discontinue.AsEnumerable().Where(x => x.discontinue_date.ToString("yyyy-MM") == processDate.ToString("yyyy-MM")).ToList();
            
            //thisMonthsDiscontinued = dataContext.prl_employee_discontinue.AsEnumerable().Where(x => x.with_salary == "Y" &&
            //        x.discontinue_date.ToString("yyyy-MM") == processDate.ToString("yyyy-MM")).ToList();

            //
            thisMonthUploadDeduction = dataContext.prl_upload_deduction.Include("prl_employee").AsEnumerable()
                     .Where(x => x.salary_month_year.Value.Date >= proStartDate && x.salary_month_year.Value.Date <= proEndDate
                      && extEmpDetails.Select(y => y.emp_id).ToList().Contains(x.emp_id))
                     .ToList();


            //salary hold list 
            thisMonthsHold = dataContext.prl_salary_hold.AsEnumerable().Where(x => x.is_holded == "Y" && x.hold_from.Value.ToString("yyyy-MM") == processDate.ToString("yyyy-MM") &&
                       x.with_salary == Convert.ToSByte(true)).ToList();

           //leave without pay employee list
            var lstLWP = new List<prl_employee_leave_without_pay>();

            lstLWP = dataContext.prl_employee_leave_without_pay.AsEnumerable().
                        Join(extEmpDetails, elwp => elwp.emp_id, l => l.emp_id, (elwp, l) => elwp).
                        Where(x => (x.strat_date.Value.Date <= proStartDate.Date && x.end_date.Value.Date >= proEndDate.Date) ||
                                   (x.strat_date.Value.Date > proStartDate.Date && x.end_date.Value.Date <= proStartDate.Date) ||
                                   (x.strat_date.Value.Date > proStartDate.Date && x.strat_date.Value.Date <= proStartDate.Date)).ToList();
            
            //create salary process details for each employee whose allowances and duductions have been calculated

            foreach (var emp in extEmpDetails)
            {
                decimal totalDaysOfUnpaid = 0;

                //if (thisMonthUploadDeduction.Select(y => y.emp_id).ToList().Contains(emp.emp_id))
                //{
                //    var unpaidLeaves = thisMonthUploadDeduction.SingleOrDefault(p => p.emp_id == emp.emp_id && p.deduction_name_id == 1);
                //    if (unpaidLeaves != null)
                //    {
                //        totalDaysOfUnpaid = unpaidLeaves.amount;
                //    }
                //}

                var spd= new prl_salary_process_detail();
                spd.emp_id = emp.emp_id;
                spd.prl_employee = emp.prl_employee;
                spd.salary_month = processDate.Date;

                spd.calculation_for_days = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(emp.prl_employee, proStartDate,proEndDate, thisMonthsDiscontinued, totalDaysOfUnpaid, thisMonthsHold);

                spd.this_month_basic = Math.Round(((emp.basic_salary / daysInMonth) * spd.calculation_for_days), 0, MidpointRounding.AwayFromZero);
                spd.current_basic = emp.basic_salary;

                #region Calculate Basic Salary Arrear for each employee
                var r = salaryReviewList.AsEnumerable().SingleOrDefault(x => x.emp_id == emp.emp_id);
                if (r != null)
                {
                    spd.current_basic = r.new_basic ;
                    //spd.this_month_basic = CalculateThisMonthBasic(spd.calculation_for_days, processDate, emp, r);
                    spd.this_month_basic = Math.Round((CalculateThisMonthBasic(spd.calculation_for_days, processDate, emp, r)), 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    var upload_arrear_basic = dataContext.prl_upload_allowance.Where(x => x.allowance_name_id == 19 && x.emp_id == emp.emp_id && x.salary_month_year.Value.Month == proStartDate.Month && x.salary_month_year.Value.Year == proStartDate.Year).SingleOrDefault();
                    if (upload_arrear_basic != null)
                    {
                        if (upload_arrear_basic.amount != null)
                        {
                            spd.this_month_basic += upload_arrear_basic.amount.Value;
                        }
                    }
                }

                #endregion

                #region Calculate Leave Without Pay
                var thisEmployee = lstLWP.Where(x => x.emp_id == emp.emp_id).ToList();
                foreach (var lwp in thisEmployee)
                {
                   var lstOfAlwByEmpId = lstAllowances.Where(x => x.emp_id == spd.emp_id).ToList();
                   CalculateLeaveWithoutPay(ref spd,processDate,lwp,ref lstOfAlwByEmpId);         
                }
                #endregion

                //sum allowance amount
                spd.total_allowance = lstAllowances.AsEnumerable().Where(x => x.emp_id == spd.emp_id).Sum(s => s.amount);
                Trace.WriteLine("---individual sum of allowance save emp id = " + spd.emp_id);
                //sum allowance arrear amount
                spd.totla_arrear_allowance = lstAllowances.AsEnumerable().Where(x => x.emp_id == spd.emp_id).Sum(s => s.arrear_amount);
                Trace.WriteLine("---individuall sum of allowance arrear save emp id = " + spd.emp_id);
                //sum deduction amount
                spd.total_deduction = lstDeductions.AsEnumerable().Where(x => x.emp_id == spd.emp_id).Sum(s => s.amount);
                Trace.WriteLine("---individual sum of deduction save emp id = " + spd.emp_id);
                //sum deduction arrear amount
                spd.total_arrear_deduction = lstDeductions.AsEnumerable().Where(x => x.emp_id == spd.emp_id).Sum(s => s.arrear_amount);
                Trace.WriteLine("---individual sum of deduction arrear save emp id = " + spd.emp_id);

                salDetailResult.Add(spd);
            }

            return salDetailResult;
        }

        private List<prl_salary_review> GetPrlSalaryReviews(DateTime processDate, List<prl_employee_details> extEmpDetails)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);
            return dataContext.prl_salary_review.AsEnumerable().Where(r => r.is_arrear_calculated == "No" && r.effective_from.Value.Date <= proEndDate && extEmpDetails.Select(y => y.emp_id).ToList().Contains(r.emp_id)).ToList();
        }

        private void CalculateLeaveWithoutPay(ref prl_salary_process_detail spd, DateTime processDate, prl_employee_leave_without_pay leaveWithoutPay,ref List<prl_salary_allowances> thisMonthsAllowanceses)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            int lwpDays = SalaryCalculationHelper.NumberOfDaysToCalculateForLeaveWithoutPay(leaveWithoutPay,proStartDate,proEndDate);

            if (leaveWithoutPay.prl_leave_without_pay_settings.Lwp_type.ToLower() == "Basic Salary")
            {
                var t = spd.this_month_basic - (spd.this_month_basic * leaveWithoutPay.prl_leave_without_pay_settings.percentage_of_basic.Value / 100);
                spd.this_month_basic = t / daysInMonth * lwpDays;
            }
            else if (leaveWithoutPay.prl_leave_without_pay_settings.Lwp_type.ToLower() == "basic")
            {
                foreach (var alw in thisMonthsAllowanceses.AsEnumerable().Where(y => y.allowance_name_id == leaveWithoutPay.prl_leave_without_pay_settings.allowance_id))
                {
                    var at = (alw.amount - (alw.amount * leaveWithoutPay.prl_leave_without_pay_settings.percentage_of_allowance.Value / 100));
                    alw.amount = at / daysInMonth * lwpDays;
                }
            }
        }

        private decimal CalculateThisMonthBasic(int numberOfDaysWorked,DateTime processDate,prl_employee_details empDetails,prl_salary_review salaryReview)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            //daysInMonth = 30; //Every month considerd as 30

            decimal basicSalaryAmount = (empDetails.basic_salary/ daysInMonth) * numberOfDaysWorked;

            if (salaryReview != null)
            {
                decimal incramentedBasic = 0;

                var empSalaryDetails = dataContext.prl_salary_process_detail.AsEnumerable()
                    .Where(x => x.salary_month.Date >= salaryReview.effective_from.Value.Date && x.salary_month.Date < proStartDate && x.emp_id == empDetails.emp_id)
                    .ToList();

                basicSalaryAmount = (salaryReview.new_basic / daysInMonth) * numberOfDaysWorked;

                incramentedBasic = salaryReview.new_basic - salaryReview.current_basic;

                if (salaryReview.effective_from < proEndDate && salaryReview.effective_from.Value.Month != proEndDate.Month)
                {
                    basicSalaryAmount = basicSalaryAmount + SalaryCalculationHelper.CalculateArrearOnBasic(salaryReview, empSalaryDetails, incramentedBasic);
                }
            }

            var upload_arrear_basic = dataContext.prl_upload_allowance.Where(x => x.allowance_name_id == 19 && x.emp_id == empDetails.emp_id && x.salary_month_year.Value.Month == proStartDate.Month && x.salary_month_year.Value.Year == proStartDate.Year).SingleOrDefault();

            if (upload_arrear_basic != null)
            {
                if (upload_arrear_basic.amount != null)
                {
                    basicSalaryAmount += upload_arrear_basic.amount.Value;
                }
            }

            return basicSalaryAmount;
        }

        public int salaryRollbacked( bool allEmployee, List<int> selectedEmployees, SalaryProcessModel _salProcess, string _UserName, int _month, int _yr)
        {
            int intResult = 0;
            MySqlCommand mySqlCommand = null;
            MySqlConnection mySqlConnection = null;
            MySqlTransaction tran = null;
            try
            {
                using (var cntx = new payroll_systemContext())
                {
                    var objectContext = ((IObjectContextAdapter)cntx).ObjectContext;
                    objectContext.Connection.Open();

                    mySqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
                    mySqlCommand = new MySqlCommand();
                    mySqlCommand.Connection = mySqlConnection;
                    mySqlConnection.Open();
                    tran = mySqlConnection.BeginTransaction();
                    
                    string _monthYear = "";

                    if (_month > 9)
                    {
                        _monthYear = _month.ToString() + _yr.ToString();
                    }
                   else
                    {
                        _monthYear = "0" + _month.ToString() + _yr.ToString();
                    }
                    
                    string _batch_no = "SALARY-"+ _monthYear;


                    var salProcess = cntx.prl_salary_process.FirstOrDefault(x => x.salary_month.Month == _month && x.salary_month.Year == _yr && x.batch_no == _batch_no);
                    if (salProcess != null)
                    {
                        /// Newly Edition for single employees data undo//
                        if (selectedEmployees != null && selectedEmployees.Count > 0)
                        {
                            foreach (var item in selectedEmployees)
                            {
                                var taxProcess = cntx.prl_employee_tax_process.FirstOrDefault(q => q.salary_process_id == salProcess.id && q.emp_id == item);

                                if (taxProcess != null)
                                {
                                    var taxSlab = cntx.prl_employee_tax_slab.Where(s => s.salary_month == taxProcess.salary_month.Month && s.salary_year == taxProcess.salary_month.Year && s.emp_id == item).ToList();
                                    if (taxSlab.Count > 0)
                                    {
                                        // First Delete
                                        DeleteEmployeeTaxSlab(taxProcess.salary_month.Month, taxProcess.salary_month.Year, item, mySqlCommand);
                                    }
                                    var taxDetail = cntx.prl_employee_tax_process_detail.Where(t => t.tax_process_id == taxProcess.id && t.emp_id == item).ToList();
                                    if (taxDetail.Count > 0)
                                    {
                                        // Second Delete
                                        DeleteEmployeeTaxDetail(salProcess.id, item, mySqlCommand);
                                    }
                                    // Third Delete
                                    DeleteEmployeeTaxProcess(salProcess.id, item, mySqlCommand);
                                }

                                var salAllowance = cntx.prl_salary_allowances.Where(a => a.salary_process_id == salProcess.id && a.emp_id == item).ToList();
                                if (salAllowance.Count > 0)
                                {
                                    // Fourth Delete
                                    DeleteSalaryAllowance(salProcess.id, item, mySqlCommand);
                                }
                                var salDeduction = cntx.prl_salary_deductions.Where(d => d.salary_process_id == salProcess.id && d.emp_id == item).ToList();
                                if (salDeduction.Count > 0)
                                {
                                    // Fifth Delete
                                    DeleteSalaryDeduction(salProcess.id, item, mySqlCommand);
                                }
                                var salDetail = cntx.prl_salary_process_detail.Where(s => s.salary_process_id == salProcess.id && s.emp_id == item).ToList();
                                if (salDetail.Count > 0)
                                {
                                    // Sixth Delete
                                    DeleteSalaryDetail(salProcess.id, item, mySqlCommand);
                                }
                                var totalSalDetail = cntx.prl_salary_process_detail.Where(s => s.salary_process_id == salProcess.id).ToList();

                                if (totalSalDetail.Count == 1)
                                {
                                    // Seventh Delete
                                    DeleteSalaryMaster(salProcess.id, mySqlCommand); 
                                }

                                var salReview = cntx.prl_salary_review.SingleOrDefault(r => r.arrear_calculated_date.Value.Month == _month && r.arrear_calculated_date.Value.Year == _yr && r.emp_id == item);
                                //var salReview = cntx.prl_salary_review.Where(r => r.arrear_calculated_date.Value.Month == _month && r.arrear_calculated_date.Value.Year == _yr && r.emp_id == item).ToList();

                                if (salReview != null)
                                {
                                    UpdateSalaryReview(_month, _yr, item, _UserName, DateTime.Now, mySqlCommand);
                                    UpdateEmpDetailsBasic(salReview.current_basic, item, _UserName, DateTime.Now, mySqlCommand);
                                } 
                            }
                        }
                        else
                        {

                            var taxProcess = cntx.prl_employee_tax_process.FirstOrDefault(q => q.salary_process_id == salProcess.id);

                            if (taxProcess != null)
                            {
                                var taxSlab = cntx.prl_employee_tax_slab.Where(s => s.salary_month == taxProcess.salary_month.Month && s.salary_year == taxProcess.salary_month.Year).ToList();
                                if (taxSlab.Count > 0)
                                {
                                    // First Delete
                                    DeleteEmployeeTaxSlab(taxProcess.salary_month.Month, taxProcess.salary_month.Year, mySqlCommand);
                                }
                                var taxDetail = cntx.prl_employee_tax_process_detail.Where(t => t.tax_process_id == taxProcess.id).ToList();
                                if (taxDetail.Count > 0)
                                {
                                    // Second Delete
                                    DeleteEmployeeTaxDetail(salProcess.id, mySqlCommand);
                                }
                                // Third Delete
                                DeleteEmployeeTaxProcess(salProcess.id, mySqlCommand);
                            }

                            var salAllowance = cntx.prl_salary_allowances.Where(a => a.salary_process_id == salProcess.id).ToList();
                            if (salAllowance.Count > 0)
                            {
                                // Fourth Delete
                                DeleteSalaryAllowance(salProcess.id, mySqlCommand);
                            }
                            var salDeduction = cntx.prl_salary_deductions.Where(d => d.salary_process_id == salProcess.id).ToList();
                            if (salDeduction.Count > 0)
                            {
                                // Fifth Delete
                                DeleteSalaryDeduction(salProcess.id, mySqlCommand);
                            }
                            var salDetail = cntx.prl_salary_process_detail.Where(s => s.salary_process_id == salProcess.id).ToList();
                            if (salDetail.Count > 0)
                            {
                                // Sixth Delete
                                DeleteSalaryDetail(salProcess.id, mySqlCommand);
                            }
                            // Seventh Delete
                            DeleteSalaryMaster(salProcess.id, mySqlCommand);

                            var salReview = cntx.prl_salary_review.Where(r => r.arrear_calculated_date.Value.Month == _month && r.arrear_calculated_date.Value.Year == _yr).ToList();
                            if (salReview != null)
                            {
                                UpdateSalaryReview(_month, _yr, _UserName, DateTime.Now, mySqlCommand);

                                foreach (var item in salReview)
                                {
                                    UpdateEmpDetailsBasic(item.current_basic, item.emp_id, _UserName, DateTime.Now, mySqlCommand);
                                }
                            }
                        }

                        tran.Commit();
                        intResult = 1;
                    }
                    else
                    {
                        intResult = -909;
                    }
                }
            }
            catch (Exception ex)
            {

                tran.Rollback();
            }
            finally
            {
                if (mySqlConnection != null)
                {
                    mySqlConnection.Close();
                }
            }
            return intResult;
        }

        //individual Delete
        public int DeleteEmployeeTaxProcess(int salProcessID, int emp_id, MySqlCommand command)
        {
            int taxDel = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_employee_tax_process WHERE salary_process_id = ?salary_process_id  and emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?salary_process_id", salProcessID);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                taxDel = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taxDel;
        }

        public int DeleteEmployeeTaxDetail(int salary_process_id, int emp_id, MySqlCommand command)
        {
            int taxDel = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_employee_tax_process_detail WHERE salary_process_id = ?salary_process_id and emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?salary_process_id", salary_process_id);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                taxDel = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taxDel;
        }

        public int DeleteEmployeeTaxSlab(int salary_month, int salary_year, int emp_id, MySqlCommand command)
        {
            int taxSlab = 0;
            try
            {
                //const string commandText = @"DELETE FROM prl_employee_tax_slab WHERE salary_process_id = ?salary_process_id";
                const string commandText = @"DELETE FROM prl_employee_tax_slab WHERE salary_month = ?salary_month and salary_year = ?salary_year and emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                //command.Parameters.AddWithValue("?salary_process_id", salary_process_id);
                command.Parameters.AddWithValue("?salary_month", salary_month);
                command.Parameters.AddWithValue("?salary_year", salary_year);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                taxSlab = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taxSlab;
        }

        public int DeleteSalaryAllowance(int salaryProcessId, int emp_id, MySqlCommand command)
        {
            int _allow = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_allowances WHERE salary_process_id = ?processID  and emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                _allow = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _allow;
        }

        public int DeleteSalaryDeduction(int salaryProcessId, int emp_id, MySqlCommand command)
        {
            int _ded = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_deductions WHERE salary_process_id = ?processID  and emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                _ded = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ded;
        }

        public int DeleteSalaryDetail(int salaryProcessId, int emp_id, MySqlCommand command)
        {
            int salDet = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_process_detail WHERE salary_process_id = ?processID and emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                salDet = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return salDet;
        }

        public int UpdateSalaryReview(int _month, int _yr, int emp_id, string _user, DateTime dt, MySqlCommand command)
        {
            int _update = 0;
            try
            {
                const string commandText = @"UPDATE prl_salary_review 
                                                SET is_arrear_calculated = ?_yes, updated_by = ?_user, updated_date = ?dt
                                            WHERE MONTH(arrear_calculated_date) = ?_month AND YEAR(arrear_calculated_date) = ?_yr AND emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?_yes", "No");
                command.Parameters.AddWithValue("?_month", _month);
                command.Parameters.AddWithValue("?_yr", _yr);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                command.Parameters.AddWithValue("?_user", _user);
                command.Parameters.AddWithValue("?dt", dt);
                _update = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _update;
        }


        ///overall delete

        public int DeleteEmployeeTaxProcess(int salProcessID, MySqlCommand command)
        {
            int taxDel = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_employee_tax_process WHERE salary_process_id = ?salary_process_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?salary_process_id", salProcessID);
                taxDel = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taxDel;
        }

        public int DeleteEmployeeTaxDetail(int salary_process_id, MySqlCommand command)
        {
            int taxDel = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_employee_tax_process_detail WHERE salary_process_id = ?salary_process_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?salary_process_id", salary_process_id);
                taxDel = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taxDel;
        }

        public int DeleteEmployeeTaxSlab(int salary_month, int salary_year, MySqlCommand command)
        {
            int taxSlab = 0;
            try
            {
                //const string commandText = @"DELETE FROM prl_employee_tax_slab WHERE salary_process_id = ?salary_process_id";
                const string commandText = @"DELETE FROM prl_employee_tax_slab WHERE salary_month = ?salary_month and salary_year = ?salary_year";
                command.Parameters.Clear();
                command.CommandText = commandText;
                //command.Parameters.AddWithValue("?salary_process_id", salary_process_id);
                command.Parameters.AddWithValue("?salary_month", salary_month);
                command.Parameters.AddWithValue("?salary_year", salary_year);
                taxSlab = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taxSlab;
        }

        public int DeleteSalaryAllowance(int salaryProcessId, MySqlCommand command)
        {
            int _allow = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_allowances WHERE salary_process_id = ?processID";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                _allow = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _allow;
        }

        public int DeleteSalaryDeduction(int salaryProcessId, MySqlCommand command)
        {
            int _ded = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_deductions WHERE salary_process_id = ?processID";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                _ded = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ded;
        }

        public int DeleteSalaryDetail(int salaryProcessId, MySqlCommand command)
        {
            int salDet = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_process_detail WHERE salary_process_id = ?processID";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                salDet = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return salDet;
        }

        public int DeleteSalaryMaster(int salaryProcessId, MySqlCommand command)
        {
            int salDet = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_salary_process WHERE id = ?processID;";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?processID", salaryProcessId);
                salDet = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return salDet;
        }


        public int UpdateEmpDetailsBasic(decimal basic_amount, int emp_id, string _user, DateTime dt, MySqlCommand command)
        {
            int _update = 0;
            try
            {
                const string commandText = @"UPDATE prl_employee_details 
                                                SET basic_salary = ?_amount, updated_by = ?_user, updated_date = ?dt
                                            WHERE emp_id = ?emp_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?_amount", basic_amount);
                command.Parameters.AddWithValue("?emp_id", emp_id);
                command.Parameters.AddWithValue("?_user", _user);
                command.Parameters.AddWithValue("?dt", dt);
                _update = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _update;
        }

        public int UpdateSalaryReview(int _month, int _yr, string _user, DateTime dt, MySqlCommand command)
        {
            int _update = 0;
            try
            {
                const string commandText = @"UPDATE prl_salary_review 
                                                SET is_arrear_calculated = ?_yes, updated_by = ?_user, updated_date = ?dt
                                            WHERE MONTH(arrear_calculated_date) = ?_month AND YEAR(arrear_calculated_date) = ?_yr";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?_yes", "No");
                command.Parameters.AddWithValue("?_month", _month);
                command.Parameters.AddWithValue("?_yr", _yr);
                command.Parameters.AddWithValue("?_user", _user);
                command.Parameters.AddWithValue("?dt", dt);
                _update = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _update;
        }


        public int FindFiscalYear(DateTime processDate)
        {
            int fiscalyR = 0;
            int _month = processDate.Month;
            int _yr = processDate.Year;
            string curYear = string.Empty;
            string prevYear = string.Empty;

            if (_month <= 6)
            {
                prevYear = (_yr - 2).ToString() + "-" + (_yr - 1).ToString();
                curYear = (_yr - 1).ToString() + "-" + _yr.ToString();
            }
            else if (_month > 6)
            {
                curYear = _yr.ToString() + "-" + (_yr + 1).ToString();
                prevYear = (_yr - 1).ToString() + "-" + _yr.ToString();
            }

            var fisYr = dataContext.prl_fiscal_year.Where(f => f.fiscal_year.Substring(0, 9) == curYear).ToList();
            
            if (fisYr != null)
            {
                if (fisYr.Count > 1)
                {
                    foreach (var item in fisYr)
                    {
                        if (curYear == item.fiscal_year)
                        {
                            fiscalyR = item.id;
                        }
                    }
                }
                else if (fisYr.Count == 1)
                {
                    fiscalyR = fisYr[0].id;
                }
                else
                {
                    throw new Exception();
                }
            }
            return fiscalyR;
        }

        private bool IsSalaryAlreadyProcessed(DateTime date, string processType, string batchNo, int? departmentId, List<int> selectedEmployee=null)
        {

            if (processType == "all")
            {
              var b = dataContext.prl_salary_process.AsEnumerable()
                    .Any(x => x.salary_month.ToString("yyyy-MM") == date.ToString("yyyy-MM") && x.batch_no == batchNo && departmentId.Value == 0);
                if (b == true)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Already processed for current settings.");
                }
                return b;
            }
            else if (processType == "department")
            {
                var b = dataContext.prl_salary_process.AsEnumerable()
                    .Any(x => x.salary_month.ToString("yyyy-MM") == date.ToString("yyyy-MM") && departmentId.Value == 0);
                if (b == true)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Already processed for current settings.");
                }
                return b;
            }
            else if (processType == "selected employee")
            {
                var b = dataContext.prl_salary_process_detail.AsEnumerable()
                    .Any(x => x.salary_month.ToString("yyyy-MM") == date.ToString("yyyy-MM") && selectedEmployee.Contains(x.emp_id));
                if (b == true)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Already processed for selected employees.");
                }
                return b;
            }
            
            return false;
        }

        public List<prl_employee_details> GetEligibleEmployeeForSalaryProcess(DateTime monthYear)
        {
            var proStartDate = new DateTime(monthYear.Year, monthYear.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(proStartDate.Year, proStartDate.Month);
            var proEndDate = new DateTime(monthYear.Year, monthYear.Month, daysInMonth);

            var result = new List<prl_employee_details>();

            using (var contxt = new payroll_systemContext())
            {
               result = contxt.prl_employee.AsEnumerable()
                   .Where(x => x.is_active == Convert.ToSByte(true) && x.joining_date.Date <= monthYear.Date)
                   .Join(contxt.prl_employee_details.Include("prl_employee").AsEnumerable(), ok => ok.id, ik => ik.emp_id, (ok, ik) => ik)
                   .GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();

                var salaryAlreadyProcessedEmpIds = contxt.prl_salary_process_detail.AsEnumerable()
                    .Where(x => x.salary_month.ToString("yyyy-MM") == monthYear.ToString("yyyy-MM"))
                    .Select(x => x.emp_id).ToList();
                try
                {
                    result.RemoveAll(x => salaryAlreadyProcessedEmpIds.Contains(x.emp_id));
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        #region Not been used 

        public List<int> GetSalaryAlreadyProcessedEmployees(DateTime monthYear)
        {
            using (var contxt = new payroll_systemContext())
            {
                return contxt.prl_salary_process_detail.AsEnumerable()
                    .Where(x => x.salary_month.ToString("yyyy-MM") == monthYear.ToString("yyyy-MM"))
                    .Select(x => x.emp_id).ToList();
            }
        }

        #endregion

        private void CalculateSalaryPreviousMonthLeftOver(DateTime monthYear,ref List<prl_salary_allowances> lstAllowanceses, ref List<prl_salary_deductions> lstDeductions,ref List<prl_salary_process_detail>  lstDetails )
        {
            /*calculate salary for employees whose salary was not processed*/
            var prev = monthYear;
            var proStartDatePrev = new DateTime(prev.Year, prev.Month, 1);
            int daysInMonthPrev = DateTime.DaysInMonth(prev.Year, prev.Month);
            var proEndDatePrev = new DateTime(prev.Year, prev.Month, daysInMonthPrev);

            var prevMonthEmps = GetEligibleLeftOverEmployeeForSalaryProcess(proEndDatePrev);

            if (prevMonthEmps == null)
            {
                return;
            }

            if (prevMonthEmps.Count == 0)
            {
                return;
            }

            var salAlwPrev = new SalaryAllowanceProcess(proEndDatePrev,proStartDatePrev,proEndDatePrev, prevMonthEmps);
            var salDedPrev = new SalaryDeductionProcess(proEndDatePrev,proStartDatePrev,proEndDatePrev, prevMonthEmps);

            //1. calculate allowance 
            var salAlwProAllPrev = salAlwPrev.Process();
            var lstAlwNotProcessedIdsPrev = salAlwPrev.GetEmployeeList().Select(x => x.emp_id).ToList().Except((salAlwProAllPrev.GetCompletedResultObjects() as List<prl_salary_allowances>).Select(x=>x.emp_id).ToList());
            var lstAlwProcessedObjectsPrev = salAlwProAllPrev.GetCompletedResultObjects() as List<prl_salary_allowances>;

            //2. calculate deduction 
            var salDedProAllPrev = salDedPrev.Process();
            var lstDedNotProcessedIdsPrev = salDedPrev.GetEmployeeList().Select(x => x.emp_id).ToList().Except((salDedProAllPrev.GetCompletedResultObjects() as List<prl_salary_deductions>).Select(x=>x.emp_id).ToList());
            var lstDedProcessedObjectsPrev = salDedProAllPrev.GetCompletedResultObjects() as List<prl_salary_deductions>;

            //3. sync allowance and deduction lists
            if (salAlwProAllPrev.ErrorOccured)
            {
                if (lstDedProcessedObjectsPrev != null)
                    lstDedProcessedObjectsPrev.RemoveAll(x => lstAlwNotProcessedIdsPrev.Contains(x.emp_id));
            }
            if (salDedProAllPrev.ErrorOccured)
            {
                if (lstAlwProcessedObjectsPrev != null)
                    lstAlwProcessedObjectsPrev.RemoveAll(x => lstDedNotProcessedIdsPrev.Contains(x.emp_id));
            }

            if (lstAlwProcessedObjectsPrev == null)
                return;
            if (lstAlwProcessedObjectsPrev.Count == 0)
                return;

            var employeeIdsPrev = lstAlwProcessedObjectsPrev.Select(x => x.emp_id).Distinct().ToList(); //after sync ids for which salary to be processed
            var extEmpDetailsPrev = dataContext.prl_employee_details.Include("prl_employee").AsEnumerable().Where(x => employeeIdsPrev.Contains(x.emp_id)).GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();
            var salaryReviewListPrev = GetPrlSalaryReviews(proEndDatePrev.Date, extEmpDetailsPrev);

            //5.generate salary process details object for each employee whose allowance and deduction have been calculated
            var lstSalaryProcessDetailsPrev = CreateSalaryDetails(proEndDatePrev, extEmpDetailsPrev, lstAlwProcessedObjectsPrev, lstDedProcessedObjectsPrev, salaryReviewListPrev);

            foreach (var x in lstAlwProcessedObjectsPrev)
            {
                lstAllowanceses.Add(x);
            }
            foreach (var y in lstDedProcessedObjectsPrev)
            {
                lstDeductions.Add(y);
            }
            foreach (var d in lstSalaryProcessDetailsPrev)
            {
                lstDetails.Add(d);
            }
        }

        public List<prl_employee_details> GetEligibleLeftOverEmployeeForSalaryProcess(DateTime monthYear)
        {
            var proStartDate = new DateTime(monthYear.Year, monthYear.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(proStartDate.Year, proStartDate.Month);
            var proEndDate = new DateTime(monthYear.Year, monthYear.Month, daysInMonth);
            var result = new List<prl_employee_details>();
            using (var contxt = new payroll_systemContext())
            {
                result = contxt.prl_employee.AsEnumerable()
                    .Where(x => x.is_active == Convert.ToSByte(true) && x.joining_date.Date <= proEndDate.Date && x.joining_date.Date >= proStartDate.Date)
                    .Join(contxt.prl_employee_details.Include("prl_employee").AsEnumerable(), ok => ok.id, ik => ik.emp_id, (ok, ik) => ik)
                    .GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();

                var salaryAlreadyProcessedEmpIds = contxt.prl_salary_process_detail.AsEnumerable()
                    .Where(x => x.salary_month.ToString("yyyy-MM") == monthYear.ToString("yyyy-MM"))
                    .Select(x => x.emp_id).ToList();

                try
                {
                    result.RemoveAll(x => salaryAlreadyProcessedEmpIds.Contains(x.emp_id));
                }
                catch (Exception)
                {
                }
            }
            return result;
        }


        public IProcessResult WorkerAllowanceProcess(DateTime processDate, DateTime salaryMonth)
        {
            decimal basic_sal = 0;
            int grade_id = 0;
            decimal factor = 0;
            List<prl_workers_allowances> worker_allowances = new List<prl_workers_allowances>();
            List<prl_allowance_staff> staff_alowance = new List<prl_allowance_staff>();
            
            List<prl_upload_staff_allowance> upload_allowances = new List<prl_upload_staff_allowance>();
            upload_allowances = dataContext.prl_upload_staff_allowance.Where(z => z.salary_month_year.Value.Year == processDate.Year && z.salary_month_year.Value.Month == processDate.Month).ToList();

            MySqlConnection connection = null;
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            MySqlTransaction tran = null;
            staff_alowance = dataContext.prl_allowance_staff.ToList();
            if (upload_allowances != null)
            {
                using (var contxt = new payroll_systemContext())
                {
                    try
                    {
                        foreach (var item in upload_allowances)
                        {
                            var wall = new prl_workers_allowances();
                            wall.allowance_process_id = BatchNumberGenerator.generateWorkerSalaryBatchNumber("ALLOWANCE", salaryMonth);
                            wall.salary_month = salaryMonth;
                            wall.process_date = processDate;
                            wall.calculation_for_days = item.no_of_entry.Value;
                            wall.emp_id = item.emp_id;
                            wall.allowance_name_id = item.allowance_name_id;

                            basic_sal = this.getBasicSalary(item.emp_id);
                            grade_id = this.getGradeId(item.emp_id);
                            var allowance = dataContext.prl_allowance_name.FirstOrDefault(q => q.id == item.allowance_name_id);
                            
                            factor = staff_alowance.FirstOrDefault(x => x.allowance_name_id == item.allowance_name_id).amount.Value;
                            if(allowance.description == "Divide")
                            {
                                wall.allowance_name = allowance.allowance_name + " (" + item.no_of_entry + ")";
                                decimal value = Convert.ToDecimal(((basic_sal / factor) * item.no_of_entry) * 2);
                                wall.amount = Math.Round(Convert.ToDecimal(((basic_sal / factor) * item.no_of_entry) * 2), 0, MidpointRounding.AwayFromZero);
                            }
                            else if (allowance.description == "Multiply")
                            {
                                wall.allowance_name = allowance.allowance_name + " (" + item.no_of_entry + ")";
                                wall.amount = Math.Round(Convert.ToDecimal(item.no_of_entry * factor), 2, MidpointRounding.AwayFromZero);
                            }
                            else if (allowance.description == "Equal")
                            {
                                wall.allowance_name = allowance.allowance_name;
                                wall.amount = Math.Round(decimal.Parse(item.no_of_entry.ToString()), 2, MidpointRounding.AwayFromZero);
                            }

                            var userIdentity = Thread.CurrentPrincipal.Identity;
                            wall.created_by = userIdentity.Name;
                            wall.created_date = DateTime.Now;
                            worker_allowances.Add(wall);
                        }

                        connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
                        connection.Open();
                        tran = connection.BeginTransaction();
                        foreach (var allowances in worker_allowances)
                        {
                            command.Parameters.Clear();
                            var salprocessText = @"INSERT INTO prl_workers_allowances (allowance_process_id, salary_month, process_date, calculation_for_days, emp_id, 
	                                    allowance_name_id, allowance_name, amount, arrear_amount, created_by, created_date, updated_by, updated_date)
                                        VALUES	(?allowance_process_id, ?salary_month, ?process_date, ?calculation_for_days, ?emp_id, ?allowance_name_id, 
	                                    ?allowance_name, ?amount, ?arrear_amount, ?created_by, ?created_date, ?updated_by, ?updated_date);";
                            command.Connection = connection;
                            command.CommandText = salprocessText;
                            command.Parameters.AddWithValue("?allowance_process_id", allowances.allowance_process_id);
                            command.Parameters.AddWithValue("?salary_month", allowances.salary_month);
                            command.Parameters.AddWithValue("?process_date", allowances.process_date);
                            command.Parameters.AddWithValue("?calculation_for_days", allowances.calculation_for_days);
                            command.Parameters.AddWithValue("?emp_id", allowances.emp_id);
                            command.Parameters.AddWithValue("?allowance_name_id", allowances.allowance_name_id);
                            command.Parameters.AddWithValue("?allowance_name", allowances.allowance_name);
                            command.Parameters.AddWithValue("?amount", allowances.amount);
                            command.Parameters.AddWithValue("?arrear_amount", allowances.arrear_amount);
                            command.Parameters.AddWithValue("?created_by", allowances.created_by);
                            command.Parameters.AddWithValue("?created_date", allowances.created_date);
                            command.Parameters.AddWithValue("?updated_by", "khurshid");
                            command.Parameters.AddWithValue("?updated_date", DateTime.Now);
                            command.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        worker.ErrorOccured = true;
                        worker.AddToErrorList("Internal System Error! Could not process worker allowance for the month");
                        tran.Rollback();
                    }
                }
            }

            return worker;
        }

        private decimal getBasicSalary(int emp_id)
        {
            decimal basic_salary = 0;
            try
            {
                basic_salary = dataContext.prl_employee_details.FirstOrDefault(q => q.emp_id == emp_id).basic_salary;
            }
            catch (Exception)
            {
                return basic_salary;
            }
            return basic_salary;
        }

        private int getGradeId(int emp_id)
        {
            int basic_salary = 0;
            try
            {
                //basic_salary = dataContext.prl_employee_details.FirstOrDefault(q => q.emp_id == emp_id).grade_id;
            }
            catch (Exception)
            {
                return basic_salary;
            }
            return basic_salary;

        }

        
    }
}