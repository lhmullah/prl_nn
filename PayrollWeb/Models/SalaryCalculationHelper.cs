using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using com.linde.Model;
using PayrollWeb.Utility;

namespace PayrollWeb.Models
{
    public class SalaryCalculationHelper
    {
        public static int NumberOfDaysWorkedBasedOnEmployeeStatus(prl_employee e, DateTime proStartDate, DateTime proEndDate, List<prl_employee_discontinue> thisMonthsDiscontinued, decimal noOfUnpaidLeave, List<prl_salary_hold> thisMonthsHold)
        {
            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;

            int calDate = 0;

            // if he or she absent than count the days here...

            if (e.joining_date.Date < proStartDate.Date)
            {
                calDate = daysInMonth;
            }
            
            if (CommonDateClass.DayMonthYearIsInRange(e.joining_date, proStartDate, proEndDate))
            {
                calDate = CommonDateClass.DateDiffernceWithLeapYr(e.joining_date, proEndDate) + 1;
            }

            //check if employee's salary is holded
            if (thisMonthsHold.Exists(h => h.emp_id == e.id))
            {
                calDate = thisMonthsHold.SingleOrDefault(l => l.emp_id == e.id).hold_from.Value.Subtract(proEndDate).Days + 1;
            }


            //check if employee is discontinued after some days servicing in this  month 
            if (thisMonthsDiscontinued.Exists(h => h.emp_id == e.id))
            {
                var discontinued_emp = thisMonthsDiscontinued.SingleOrDefault(l => l.emp_id == e.id);

                if (discontinued_emp.with_salary == "Y" || discontinued_emp.without_salary == "N")
                {
                    if (CommonDateClass.DayMonthYearIsInRange(e.joining_date, proStartDate, proEndDate))
                    {
                        calDate = CommonDateClass.DateDiffernceWithLeapYr(e.joining_date, discontinued_emp.discontinue_date) + 1;
                    }
                    else
                    {
                        calDate = CommonDateClass.DateDiffernceWithLeapYr(proStartDate, discontinued_emp.discontinue_date) + 1;                        
                    }
                }
                else if (discontinued_emp.with_salary == "N" || discontinued_emp.without_salary == "Y"|| discontinued_emp.is_active =="N")
                {
                    calDate = 0;
                }
            }

            //Unpaid Leave Deduct from calculative date

            int noOfDaysUL = Convert.ToInt16(noOfUnpaidLeave);

            if (noOfDaysUL > 0)
            {
                if (calDate >= noOfDaysUL)
                {
                    calDate = calDate - noOfDaysUL;
                }
                else
                {
                    calDate = 0;
                }
            }
            

            return calDate;
        }
        public static int NumberOfDaysToCalculateForLeaveWithoutPay(prl_employee_leave_without_pay elwp, DateTime proStartDate, DateTime proEndDate)
        {

            int daysInMonth = proEndDate.Date.Subtract(proStartDate.Date).Days + 1;
            
            int calDate = 0;

            // if he or she absent than count the days here...
            ///////////////////////

            ////////////////////////

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

        public static decimal CalculateArrearOnBasic(prl_salary_review sr, List<prl_salary_process_detail> listSalaryProcessDetails, decimal incramentedBasic)
        {
            

            var adjAmountBasic = incramentedBasic;
            decimal adjAmt = 0;

            foreach (var sal in listSalaryProcessDetails)
            {
                int adjDays = 0; 
                DateTime srMonthStartDate = new DateTime(sr.effective_from.Value.Year, sr.effective_from.Value.Month, sr.effective_from.Value.Day);

                var daysInMonth = DateTime.DaysInMonth(sal.salary_month.Year, sal.salary_month.Month);

                    var endofmonth = new DateTime(sal.salary_month.Year, sal.salary_month.Month, daysInMonth);
                    //var adjDays = endofmonth.Subtract(sr.effective_from.Value.Date).Days;
                    if (sal.salary_month.ToString("yyyy-MM") == sr.effective_from.Value.ToString("yyyy-MM"))
                    {
                       adjDays = CommonDateClass.DateDiffernceWithLeapYr(srMonthStartDate, endofmonth) + 1;
                    }
                    else
                    {
                        adjDays = daysInMonth;
                    }
                    

                    //DateTime toArrearCalcMonth = new DateTime(sal.salary_month.AddMonths(-1).Year, sal.salary_month.AddMonths(-1).Month, DateTime.DaysInMonth(sal.salary_month.AddMonths(-1).Year, sal.salary_month.AddMonths(-1).Month));
                    //int adjDays = CommonDateClass.DateDiffernceWithLeapYr(srMonthStartDate, toArrearCalcMonth); 

                    if (sal.calculation_for_days >= adjDays)
                    {
                        adjAmt = adjAmt + (adjAmountBasic / daysInMonth * adjDays);
                    }
                    else
                    {
                        adjAmt = adjAmt + (adjAmountBasic / daysInMonth * sal.calculation_for_days);
                    }
                
            }


            return adjAmt;
        }

        public static prl_salary_allowances CalculateAllowance(int numberOfDays, DateTime processStartDate, DateTime proEndDate,
            prl_employee emp,prl_employee_details lastestDetails, prl_allowance_configuration conf, prl_salary_review salaryReview = null)
        {

            //var lstGradeIds = lstGrades.AsEnumerable().Select(x => x.id);
            //if (!lstGradeIds.Contains(lastestDetails.grade_id))
            //    return null;
            
            //if(conf.gender.ToLower() != "regardless" && emp.gender.ToLower() != conf.gender.ToLower())
            //    return null;

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

           decimal totalArrearAmount = 0;
           
            if (salaryReview != null)
            {
                if (salaryReview.effective_from < proEndDate && salaryReview.effective_from.Value.Month != proEndDate.Month)
                {
                    var adjustment = salaryReview.new_basic - salaryReview.current_basic;
                    int arrCalDate = 0;

                    DateTime srMonthStartDate = new DateTime(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month, salaryReview.effective_from.Value.Day);

                    DateTime srMonthEndDate = new DateTime(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month, DateTime.DaysInMonth(salaryReview.effective_from.Value.Year, salaryReview.effective_from.Value.Month));
                    int srDaysIfthisMonth = srMonthEndDate.Subtract(salaryReview.effective_from.Value.Date).Days + 1;
                    DateTime toArrearCalcMonth = new DateTime(sa.salary_month.AddMonths(-1).Year, sa.salary_month.AddMonths(-1).Month, DateTime.DaysInMonth(sa.salary_month.AddMonths(-1).Year, sa.salary_month.AddMonths(-1).Month));

                    int srDays = CommonDateClass.DateDiffernceWithLeapYr(srMonthStartDate, toArrearCalcMonth) + 1;

                    if (CommonDateClass.DayMonthYearIsInRange(sa.salary_month.Date, srMonthStartDate, srMonthEndDate))
                    {
                        if (sa.calculation_for_days > srDaysIfthisMonth)
                            arrCalDate = srDaysIfthisMonth;
                        arrCalDate = sa.calculation_for_days;
                    }

                    if (sa.salary_month.Date > srMonthEndDate.Date)
                    {
                        //if (actualDate >= srDays)
                        //{
                        arrCalDate = srDays;
                        //}
                        //else
                        //{
                        //   arrCalDate = sa.calculation_for_days;
                        //}
                    }

                    //sa.arrear_amount = totalArrearAmount + (adjustment / daysInMonth * arrCalDate);
                    totalArrearAmount = (adjustment / daysInMonth) * arrCalDate;
                }
            }

            if (conf.flat_amount != null)
            {
                sa.amount = conf.flat_amount.Value / daysInMonth * actualDate;
                if (totalArrearAmount > 0)
                {
                    sa.arrear_amount = totalArrearAmount * (conf.flat_amount.Value / actualBasic); // the allowance flat amount is how many percentage of basic that is multipied
                }
            }
            else
            {
                if (conf.percent_amount != null)
                {
                  if (salaryReview != null && salaryReview.effective_from.Value.Month == proEndDate.Month
                    && salaryReview.effective_from.Value.Year == proEndDate.Year)
                   {
                       var fristDateOftheSalaryMonth = CommonDateClass.FirstDateForCurrentMonth(proEndDate);
                      
                       int previousBasicPaidDays = CommonDateClass.DateDiffernceWithLeapYr(fristDateOftheSalaryMonth, salaryReview.effective_from.Value);
                       int srDays = CommonDateClass.DateDiffernceWithLeapYr(salaryReview.effective_from.Value, proEndDate.Date) + 1;
                       if (srDays <= actualDate && previousBasicPaidDays > 0)
                       {
                           sa.amount = ((salaryReview.current_basic * (conf.percent_amount.Value / 100)) / daysInMonth) * previousBasicPaidDays;
                           sa.amount += ((actualBasic * (conf.percent_amount.Value / 100)) / daysInMonth) * srDays;
                       }
                       else
                       {
                           sa.amount = ((actualBasic * (conf.percent_amount.Value / 100)) / daysInMonth) * actualDate;
                       }
     
                   }
                  else
                  {
                      sa.amount = ((actualBasic * (conf.percent_amount.Value / 100)) / daysInMonth) * actualDate;

                      if (totalArrearAmount > 0)
                      {
                          sa.arrear_amount = totalArrearAmount * (conf.percent_amount.Value / 100); // the allowance is how many percentage of basic that is multipied
                      }
                  }
                }
                else
                {
                    sa.amount = (actualBasic / daysInMonth) * actualDate;

                    if (totalArrearAmount > 0)
                    {
                        sa.arrear_amount = totalArrearAmount;
                    }
                }

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
            sa.arrear_amount = 0;

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
                if (salaryReview.effective_from < proEndDate && salaryReview.effective_from.Value.Month != proEndDate.Month)
                {
                    var adjustment = salaryReview.new_basic - salaryReview.current_basic;

                    decimal totalArrearAmount = 0;

                    var arrcalDate = NumberOfDaysForIndividualAllowance(indv, proStartDate, proEndDate);
                    if (arrcalDate > numberOfDays)
                        arrcalDate = numberOfDays;
                    sa.arrear_amount = totalArrearAmount + (adjustment / daysInMonth * arrcalDate);
                }
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

            foreach (var v in lst)
            {
                var sa = new prl_salary_allowances();
                sa.emp_id = emp.id;
                sa.salary_month = processDate.Date;
                sa.allowance_name_id = v.allowance_name_id;
                sa.calculation_for_days = daysInMonth;
                sa.amount = Convert.ToDecimal(v.amount);
                sa.arrear_amount = 0;
                lstSalAl.Add(sa);
            }
            return lstSalAl;
        }

        public static prl_salary_deductions CalculateDeduction(int numberOfDays, DateTime proStartDate,DateTime proEndDate,
           prl_employee emp, prl_employee_details lastestDetails, prl_deduction_configuration conf, prl_salary_review salaryReview = null)
        {
            //var lstGradeIds = lstGrades.AsEnumerable().Select(x => x.id);
            //if (!lstGradeIds.Contains(lastestDetails.grade_id))
            //    return null;

            //if (conf.gender.ToLower() != "regardless" && emp.gender.ToLower() != conf.gender.ToLower())
            //    return null;

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

            int monthCount = 0;
            
            if (numberOfDays <= actualDate)
            {
                if (conf.is_monthly == 1)
                {
                    monthCount = (int)Math.Round(Convert.ToDecimal(actualDate / daysInMonth), 0, MidpointRounding.AwayFromZero);
                }
                actualDate = numberOfDays;
            }
                

            var sa = new prl_salary_deductions();
            sa.emp_id = emp.id;
            sa.salary_month = proEndDate.Date;
            sa.deduction_name_id = conf.deduction_name_id;
            sa.calculation_for_days = actualDate;
            sa.arrear_amount = 0;

            if (conf.flat_amount != null)
            {
                if (conf.is_monthly == 1)
                {
                    sa.amount = conf.flat_amount.Value / monthCount;
                }
                else
                {
                    sa.amount = conf.flat_amount.Value / daysInMonth * actualDate; 
                }
            }
            else
            {
                if (conf.is_monthly == 1)
                {
                    sa.amount =  (actualBasic * (conf.percent_amount.Value / 100)) / monthCount;
                }
                else
                {
                    sa.amount = ((actualBasic * (conf.percent_amount.Value / 100)) / daysInMonth) * actualDate;
                }
            }

            if (salaryReview != null)
            {
                if (salaryReview.effective_from < proEndDate && salaryReview.effective_from.Value.Month != proEndDate.Month)
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
            int monthCount = 0;

            if (actualDays >= numberOfDays)
            {
                monthCount = (int)Math.Round(Convert.ToDecimal(actualDays / daysInMonth), 0, MidpointRounding.AwayFromZero);
                actualDays = numberOfDays;
            }

            if (indv.flat_amount > 0)
            {
                if (monthCount > 0)
                {
                    sa.amount = indv.flat_amount.Value / monthCount;
                }
                else
                {
                    sa.amount = indv.flat_amount.Value / daysInMonth * actualDays;
                }
                
            }
            else
            {
                if (monthCount>0)
                {
                    sa.amount = (basic * (indv.percentage.Value / 100)) / monthCount;
                }
                else
                {
                    sa.amount = ((basic * (indv.percentage.Value / 100)) / daysInMonth) * actualDays;
                } 

            }

            sa.calculation_for_days = actualDays;
            sa.arrear_amount = 0;

          if (salaryReview != null)
            {
                if (salaryReview.effective_from < proEndDate && salaryReview.effective_from.Value.Month != proEndDate.Month)
                {
                    var adjustment = salaryReview.new_basic - salaryReview.current_basic;

                    decimal totalArrearAmount = 0;

                    var arrcalDate = NumberOfDaysForIndividualDeductionCalculation(indv, proStartDate, proEndDate);
                    if (arrcalDate > numberOfDays)
                        arrcalDate = numberOfDays;

                    sa.arrear_amount = totalArrearAmount + (adjustment / daysInMonth * arrcalDate); 
                }
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
                returnDays = proStartDate.Date.Subtract(conf.effective_from.Value.Date).Days + 1;
            }
            //assume activation on March 1 
            else if (conf.effective_to.Value.Date >= proStartDate && (conf.effective_from == null || conf.effective_from.Value.Date >= proEndDate))
            {
                returnDays = conf.effective_to.Value.Date.Subtract(proEndDate.Date).Days + 1;
            }
            //assume activation on March 1 and ends before March 31
            else
            {
                returnDays = conf.effective_to.Value.Date.Subtract(conf.effective_from.Value.Date).Days + 1;
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
            //    var sa = new prl_salary_deductions();
            //    sa.emp_id = emp.id;
            //    sa.salary_month = processDate.Date;
            //    sa.deduction_name_id = v.deduction_name_id;
            //    sa.calculation_for_days = daysInMonth;
            //    sa.amount = v.amount;
            //    sa.arrear_amount = 0;
            //    lstSalDed.Add(sa);
            //}
            return lstSalDed;
        }
    }
}