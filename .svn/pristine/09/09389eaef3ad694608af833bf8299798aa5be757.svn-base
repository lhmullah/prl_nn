using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using com.gpit.DataContext;
using com.gpit.Model;
using PayrollWeb.ViewModels;
using System.Transactions;
using System.Data.Objects;
using System.Data.Entity;
using PayrollWeb.Utility;
using System.Data.Entity.Infrastructure;
using Jace;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Diagnostics;
using PayrollWeb.Models;

namespace PayrollWeb.Service
{
    public class IncomeTaxService
    {
        private payroll_systemContext dataContext;
        private IProcessResult result;

        public IncomeTaxService(payroll_systemContext context)
        {
            this.dataContext = context;
        }

        ~IncomeTaxService()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dataContext != null)
                {
                    dataContext.Dispose();
                    dataContext = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int process_monthly_incomeTax(List<prl_employee> employeeList, string batchNo, DateTime salaryMonth, DateTime salaryProcessDate, int pFiscalYear, string processUser)
        {
            //var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            //objectContext.Connection.Open();
            MySqlCommand mySqlCommand = null;
            MySqlConnection mySqlConnection = null;
            MySqlTransaction tran = null;

            int _result = 0;
            try
            {
                decimal max_investment_allowed = 0;
                decimal max_investment_Pecentage_allowed = 0;
                decimal taxAge = 0;
                decimal min_tax = 0;

                //For Finding yearly Basic
                decimal thisMonthBasic = 0;
                decimal currentBasic = 0;
                decimal projectedBasic = 0;
                decimal actualBasic = 0;
                decimal yearlyBasic = 0;
                //For Finding yearly Basic

                //PF
                decimal thisMonthPF = 0;
                decimal currentPF = 0;
                decimal projectedPF = 0;
                decimal actualPF = 0;
                decimal yearlyPF = 0;
                //PF

                //Festival
                decimal projectedFestival = 0;
                decimal actualFestival = 0;
                decimal yearlyFestival = 0;
                //Festival

                decimal freeCar = 0;

                int _reminingMonth = 0;
                int _actualMonth = 0;

                //Tax
                decimal taxableIncome = 0;
                //Tax


                foreach (var item in employeeList)
                {
                    List<prl_salary_allowances> allowList = new List<prl_salary_allowances>();
                    List<EmployeeSalaryAllowance> salallList = new List<EmployeeSalaryAllowance>();

                    var _salaryPrss = dataContext.prl_salary_process.FirstOrDefault(s => s.batch_no == batchNo);
                    if (_salaryPrss != null)
                    {
                        //For Finding yearly Basic
                        thisMonthBasic = 0; currentBasic = 0; projectedBasic = 0; actualBasic = 0; yearlyBasic = 0;
                        //For Finding yearly Basic

                        //PF
                        thisMonthPF = 0; currentPF = 0; projectedPF = 0; actualPF = 0; yearlyPF = 0;
                        //PF

                        //Festival
                        projectedFestival = 0; actualFestival = 0; yearlyFestival = 0;
                        //Festival

                        //Basic Salary
                        var salaryDet = dataContext.prl_salary_process_detail.FirstOrDefault(s => s.salary_process_id == _salaryPrss.id && s.emp_id == item.id);
                        currentBasic = salaryDet.current_basic; // Current Basic
                        thisMonthBasic = salaryDet.this_month_basic.Value; // This Month Basic 
                        _reminingMonth = FindProjectedMonth(salaryMonth.Month);
                        projectedBasic = salaryDet.current_basic * _reminingMonth; // projected basic
                        _actualMonth = FindActualMonth(salaryMonth.Month);
                        actualBasic = thisMonthBasic * _actualMonth; // Actual Basic
                        yearlyBasic = projectedBasic + actualBasic; // yearly Basic
                        //Basic Salary

                        //Provident Fund
                        thisMonthPF = salaryDet.pf_amount;
                        currentPF = currentBasic * 10 / 100;
                        projectedPF = currentPF * _reminingMonth;
                        actualPF = thisMonthPF * _actualMonth;
                        yearlyPF = projectedPF + actualPF;
                        //Provident Fund

                        allowList = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == _salaryPrss.id && x.emp_id == item.id).ToList();
                        if (allowList.Count > 0)
                        {
                            string allWName = "";
                            foreach (var allW in allowList)
                            {
                                var AllwConfig = dataContext.prl_allowance_configuration.FirstOrDefault(q => q.allowance_name_id == allW.allowance_name_id);
                                if (AllwConfig.is_taxable == 1)
                                {
                                    allWName = dataContext.prl_allowance_name.FirstOrDefault(a => a.id == allW.allowance_name_id).allowance_name;
                                    EmployeeSalaryAllowance salAllW = new EmployeeSalaryAllowance();
                                    salAllW.allowanceid = allW.allowance_name_id;
                                    salAllW.allowancename = allWName;
                                    salAllW.this_month_amount = allW.amount;

                                    if (AllwConfig.percent_amount > 0)
                                        salAllW.current_amount = (currentBasic * AllwConfig.percent_amount / 100).Value;
                                    else
                                        salAllW.current_amount = AllwConfig.flat_amount.Value;

                                    salAllW.projected_amount = salAllW.current_amount * _reminingMonth;
                                    salAllW.actual_amount = salAllW.this_month_amount * _actualMonth;
                                    salAllW.yearly_amount = salAllW.actual_amount + salAllW.projected_amount;
                                    //if (AllwConfig.exempted_amount > 0)
                                    //    salAllW.exempted_amount = AllwConfig.exempted_amount.Value;

                                    salallList.Add(salAllW);
                                }
                            }
                        }

                        // ToDo ::
                        // 365 days.. actual basic
                        // basic*2/365*no of days from joining to 31st december
                        //Festival Bonus

                        //string dtDate = "31/12/" + salaryMonth.Year.ToString();
                        string dtDate = salaryMonth.Year.ToString() + "-12-" + "31"; //"31/12/" + salaryMonth.Year.ToString();
                        DateTime dtLastDayOfYear = DateTime.ParseExact(dtDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //DateTime dtLastDayOfYear = Convert.ToDateTime(dtDate);
                        long bonusDays = 0;
                        if (item.joining_date.Year == salaryMonth.Year)
                        {
                            bonusDays = CommonDateClass.DateDifference(item.joining_date, dtLastDayOfYear);
                        }
                        else
                        {
                            bonusDays = 365;
                        }
                        yearlyFestival = (currentBasic * 2 / 365) * bonusDays;

                        #region Bonus Code for CBL
                        //List<prl_bonus_process> _bonusMasterList = _context.prl_bonus_process.Where(e => e.festival_date.Value.Month == 8 && e.festival_date.Value.Year == salaryMonth.Year).ToList();
                        //prl_bonus_process_detail _bonusDetail = new prl_bonus_process_detail();
                        //if (_bonusMasterList.Count > 0)
                        //{
                        //    if (_bonusMasterList.Count == 2)
                        //    {
                        //        foreach (var item1 in _bonusMasterList)
                        //        {
                        //            _bonusDetail = _context.prl_bonus_process_detail.FirstOrDefault(e => e.bonus_process_id == item1.id && e.emp_id == item.id);
                        //            actualFestival += _bonusDetail.amount;
                        //            yearlyFestival = actualFestival;
                        //        }
                        //    }
                        //    else if (_bonusMasterList.Count == 1)
                        //    {
                        //        foreach (var item2 in _bonusMasterList)
                        //        {
                        //            _bonusDetail = new prl_bonus_process_detail();
                        //            _bonusDetail = _context.prl_bonus_process_detail.FirstOrDefault(e => e.bonus_process_id == item2.id && e.emp_id == item.id);
                        //            if (_bonusDetail != null)
                        //            {
                        //                actualFestival = _bonusDetail.amount;
                        //                projectedFestival = currentBasic;
                        //                yearlyFestival = actualFestival + projectedFestival;
                        //            }
                        //            else
                        //            {
                        //                yearlyFestival = currentBasic;
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    yearlyFestival = currentBasic * 2;
                        //}
                        #endregion

                        //Festival Bonus

                        decimal currentChildAllowName = 0;
                        decimal projectedChilAllowance = 0;
                        decimal yearlyChildAllowance = 0;
                        //ToDo: Childer Allowance should be projected and actual and added to taxable income
                        prl_employee_children_allowance childAllw = dataContext.prl_employee_children_allowance.FirstOrDefault(c => c.emp_id == item.id && c.is_active == 1);
                        if (childAllw != null)
                        {
                            currentChildAllowName = childAllw.amount;
                            projectedChilAllowance = currentChildAllowName * _reminingMonth;
                            yearlyChildAllowance = currentChildAllowName + projectedChilAllowance;
                        }
                        //ToDo:


                        var taxDetail = new prl_income_tax_parameter_details();

                        //Free Car
                        freeCar = 0;
                        decimal free_car_rate = 0;
                        try
                        {
                            taxDetail = dataContext.prl_income_tax_parameter_details.FirstOrDefault(e => e.fiscal_year_id == pFiscalYear);
                            if (taxDetail != null)
                                free_car_rate = taxDetail.free_car.Value;
                        }
                        catch
                        {

                            free_car_rate = 0;
                        }

                        prl_employee_free_car _empFreeCar = dataContext.prl_employee_free_car.FirstOrDefault(e => e.emp_id == item.id);
                        if (_empFreeCar != null)
                        {
                            freeCar = yearlyBasic * free_car_rate / 100;
                        }
                        //Free Car

                        //Total Taxable Income
                        // Need apply changes which one is 100% taxable

                        taxableIncome = yearlyBasic + yearlyFestival + yearlyPF + freeCar + yearlyChildAllowance;

                        decimal totalTaxableTransport = 0;
                        decimal actualTransportExemption = 0;

                        decimal totalTaxableHouse = 0;
                        decimal actualHouseExemption = 0;

                        decimal totalTaxableLFA = 0;
                        decimal actualLFAExemtion = 0;

                        decimal totalTaxableMedical = 0;
                        decimal actualMedicalExemption = 0;

                        foreach (var a in salallList)
                        {
                            if (a.allowancename.Contains("House"))
                            {
                                totalTaxableHouse = a.yearly_amount - taxDetail.house_rent_not_exceding.Value;
                                actualHouseExemption = taxDetail.house_rent_not_exceding.Value;
                                if (totalTaxableHouse < 0)
                                {
                                    totalTaxableHouse = 0;
                                    actualHouseExemption = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableHouse;
                            }
                            else if (a.allowancename.Contains("Medical"))
                            {
                                totalTaxableMedical = a.yearly_amount - (a.yearly_amount / taxDetail.medical_exemtion_percentage.Value * 100);
                                actualHouseExemption = (a.yearly_amount / taxDetail.medical_exemtion_percentage.Value * 100);
                                if (totalTaxableMedical < 0)
                                {
                                    totalTaxableMedical = 0;
                                    actualHouseExemption = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableMedical;
                            }
                            else if (a.allowancename.Contains("Transport"))
                            {
                                totalTaxableTransport = a.yearly_amount - taxDetail.max_conveyance_allowance_monthly.Value;
                                actualTransportExemption = taxDetail.max_conveyance_allowance_monthly.Value;
                                if (totalTaxableTransport < 0)
                                {
                                    totalTaxableTransport = 0;
                                    actualTransportExemption = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableTransport;
                            }
                            else if (a.allowancename.Contains("LFA"))
                            {
                                totalTaxableLFA = a.yearly_amount - (a.yearly_amount / taxDetail.lfa_exemtion_percentage.Value * 100);
                                actualLFAExemtion = (a.yearly_amount / taxDetail.lfa_exemtion_percentage.Value * 100);
                                if (totalTaxableLFA < 0)
                                {
                                    totalTaxableLFA = 0;
                                    actualLFAExemtion = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableLFA;
                            }
                            else
                            {
                                taxableIncome += a.yearly_amount;
                            }
                        }

                        List<prl_over_time_amount> otList = dataContext.prl_over_time_amount.Where(o => o.emp_id == item.id && o.month_year.Month == salaryMonth.Month && o.month_year.Year == salaryMonth.Year).ToList();
                        foreach (var _ot in otList)
                        {
                            taxableIncome += _ot.pay_total.Value;
                        }

                        //Total Taxable Income

                        //Tax Parameter Settings
                        if (taxDetail != null)
                        {
                            max_investment_allowed = taxDetail.max_investment_amount.Value;
                            max_investment_Pecentage_allowed = taxDetail.max_investment_percentage.Value;
                            taxAge = taxDetail.max_tax_age.Value;
                            min_tax = taxDetail.min_tax_amount.Value;
                        }
                        //Tax Parameter Settings

                        //Tax Slab
                        List<prl_income_tax_parameter> taxSlab = new List<prl_income_tax_parameter>();
                        taxSlab = dataContext.prl_income_tax_parameter.Where(objID => (objID.fiscal_year_id == pFiscalYear && objID.gender == item.gender)).ToList();
                        int maxNumberItem = taxSlab.Count;
                        decimal lastSlabAmount = taxSlab[maxNumberItem - 1].slab_maximum_amount - taxSlab[maxNumberItem - 2].slab_maximum_amount;
                        decimal lastSlabPercentage = taxSlab[maxNumberItem - 1].slab_percentage;
                        decimal secondlastSlabAmount = taxSlab[maxNumberItem - 2].slab_maximum_amount - taxSlab[maxNumberItem - 3].slab_maximum_amount;
                        decimal secondlastSlabPercentage = taxSlab[maxNumberItem - 2].slab_percentage;
                        decimal thirdlastSlabAmount = taxSlab[maxNumberItem - 3].slab_maximum_amount - taxSlab[maxNumberItem - 4].slab_maximum_amount;
                        decimal thidlastSlabPercentage = taxSlab[maxNumberItem - 3].slab_percentage;
                        decimal forthlastSlabAmount = taxSlab[maxNumberItem - 4].slab_maximum_amount - taxSlab[maxNumberItem - 5].slab_maximum_amount;
                        decimal forthlastSlabPercentage = taxSlab[maxNumberItem - 4].slab_percentage;
                        decimal fifthlastSlabAmount = taxSlab[maxNumberItem - 5].slab_maximum_amount;
                        decimal fifthlastSlabPercentage = taxSlab[maxNumberItem - 5].slab_percentage;

                        decimal _TaxableIncome = taxableIncome;
                        decimal TaxPayableAmount = 0;

                        decimal firstSlabAmount = 0;
                        decimal secondSlabAmount = 0;
                        decimal thirdSlabAmount = 0;
                        decimal forthSlabAmount = 0;
                        decimal fifthSlabAmount = 0;
                        decimal sisthSlabAmount = 0;

                        ArrayList arrys = new ArrayList();
                        List<prl_employee_tax_slab> taxItemList = new List<prl_employee_tax_slab>();
                        prl_employee_tax_slab taxItem;

                        if (_TaxableIncome <= fifthlastSlabAmount)
                        {
                            TaxPayableAmount = 0;
                            taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, fifthlastSlabAmount, fifthlastSlabPercentage, _TaxableIncome, TaxPayableAmount);
                            taxItemList.Add(taxItem);
                        }
                        if (_TaxableIncome > fifthlastSlabAmount)
                        {
                            decimal netTaxPayableAmount = _TaxableIncome - fifthlastSlabAmount;

                            taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, fifthlastSlabAmount, fifthlastSlabPercentage, fifthlastSlabAmount, 0);
                            taxItemList.Add(taxItem);

                            if (netTaxPayableAmount <= forthlastSlabAmount)
                            {
                                TaxPayableAmount = (netTaxPayableAmount * forthlastSlabPercentage) / 100;

                                taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, forthlastSlabAmount, forthlastSlabPercentage, netTaxPayableAmount, TaxPayableAmount);
                                taxItemList.Add(taxItem);
                            }

                            if (netTaxPayableAmount > forthlastSlabAmount)
                            {

                                decimal reminderAmount = netTaxPayableAmount - forthlastSlabAmount;
                                firstSlabAmount = (forthlastSlabAmount * forthlastSlabPercentage) / 100;

                                taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, forthlastSlabAmount, forthlastSlabPercentage, forthlastSlabAmount, firstSlabAmount);
                                taxItemList.Add(taxItem);

                                if (reminderAmount <= thirdlastSlabAmount)
                                {
                                    secondSlabAmount = (reminderAmount * thidlastSlabPercentage) / 100;

                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, thirdlastSlabAmount, thidlastSlabPercentage, reminderAmount, secondSlabAmount);
                                    taxItemList.Add(taxItem);

                                    TaxPayableAmount = firstSlabAmount + secondSlabAmount + thirdSlabAmount + forthSlabAmount + fifthSlabAmount + sisthSlabAmount;
                                }
                                if (reminderAmount > thirdlastSlabAmount)
                                {
                                    decimal secondReminderAmount = reminderAmount - thirdlastSlabAmount;
                                    thirdSlabAmount = (thirdlastSlabAmount * thidlastSlabPercentage) / 100;

                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, thirdlastSlabAmount, thidlastSlabPercentage, thirdlastSlabAmount, thirdSlabAmount);
                                    taxItemList.Add(taxItem);

                                    if (secondReminderAmount <= secondlastSlabAmount)
                                    {
                                        forthSlabAmount = (secondReminderAmount * secondlastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, secondlastSlabAmount, secondlastSlabPercentage, secondReminderAmount, forthSlabAmount);
                                        taxItemList.Add(taxItem);
                                    }

                                    if (secondReminderAmount > secondlastSlabAmount)
                                    {
                                        decimal thirdReminder = (secondReminderAmount - secondlastSlabAmount);
                                        fifthSlabAmount = (secondlastSlabAmount * secondlastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, secondlastSlabAmount, secondlastSlabPercentage, secondlastSlabAmount, fifthSlabAmount);
                                        taxItemList.Add(taxItem);

                                        sisthSlabAmount = (thirdReminder * lastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, lastSlabAmount, lastSlabPercentage, thirdReminder, sisthSlabAmount);
                                        taxItemList.Add(taxItem);
                                    }

                                    TaxPayableAmount = firstSlabAmount + secondSlabAmount + thirdSlabAmount + forthSlabAmount + fifthSlabAmount + sisthSlabAmount;

                                }
                            }
                        }
                        //Tax Slab

                        // Investment Rebate
                        decimal otherInvestment = 0;
                        decimal minRebate = 0;
                        decimal invRebate = (taxableIncome - yearlyPF) * taxDetail.max_investment_percentage.Value / 100;

                        otherInvestment = invRebate - (yearlyPF * 2); // OtherInvetment

                        decimal invPerRebate = taxDetail.max_inv_exempted_percentage.Value;

                        if (invRebate > max_investment_allowed)
                        {
                            minRebate = max_investment_allowed * invPerRebate / 100;
                        }
                        else
                        {
                            minRebate = invRebate * invPerRebate / 100;
                        }

                        //  ToDo:: Other Investment Should be incorporated

                        //

                        // Investment Rebate

                        //yearly Income Tax
                        double yearlyIncomeTax = 0;

                        if (TaxPayableAmount == 0)
                        {
                            yearlyIncomeTax = 0;
                        }
                        else if (minRebate > TaxPayableAmount)
                        {
                            yearlyIncomeTax = double.Parse(taxDetail.min_tax_amount.ToString());
                        }
                        else
                        {
                            if ((TaxPayableAmount - minRebate) <= taxDetail.min_tax_amount)
                            {
                                yearlyIncomeTax = double.Parse(taxDetail.min_tax_amount.ToString());
                            }
                            else
                            {
                                yearlyIncomeTax = double.Parse((TaxPayableAmount - minRebate).ToString());
                            }
                        }

                        // ToDo :: Tax Refund For Employee
                        decimal Tax_Refund = 0;
                        var taxRefund = dataContext.prl_income_tax_refund.FirstOrDefault(w => w.emp_id == item.id && w.fiscal_year_id == pFiscalYear && w.month_year == salaryMonth);
                        if (taxRefund != null)
                        {
                            Tax_Refund = taxRefund.refund_amount.Value;
                            yearlyIncomeTax = yearlyIncomeTax - double.Parse(Tax_Refund.ToString());
                        }
                        // yearlyIncomeTax = yearlyIncomeTax - TaxRefund

                        decimal _previousTax = 0;
                        double YearlyLiabilities = 0;
                        try
                        {
                            _previousTax = dataContext.prl_employee_tax_process.Where(e => e.fiscal_year_id == pFiscalYear && e.emp_id == item.id).Sum(q => q.monthly_tax);
                        }
                        catch
                        {
                            _previousTax = 0;
                        }
                        yearlyIncomeTax = yearlyIncomeTax - double.Parse(_previousTax.ToString());
                        YearlyLiabilities = yearlyIncomeTax - double.Parse(_previousTax.ToString());
                        //yearly Income Tax

                        //Monthly Tax
                        double MonthlyTax = 0;
                        MonthlyTax = YearlyLiabilities / TaxRemainingMonth(salaryMonth.Month);
                        //MOnthly Tax

                        //Save Data
                        #region Save Data

                        var taxMaster = new prl_employee_tax_process();
                        taxMaster.salary_process_id = salaryDet.salary_process_id;
                        taxMaster.emp_id = item.id;
                        taxMaster.fiscal_year_id = pFiscalYear;
                        taxMaster.salary_month = salaryMonth;
                        taxMaster.yearly_tax = decimal.Parse(yearlyIncomeTax.ToString());
                        taxMaster.monthly_tax = decimal.Parse(MonthlyTax.ToString());
                        taxMaster.created_by = "";
                        taxMaster.created_date = DateTime.Now;
                        dataContext.prl_employee_tax_process.Add(taxMaster);
                        dataContext.SaveChanges();


                        //Basic
                        var taxDet = new prl_employee_tax_process_detail();
                        taxDet.tax_process_id = taxMaster.id;
                        taxDet.emp_id = item.id;
                        taxDet.tax_item = "Basic";
                        taxDet.gross_annual_income = yearlyBasic;
                        taxDet.less_exempted = 0;
                        taxDet.total_taxable_income = yearlyBasic;
                        dataContext.prl_employee_tax_process_detail.Add(taxDet);

                        //All Allowances
                        foreach (var sal in salallList)
                        {
                            taxDet = new prl_employee_tax_process_detail();
                            taxDet.tax_process_id = taxMaster.id;
                            taxDet.emp_id = item.id;
                            taxDet.tax_item = sal.allowancename;
                            taxDet.gross_annual_income = sal.yearly_amount;

                            if (sal.allowancename.Contains("House"))
                            {
                                taxDet.less_exempted = actualHouseExemption;
                                taxDet.total_taxable_income = totalTaxableHouse;
                            }
                            else if (sal.allowancename.Contains("Medical"))
                            {
                                taxDet.less_exempted = actualMedicalExemption;
                                taxDet.total_taxable_income = totalTaxableMedical;
                            }
                            else if (sal.allowancename.Contains("Transport"))
                            {
                                taxDet.less_exempted = actualTransportExemption;
                                taxDet.total_taxable_income = totalTaxableTransport;
                            }
                            else if (sal.allowancename.Contains("LFA"))
                            {
                                taxDet.less_exempted = actualLFAExemtion;
                                taxDet.total_taxable_income = totalTaxableLFA;
                            }
                            else
                            {
                                taxDet.less_exempted = sal.yearly_amount;
                                taxDet.total_taxable_income = sal.yearly_amount;
                            }
                            dataContext.prl_employee_tax_process_detail.Add(taxDet);
                        }

                        //Bonus
                        taxDet = new prl_employee_tax_process_detail();
                        taxDet.tax_process_id = taxMaster.id;
                        taxDet.emp_id = item.id;
                        taxDet.tax_item = "Bonus";
                        taxDet.gross_annual_income = yearlyFestival;
                        taxDet.less_exempted = 0;
                        taxDet.total_taxable_income = yearlyFestival;
                        dataContext.prl_employee_tax_process_detail.Add(taxDet);

                        //PF
                        taxDet = new prl_employee_tax_process_detail();
                        taxDet.tax_process_id = taxMaster.id;
                        taxDet.emp_id = item.id;
                        taxDet.tax_item = "PF";
                        taxDet.gross_annual_income = yearlyPF;
                        taxDet.less_exempted = 0;
                        taxDet.total_taxable_income = yearlyPF;
                        dataContext.prl_employee_tax_process_detail.Add(taxDet);


                        //Over Time
                        foreach (var _ot in otList)
                        {
                            var ot_name = dataContext.prl_over_time_configuration.FirstOrDefault(t => t.id == _ot.over_time_config_id);
                            taxDet = new prl_employee_tax_process_detail();
                            taxDet.tax_process_id = taxMaster.id;
                            taxDet.emp_id = item.id;
                            taxDet.tax_item = ot_name.name;
                            taxDet.gross_annual_income = _ot.pay_total;
                            taxDet.less_exempted = 0;
                            taxDet.total_taxable_income = _ot.pay_total;
                            dataContext.prl_employee_tax_process_detail.Add(taxDet);
                        }

                        //Children Allowance
                        if (childAllw != null)
                        {
                            taxDet = new prl_employee_tax_process_detail();
                            taxDet.tax_process_id = taxMaster.id;
                            taxDet.emp_id = item.id;
                            taxDet.tax_item = "Acting/Child Edu/Transfer/Others Allowance";
                            taxDet.gross_annual_income = childAllw.amount;
                            taxDet.less_exempted = 0;
                            taxDet.total_taxable_income = childAllw.amount;
                            dataContext.prl_employee_tax_process_detail.Add(taxDet);
                        }

                        //Tax Slab
                        foreach (var _tax in taxItemList)
                        {
                            prl_employee_tax_slab eSlab = new prl_employee_tax_slab();
                            eSlab.emp_id = item.id;
                            eSlab.tax_process_id = taxMaster.id;
                            eSlab.fiscal_year_id = pFiscalYear;
                            eSlab.salary_date = salaryMonth;
                            eSlab.salary_month = salaryMonth.Month;
                            eSlab.salary_year = salaryMonth.Year;
                            eSlab.current_rate = _tax.current_rate;
                            eSlab.parameter = _tax.parameter;
                            eSlab.taxable_income = _tax.taxable_income;
                            eSlab.tax_liability = _tax.tax_liability;
                            eSlab.created_by = "";
                            eSlab.created_date = DateTime.Now;
                            dataContext.prl_employee_tax_slab.Add(eSlab);
                        }
                        #endregion
                        //dataContext.SaveChanges();

                        


                        //Save Data
                    }

                }
                _result = 1;

            }
            catch (Exception ex)
            {
                _result = -99;
                throw ex;
            }
            finally
            {
                
            }
            return _result;
        }

        public IProcessResult process_incomeTax(List<prl_employee> employeeList, string batchNo, DateTime salaryMonth, DateTime salaryProcessDate, int pFiscalYear, string processUser)
        {

            int e_id = 0;
            MySqlCommand mySqlCommand = null;
            MySqlConnection mySqlConnection = null;

            mySqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
            mySqlCommand = new MySqlCommand();
            mySqlCommand.Connection = mySqlConnection;
            mySqlConnection.Open();


            int _result = 0;

            decimal max_investment_allowed = 0;
            decimal max_investment_Pecentage_allowed = 0;
            decimal taxAge = 0;
            decimal min_tax = 0;

            //For Finding yearly Basic
            decimal thisMonthBasic = 0;
            decimal currentBasic = 0;
            decimal projectedBasic = 0;
            decimal actualBasic = 0;
            decimal yearlyBasic = 0;
            decimal previousBasic = 0;
            //For Finding yearly Basic

            //PF
            decimal thisMonthPF = 0;
            decimal currentPF = 0;
            decimal projectedPF = 0;
            decimal actualPF = 0;
            decimal yearlyPF = 0;
            decimal previousPF = 0;
            //PF

            //Festival
            decimal projectedFestival = 0;
            decimal actualFestival = 0;
            decimal yearlyFestival = 0;
            //Festival

            decimal freeCar = 0;

            int _reminingMonth = 0;
            int _actualMonth = 0;

            //Tax
            decimal taxableIncome = 0;
            //Tax

            int fiscalYearStart = 7;

            foreach (var item in employeeList)
            {
                if (item.id == 61455)
                { 
                    
                }
                List<prl_salary_allowances> allowList = new List<prl_salary_allowances>();
                List<EmployeeSalaryAllowance> salallList = new List<EmployeeSalaryAllowance>();

                MySqlTransaction tran = null;
                MySqlTransaction tran2 = null;
                try
                {

                    var _salaryPrss = dataContext.prl_salary_process.FirstOrDefault(s => s.batch_no == batchNo);
                    if (_salaryPrss != null)
                    {
                        //For Finding yearly Basic
                        thisMonthBasic = 0; currentBasic = 0; projectedBasic = 0; actualBasic = 0; yearlyBasic = 0;
                        //For Finding yearly Basic

                        //PF
                        thisMonthPF = 0; currentPF = 0; projectedPF = 0; actualPF = 0; yearlyPF = 0;
                        //PF

                        //Festival
                        projectedFestival = 0; actualFestival = 0; yearlyFestival = 0;
                        //Festival

                        //Basic Salary


                        //for (int i = fiscalYearStart; i < salaryMonth.Month; i++)
                        //{
                        //    try
                        //    {
                        //        var v_Basic = new prl_salary_process_detail();
                        //        v_Basic = dataContext.prl_salary_process_detail.FirstOrDefault(e => e.emp_id == item.id && e.salary_month.Month == salaryMonth.Month && e.salary_month.Year == salaryMonth.Year);
                        //        if (v_Basic != null)
                        //        {
                        //            previousBasic += v_Basic.this_month_basic.Value;
                        //        }
                        //    }
                        //    catch
                        //    {
                        //        previousBasic = 0;
                        //    }
                        //}

                        #region previous basic
                        
                        int fiscal_Yr = salaryMonth.Year;
                        int currMonth = salaryMonth.Month;
                        string e_date = "";
                        string s_date = fiscal_Yr.ToString() + "-" + fiscalYearStart + "-" + "30";
                        if (currMonth != 7)
                        {
                            e_date = fiscal_Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + 30;
                        }
                        else
                        {
                            e_date = fiscal_Yr.ToString() + "-" + currMonth.ToString() + "-" + 30;
                        }

                        mySqlCommand.Parameters.Clear();
                        var select_cmd = @"SELECT SUM(this_month_basic) FROM prl_salary_process_detail WHERE salary_month BETWEEN ?s_datee AND ?e_datee AND emp_id = ?emp_iid;";
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = select_cmd;
                        mySqlCommand.Parameters.AddWithValue("?emp_iid", item.id);
                        mySqlCommand.Parameters.AddWithValue("?s_datee", s_date);
                        mySqlCommand.Parameters.AddWithValue("?e_datee", e_date);

                        string resVal = "";
                        using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                        {
                            foreach (var dr in msReader)
                            {
                                if (System.DBNull.Value != null)
                                {
                                    while (msReader.Read())
                                        resVal = msReader.GetString(0);
                                }
                            }
                        }
                        
                        if (resVal != "")
                            previousBasic = decimal.Parse(resVal);
                        #endregion


                        var salaryDet = dataContext.prl_salary_process_detail.FirstOrDefault(s => s.salary_process_id == _salaryPrss.id && s.emp_id == item.id);
                        currentBasic = salaryDet.current_basic; // Current Basic
                        thisMonthBasic = salaryDet.this_month_basic.Value; // This Month Basic 
                        _reminingMonth = FindProjectedMonth(salaryMonth.Month);
                        projectedBasic = salaryDet.current_basic * _reminingMonth; // projected basic
                        _actualMonth = FindActualMonth(salaryMonth.Month);
                        //actualBasic = thisMonthBasic * _actualMonth; // Actual Basic
                        //yearlyBasic = projectedBasic + actualBasic; // yearly Basic
                        yearlyBasic = previousBasic + thisMonthBasic + projectedBasic; // Chnge for getting previous payment in Basic Salary
                        //Basic Salary

                        //Provident Fund
                        try 
	                    {	        
		                    previousPF = previousBasic * 10 / 100;
	                    }
	                    catch 
	                    {
                            previousPF = 0;
	                    }
                        
                        thisMonthPF = salaryDet.pf_amount;
                        currentPF = currentBasic * 10 / 100;
                        projectedPF = currentPF * _reminingMonth;
                        actualPF = thisMonthPF * _actualMonth;
                        //yearlyPF = projectedPF + actualPF;
                        yearlyPF = previousPF + currentPF + projectedPF; // Chnge for getting previous payment in PF
                        //Provident Fund

                        allowList = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == _salaryPrss.id && x.emp_id == item.id).ToList();
                        if (allowList.Count > 0)
                        {
                            string allWName = "";
                            foreach (var allW in allowList)
                            {
                                var AllwConfig = dataContext.prl_allowance_configuration.FirstOrDefault(q => q.allowance_name_id == allW.allowance_name_id);
                                if (AllwConfig.is_taxable == 1)
                                {
                                    allWName = dataContext.prl_allowance_name.FirstOrDefault(a => a.id == allW.allowance_name_id).allowance_name;
                                    EmployeeSalaryAllowance salAllW = new EmployeeSalaryAllowance();
                                    salAllW.allowanceid = allW.allowance_name_id;
                                    salAllW.allowancename = allWName;
                                    salAllW.this_month_amount = allW.amount;


                                    //for (int i = fiscalYearStart; i < salaryMonth.Month; i++)
                                    //{
                                    //    try
                                    //    {
                                    //        var v_Allowance = new prl_salary_allowances();
                                    //        v_Allowance = dataContext.prl_salary_allowances.FirstOrDefault(e => e.emp_id == item.id && e.salary_month.Month == salaryMonth.Month && e.salary_month.Year == salaryMonth.Year);
                                    //        if (v_Allowance != null)
                                    //        {
                                    //            salAllW.actual_amount += (v_Allowance.amount + v_Allowance.arrear_amount.Value);
                                    //        }
                                    //    }
                                    //    catch
                                    //    {

                                    //    }
                                    //}

                                    #region previous Allowances

                                    var allw_cmd = @"SELECT SUM(amount) + SUM(IFNULL(arrear_amount,0)) FROM prl_salary_allowances WHERE salary_month BETWEEN ?s_date AND ?e_date AND emp_id = ?emp_id AND allowance_name_id = ?allw_name_id;";
                                    mySqlCommand.Connection = mySqlConnection;
                                    mySqlCommand.CommandText = allw_cmd;
                                    mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                    mySqlCommand.Parameters.AddWithValue("?s_date", s_date);
                                    mySqlCommand.Parameters.AddWithValue("?e_date", e_date);
                                    mySqlCommand.Parameters.AddWithValue("?allw_name_id", allW.allowance_name_id);

                                    string allwVal = "";
                                    using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                                    {
                                        foreach (var dr in msReader)
                                        {
                                            if (System.DBNull.Value != null)
                                            {
                                                while (msReader.Read())
                                                    allwVal = msReader.GetString(0);
                                            }
                                        }
                                    }
                                    mySqlCommand.Parameters.Clear();
                                    if (allwVal != "")
                                        salAllW.actual_amount = decimal.Parse(allwVal);

                                    #endregion


                                    if (AllwConfig.percent_amount > 0)
                                    {
                                        salAllW.current_amount = (currentBasic * AllwConfig.percent_amount / 100).Value;
                                    }
                                    else
                                    {
                                        salAllW.current_amount = AllwConfig.flat_amount.Value;
                                    }
                                    salAllW.projected_amount = salAllW.current_amount * _reminingMonth;
                                    //salAllW.actual_amount = salAllW.this_month_amount * _actualMonth;
                                    salAllW.yearly_amount = salAllW.actual_amount + salAllW.projected_amount;
                                    
                                    //if (AllwConfig.exempted_amount > 0)
                                    //    salAllW.exempted_amount = AllwConfig.exempted_amount.Value;

                                    salallList.Add(salAllW);
                                }
                            }
                        }

                        // ToDo ::
                        // 365 days.. actual basic
                        // basic*2/365*no of days from joining to 31st december
                        //Festival Bonus

                        //string dtDate = "31/12/" + salaryMonth.Year.ToString();
                        string dtDate = salaryMonth.Year.ToString() + "-12-" + "31"; //"31/12/" + salaryMonth.Year.ToString();
                        DateTime dtLastDayOfYear = DateTime.ParseExact(dtDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        //DateTime dtLastDayOfYear = Convert.ToDateTime(dtDate);
                        long bonusDays = 0;
                        if (item.joining_date.Year == salaryMonth.Year)
                        {
                            bonusDays = CommonDateClass.DateDifference(item.joining_date, dtLastDayOfYear);
                        }
                        else
                        {
                            bonusDays = 365;
                        }
                        yearlyFestival = (currentBasic * 2 / 365) * bonusDays;

                        #region Bonus Code for CBL
                        //List<prl_bonus_process> _bonusMasterList = _context.prl_bonus_process.Where(e => e.festival_date.Value.Month == 8 && e.festival_date.Value.Year == salaryMonth.Year).ToList();
                        //prl_bonus_process_detail _bonusDetail = new prl_bonus_process_detail();
                        //if (_bonusMasterList.Count > 0)
                        //{
                        //    if (_bonusMasterList.Count == 2)
                        //    {
                        //        foreach (var item1 in _bonusMasterList)
                        //        {
                        //            _bonusDetail = _context.prl_bonus_process_detail.FirstOrDefault(e => e.bonus_process_id == item1.id && e.emp_id == item.id);
                        //            actualFestival += _bonusDetail.amount;
                        //            yearlyFestival = actualFestival;
                        //        }
                        //    }
                        //    else if (_bonusMasterList.Count == 1)
                        //    {
                        //        foreach (var item2 in _bonusMasterList)
                        //        {
                        //            _bonusDetail = new prl_bonus_process_detail();
                        //            _bonusDetail = _context.prl_bonus_process_detail.FirstOrDefault(e => e.bonus_process_id == item2.id && e.emp_id == item.id);
                        //            if (_bonusDetail != null)
                        //            {
                        //                actualFestival = _bonusDetail.amount;
                        //                projectedFestival = currentBasic;
                        //                yearlyFestival = actualFestival + projectedFestival;
                        //            }
                        //            else
                        //            {
                        //                yearlyFestival = currentBasic;
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    yearlyFestival = currentBasic * 2;
                        //}
                        #endregion

                        //Festival Bonus

                        decimal currentChildAllowName = 0;
                        decimal projectedChilAllowance = 0;
                        decimal yearlyChildAllowance = 0;
                        //ToDo: Childer Allowance should be projected and actual and added to taxable income
                        prl_employee_children_allowance childAllw = dataContext.prl_employee_children_allowance.FirstOrDefault(c => c.emp_id == item.id && c.is_active == 1);
                        if (childAllw != null)
                        {
                            currentChildAllowName = childAllw.amount;
                            projectedChilAllowance = currentChildAllowName * _reminingMonth;
                            yearlyChildAllowance = currentChildAllowName + projectedChilAllowance;
                        }
                        //ToDo:


                        var taxDetail = new prl_income_tax_parameter_details();

                        //Free Car
                        freeCar = 0;
                        decimal free_car_rate = 0;
                        try
                        {
                            taxDetail = dataContext.prl_income_tax_parameter_details.FirstOrDefault(e => e.fiscal_year_id == pFiscalYear);
                            if (taxDetail != null)
                                free_car_rate = taxDetail.free_car.Value;
                        }
                        catch
                        {

                            free_car_rate = 0;
                        }

                        prl_employee_free_car _empFreeCar = dataContext.prl_employee_free_car.FirstOrDefault(e => e.emp_id == item.id);
                        if (_empFreeCar != null)
                        {
                            freeCar = yearlyBasic * free_car_rate / 100;
                        }
                        //Free Carn

                        //Total Taxable Income
                        // Need apply changes which one is 100% taxable

                        taxableIncome = yearlyBasic + yearlyFestival + yearlyPF + freeCar + yearlyChildAllowance;

                        decimal totalTaxableTransport = 0;
                        decimal actualTransportExemption = 0;

                        decimal totalTaxableHouse = 0;
                        decimal actualHouseExemption = 0;

                        decimal totalTaxableLFA = 0;
                        decimal actualLFAExemtion = 0;

                        decimal totalTaxableMedical = 0;
                        decimal actualMedicalExemption = 0;

                        foreach (var a in salallList)
                        {
                            if (a.allowancename.Contains("House"))
                            {
                                totalTaxableHouse = a.yearly_amount - taxDetail.house_rent_not_exceding.Value;
                                actualHouseExemption = taxDetail.house_rent_not_exceding.Value;
                                if (totalTaxableHouse < 0)
                                {
                                    totalTaxableHouse = 0;
                                    actualHouseExemption = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableHouse;
                            }
                            else if (a.allowancename.Contains("Medical"))
                            {
                                totalTaxableMedical = a.yearly_amount - (a.yearly_amount / taxDetail.medical_exemtion_percentage.Value * 100);
                                actualHouseExemption = (a.yearly_amount / taxDetail.medical_exemtion_percentage.Value * 100);
                                if (totalTaxableMedical < 0)
                                {
                                    totalTaxableMedical = 0;
                                    actualHouseExemption = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableMedical;
                            }
                            else if (a.allowancename.Contains("Transport"))
                            {
                                totalTaxableTransport = a.yearly_amount - taxDetail.max_conveyance_allowance_monthly.Value;
                                actualTransportExemption = taxDetail.max_conveyance_allowance_monthly.Value;
                                if (totalTaxableTransport < 0)
                                {
                                    totalTaxableTransport = 0;
                                    actualTransportExemption = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableTransport;
                            }
                            else if (a.allowancename.Contains("LFA"))
                            {
                                totalTaxableLFA = a.yearly_amount - (a.yearly_amount / taxDetail.lfa_exemtion_percentage.Value * 100);
                                actualLFAExemtion = (a.yearly_amount / taxDetail.lfa_exemtion_percentage.Value * 100);
                                if (totalTaxableLFA < 0)
                                {
                                    totalTaxableLFA = 0;
                                    actualLFAExemtion = a.yearly_amount;
                                }
                                taxableIncome += totalTaxableLFA;
                            }
                            else
                            {
                                taxableIncome += a.yearly_amount;
                            }
                        }

                        List<prl_over_time_amount> otList = dataContext.prl_over_time_amount.Where(o => o.emp_id == item.id && o.month_year.Month == salaryMonth.Month && o.month_year.Year == salaryMonth.Year).ToList();
                        if (otList != null)
                        {
                            foreach (var _ot in otList)
                            {
                                if(_ot.pay_total != null)
                                    taxableIncome += _ot.pay_total.Value;
                            }
                        }
                        //Total Taxable Income

                        //Tax Parameter Settings
                        if (taxDetail != null)
                        {
                            max_investment_allowed = taxDetail.max_investment_amount.Value;
                            max_investment_Pecentage_allowed = taxDetail.max_investment_percentage.Value;
                            taxAge = taxDetail.max_tax_age.Value;
                            min_tax = taxDetail.min_tax_amount.Value;
                        }
                        //Tax Parameter Settings

                        //Tax Slab
                        List<prl_income_tax_parameter> taxSlab = new List<prl_income_tax_parameter>();
                        taxSlab = dataContext.prl_income_tax_parameter.Where(objID => (objID.fiscal_year_id == pFiscalYear && objID.gender == item.gender)).ToList();
                        int maxNumberItem = taxSlab.Count;
                        decimal lastSlabAmount = taxSlab[maxNumberItem - 1].slab_maximum_amount - taxSlab[maxNumberItem - 2].slab_maximum_amount;
                        decimal lastSlabPercentage = taxSlab[maxNumberItem - 1].slab_percentage;
                        decimal secondlastSlabAmount = taxSlab[maxNumberItem - 2].slab_maximum_amount - taxSlab[maxNumberItem - 3].slab_maximum_amount;
                        decimal secondlastSlabPercentage = taxSlab[maxNumberItem - 2].slab_percentage;
                        decimal thirdlastSlabAmount = taxSlab[maxNumberItem - 3].slab_maximum_amount - taxSlab[maxNumberItem - 4].slab_maximum_amount;
                        decimal thidlastSlabPercentage = taxSlab[maxNumberItem - 3].slab_percentage;
                        decimal forthlastSlabAmount = taxSlab[maxNumberItem - 4].slab_maximum_amount - taxSlab[maxNumberItem - 5].slab_maximum_amount;
                        decimal forthlastSlabPercentage = taxSlab[maxNumberItem - 4].slab_percentage;
                        decimal fifthlastSlabAmount = taxSlab[maxNumberItem - 5].slab_maximum_amount;
                        decimal fifthlastSlabPercentage = taxSlab[maxNumberItem - 5].slab_percentage;

                        decimal _TaxableIncome = taxableIncome;
                        decimal TaxPayableAmount = 0;

                        decimal firstSlabAmount = 0;
                        decimal secondSlabAmount = 0;
                        decimal thirdSlabAmount = 0;
                        decimal forthSlabAmount = 0;
                        decimal fifthSlabAmount = 0;
                        decimal sisthSlabAmount = 0;

                        ArrayList arrys = new ArrayList();
                        List<prl_employee_tax_slab> taxItemList = new List<prl_employee_tax_slab>();
                        prl_employee_tax_slab taxItem;

                        if (_TaxableIncome <= fifthlastSlabAmount)
                        {
                            TaxPayableAmount = 0;
                            taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, fifthlastSlabAmount, fifthlastSlabPercentage, _TaxableIncome, TaxPayableAmount);
                            taxItemList.Add(taxItem);
                        }
                        if (_TaxableIncome > fifthlastSlabAmount)
                        {
                            decimal netTaxPayableAmount = _TaxableIncome - fifthlastSlabAmount;

                            taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, fifthlastSlabAmount, fifthlastSlabPercentage, fifthlastSlabAmount, 0);
                            taxItemList.Add(taxItem);

                            if (netTaxPayableAmount <= forthlastSlabAmount)
                            {
                                TaxPayableAmount = (netTaxPayableAmount * forthlastSlabPercentage) / 100;

                                taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, forthlastSlabAmount, forthlastSlabPercentage, netTaxPayableAmount, TaxPayableAmount);
                                taxItemList.Add(taxItem);
                            }

                            if (netTaxPayableAmount > forthlastSlabAmount)
                            {

                                decimal reminderAmount = netTaxPayableAmount - forthlastSlabAmount;
                                firstSlabAmount = (forthlastSlabAmount * forthlastSlabPercentage) / 100;

                                taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, forthlastSlabAmount, forthlastSlabPercentage, forthlastSlabAmount, firstSlabAmount);
                                taxItemList.Add(taxItem);

                                if (reminderAmount <= thirdlastSlabAmount)
                                {
                                    secondSlabAmount = (reminderAmount * thidlastSlabPercentage) / 100;

                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, thirdlastSlabAmount, thidlastSlabPercentage, reminderAmount, secondSlabAmount);
                                    taxItemList.Add(taxItem);

                                    TaxPayableAmount = firstSlabAmount + secondSlabAmount + thirdSlabAmount + forthSlabAmount + fifthSlabAmount + sisthSlabAmount;
                                }
                                if (reminderAmount > thirdlastSlabAmount)
                                {
                                    decimal secondReminderAmount = reminderAmount - thirdlastSlabAmount;
                                    thirdSlabAmount = (thirdlastSlabAmount * thidlastSlabPercentage) / 100;

                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, thirdlastSlabAmount, thidlastSlabPercentage, thirdlastSlabAmount, thirdSlabAmount);
                                    taxItemList.Add(taxItem);

                                    if (secondReminderAmount <= secondlastSlabAmount)
                                    {
                                        forthSlabAmount = (secondReminderAmount * secondlastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, secondlastSlabAmount, secondlastSlabPercentage, secondReminderAmount, forthSlabAmount);
                                        taxItemList.Add(taxItem);
                                    }

                                    if (secondReminderAmount > secondlastSlabAmount)
                                    {
                                        decimal thirdReminder = (secondReminderAmount - secondlastSlabAmount);
                                        fifthSlabAmount = (secondlastSlabAmount * secondlastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, secondlastSlabAmount, secondlastSlabPercentage, secondlastSlabAmount, fifthSlabAmount);
                                        taxItemList.Add(taxItem);

                                        sisthSlabAmount = (thirdReminder * lastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, lastSlabAmount, lastSlabPercentage, thirdReminder, sisthSlabAmount);
                                        taxItemList.Add(taxItem);
                                    }

                                    TaxPayableAmount = firstSlabAmount + secondSlabAmount + thirdSlabAmount + forthSlabAmount + fifthSlabAmount + sisthSlabAmount;

                                }
                            }
                        }
                        //Tax Slab

                        // Investment Rebate
                        decimal otherInvestment = 0;
                        decimal minRebate = 0;
                        decimal invRebate = (taxableIncome - yearlyPF) * taxDetail.max_investment_percentage.Value / 100;

                        otherInvestment = invRebate - (yearlyPF * 2); // OtherInvetment

                        decimal invPerRebate = taxDetail.max_inv_exempted_percentage.Value;

                        if (invRebate > max_investment_allowed)
                        {
                            minRebate = max_investment_allowed * invPerRebate / 100;
                        }
                        else
                        {
                            minRebate = invRebate * invPerRebate / 100;
                        }

                        //  ToDo:: Other Investment Should be incorporated

                        //

                        // Investment Rebate

                        //yearly Income Tax
                        double yearlyIncomeTax = 0;

                        if (TaxPayableAmount == 0)
                        {
                            yearlyIncomeTax = 0;
                        }
                        else if (minRebate > TaxPayableAmount)
                        {
                            yearlyIncomeTax = double.Parse(taxDetail.min_tax_amount.ToString());
                        }
                        else
                        {
                            if ((TaxPayableAmount - minRebate) <= taxDetail.min_tax_amount)
                            {
                                yearlyIncomeTax = double.Parse(taxDetail.min_tax_amount.ToString());
                            }
                            else
                            {
                                yearlyIncomeTax = double.Parse((TaxPayableAmount - minRebate).ToString());
                            }
                        }

                        // ToDo :: Tax Refund For Employee
                        decimal Tax_Refund = 0;
                        var taxRefund = dataContext.prl_income_tax_refund.FirstOrDefault(w => w.emp_id == item.id && w.fiscal_year_id == pFiscalYear && w.month_year == salaryMonth);
                        if (taxRefund != null)
                        {
                            Tax_Refund = taxRefund.refund_amount.Value;
                            yearlyIncomeTax = yearlyIncomeTax - double.Parse(Tax_Refund.ToString());
                        }
                        // yearlyIncomeTax = yearlyIncomeTax - TaxRefund

                        decimal _previousTax = 0;
                        double YearlyLiabilities = 0;
                        try
                        {
                            _previousTax = dataContext.prl_employee_tax_process.Where(e => e.fiscal_year_id == pFiscalYear && e.emp_id == item.id).Sum(q => q.monthly_tax);
                        }
                        catch
                        {
                            _previousTax = 0;
                        }
                        yearlyIncomeTax = yearlyIncomeTax - double.Parse(_previousTax.ToString());
                        YearlyLiabilities = yearlyIncomeTax - double.Parse(_previousTax.ToString());
                        //yearly Income Tax

                        //Monthly Tax
                        double MonthlyTax = 0;
                        MonthlyTax = YearlyLiabilities / TaxRemainingMonth(salaryMonth.Month);
                        //Monthly Tax

                        e_id = item.id;
                        #region Save Data
                        
                        tran = mySqlConnection.BeginTransaction();
                        int taxProcessId = 0;
                        mySqlCommand.Parameters.Clear();
                        var taxprocessText = @"INSERT INTO prl_employee_tax_process
                                                (emp_id,salary_process_id,fiscal_year_id,salary_month,yearly_tax,monthly_tax,created_by,created_date)
                                                VALUES (?emp_id,?salary_process_id,?fiscal_year_id,?salary_month,?yearly_tax,?monthly_tax,?created_by,?created_date);";

                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = taxprocessText;
                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                        mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                        mySqlCommand.Parameters.AddWithValue("?fiscal_year_id", pFiscalYear);
                        mySqlCommand.Parameters.AddWithValue("?salary_month", salaryMonth);
                        mySqlCommand.Parameters.AddWithValue("?yearly_tax", decimal.Parse(yearlyIncomeTax.ToString()));
                        mySqlCommand.Parameters.AddWithValue("?monthly_tax", decimal.Parse(MonthlyTax.ToString()));
                        mySqlCommand.Parameters.AddWithValue("?created_by", processUser);
                        mySqlCommand.Parameters.AddWithValue("?created_date", DateTime.Now);

                        mySqlCommand.ExecuteNonQuery();
                        taxProcessId = (int)mySqlCommand.LastInsertedId;
                        tran.Commit();
                        mySqlCommand.Parameters.Clear();
                        //Basic
                        

                        tran2 = mySqlConnection.BeginTransaction();
                        mySqlCommand.Parameters.Clear();
                        string taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item,gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item,?gross_annual_income,?less_exempted,?total_taxable_income);";

                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = taxprocessDetText;
                        mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                        mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                        mySqlCommand.Parameters.AddWithValue("?tax_item", "Basic");
                        mySqlCommand.Parameters.AddWithValue("?gross_annual_income", yearlyBasic);
                        mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                        mySqlCommand.Parameters.AddWithValue("?total_taxable_income", yearlyBasic);
                        mySqlCommand.ExecuteNonQuery();
                        //tran2.Commit();

                        //All Allowances
                        foreach (var sal in salallList)
                        {
                            mySqlCommand.Parameters.Clear();
                            taxprocessDetText = "";
                            taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item,gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item,?gross_annual_income,?less_exempted,?total_taxable_income);";

                            mySqlCommand.Connection = mySqlConnection;
                            mySqlCommand.CommandText = taxprocessDetText;

                            mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                            mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                            mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                            mySqlCommand.Parameters.AddWithValue("?tax_item", sal.allowancename);
                            mySqlCommand.Parameters.AddWithValue("?gross_annual_income", sal.yearly_amount);

                            if (sal.allowancename.Contains("House"))
                            {
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", actualHouseExemption);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableHouse);
                            }
                            else if (sal.allowancename.Contains("Medical"))
                            {
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", actualHouseExemption);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableHouse);
                            }
                            else if (sal.allowancename.Contains("Transport"))
                            {
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", actualHouseExemption);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableHouse);
                            }
                            else if (sal.allowancename.Contains("LFA"))
                            {
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", actualHouseExemption);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableHouse);
                            }
                            else
                            {
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", actualHouseExemption);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableHouse);
                            }
                            mySqlCommand.ExecuteNonQuery();

                        }

                        //Bonus

                        mySqlCommand.Parameters.Clear();
                        taxprocessDetText = "";
                        taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item,gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item,?gross_annual_income,?less_exempted,?total_taxable_income);";

                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = taxprocessDetText;

                        mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                        mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                        mySqlCommand.Parameters.AddWithValue("?tax_item", "Bonus");
                        mySqlCommand.Parameters.AddWithValue("?gross_annual_income", yearlyFestival);
                        mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                        mySqlCommand.Parameters.AddWithValue("?total_taxable_income", yearlyFestival);
                        mySqlCommand.ExecuteNonQuery();


                        //PF
                        mySqlCommand.Parameters.Clear();
                        taxprocessDetText = "";
                        taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item,gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item,?gross_annual_income,?less_exempted,?total_taxable_income);";

                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = taxprocessDetText;

                        mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                        mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                        mySqlCommand.Parameters.AddWithValue("?tax_item", "PF");
                        mySqlCommand.Parameters.AddWithValue("?gross_annual_income", yearlyPF);
                        mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                        mySqlCommand.Parameters.AddWithValue("?total_taxable_income", yearlyPF);
                        mySqlCommand.ExecuteNonQuery();

                        //Over Time
                        foreach (var _ot in otList)
                        {
                            var ot_name = dataContext.prl_over_time_configuration.FirstOrDefault(t => t.id == _ot.over_time_config_id);
                            if (ot_name != null)
                            {
                                mySqlCommand.Parameters.Clear();
                                taxprocessDetText = "";
                                taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item,gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item,?gross_annual_income,?less_exempted,?total_taxable_income);";

                                mySqlCommand.Connection = mySqlConnection;
                                mySqlCommand.CommandText = taxprocessDetText;

                                mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                                mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                                mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                mySqlCommand.Parameters.AddWithValue("?tax_item", ot_name.name);
                                mySqlCommand.Parameters.AddWithValue("?gross_annual_income", _ot.pay_total);
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", _ot.pay_total);
                                mySqlCommand.ExecuteNonQuery();
                            }
                        }

                        //Children Allowance
                        if (childAllw != null)
                        {
                            mySqlCommand.Parameters.Clear();
                            taxprocessDetText = "";
                            taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item,gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item,?gross_annual_income,?less_exempted,?total_taxable_income);";

                            mySqlCommand.Connection = mySqlConnection;
                            mySqlCommand.CommandText = taxprocessDetText;

                            mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                            mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                            mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                            mySqlCommand.Parameters.AddWithValue("?tax_item", "Acting/Child Edu/Transfer/Others Allowance");
                            mySqlCommand.Parameters.AddWithValue("?gross_annual_income", childAllw.amount);
                            mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                            mySqlCommand.Parameters.AddWithValue("?total_taxable_income", childAllw.amount);
                            mySqlCommand.ExecuteNonQuery();
                        }

                        //Tax Slab
                        foreach (var _tax in taxItemList)
                        {
                            mySqlCommand.Parameters.Clear();
                            string taxprocessSlab = "";
                            taxprocessSlab = @"INSERT INTO prl_employee_tax_slab
                                                    (emp_id,tax_process_id,salary_process_id,fiscal_year_id,salary_date,salary_month,salary_year,current_rate,parameter,
                                                        taxable_income,tax_liability,created_by,created_date)
                                                    VALUES (?emp_id,?tax_process_id,?salary_process_id,?fiscal_year_id,?salary_date,?salary_month,?salary_year,?current_rate,?parameter,
                                                        ?taxable_income,?tax_liability,?created_by,?created_date);";

                            mySqlCommand.Connection = mySqlConnection;
                            mySqlCommand.CommandText = taxprocessSlab;

                            mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                            mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                            mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                            mySqlCommand.Parameters.AddWithValue("?fiscal_year_id", pFiscalYear);
                            mySqlCommand.Parameters.AddWithValue("?salary_date", salaryMonth);
                            mySqlCommand.Parameters.AddWithValue("?salary_month", salaryMonth.Month);
                            mySqlCommand.Parameters.AddWithValue("?salary_year", salaryMonth.Year);
                            mySqlCommand.Parameters.AddWithValue("?current_rate", _tax.current_rate);
                            mySqlCommand.Parameters.AddWithValue("?parameter", _tax.parameter);
                            mySqlCommand.Parameters.AddWithValue("?taxable_income", _tax.taxable_income);
                            mySqlCommand.Parameters.AddWithValue("?tax_liability", _tax.tax_liability);
                            mySqlCommand.Parameters.AddWithValue("?created_by", processUser);
                            mySqlCommand.Parameters.AddWithValue("?created_date", DateTime.Now);
                            mySqlCommand.ExecuteNonQuery();
                        }

                        if (decimal.Parse(MonthlyTax.ToString()) > 0)
                        {
                            mySqlCommand.Parameters.Clear();
                            string updateSalProcessDet = "";
                            updateSalProcessDet = @"UPDATE prl_salary_process_detail
                                                    SET total_monthly_tax = ?total_monthly_tax
                                                WHERE salary_process_id = ?salary_process_id AND emp_id = ?emp_id;";

                            mySqlCommand.Connection = mySqlConnection;
                            mySqlCommand.CommandText = updateSalProcessDet;

                            mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                            mySqlCommand.Parameters.AddWithValue("?salary_process_id", salaryDet.salary_process_id);
                            mySqlCommand.Parameters.AddWithValue("?total_monthly_tax", decimal.Parse(MonthlyTax.ToString()));
                            mySqlCommand.ExecuteNonQuery();
                        }

                        tran2.Commit();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("--- Error in salary process saving emp data emp id = " + item.id);
                    Trace.WriteLine("--- Error in salary process saving emp data msg = " + ex.Message);
                    result.ErrorOccured = true;
                    result.AddToErrorList("Problem in tax process.");
                    if (tran2 != null)
                        tran2.Rollback();
                }
            }
            return result;
        }

        private static prl_employee_tax_slab GetTaxCertificateSlabWiseItem(int empid, int pFiscalYrID, DateTime salaryMonth, decimal onNextAmount, decimal currentPercentage, decimal TaxableIncome, decimal taxLiability)
        {
            prl_employee_tax_slab taxItem;
            taxItem = new prl_employee_tax_slab();
            taxItem.emp_id = empid;
            taxItem.fiscal_year_id = pFiscalYrID;
            taxItem.current_rate = currentPercentage;
            taxItem.taxable_income = TaxableIncome;
            taxItem.tax_liability = taxLiability;
            taxItem.parameter = "On Next " + onNextAmount;
            taxItem.salary_month = salaryMonth.Month;
            taxItem.salary_year = salaryMonth.Year;
            taxItem.salary_date = salaryMonth;
            return taxItem;
        }

        private static int FindProjectedMonth(int _month)
        {
            int _projectedMonth = 0;
            if (_month == 7)
            {
                _projectedMonth = 11;
            }
            else if (_month == 8)
            {
                _projectedMonth = 10;
            }
            else if (_month == 9)
            {
                _projectedMonth = 9;
            }
            else if (_month == 10)
            {
                _projectedMonth = 8;
            }
            else if (_month == 11)
            {
                _projectedMonth = 7;
            }
            else if (_month == 12)
            {
                _projectedMonth = 6;
            }
            else if (_month == 1)
            {
                _projectedMonth = 5;
            }
            else if (_month == 2)
            {
                _projectedMonth = 4;
            }
            else if (_month == 3)
            {
                _projectedMonth = 3;
            }
            else if (_month == 4)
            {
                _projectedMonth = 2;
            }
            else if (_month == 5)
            {
                _projectedMonth = 1;
            }
            else if (_month == 6)
            {
                _projectedMonth = 0;
            }
            return _projectedMonth;
        }

        private static int FindActualMonth(int _month)
        {
            int _actualMonth = 0;
            if (_month == 7)
            {
                _actualMonth = 1;
            }
            else if (_month == 8)
            {
                _actualMonth = 2;
            }
            else if (_month == 9)
            {
                _actualMonth = 3;
            }
            else if (_month == 10)
            {
                _actualMonth = 4;
            }
            else if (_month == 11)
            {
                _actualMonth = 5;
            }
            else if (_month == 12)
            {
                _actualMonth = 6;
            }
            else if (_month == 1)
            {
                _actualMonth = 7;
            }
            else if (_month == 2)
            {
                _actualMonth = 8;
            }
            else if (_month == 3)
            {
                _actualMonth = 9;
            }
            else if (_month == 4)
            {
                _actualMonth = 10;
            }
            else if (_month == 5)
            {
                _actualMonth = 11;
            }
            else if (_month == 6)
            {
                _actualMonth = 12;
            }
            return _actualMonth;
        }

        private static int TaxRemainingMonth(int _month)
        {
            int _TaxRemainingMonth = 0;
            if (_month == 7)
            {
                _TaxRemainingMonth = 12;
            }
            else if (_month == 8)
            {
                _TaxRemainingMonth = 11;
            }
            else if (_month == 9)
            {
                _TaxRemainingMonth = 10;
            }
            else if (_month == 10)
            {
                _TaxRemainingMonth = 9;
            }
            else if (_month == 11)
            {
                _TaxRemainingMonth = 8;
            }
            else if (_month == 12)
            {
                _TaxRemainingMonth = 7;
            }
            else if (_month == 1)
            {
                _TaxRemainingMonth = 6;
            }
            else if (_month == 2)
            {
                _TaxRemainingMonth = 5;
            }
            else if (_month == 3)
            {
                _TaxRemainingMonth = 4;
            }
            else if (_month == 4)
            {
                _TaxRemainingMonth = 3;
            }
            else if (_month == 5)
            {
                _TaxRemainingMonth = 2;
            }
            else if (_month == 6)
            {
                _TaxRemainingMonth = 1;
            }
            return _TaxRemainingMonth;
        }
    }
}