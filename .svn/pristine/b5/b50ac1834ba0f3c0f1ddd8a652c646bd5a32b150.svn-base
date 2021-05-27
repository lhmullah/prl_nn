using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using com.gpit.Model;
using PayrollWeb.Utility;

namespace PayrollWeb.Models
{
    public class SalaryCalculationHelper
    {
        public static int NumberOfDaysWorkedBasedOnEmployeeStatus(prl_employee e, DateTime proStartDate, DateTime proEndDate, List<prl_salary_hold> thisMonthsHold)
        {
            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;
           
            int calDate = 0;

            if (e.joining_date.Date < proStartDate.Date)
            {
                calDate = daysInMonth;
            }

            if (CommonDateClass.DayMonthYearIsInRange(e.joining_date, proStartDate, proEndDate))
            {
                calDate = proEndDate.Subtract(e.joining_date).Days + 1;
            }
            // Check this condition by Mahbub Vai
            if (e.termination_date != null)
            {
                calDate = e.termination_date.Value.Subtract(proStartDate).Days + 1;
            }

            //check if employee's salary is holded
            if (thisMonthsHold.Exists(h => h.emp_id == e.id))
            {
                calDate = thisMonthsHold.SingleOrDefault(l => l.emp_id == e.id).hold_from.Value.Subtract(proEndDate).Days + 1;
            }

            return calDate;
        }
        public static int NumberOfDaysToCalculateForLeaveWithoutPay(prl_employee_leave_without_pay elwp, DateTime proStartDate, DateTime proEndDate)
        {

            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;
            
            int calDate = 0;

            //starts in this month lets say we r processing March salary
            if (elwp.strat_date >= proStartDate && elwp.end_date.Value.Date <= proEndDate)
            {
                calDate = elwp.end_date.Value.Subtract(elwp.strat_date.Value).Days + 1;
            }
            else if (elwp.strat_date >= proStartDate && elwp.end_date.Value.Date > proEndDate)
            {
                calDate = proEndDate.Subtract(elwp.strat_date.Value).Days + 1;
            }
            else if (elwp.strat_date < proStartDate && elwp.end_date.Value.Date <= proEndDate)
            {
                calDate = elwp.end_date.Value.Subtract(proStartDate).Days + 1;
            }
            else
            {
                calDate = daysInMonth;
            }

            return calDate;
        }
        public static int NumberOfDaysForAllowanceCalculation(prl_allowance_configuration conf, DateTime proStartDate, DateTime proEndDate)
        {
            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;

            int returnDays = 0;
            //assume we are processing march 25th month salary allowance activated in february 26 then return emp working day val
            if (conf.activation_date.Value.Date < proStartDate && (conf.deactivation_date == null || conf.deactivation_date.Value.Date >= proEndDate))
                return daysInMonth;

            //assume we are processing march 25th month salary allowance activated in february 26 and 
            //the allowance is deactivated before 31st of March then return emp working day val
            else if (conf.activation_date.Value.Date < proStartDate && (conf.deactivation_date.Value.Date < proEndDate))
            {
                returnDays = conf.deactivation_date.Value.Date.Subtract(proStartDate.Date).Days + 1;
            }
            //assume activation on March 1
            else if (conf.activation_date.Value.Date >= proStartDate && conf.activation_date.Value <= proEndDate.Date && (conf.deactivation_date == null || conf.deactivation_date.Value.Date >= proEndDate))
            {
                returnDays = proEndDate.Date.Subtract(conf.activation_date.Value.Date).Days + 1;
            }
            //assume activation on March 1 and ends before March 31
            //else
            //{
            //    returnDays = conf.deactivation_date.Value.Date.Subtract(conf.activation_date.Value.Date).Days + 1;
            //}
            return returnDays;
        }
        public static int NumberOfDaysForIndividualAllowance(prl_employee_individual_allowance conf, DateTime proStartDate, DateTime proEndDate)
        {
            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;
            
            int returnDays = 0;
            //assume we are processing march 25th month salary allowance activated in february 26 then return emp working day val
            if (conf.effective_from.Value.Date < proStartDate && (conf.effective_to == null || conf.effective_from.Value.Date >= proEndDate))
                return daysInMonth;
            //assume we are processing march 25th month salary allowance activated in february 26 and 
            //the allowance is deactivated before 31st of March then return emp working day val
            else if (conf.effective_to.Value.Date < proStartDate && (conf.effective_from.Value.Date < proEndDate))
            {
                returnDays = conf.effective_from.Value.Date.Subtract(proStartDate.Date).Days + 1;
            }
            //assume activation on March 1
            else if (conf.effective_to.Value.Date >= proStartDate && (conf.effective_from == null || conf.effective_from.Value.Date >= proEndDate))
            {
                returnDays = proEndDate.Date.Subtract(conf.effective_to.Value.Date).Days + 1;
            }
            //assume activation on March 1 and ends before March 31
            else
            {
                returnDays = conf.effective_from.Value.Date.Subtract(conf.effective_to.Value.Date).Days + 1;
            }

            return returnDays;
        }
        public static decimal CalculateArrearOnBasic(prl_salary_review sr, List<prl_salary_process_detail> listSalaryProcessDetails)
        {
            var adjAmountBasic = sr.new_basic - sr.current_basic;
            decimal adjAmt = 0;

            foreach (var sal in listSalaryProcessDetails)
            {
                if (sal.salary_month.ToString("yyyy-MM") == sr.effective_from.Value.ToString("yyyy-MM"))
                {
                    var dayinmon = DateTime.DaysInMonth(sal.salary_month.Year, sal.salary_month.Month);
                    var endofmonth = new DateTime(sal.salary_month.Year, sal.salary_month.Month, dayinmon);
                    var adjDays = endofmonth.Subtract(sr.effective_from.Value.Date).Days;

                    if (sal.calculation_for_days >= adjDays)
                    {
                        adjAmt = adjAmt + (adjAmountBasic/dayinmon*adjDays);
                    }
                    else
                    {
                        adjAmt = adjAmt + (adjAmountBasic/dayinmon*sal.calculation_for_days);
                    }
                }
            }
            return adjAmt;
        }
        public static prl_salary_allowances CalculateAllowance(int numberOfDays, DateTime processStartDate, DateTime proEndDate,
            prl_employee emp,prl_employee_details lastestDetails, prl_allowance_configuration conf,
            List<prl_grade> lstGrades,prl_salary_review salaryReview = null)
        {

            var lstGradeIds = lstGrades.AsEnumerable().Select(x => x.id);
            if (!lstGradeIds.Contains(lastestDetails.grade_id))
                return null;
            
            if(conf.gender.ToLower() != "regardless" && emp.gender.ToLower() != conf.gender.ToLower())
                return null;

            var daysInMonth = DateTime.DaysInMonth(proEndDate.Year, proEndDate.Month); //proEndDate.Date.Subtract(processStartDate.Date).Days + 1;

            decimal actualBasic = lastestDetails.basic_salary;

            if (salaryReview != null)
            {
                if (salaryReview.emp_id == emp.id && salaryReview.is_arrear_calculated == "No" && salaryReview.effective_from.Value.Date <= proEndDate)
                {
                    actualBasic = salaryReview.new_basic;
                }
            }
            int actualDate = NumberOfDaysForAllowanceCalculation(conf, processStartDate,proEndDate);

            if (numberOfDays <= actualDate)
                actualDate = numberOfDays;

            var sa = new prl_salary_allowances();
            sa.salary_month = proEndDate.Date;
            sa.emp_id = emp.id;
            sa.allowance_name_id = conf.allowance_name_id;
            sa.calculation_for_days = actualDate;
            if (conf.flat_amount != null)
            {
                sa.amount = conf.flat_amount.Value/daysInMonth*actualDate;
            }
            else
            {
                sa.amount = ((actualBasic*(conf.percent_amount.Value/100))/daysInMonth)*actualDate;
            }

            if (salaryReview != null)
            {
                var adjustment = salaryReview.new_basic - salaryReview.current_basic;
                decimal totalArrearAmount = 0;
                int arrCalDate = 0;
                DateTime srMonthStartDate = new DateTime(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month, 1);
                DateTime srMonthEndDate = new DateTime(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month, DateTime.DaysInMonth(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month));
                int srDays = srMonthEndDate.Subtract(salaryReview.effective_from.Value.Date).Days + 1;
                if (CommonDateClass.DayMonthYearIsInRange(sa.salary_month.Date, srMonthStartDate, srMonthEndDate))
                {
                    if (sa.calculation_for_days > srDays)
                        arrCalDate = srDays;
                    arrCalDate = sa.calculation_for_days;
                }

                if (sa.salary_month.Date > srMonthEndDate.Date)
                    arrCalDate = sa.calculation_for_days;
                
                sa.arrear_amount = totalArrearAmount + (adjustment/daysInMonth*arrCalDate);
            }

            return sa;
        }
        public static prl_salary_allowances CalculateIndividualAllowance(int numberOfDays, prl_employee emp,DateTime proStartDate,DateTime proEndDate,
            prl_employee_details lastestDetails, prl_employee_individual_allowance indv, prl_salary_review salaryReview = null)
        {
            var daysInMonth = DateTime.DaysInMonth(proEndDate.Year, proEndDate.Month);//proEndDate.Date.Subtract(proStartDate.Date).Days + 1;
            
            decimal basic = 0;

            if (salaryReview != null)
                basic = salaryReview.new_basic;
            else
                basic = lastestDetails.basic_salary;
            
            var sa = new prl_salary_allowances();
            sa.emp_id = emp.id;
            sa.salary_month = proEndDate.Date;
            sa.allowance_name_id = indv.allowance_name_id;

            int actualDays = SalaryCalculationHelper.NumberOfDaysForIndividualAllowance(indv, proStartDate,proEndDate);
            if (actualDays >= numberOfDays)
                actualDays = numberOfDays;

            if (indv.flat_amount > 0)
            {
                sa.amount = indv.flat_amount.Value / actualDays;
            }
            else
            {
                sa.amount = ((basic * (indv.percentage.Value / 100)) / daysInMonth) * actualDays;
                sa.calculation_for_days = actualDays;
            }
            if (salaryReview != null)
            {
                var adjustment = salaryReview.new_basic - salaryReview.current_basic;

                decimal totalArrearAmount = 0;
                
                var arrcalDate = NumberOfDaysForIndividualAllowance(indv, proStartDate,proEndDate);
                if (arrcalDate > numberOfDays)
                    arrcalDate = numberOfDays;
                sa.arrear_amount = totalArrearAmount + (adjustment / daysInMonth * arrcalDate);
            }
            return sa;
        }
        public static int CalDaysBasedOnChildrenAllowanceConf(int numberOfDays,DateTime proStartDate,DateTime proEndDate,prl_employee emp,prl_employee_children_allowance childrenAllowance)
        {
            int returnDays = 0;
            //assume we are processing march 25th month salary allowance activated in february 26 then return emp working day val
            if (childrenAllowance.effective_from.Value.Date <= proStartDate)
                return numberOfDays;
            //assume activation on March 1
            if (childrenAllowance.effective_from.Value.Date > proStartDate && (childrenAllowance.effective_from.Value.Date <= proEndDate))
            {
                returnDays = proEndDate.Date.Subtract(childrenAllowance.effective_from.Value.Date).Days + 1;
            }
            return returnDays;
        }
        public static prl_salary_allowances CalculateChildrenAllowance(int numberOfDays, DateTime proStartDate, DateTime proEndDate, prl_employee emp, prl_employee_children_allowance prlEmployeeChildrenAllowance)
        {
            var sa = new prl_salary_allowances();
            sa.allowance_name_id = prlEmployeeChildrenAllowance.id;
            sa.salary_month = proEndDate.Date;
            sa.calculation_for_days = CalDaysBasedOnChildrenAllowanceConf(numberOfDays,proStartDate,proEndDate,emp,prlEmployeeChildrenAllowance);
            sa.amount = prlEmployeeChildrenAllowance.amount / sa.calculation_for_days;
            return sa;
        }
        public static  List<prl_salary_allowances> CalculateUploadedAllowance(DateTime processDate,prl_employee emp,List<prl_upload_allowance> lst)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            var lstSalAl = new List<prl_salary_allowances>();

            //foreach (var v in lst)
            //{
            //    var sa = new prl_salary_allowances();
            //    sa.emp_id = emp.id;
            //    sa.salary_month = processDate.Date;
            //    sa.allowance_name_id = v.allowance_name_id;
            //    sa.calculation_for_days = daysInMonth;
            //    sa.amount = v.amount;
            //    sa.arrear_amount = 0;
            //    lstSalAl.Add(sa);
            //}
            return lstSalAl;
        }
        public static prl_salary_deductions CalculateDeduction(int numberOfDays, DateTime proStartDate,DateTime proEndDate,
           prl_employee emp, prl_employee_details lastestDetails, prl_deduction_configuration conf,
           List<prl_grade> lstGrades, prl_salary_review salaryReview = null)
        {

            var lstGradeIds = lstGrades.AsEnumerable().Select(x => x.id);
            if (!lstGradeIds.Contains(lastestDetails.grade_id))
                return null;

            if (conf.gender.ToLower() != "regardless" && emp.gender.ToLower() != conf.gender.ToLower())
                return null;

            var daysInMonth = DateTime.DaysInMonth(proEndDate.Year, proEndDate.Month);//proEndDate.Date.Subtract(proStartDate.Date).Days + 1;

            decimal actualBasic = lastestDetails.basic_salary;

            if (salaryReview != null)
            {
                if (salaryReview.emp_id == emp.id && salaryReview.is_arrear_calculated == "No" &&
                    salaryReview.effective_from.Value.Date <= proEndDate)
                {
                    actualBasic = salaryReview.new_basic;
                }
            }
            int actualDate = NumberOfDaysForDeductionCalculation(conf, proStartDate,proEndDate);

            if (numberOfDays <= actualDate)
                actualDate = numberOfDays;

            var sa = new prl_salary_deductions();
            sa.emp_id = emp.id;
            sa.salary_month = proEndDate.Date;
            sa.deduction_name_id = conf.deduction_name_id;
            sa.calculation_for_days = actualDate;
            if (conf.flat_amount != null)
            {
                sa.amount = conf.flat_amount.Value / daysInMonth * actualDate;
            }
            else
            {
                sa.amount = ((actualBasic * (conf.percent_amount.Value / 100)) / daysInMonth) * actualDate;
            }

            if (salaryReview != null)
            {
                var adjustment = salaryReview.new_basic - salaryReview.current_basic;
                decimal totalArrearAmount = 0;
                int arrCalDate = 0;
                DateTime srMonthStartDate = new DateTime(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month, 1);
                DateTime srMonthEndDate = new DateTime(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month, DateTime.DaysInMonth(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month));
                int srDays = srMonthEndDate.Subtract(salaryReview.effective_from.Value.Date).Days + 1;
                if (CommonDateClass.DayMonthYearIsInRange(sa.salary_month.Date, srMonthStartDate, srMonthEndDate))
                {
                    if (sa.calculation_for_days > srDays)
                        arrCalDate = srDays;
                    arrCalDate = sa.calculation_for_days;
                }

                if (sa.salary_month.Date > srMonthEndDate.Date)
                    arrCalDate = sa.calculation_for_days;

                sa.arrear_amount = totalArrearAmount + (adjustment / daysInMonth * arrCalDate);
            }

            return sa;
        }
        public static prl_salary_deductions CalculateIndividualDeduction(int numberOfDays, prl_employee emp, DateTime proStartDate,DateTime proEndDate,
            prl_employee_details lastestDetails, prl_employee_individual_deduction indv, prl_salary_review salaryReview = null)
        {

            var daysInMonth = DateTime.DaysInMonth(proEndDate.Year,proEndDate.Month);//proEndDate.Date.Subtract(proStartDate.Date).Days + 1;

            decimal basic = 0;

            if (salaryReview != null)
                basic = salaryReview.new_basic;
            else
                basic = lastestDetails.basic_salary;

            var sa = new prl_salary_deductions();
            sa.emp_id = emp.id;
            sa.salary_month = proEndDate.Date;
            sa.deduction_name_id = indv.deduction_name_id;

            int actualDays = SalaryCalculationHelper.NumberOfDaysForIndividualDeductionCalculation(indv, proStartDate,proEndDate);
            if (actualDays >= numberOfDays)
                actualDays = numberOfDays;

            if (indv.flat_amount > 0)
            {
                sa.amount = indv.flat_amount.Value / actualDays;
            }
            else
            {
                sa.amount = ((basic * (indv.percentage.Value / 100)) / daysInMonth) * actualDays;
                sa.calculation_for_days = actualDays;

            }
            if (salaryReview != null)
            {
                var adjustment = salaryReview.new_basic - salaryReview.current_basic;

                decimal totalArrearAmount = 0;

                var arrcalDate = NumberOfDaysForIndividualDeductionCalculation(indv, proStartDate,proEndDate);
                if (arrcalDate > numberOfDays)
                    arrcalDate = numberOfDays;
                sa.arrear_amount = totalArrearAmount + (adjustment / daysInMonth * arrcalDate);
            }
            return sa;
        }
        public static int NumberOfDaysForDeductionCalculation(prl_deduction_configuration conf, DateTime proStartDate, DateTime proEndDate)
        {
            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;

            int returnDays = 0;
            //assume we are processing march 25th month salary allowance activated in february 26 then return emp working day val
            if (conf.activation_date.Value.Date < proStartDate && (conf.deactivation_date == null || conf.deactivation_date.Value.Date >= proEndDate))
                return daysInMonth;
            //assume we are processing march 25th month salary allowance activated in february 26 and 
            //the allowance is deactivated before 31st of March then return emp working day val
            else if (conf.activation_date.Value.Date < proStartDate && (conf.deactivation_date.Value.Date < proEndDate))
            {
                returnDays = conf.deactivation_date.Value.Date.Subtract(proStartDate.Date).Days + 1;
            }
            //assume activation on March 1
            else if (conf.activation_date.Value.Date >= proStartDate && conf.activation_date.Value <=proEndDate.Date && (conf.deactivation_date == null || conf.deactivation_date.Value.Date >= proEndDate))
            {
                returnDays = proEndDate.Date.Subtract(conf.activation_date.Value.Date).Days + 1;
            }
            //assume activation on March 1 and ends before March 31
            //else
            //{
            //    returnDays = conf.deactivation_date.Value.Date.Subtract(conf.activation_date.Value.Date).Days + 1;
            //}

            return returnDays;
        }
        public static int NumberOfDaysForIndividualDeductionCalculation(prl_employee_individual_deduction conf, DateTime proStartDate,DateTime proEndDate)
        {

            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;

            int returnDays = 0;
            //assume we are processing march 25th month salary allowance activated in february 26 then return emp working day val
            if (conf.effective_from.Value.Date < proStartDate && (conf.effective_to == null || conf.effective_from.Value.Date >= proEndDate))
                return daysInMonth;
            //assume we are processing march 25th month salary allowance activated in february 26 and 
            //the allowance is deactivated before 31st of March then return emp working day val
            else if (conf.effective_to.Value.Date < proStartDate && (conf.effective_from.Value.Date < proEndDate))
            {
                returnDays = conf.effective_from.Value.Date.Subtract(proStartDate.Date).Days + 1;
            }
            //assume activation on March 1
            else if (conf.effective_to.Value.Date >= proStartDate && (conf.effective_from == null || conf.effective_from.Value.Date >= proEndDate))
            {
                returnDays = proEndDate.Date.Subtract(conf.effective_to.Value.Date).Days + 1;
            }
            //assume activation on March 1 and ends before March 31
            else
            {
                returnDays = conf.effective_from.Value.Date.Subtract(conf.effective_to.Value.Date).Days + 1;
            }

           return returnDays;
        }
        public static List<prl_salary_deductions> CalculateUploadedDeduction(DateTime processDate, prl_employee emp, List<prl_upload_deduction> lst)
        {
            var proStartDate = new DateTime(processDate.Year, processDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(processDate.Year, processDate.Month);
            var proEndDate = new DateTime(processDate.Year, processDate.Month, daysInMonth);

            var lstSalDed = new List<prl_salary_deductions>();

            //foreach (var v in lst)
            //{
            //    var sa = new prl_salary_allowances();
            //    sa.emp_id = emp.id;
            //    sa.salary_month = processDate.Date;
            //    sa.allowance_name_id = v.allowance_name_id;
            //    sa.calculation_for_days = daysInMonth;
            //    sa.amount = v.amount;
            //    sa.arrear_amount = 0;
            //    lstSalAl.Add(sa);
            //}
            return lstSalDed;
        }
    }
}