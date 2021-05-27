using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using com.gpit.DataContext;
using com.gpit.Model;
using MySql.Data.MySqlClient;
using PayrollWeb.Models;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Service
{
    public class SalaryService
    {
        private payroll_systemContext dataContext;
        private List<prl_salary_hold> thisMonthsHold;
        private List<prl_salary_process_detail> lstSalaryProcessDetails;
        private IProcessResult result;

        public SalaryService(payroll_systemContext dataContext)
        {
            this.dataContext = dataContext;
            lstSalaryProcessDetails = new List<prl_salary_process_detail>();
            result = new SalaryProcessResult(ProcessType.SALARY);
        }

        public IProcessResult ProcessSalary(bool allEmployee,List<int> selectedEmployees,int gradeId,int divisionId,int deptId ,DateTime processDate,DateTime paymentDate)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            DateTime proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            ISalaryProcess salAlw;
            ISalaryProcess salDed;

            //1. create salary process object 
            var sp = new prl_salary_process();
            sp.batch_no = BatchNumberGenerator.generateSalaryBatchNumber("SALARY", paymentDate);
            sp.process_date = processDate.Date;
            sp.payment_date = paymentDate.Date;
            sp.salary_month = processDate.Date;
            sp.is_disbursed = "N";

            if (allEmployee && gradeId==0 && divisionId==0 && deptId==0 )
            {
                if (IsSalaryAlreadyProcessed(paymentDate.Date, "all", null, 0, 0, 0, null))
                {
                    return result;
                }
                var emps = GetEligibleEmployeeForSalaryProcess(processDate.Date);
                salAlw = new SalaryAllowanceProcess(processDate.Date,proStartDate,proEndDate,emps);
                salDed = new SalaryDeductionProcess(processDate.Date,proStartDate,proEndDate,emps);
                sp.company_id = 1;
                sp.division_id = 0;
                sp.department_id = 0;
                sp.grade_id = 0;
            }
            else if(selectedEmployees!=null && selectedEmployees.Count > 0)
            {
                if (IsSalaryAlreadyProcessed(paymentDate.Date, "selected employee", null, null, null, null, selectedEmployees))
                {
                    return result;
                }
                var emps = GetEligibleEmployeeForSalaryProcess(processDate.Date);
                emps = emps.Where(x => selectedEmployees.Contains(x.emp_id)).ToList();
                salAlw = new SalaryAllowanceProcess(processDate.Date,proStartDate,proEndDate, emps);
                salDed = new SalaryDeductionProcess(processDate.Date,proStartDate,proEndDate, emps);
                sp.company_id = 1;
                sp.division_id = 0;
                sp.department_id = 0;
                sp.grade_id = 0;

            }
            else if(divisionId >0)
            {
                if (IsSalaryAlreadyProcessed(paymentDate.Date, "division", null, divisionId, null, null, null))
                {
                    return result;
                }
                var emps = GetEligibleEmployeeForSalaryProcess(processDate.Date);
                emps = emps.Where(x => x.division_id==divisionId).ToList();

                if (emps.Count <= 0)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Salary already processed for this division.");
                    return result;
                }

                salAlw = new SalaryAllowanceProcess(processDate,proStartDate,proEndDate,emps);
                salDed = new SalaryDeductionProcess(processDate,proStartDate,proEndDate, emps);
                sp.company_id = 1;
                sp.division_id = divisionId;
                sp.department_id = 0;
                sp.grade_id = 0;
            }
            else if(deptId >0 )
            {
                if (IsSalaryAlreadyProcessed(paymentDate.Date, "department", null, null, deptId, null, null))
                {
                    return result;
                }
                var emps = GetEligibleEmployeeForSalaryProcess(processDate.Date);
                emps = emps.Where(x => x.department_id==deptId).ToList();

                if (emps.Count <= 0)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Salary already processed for this department.");
                    return result;
                }
                salAlw = new SalaryAllowanceProcess(processDate.Date,proStartDate,proEndDate,emps);
                salDed = new SalaryDeductionProcess(processDate.Date,proStartDate,proEndDate,emps);
                sp.company_id = 1;
                sp.division_id = 0;
                sp.department_id = deptId;
                sp.grade_id = 0;
            }
            else
            {
                return null;
            }

            var userIdentity = Thread.CurrentPrincipal.Identity;
            sp.created_by = userIdentity.Name;
            sp.updated_by = userIdentity.Name;

            //1. calculate allowance 
            var salAlwProAll = salAlw.Process();
            var lstAlwNotProcessedIds = salAlw.GetEmployeeList().Select(x => x.emp_id).ToList().Except((salAlwProAll.GetCompletedResultObjects() as List<prl_salary_allowances>).Select(x=>x.emp_id).ToList());
            var lstAlwProcessedObjects = salAlwProAll.GetCompletedResultObjects() as List<prl_salary_allowances>;
            salAlwProAll.GetErrors.ForEach(x=> { result.AddToErrorList(x); });

            //2. calculate deduction 
            var salDedProAll = salDed.Process();
            var lstDedNotProcessedIds = salDed.GetEmployeeList().Select(x => x.emp_id).ToList().Except((salDedProAll.GetCompletedResultObjects() as List<prl_salary_deductions>).Select(x => x.emp_id).ToList());
            var lstDedProcessedObjects = salDedProAll.GetCompletedResultObjects() as List<prl_salary_deductions>;
            salDedProAll.GetErrors.ForEach(x=>{result.AddToErrorList(x);});

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
            var salaryReviewList = GetPrlSalaryReviews(processDate.Date);

            //5.generate salary process details object for each employee whose allowance and deduction have been calculated
            lstSalaryProcessDetails = CreateSalaryDetails(processDate,extEmpDetails,lstAlwProcessedObjects,lstDedProcessedObjects,salaryReviewList);
            
            //var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;

            //calculate salary for left over employees of previous month
            var prevMon = processDate.AddMonths(-1);
            var prevSalaryDetails = new List<prl_salary_process_detail>();
            CalculateSalaryPreviousMonthLeftOver(prevMon, ref lstAlwProcessedObjects, ref lstDedProcessedObjects, ref prevSalaryDetails);


            MySqlConnection connection = null;
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            MySqlTransaction tran = null;
            int salaryProcessId = 0;
            //string _batch = BatchNumberGenerator.generateSalaryBatchNumber("SALARY", paymentDate);
            //sp.batch_no = _batch;
            try
            {
                connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
                connection.Open();
                tran = connection.BeginTransaction();

                //1. insert salary process table and get the process id
                var salprocessText = @"INSERT INTO prl_salary_process (batch_no, salary_month, process_date, payment_date, 
	                                    company_id, division_id, department_id, grade_id, gender, is_disbursed, created_by, 
	                                    created_date, updated_by, updated_date)
                                        VALUES	(?batch_no, ?salary_month, ?process_date, ?payment_date, ?company_id, 
	                                    ?division_id, ?department_id, ?grade_id, ?gender, ?is_disbursed, ?created_by, 
	                                    ?created_date, ?updated_by, ?updated_date);";
                command.Connection = connection;
                command.CommandText = salprocessText;
                command.Parameters.AddWithValue("?batch_no", sp.batch_no);
                command.Parameters.AddWithValue("?salary_month", sp.salary_month);
                command.Parameters.AddWithValue("?process_date", sp.process_date);
                command.Parameters.AddWithValue("?payment_date", sp.payment_date);
                command.Parameters.AddWithValue("?company_id", sp.company_id);
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
                
                MySqlTransaction tran2 = null;
                foreach (var spd in lstSalaryProcessDetails)
                {
                    try
                    {
                        tran2 = connection.BeginTransaction();
                        //insert individual salary allowances
                        foreach (var sa in lstAlwProcessedObjects.Where(x=>x.emp_id==spd.emp_id))
                        {
                            command.Parameters.Clear();
                            command.CommandText = @"INSERT INTO prl_salary_allowances (salary_process_id, salary_month, 
	                                            calculation_for_days, emp_id, allowance_name_id, amount, arrear_amount)
                                                VALUES	(?salary_process_id, ?salary_month, ?calculation_for_days, 
	                                            ?emp_id, ?allowance_name_id, ?amount, ?arrear_amount);";
                            command.Parameters.AddWithValue("?salary_process_id", salaryProcessId);
                            command.Parameters.AddWithValue("?salary_month", sa.salary_month.Date);
                            command.Parameters.AddWithValue("?calculation_for_days", sa.calculation_for_days);
                            command.Parameters.AddWithValue("?emp_id", sa.emp_id);
                            command.Parameters.AddWithValue("?allowance_name_id", sa.allowance_name_id);
                            command.Parameters.AddWithValue("?amount", sa.amount);
                            command.Parameters.AddWithValue("?arrear_amount", sa.arrear_amount);
                            command.ExecuteNonQuery();
                            Trace.WriteLine("---individual allowance save emp id = "+sa.emp_id);
                        }


                        //insert individual salary deductions
                        foreach (var de in lstDedProcessedObjects.Where(x => x.emp_id == spd.emp_id))
                        {
                            command.Parameters.Clear();
                            command.CommandText = @"INSERT INTO prl_salary_deductions (salary_process_id, 
	                                        salary_month, calculation_for_days, emp_id, deduction_name_id, 	amount, arrear_amount	)
	                                        VALUES
	                                        (?salary_process_id, ?salary_month, ?calculation_for_days,  ?emp_id, 
	                                        ?deduction_name_id, ?amount,  ?arrear_amount);";
                            command.Parameters.AddWithValue("?salary_process_id", salaryProcessId);
                            command.Parameters.AddWithValue("?salary_month", de.salary_month.Date);
                            command.Parameters.AddWithValue("?calculation_for_days", de.calculation_for_days);
                            command.Parameters.AddWithValue("?emp_id", de.emp_id);
                            command.Parameters.AddWithValue("?deduction_name_id", de.deduction_name_id);
                            command.Parameters.AddWithValue("?amount", de.amount);
                            command.Parameters.AddWithValue("?arrear_amount", de.arrear_amount);
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
                            command.Parameters.AddWithValue("?arrear_calculated_date", processDate.Date);
                            command.ExecuteNonQuery();
                            Trace.WriteLine("---Salary Review Updated for employee id "+spd.emp_id);
                        }

                        if (prevSalaryDetails.Count > 0)
                        {
                            var prevDtl = prevSalaryDetails.Where(x => x.emp_id == spd.emp_id).FirstOrDefault();
                            if (prevDtl != null)
                            {
                                spd.this_month_basic += prevDtl.this_month_basic;
                                spd.total_allowance += prevDtl.total_allowance;
                                spd.totla_arrear_allowance += prevDtl.totla_arrear_allowance;
                                spd.total_deduction += prevDtl.total_deduction;
                                spd.total_arrear_deduction += prevDtl.total_arrear_deduction;
                                spd.total_overtime += prevDtl.total_overtime;
                                spd.total_overtime_arrear += prevDtl.total_overtime_arrear;
                                spd.pf_arrear += prevDtl.pf_arrear;
                                spd.pf_amount += prevDtl.pf_amount;
                                spd.total_monthly_tax += prevDtl.total_monthly_tax;
                                spd.total_bonus += prevDtl.total_bonus;
                                //spd.current_basic += prevDtl.current_basic;

                                prevSalaryDetails.Remove(prevDtl);
                            }
                        }


                        //insert individual salary process detail 
                        var pfArrear = spd.this_month_basic - spd.current_basic;
                        if (pfArrear < 0)
                            pfArrear = 0;
                        command.Parameters.Clear();
                        command.CommandText = @"INSERT INTO prl_salary_process_detail (	salary_process_id, salary_month, emp_id, 
                                        calculation_for_days, current_basic, this_month_basic, total_allowance, 
                                        totla_arrear_allowance, pf_amount, pf_arrear, total_deduction, total_arrear_deduction, 
                                        total_monthly_tax, total_overtime, total_overtime_arrear,  total_bonus, net_pay)
                                        VALUES
                                        ( ?salary_process_id, ?salary_month, ?emp_id, ?calculation_for_days, ?current_basic, 
                                        ?this_month_basic, ?total_allowance, ?totla_arrear_allowance, ?pf_amount, ?pf_arrear, 
                                        ?total_deduction, ?total_arrear_deduction, ?total_monthly_tax, ?total_overtime,  ?total_overtime_arrear, 
                                        ?total_bonus, ?net_pay);";
                        command.Parameters.AddWithValue("?salary_process_id", salaryProcessId);
                        command.Parameters.AddWithValue("?salary_month", spd.salary_month.Date);
                        command.Parameters.AddWithValue("?emp_id", spd.emp_id);
                        command.Parameters.AddWithValue("?calculation_for_days", spd.calculation_for_days);
                        command.Parameters.AddWithValue("?current_basic", spd.current_basic);
                        command.Parameters.AddWithValue("?this_month_basic", spd.this_month_basic);
                        command.Parameters.AddWithValue("?total_allowance", spd.total_allowance);
                        command.Parameters.AddWithValue("?totla_arrear_allowance", spd.totla_arrear_allowance);
                        command.Parameters.AddWithValue("?pf_amount", spd.this_month_basic.Value * (decimal)0.10);
                        command.Parameters.AddWithValue("?pf_arrear", pfArrear);
                        command.Parameters.AddWithValue("?total_deduction", spd.total_deduction);
                        command.Parameters.AddWithValue("?total_arrear_deduction", spd.total_arrear_deduction);
                        command.Parameters.AddWithValue("?total_monthly_tax", 0);
                        command.Parameters.AddWithValue("?total_overtime", spd.total_overtime);
                        command.Parameters.AddWithValue("?total_overtime_arrear", spd.total_overtime_arrear);
                        command.Parameters.AddWithValue("?total_bonus", 0);
                        command.Parameters.AddWithValue("?net_pay", 0);
                        command.ExecuteNonQuery();
                        tran2.Commit();
                        Trace.WriteLine("---individual salary process details save successful emp id = " + spd.emp_id);
                    }
                    catch (Exception  ex)
                    {
                        result.ErrorOccured = true;
                        result.AddToErrorList("Could not process salary for employee "+ spd.prl_employee.emp_no);
                        tran2.Rollback();
                    }
                }

                int taxResult = 0;
                IncomeTaxService ins = new IncomeTaxService(dataContext);
                var _res = ins.process_incomeTax(extEmpDetails.Select(x=>x.prl_employee).ToList(), sp.batch_no, paymentDate, sp.process_date.Value, FindFiscalYear(processDate), "");

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

            return result;
        }

        private List<prl_salary_process_detail> CreateSalaryDetails(DateTime processDate, List<prl_employee_details> extEmpDetails, List<prl_salary_allowances> lstAllowances, List<prl_salary_deductions> lstDeductions, List<prl_salary_review> salaryReviewList)
        {
            var salDetailResult = new List<prl_salary_process_detail>();

            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

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
                var spd= new prl_salary_process_detail();
                spd.emp_id = emp.emp_id;
                spd.prl_employee = emp.prl_employee;
                spd.salary_month = processDate.Date;
                spd.calculation_for_days = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(emp.prl_employee, proStartDate,proEndDate,thisMonthsHold);
                spd.this_month_basic = (emp.basic_salary/daysInMonth)*spd.calculation_for_days;
                spd.current_basic = emp.basic_salary;
                
                #region Calculate Basic Salary Arrear for each employee
                var r = salaryReviewList.AsEnumerable().SingleOrDefault(x => x.emp_id == emp.emp_id);
                if (r != null)
                {
                    spd.current_basic = r.new_basic;
                    spd.this_month_basic = CalculateThisMonthBasic(spd.calculation_for_days, processDate, emp, r);
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

                var overtime = GetOverTimeAmounts(emp.emp_id, processDate.Date);
                spd.total_overtime = overtime.AsEnumerable().Sum(x => x.pay_total);
                Trace.WriteLine("---overtime payment calculated for emp id = "+spd.emp_id);
                spd.total_overtime_arrear = overtime.AsEnumerable().Sum(x => x.arrear_amount);
                Trace.WriteLine("---overtime arrear payment calculated for emp id = " + spd.emp_id);

                salDetailResult.Add(spd);
            }

            return salDetailResult;
        }

        private List<prl_salary_review> GetPrlSalaryReviews(DateTime processDate)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);
            return dataContext.prl_salary_review.AsEnumerable().Where(r => r.is_arrear_calculated == "No" && r.effective_from.Value.Date <= proEndDate).ToList();
        }

        private void CalculateLeaveWithoutPay(ref prl_salary_process_detail spd, DateTime processDate, prl_employee_leave_without_pay leaveWithoutPay,ref List<prl_salary_allowances> thisMonthsAllowanceses)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            int lwpDays = SalaryCalculationHelper.NumberOfDaysToCalculateForLeaveWithoutPay(leaveWithoutPay,proStartDate,proEndDate);

            if (leaveWithoutPay.prl_leave_without_pay_settings.Lwp_type.ToLower() == "basic")
            {
                var t = spd.this_month_basic - (spd.this_month_basic * leaveWithoutPay.prl_leave_without_pay_settings.percentage_of_basic.Value / 100);
                spd.this_month_basic = t / daysInMonth * lwpDays;
            }
            else if (leaveWithoutPay.prl_leave_without_pay_settings.Lwp_type.ToLower() == "gross")
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

            decimal basicSalaryAmount = (empDetails.basic_salary/daysInMonth)*numberOfDaysWorked;
            
            if (salaryReview != null)
            {
                var empSalaryDetails = dataContext.prl_salary_process_detail.AsEnumerable()
                    .Where(x => x.salary_month.Date >= salaryReview.effective_from.Value.Date && x.salary_month.Date < proStartDate)
                    .ToList();
                basicSalaryAmount = salaryReview.new_basic/daysInMonth;

                basicSalaryAmount = basicSalaryAmount + SalaryCalculationHelper.CalculateArrearOnBasic(salaryReview, empSalaryDetails);
            }
            return basicSalaryAmount;
        }

        public int salaryRollbacked(SalaryProcessModel _salProcess, string _UserName, int _fiscalYr, int _month, int _yr)
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

                    var salProcess = cntx.prl_salary_process.FirstOrDefault(x => x.salary_month.Value.Month == _month && x.salary_month.Value.Year == _yr);
                    if (salProcess != null)
                    {
                        var taxProcess = cntx.prl_employee_tax_process.FirstOrDefault(q => q.salary_process_id == salProcess.id);
                        if (taxProcess != null)
                        {
                            var taxDetail = cntx.prl_employee_tax_process_detail.Where(t => t.tax_process_id == taxProcess.id).ToList();
                            if (taxDetail != null)
                            {
                                DeleteEmployeeTaxDetail(salProcess.id, mySqlCommand);
                            }
                            var taxSlab = cntx.prl_employee_tax_slab.Where(s => s.tax_process_id == taxProcess.id).ToList();
                            if (taxSlab != null)
                            {
                                DeleteEmployeeTaxSlab(salProcess.id, mySqlCommand);
                            }
                            DeleteEmployeeTaxProcess(salProcess.id, mySqlCommand);
                        }
                        var salAllowance = cntx.prl_salary_allowances.Where(a => a.salary_process_id == salProcess.id).ToList();
                        if (salAllowance != null)
                        {
                            DeleteSalaryAllowance(salProcess.id, mySqlCommand);
                        }
                        var salDeduction = cntx.prl_salary_deductions.Where(d => d.salary_process_id == salProcess.id).ToList();
                        if (salDeduction != null)
                        {
                            DeleteSalaryDeduction(salProcess.id, mySqlCommand);
                        }
                        var salDetail = cntx.prl_salary_process_detail.Where(s => s.salary_process_id == salProcess.id).ToList();
                        if (salDetail != null)
                        {
                            DeleteSalaryDetail(salProcess.id, mySqlCommand);
                        }
                        DeleteSalaryMaster(salProcess.id, mySqlCommand);

                        var salReview = cntx.prl_salary_review.Where(r => r.arrear_calculated_date.Value.Month == _month && r.arrear_calculated_date.Value.Year == _yr).ToList();
                        if (salReview != null)
                        {
                            UpdateSalaryReview(_month, _yr, "", DateTime.Now, mySqlCommand);
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

        public int DeleteEmployeeTaxSlab(int salary_process_id, MySqlCommand command)
        {
            int taxSlab = 0;
            try
            {
                const string commandText = @"DELETE FROM prl_employee_tax_slab WHERE salary_process_id = ?salary_process_id";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?salary_process_id", salary_process_id);
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

        private List<prl_over_time_amount> GetOverTimeAmounts(int employeeId,DateTime processMonthYear)
        {
           return dataContext.prl_over_time_amount.AsEnumerable().Where(x => x.emp_id == employeeId && 
               x.month_year.ToString("yyyy-MM") == processMonthYear.ToString("yyyy-MM")).ToList();
        }

        private int FindFiscalYear(DateTime processDate)
        {
            int fiscalyR = 0;
            int _month = processDate.Month;
            int _yr = processDate.Year;
            string curYear = _yr.ToString() + "-" + (_yr + 1).ToString();
            string prevYear = (_yr - 1).ToString() + "-" + _yr.ToString();
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

        private bool IsSalaryAlreadyProcessed(DateTime date,string processType,int? companyId, int? divisionId, int? departmentId,int? grade , List<int> selectedEmployee=null)
        {

            if (processType == "all")
            {
              var b =  dataContext.prl_salary_process.AsEnumerable()
                    .Any(x =>x.salary_month.Value.ToString("yyyy-MM") == date.ToString("yyyy-MM") && x.grade_id.Value == 0 && departmentId.Value == 0 && divisionId.Value == 0);
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
                    .Any(x => x.salary_month.Value.ToString("yyyy-MM") == date.ToString("yyyy-MM") && departmentId.Value == 0);
                if (b == true)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Already processed for current settings.");
                }
                return b;
            }
            else if (processType == "division")
            {
                var b = dataContext.prl_salary_process.AsEnumerable()
                    .Any(x => x.salary_month.Value.ToString("yyyy-MM") == date.ToString("yyyy-MM") && divisionId.Value == 0);
                if (b == true)
                {
                    result.ErrorOccured = true;
                    result.AddToErrorList("Already processed for current settings.");
                }
                return b;
            }
            else if (processType == "grade")
            {
                var b = dataContext.prl_salary_process.AsEnumerable()
                    .Any(x => x.salary_month.Value.ToString("yyyy-MM") == date.ToString("yyyy-MM") && grade.Value == 0);
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
                   .Join(contxt.prl_employee_details.Include("prl_employee").AsEnumerable(),ok=>ok.id,ik=>ik.emp_id,(ok,ik)=> ik)
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

        public List<int> GetSalaryAlreadyProcessedEmployees(DateTime monthYear)
        {
            using (var contxt = new payroll_systemContext())
            {
                return contxt.prl_salary_process_detail.AsEnumerable()
                    .Where(x => x.salary_month.ToString("yyyy-MM") == monthYear.ToString("yyyy-MM"))
                    .Select(x => x.emp_id).ToList();
            }
        }

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
            var salaryReviewListPrev = GetPrlSalaryReviews(proEndDatePrev.Date);

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
    }
}