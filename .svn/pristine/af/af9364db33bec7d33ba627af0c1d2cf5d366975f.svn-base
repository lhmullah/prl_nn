using System;
using System.Collections.Generic;
using System.Linq;
using com.gpit.DataContext;
using com.gpit.Model;

namespace PayrollWeb.Models
{
    public class SalaryDeductionProcess : ISalaryProcess
    {
        private DateTime processMonthYear;
        
        private List<prl_salary_deductions> lstSalAl;
        private DateTime proStartDate;
        private int daysInMonth;
        private DateTime proEndDate;
        private List<prl_salary_hold> thisMonthsHold;
        private payroll_systemContext contxt = null;
        private List<prl_deduction_configuration> gradeAndGenderBased;
        private IProcessResult result;
        private List<prl_employee_details> extEmpDetails;

        public SalaryDeductionProcess(DateTime processMonthYear,DateTime processMonthYearStartDate,DateTime processMonthYearEndDate, List<prl_employee_details> lstEmployeeSalaryToProcess)
        {
            this.processMonthYear = processMonthYear;
            lstSalAl = new List<prl_salary_deductions>();
            thisMonthsHold = new List<prl_salary_hold>();
            gradeAndGenderBased = new List<prl_deduction_configuration>();
            result = new DeductionProcessResult(ProcessType.DEDUCTION);
            extEmpDetails = lstEmployeeSalaryToProcess;
            proStartDate = processMonthYearStartDate;
            proEndDate = processMonthYearEndDate;
        }
        public IProcessResult Process()
        {
            using (contxt = new payroll_systemContext())
            {
                proStartDate = new DateTime(processMonthYear.Year, processMonthYear.Month, 1);
                daysInMonth = DateTime.DaysInMonth(proStartDate.Year, proStartDate.Month);
                proEndDate = new DateTime(processMonthYear.Year, processMonthYear.Month, daysInMonth);

                //1. All employees eligible for this months 
                //2. Need To chk HOLD Employee LIst
                //3. joining date <= process date
                thisMonthsHold = contxt.prl_salary_hold.AsEnumerable().
                    Where(x => x.is_holded =="Y" && x.hold_from.Value.ToString("yyyy-MM")==processMonthYear.ToString("yyyy-MM") &&
                        x.with_salary == Convert.ToSByte(true)).ToList();

                //2. read from all general configuration 
                gradeAndGenderBased = contxt.prl_deduction_configuration.Include("prl_deduction_name").AsEnumerable().
                    Where(x => x.is_active == Convert.ToSByte(true) && x.is_individual==Convert.ToSByte(false) &&
                    (x.deactivation_date == null || (x.activation_date.Value.Date <= proStartDate && (x.deactivation_date.Value.Date >= proEndDate || x.deactivation_date.Value.Date <= proEndDate)) ||
                    (x.activation_date.Value.Date >= proStartDate && (x.deactivation_date.Value.Date >= proEndDate || x.deactivation_date.Value.Date >= proEndDate)))).ToList();


                //calculate gender and grade based deductions

                CalculateGeneralTypeDedutions(gradeAndGenderBased);
                //calculate individual deductions
                CalculateIndividualDeduction();
                //calculate uploaded deductions
                CalculateUploadedDeduction(extEmpDetails.Select(x=>x.emp_id).ToList());
            }

           
            result.AddCompletedResultObjects(lstSalAl);
            return result;
        }
        private void CalculateGeneralTypeDedutions(List<prl_deduction_configuration> lstConfigurations)
        {
            foreach (var alwConf in lstConfigurations)
            {
                try
                {
                    var lstOfGrades = contxt.prl_deduction_name.Include("prl_grade").SingleOrDefault(g => g.id == alwConf.deduction_name_id).prl_grade.ToList();

                    foreach (var x in extEmpDetails)
                    {
                        try
                        {
                            int daysWorked = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(x.prl_employee, proStartDate,proEndDate, thisMonthsHold);
                            var sr = contxt.prl_salary_review.AsEnumerable().SingleOrDefault(r => r.emp_id == x.emp_id && r.is_arrear_calculated.ToLower() == "no" && r.effective_from.Value.Date <= proEndDate);
                            var sal = SalaryCalculationHelper.CalculateDeduction(daysWorked, proStartDate,proEndDate, x.prl_employee, x, alwConf, lstOfGrades, sr);
                            if (sal != null)
                            {
                                lstSalAl.Add(sal);
                            }
                        }
                        catch (Exception exception)
                        {
                            result.AddToErrorList("Could not calculate deduction "+alwConf.prl_deduction_name.deduction_name+" for employee "+ x.prl_employee.emp_no);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        private void CalculateIndividualDeduction()
        {
            foreach (var emp in extEmpDetails)
            {
                var lst = contxt.prl_employee_individual_deduction.Include("prl_deduction_name").AsEnumerable()
                .Where(x => x.emp_id == emp.emp_id && x.effective_from.Value.Date <= proEndDate && x.effective_to.Value.Date >= proStartDate)
                .ToList();
                
                int days = SalaryCalculationHelper.NumberOfDaysWorkedBasedOnEmployeeStatus(emp.prl_employee, proStartDate,proEndDate, thisMonthsHold);
                var sr = contxt.prl_salary_review.AsEnumerable().SingleOrDefault(r => r.emp_id == emp.emp_id && r.is_arrear_calculated == "No" && r.effective_from.Value.Date <= proEndDate);

                foreach (var ial in lst)
                {
                    try
                    {
                        var sa = SalaryCalculationHelper.CalculateIndividualDeduction(days, emp.prl_employee, proStartDate,proEndDate, emp, ial, sr);
                        if (sa != null)
                        {
                            lstSalAl.Add(sa);
                        }
                    }
                    catch (Exception exception)
                    {
                        result.AddToErrorList("Could not calculate individual deduction for employee "+emp.prl_employee.emp_no);
                    }
                }
            }

        }
        private void CalculateUploadedDeduction(List<int> empIds)
        {
            var lst = contxt.prl_upload_deduction.Include("prl_employee").AsEnumerable()
                    .Where(x => x.salary_month_year.Value.Date >= proStartDate && x.salary_month_year.Value.Date <= proEndDate
                     && extEmpDetails.Select(y=>y.emp_id).ToList().Contains(x.emp_id))
                    .ToList();
            foreach (var d in lst.Where(x => empIds.Contains(x.emp_id)))
            {
                var sa = new prl_salary_deductions();
                try
                {
                    sa.emp_id = d.emp_id;
                    sa.amount = d.amount;
                    sa.arrear_amount = 0;
                    sa.deduction_name_id = d.deduction_name_id;
                    sa.salary_month = d.salary_month_year.Value.Date;
                    sa.calculation_for_days = daysInMonth;
                    lstSalAl.Add(sa);
                }
                catch (Exception ex)
                {
                    result.AddToErrorList("Could not calculate uploaded deduction for employee "+d.prl_employee.emp_no);
                }
            }
        }
        public List<prl_employee_details> GetEmployeeList()
        {
            return extEmpDetails;
        }
    }
}