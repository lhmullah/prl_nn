using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Misc;
using com.gpit.DataContext;
using com.gpit.Model;

namespace PayrollWeb.Models
{
    public class SalaryAllowanceProcess : ISalaryProcess
    {
        private DateTime processMonthYear;
        
        private List<prl_salary_allowances> lstSalAl;
        private DateTime proStartDate;
        private int daysInMonth;
        private DateTime proEndDate;
        private List<prl_salary_hold> thisMonthsHold;
        private payroll_systemContext contxt;
        private List<prl_allowance_configuration> gradeAndGenderBased;
        private IProcessResult result;
        private List<prl_employee_children_allowance> eligibleForChildrenAllowances;
        private List<prl_employee_details> extEmpDetails;

        public SalaryAllowanceProcess(DateTime processMonthYear, DateTime processMonthYearFromDate, DateTime processMonthYearEndDate, List<prl_employee_details> lstEmployeeSalaryToProcess)
        {
            this.processMonthYear = processMonthYear;
            lstSalAl = new List<prl_salary_allowances>();
            thisMonthsHold = new ListStack<prl_salary_hold>();
            gradeAndGenderBased = new List<prl_allowance_configuration>();
            result = new AllowanceProcessResult(ProcessType.ALLOWANCE);
            eligibleForChildrenAllowances = new List<prl_employee_children_allowance>();
            extEmpDetails = lstEmployeeSalaryToProcess;
            proStartDate = processMonthYearFromDate;
            proEndDate = processMonthYearEndDate;
        }
        public IProcessResult Process()
        {
            using (contxt = new payroll_systemContext())
            {
                daysInMonth = DateTime.DaysInMonth(proStartDate.Year, proStartDate.Month);

                thisMonthsHold = contxt.prl_salary_hold.AsEnumerable().Where(x => x.is_holded =="Y" && 
                        x.hold_from.Value.ToString("yyyy-MM")==processMonthYear.ToString("yyyy-MM") &&
                        x.with_salary == Convert.ToSByte(true)).ToList();

                eligibleForChildrenAllowances = extEmpDetails.AsEnumerable().
                    Join(contxt.prl_employee_children_allowance.Include("prl_employee"), ouk => ouk.emp_id, ink => ink.emp_id, (ouk, ink) => ink).
                    Where(x => x.is_active == Convert.ToSByte(true)).ToList();

                //2. read from all general configuration 
                gradeAndGenderBased = contxt.prl_allowance_configuration.Include("prl_allowance_name").AsEnumerable().
                    Where(x => x.is_active == Convert.ToSByte(true) && x.is_individual==Convert.ToSByte(false) &&
                    (x.deactivation_date == null || (x.activation_date.Value.Date <= proStartDate && (x.deactivation_date.Value.Date >= proEndDate || x.deactivation_date.Value.Date <= proEndDate)) ||
                    (x.activation_date.Value.Date >= proStartDate && (x.deactivation_date.Value.Date >= proEndDate || x.deactivation_date.Value.Date >= proEndDate))) &&
                    !x.prl_allowance_name.allowance_name.ToLower().Contains("children")).ToList();

                //1.calculate gendar and grade wise
                CalculateGeneralTypeAllowances(gradeAndGenderBased);
                //2.calculate individual allowance
                CalculateIndividualAllowance();
                //3.calculate children allowance
                CalculateChildrenAllowance(eligibleForChildrenAllowances);
                //4.calculate uploaded allowance
                CalculateUploadedAllowance();
            }
            result.AddCompletedResultObjects(lstSalAl);
            return result;
        }
        private void CalculateGeneralTypeAllowances(List<prl_allowance_configuration> lstAllowanceConfigurations)
        {
            foreach (var alwConf in lstAllowanceConfigurations)
            {
                try
                {
                   var lstOfGrades = contxt.prl_allowance_name.Include("prl_grade").SingleOrDefault(g => g.id == alwConf.allowance_name_id).prl_grade.ToList();

                    foreach (var x in extEmpDetails)
                    {
                        try
                        {
                            int daysWorked = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(x.prl_employee, proStartDate,proEndDate, thisMonthsHold);
                            var sr = contxt.prl_salary_review.AsEnumerable().SingleOrDefault(r => r.emp_id == x.emp_id && r.is_arrear_calculated.ToLower() == "no" && r.effective_from.Value.Date <= proEndDate);
                            var sal = SalaryCalculationHelper.CalculateAllowance(daysWorked, proStartDate,proEndDate, x.prl_employee, x, alwConf, lstOfGrades, sr);
                            if (sal != null)
                            {
                                lstSalAl.Add(sal);
                            }
                        }
                        catch (Exception exception)
                        {
                            result.AddToErrorList("Could not calculate allowance "+alwConf.prl_allowance_name.allowance_name +" for employee "+x.prl_employee.emp_no);
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private void CalculateIndividualAllowance()
        {
            foreach (var emp in extEmpDetails)
            {
                var lst = contxt.prl_employee_individual_allowance.Include("prl_allowance_name").AsEnumerable().Where(x => x.emp_id == emp.emp_id && x.effective_from.Value.Date <= proEndDate && x.effective_to.Value.Date >= proStartDate).ToList();
                int days = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(emp.prl_employee,proStartDate,proEndDate,thisMonthsHold);
                var sr = contxt.prl_salary_review.AsEnumerable().SingleOrDefault(r => r.emp_id == emp.id && r.is_arrear_calculated == "No" && r.effective_from.Value.Date <= proEndDate);
                foreach (var ial in lst)
                {
                    try
                    {
                        var sa = SalaryCalculationHelper.CalculateIndividualAllowance(days, emp.prl_employee, proStartDate,proEndDate, emp, ial, sr);
                        if (sa != null)
                        {
                            lstSalAl.Add(sa);
                        }
                    }
                    catch (Exception exception)
                    {
                        result.AddToErrorList("Could not calculate individual allowance "+ ial.prl_allowance_name.allowance_name+" for employee "+emp.prl_employee.emp_no);
                    }
                }
            }
                
        }

        private void CalculateChildrenAllowance(List<prl_employee_children_allowance> lstChildrenAllowances)
        {
            foreach (var x in lstChildrenAllowances)
            {
                try
                {
                    var emp = extEmpDetails.AsEnumerable().SingleOrDefault(e => e.emp_id == x.emp_id);
                    int days = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(emp.prl_employee, proStartDate,proEndDate, thisMonthsHold);
                    var sa = SalaryCalculationHelper.CalculateChildrenAllowance(days, proStartDate,proEndDate, emp.prl_employee, x);
                    if (sa != null)
                    {
                        lstSalAl.Add(sa);
                    }
                }
                catch (Exception ex)
                {
                    result.AddToErrorList("Could not calculate children allowance for employee "+x.prl_employee.emp_no);
                }
            }
        }

        private void CalculateUploadedAllowance()
        {
            var lst = contxt.prl_upload_allowance.Include("prl_employee").AsEnumerable()
                .Where(x => x.salary_month_year.Value.Date >= proStartDate && x.salary_month_year.Value.Date <= proEndDate
                        && extEmpDetails.Select(y=>y.emp_id).ToList().Contains(x.emp_id))
                .ToList();
            foreach (var prlUploadAllowance in lst)
            {
                var sa = new prl_salary_allowances();
                try
                {
                    sa.emp_id = prlUploadAllowance.emp_id;
                    sa.amount = prlUploadAllowance.amount.Value;
                    sa.arrear_amount = 0;
                    sa.allowance_name_id = prlUploadAllowance.allowance_name_id;
                    sa.salary_month = prlUploadAllowance.salary_month_year.Value.Date;
                    sa.calculation_for_days = daysInMonth;
                    lstSalAl.Add(sa);
                }
                catch (Exception ex)
                {
                    result.AddToErrorList("Could not calculate uploaded allowance for employee "+prlUploadAllowance.prl_employee.emp_no);
                }
            }
        }

        public List<prl_employee_details> GetEmployeeList()
        {
            return extEmpDetails;
        }
    }
}