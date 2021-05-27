using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using com.linde.DataContext;
using com.linde.Model;
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

            //Festival
            decimal currentFestival = 0;
            decimal projectedFestival = 0;
            decimal actualFestival = 0;
            decimal yearlyFestival = 0;
            //Festival

            int _reminingMonth = 0;
            int _actualMonth = 0;

            //Tax
            decimal taxableIncome = 0;
            //Tax

            //PF
            decimal thisMonthPF = 0;
            decimal currentPF = 0;
            decimal projectedPF = 0;
            decimal actualPF = 0;
            decimal yearlyPF = 0;
            decimal previousPF = 0;

            //PF
            int fiscalYrStartMonth = 7;

            foreach (var item in employeeList)
            {
                List<prl_salary_allowances> allowList = new List<prl_salary_allowances>();
                List<EmployeeSalaryAllowance> salallList = new List<EmployeeSalaryAllowance>();

                MySqlTransaction tran = null;
                MySqlTransaction tran2 = null;
                try
                {

                    var _salaryPrss = dataContext.prl_salary_process.FirstOrDefault(s => s.batch_no == batchNo);

                    if (_salaryPrss != null)
                    {
                        var SpDetails = dataContext.prl_salary_process_detail.FirstOrDefault(s => s.salary_process_id == _salaryPrss.id && s.emp_id == item.id);

                        if (SpDetails != null && SpDetails.calculation_for_days != 0)
                        {

                            //For Finding yearly Basic
                            thisMonthBasic = 0; currentBasic = 0;
                            currentBasic = 0; projectedBasic = 0; actualBasic = 0; yearlyBasic = 0;
                            //For Finding yearly Basic

                            //PF
                            thisMonthPF = 0; currentPF = 0; projectedPF = 0; actualPF = 0; yearlyPF = 0;
                            //PF

                            //Festival
                            actualFestival = 0; currentFestival = 0; projectedFestival = 0; yearlyFestival = 0;
                            //Festival

                            //Basic Salary


                            //for (int i = fiscalYrStartMonth; i < salaryMonth.Month; i++)
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
                            string s_date = "";

                            if (currMonth > 6)
                            {
                                s_date = fiscal_Yr.ToString() + "-" + fiscalYrStartMonth + "-" + "1";
                            }
                            else
                            {
                                s_date = (fiscal_Yr - 1).ToString() + "-" + fiscalYrStartMonth + "-" + "1";
                            }


                            if (currMonth == 1)
                            {
                                e_date = (fiscal_Yr - 1).ToString() + "-" + 12 + "-" + 31;
                            }
                            else if (currMonth == 7)
                            {
                                e_date = fiscal_Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
                            }
                            else
                            {
                                e_date = fiscal_Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + CommonDateClass.LastDateForCurrentMonth(salaryMonth.AddMonths(-1)).Day;
                            }

                            DateTime frmDate = Convert.ToDateTime(s_date);
                            DateTime toDate = Convert.ToDateTime(e_date);
                            DateTime endDateFsYr = Convert.ToDateTime(frmDate.Year + 1 + "-6-30");

                            mySqlCommand.Parameters.Clear();
                            var select_cmd = @"SELECT IFNULL(SUM(this_month_basic), 0) FROM prl_salary_process_detail WHERE salary_month BETWEEN ?s_datee AND ?e_datee AND emp_id = ?emp_iid;";
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
                                        //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
                                        resVal = msReader.GetString(0);
                                    }
                                }
                            }

                            if (resVal != "")
                                previousBasic = decimal.Parse(resVal);
                            #endregion


                            currentBasic = item.prl_employee_details.Where(x => x.emp_id == item.id).First().basic_salary;
                            currentBasic = SpDetails.current_basic; // Current Basic
                            thisMonthBasic = SpDetails.this_month_basic.Value; // This Month Basic 
                            _reminingMonth = FindProjectedMonth(salaryMonth.Month);
                            projectedBasic = SpDetails.current_basic * _reminingMonth; // projected basic
                            _actualMonth = FindActualMonth(salaryMonth.Month);
                            //actualBasic = thisMonthBasic * _actualMonth; // Actual Basic
                            //yearlyBasic = projectedBasic + actualBasic; // yearly Basic
                            yearlyBasic = previousBasic + thisMonthBasic + projectedBasic; // Chnge for getting previous payment in Basic Salary
                            //Basic Salary

                            //PF

                            #region previous PF

                            mySqlCommand.Parameters.Clear();
                            var select_command = @"SELECT IFNULL(SUM(pf_amount), 0) FROM prl_salary_process_detail WHERE salary_month BETWEEN ?s_datee AND ?e_datee AND emp_id = ?emp_iid;";
                            mySqlCommand.Connection = mySqlConnection;
                            mySqlCommand.CommandText = select_command;
                            mySqlCommand.Parameters.AddWithValue("?emp_iid", item.id);
                            mySqlCommand.Parameters.AddWithValue("?s_datee", s_date);
                            mySqlCommand.Parameters.AddWithValue("?e_datee", e_date);


                            string resPfVal = "";
                            using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                            {

                                foreach (var dr in msReader)
                                {
                                    if (System.DBNull.Value != null)
                                    {
                                        //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
                                        resPfVal = msReader.GetString(0);
                                    }
                                }
                            }

                            if (resPfVal != "")
                                previousPF = decimal.Parse(resPfVal);
                            #endregion

                            currentPF = SpDetails != null ? (decimal?)SpDetails.pf_amount ?? 0 : 0;
                            //thisMonthPF = SpDetails.pf_amount;
                            projectedPF = currentPF * _reminingMonth;

                            yearlyPF = previousPF + currentPF + projectedPF; // Change for getting previous payment in PF
                            //Provident Fund

                            allowList = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == _salaryPrss.id && x.emp_id == item.id).ToList();
                            if (allowList.Count > 0)
                            {
                                string allWName = "";
                                foreach (var allW in allowList)
                                {
                                    allWName = dataContext.prl_allowance_name.FirstOrDefault(a => a.id == allW.allowance_name_id).allowance_name;
                                    if (allWName != "Basic Salary")
                                    {
                                        var AllwConfig = dataContext.prl_allowance_configuration.FirstOrDefault(q => q.allowance_name_id == allW.allowance_name_id);
                                        if (AllwConfig.is_taxable == 1)
                                        {
                                            EmployeeSalaryAllowance salAllW = new EmployeeSalaryAllowance();
                                            salAllW.allowanceid = allW.allowance_name_id;
                                            salAllW.allowancename = allWName;
                                            salAllW.this_month_amount = allW.amount + ((decimal?)allW.arrear_amount ?? 0);

                                            #region previous Allowances

                                            var allw_cmd = @"SELECT IFNULL(SUM(amount), 0)  + IFNULL(SUM(arrear_amount),0) FROM prl_salary_allowances WHERE salary_month BETWEEN ?s_date AND ?e_date AND emp_id = ?emp_id AND allowance_name_id = ?allw_name_id;";
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
                                                        //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
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
                                                if (item.id == 21)
                                                {
                                                    if (salAllW.allowanceid == 2)
                                                    {
                                                        salAllW.current_amount = (currentBasic * (decimal)0.263157894736842);
                                                    }
                                                    else if (salAllW.allowanceid == 3)
                                                    {
                                                        salAllW.current_amount = (currentBasic * (decimal)0.105263157894737);
                                                    }
                                                    else if (salAllW.allowanceid == 4)
                                                    {
                                                        salAllW.current_amount = (currentBasic * (decimal)0.105263157894737);
                                                    }
                                                    else
                                                    {
                                                        salAllW.current_amount = (currentBasic * AllwConfig.percent_amount / 100).Value;
                                                    }
                                                }
                                                else
                                                {
                                                    salAllW.current_amount = (currentBasic * AllwConfig.percent_amount / 100).Value;
                                                }

                                            }
                                            else
                                            {
                                                salAllW.current_amount = allW.amount;
                                                //salAllW.current_amount = AllwConfig.flat_amount.Value; //commented by Lukman
                                            }

                                            if (allWName.Contains("House Rent Allowance") || allWName.Contains("Car/MC Allowance") || allWName.Contains("Conveyance Allowance"))
                                            {
                                                salAllW.projected_amount = salAllW.current_amount * _reminingMonth;
                                            }

                                            //salAllW.actual_amount = salAllW.this_month_amount * _actualMonth;
                                            //salAllW.yearly_amount = salAllW.actual_amount + salAllW.projected_amount; //Commented By Lukman

                                            salAllW.yearly_amount = salAllW.actual_amount + salAllW.this_month_amount + salAllW.projected_amount;

                                            //if (AllwConfig.exempted_amount > 0)
                                            //    salAllW.exempted_amount = AllwConfig.exempted_amount.Value;

                                            salallList.Add(salAllW);
                                        }
                                    }
                                }
                            }

                            //check other allowances which previously paid within the fiscal year but not in the current month
                            var lstAllowances = new List<int>();
                            var allAllowancesThisYr = new List<prl_salary_allowances>();
                            lstAllowances = salallList.AsEnumerable().Select(x => x.allowanceid).Distinct().ToList();
                            allAllowancesThisYr = dataContext.prl_salary_allowances.AsEnumerable().Where(x => x.salary_month >= frmDate && x.salary_month <= toDate && x.emp_id == item.id).GroupBy(x => x.allowance_name_id).Select(grp => grp.FirstOrDefault()).ToList();

                            foreach (var at in allAllowancesThisYr)
                            {
                                if (!lstAllowances.Contains(at.allowance_name_id))
                                {
                                    string allWName = "";

                                    var AllwConfig = dataContext.prl_allowance_configuration.FirstOrDefault(q => q.allowance_name_id == at.allowance_name_id);
                                    if (AllwConfig.is_taxable == 1)
                                    {
                                        allWName = dataContext.prl_allowance_name.FirstOrDefault(a => a.id == at.allowance_name_id).allowance_name;
                                        EmployeeSalaryAllowance salAllW = new EmployeeSalaryAllowance();
                                        salAllW.allowanceid = at.allowance_name_id;
                                        salAllW.allowancename = allWName;
                                        salAllW.this_month_amount = 0;

                                        #region previous Allowances

                                        var allw_cmd = @"SELECT IFNULL(SUM(amount), 0) + IFNULL(SUM(arrear_amount),0) FROM prl_salary_allowances WHERE salary_month BETWEEN ?s_date AND ?e_date AND emp_id = ?emp_id AND allowance_name_id = ?allw_name_id;";
                                        mySqlCommand.Connection = mySqlConnection;
                                        mySqlCommand.CommandText = allw_cmd;
                                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                        mySqlCommand.Parameters.AddWithValue("?s_date", s_date);
                                        mySqlCommand.Parameters.AddWithValue("?e_date", e_date);
                                        mySqlCommand.Parameters.AddWithValue("?allw_name_id", at.allowance_name_id);

                                        string allwVal = "";
                                        using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                                        {
                                            foreach (var dr in msReader)
                                            {
                                                if (System.DBNull.Value != null)
                                                {
                                                    //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
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
                                            salAllW.current_amount = 0;
                                            //salAllW.current_amount = AllwConfig.flat_amount.Value; //commented by Lukman
                                        }

                                        salAllW.projected_amount = salAllW.current_amount * _reminingMonth;

                                        salAllW.yearly_amount = salAllW.actual_amount + salAllW.current_amount + salAllW.projected_amount;

                                        //if (AllwConfig.exempted_amount > 0)
                                        // salAllW.exempted_amount = AllwConfig.exempted_amount.Value;

                                        salallList.Add(salAllW);
                                    }
                                }
                            }

                            int fsId = FindFiscalYear(salaryMonth);
                            string fsYear = dataContext.prl_fiscal_year.Where(x => x.id == fsId).FirstOrDefault().fiscal_year;
                            // ToDo ::
                            // 365 days.. actual basic
                            // basic*2/365*no of days from joining to 31st december

                            #region Festival Bonus Code

                            // consider "process date" because bonus can be paid the month before the festival month
                            // process_month and festival can be two separate fiscal year.

                            var bonusThisMonth = dataContext.prl_bonus_process.SingleOrDefault(e => e.process_date.Month == salaryMonth.Month && e.process_date.Year == salaryMonth.Year);

                            List<prl_bonus_process> _noOfBonusProcessedthisYr = dataContext.prl_bonus_process.Where(e => e.fiscal_year_id == fsId).ToList();

                            prl_bonus_process_detail _bonusDetail = new prl_bonus_process_detail();

                            // If he or she consider confirmation for tax then uncomment please
                            //if (item.is_confirmed == 1)
                            //{
                            var bonus_cmd = @"SELECT IFNULL(SUM(amount), 0) 
                            FROM prl_bonus_process_detail bpd
                            LEFT JOIN prl_bonus_process bp ON bp.id = bpd.bonus_process_id
                            LEFT JOIN prl_bonus_configuration bc ON bc.bonus_name_id = bp.bonus_name_id

                            WHERE bc.is_taxable = 'YES' AND
                            bc.effective_from BETWEEN ?s_date AND ?e_date AND emp_id = ?emp_id AND bp.fiscal_year_id = ?fiscal_year_id;";

                            mySqlCommand.Connection = mySqlConnection;
                            mySqlCommand.CommandText = bonus_cmd;
                            mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                            mySqlCommand.Parameters.AddWithValue("?s_date", s_date);
                            mySqlCommand.Parameters.AddWithValue("?e_date", e_date);
                            mySqlCommand.Parameters.AddWithValue("?fiscal_year_id", fsId);

                            string bonusVal = "";

                            using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                            {
                                foreach (var dr in msReader)
                                {
                                    if (System.DBNull.Value != null)
                                    {
                                        //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
                                        bonusVal = msReader.GetString(0);
                                    }
                                }
                            }

                            mySqlCommand.Parameters.Clear();
                            if (bonusVal != "")
                                actualFestival = decimal.Parse(bonusVal);

                            decimal festivalAmount = 0;

                            if (_noOfBonusProcessedthisYr.Count > 0)
                            {
                                if (_noOfBonusProcessedthisYr.Count >= 2)
                                {
                                    foreach (var item1 in _noOfBonusProcessedthisYr)
                                    {
                                        _bonusDetail = dataContext.prl_bonus_process_detail.FirstOrDefault(e => e.bonus_process_id == item1.id && e.emp_id == item.id);
                                        if (_bonusDetail != null)
                                        {
                                            var _bonusConfig = dataContext.prl_bonus_configuration.FirstOrDefault(e => e.bonus_name_id == _bonusDetail.prl_bonus_process.bonus_name_id && e.is_taxable.ToLower() == "yes");

                                            if (bonusThisMonth != null && bonusThisMonth.id == _bonusDetail.bonus_process_id)
                                            {
                                                currentFestival = _bonusDetail.amount;
                                            }
                                            if (_bonusConfig != null)
                                            {
                                                festivalAmount += _bonusDetail != null ? _bonusDetail.amount : 0;
                                                yearlyFestival = festivalAmount;
                                            }
                                        }
                                    }
                                }
                                else if (_noOfBonusProcessedthisYr.Count == 1)
                                {
                                    foreach (var item2 in _noOfBonusProcessedthisYr)
                                    {
                                        _bonusDetail = new prl_bonus_process_detail();
                                        prl_bonus_configuration _bnsConfig = new prl_bonus_configuration();
                                        _bonusDetail = dataContext.prl_bonus_process_detail.FirstOrDefault(e => e.bonus_process_id == item2.id && e.emp_id == item.id);
                                        _bnsConfig = dataContext.prl_bonus_configuration.FirstOrDefault(x => x.bonus_name_id == item2.bonus_name_id && x.is_taxable.ToLower() == "yes");

                                        if (_bonusDetail != null && _bnsConfig != null)
                                        {
                                            if (bonusThisMonth != null)
                                            {
                                                currentFestival = _bonusDetail.amount;
                                            }

                                            // If this month discontinued 

                                            if (_reminingMonth == 0)
                                            {
                                                projectedFestival = 0;
                                            }
                                            else
                                            {
                                                //commented projectedFestival for one Festival Bonus
                                                projectedFestival = (decimal)(currentBasic * (_bnsConfig.percentage_of_basic / 100));
                                            }

                                            yearlyFestival = actualFestival + currentFestival + projectedFestival;
                                        }
                                        else
                                        {
                                            // If this month discontinued 

                                            if (_reminingMonth == 0)
                                            {
                                                projectedFestival = 0;
                                            }
                                            else
                                            {
                                                //commented projectedFestival for one Festival Bonus
                                                projectedFestival = (decimal)(currentBasic * (_bnsConfig.percentage_of_basic / 100));
                                            }

                                            yearlyFestival = actualFestival + currentFestival + projectedFestival;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                //commented projectedFestival for one Festival Bonus
                                projectedFestival = currentBasic;
                                yearlyFestival = currentBasic;
                            }
                            //}


                            #endregion

                            //Festival Bonus

                            //Festival Bonus

                            var taxDetail = new prl_income_tax_parameter_details();
                            //Free Car
                            decimal freeCar = 0;
                            decimal free_car_rate = 0;
                            try
                            {
                                taxDetail = dataContext.prl_income_tax_parameter_details.FirstOrDefault(e => e.fiscal_year_id == pFiscalYear);
                                if (taxDetail != null)
                                    free_car_rate = taxDetail.free_car.Value / 100;
                            }
                            catch
                            {

                                free_car_rate = 0;
                            }

                            prl_employee_free_car _empFreeCar = dataContext.prl_employee_free_car.FirstOrDefault(e => e.emp_id == item.id);
                            if (_empFreeCar != null)
                            {
                                freeCar = (yearlyBasic * free_car_rate);
                            }
                            //Free Car

                            //Total Taxable Income
                            // Need apply changes which one is 100% taxable


                            taxableIncome = yearlyBasic + yearlyPF + yearlyFestival;

                            //taxableIncome = yearlyBasic + yearlyPF;

                            decimal totalTaxableConveyance = 0;
                            decimal actualConveyanceExemption = 0;

                            decimal totalTaxableHouse = 0;
                            decimal actualHouseExemption = 0;

                            decimal totalTaxableMedical = 0;
                            decimal actualMedicalExemption = 0;

                            decimal totalTaxableLFA = 0;
                            decimal actualLFAExemption = 0;

                            decimal totalTaxableLFAArrear = 0;
                            decimal actualLFAExemptionArrear = 0;

                            decimal totalAnnualIncomeAll = 0;
                            decimal totalAnnualIncomeProjected = yearlyBasic + yearlyPF + freeCar;
                            decimal totalAnnualIncomeWithoutOnceOff = 0; //Total Annual Income(All - Once off)
                            decimal currentMonthOnceOffAmount = 0;


                            // projectedList

                            var ProjectedAllowancelist = dataContext.prl_allowance_configuration.AsEnumerable().Where(x => x.is_once_off_tax == Convert.ToSByte(false)
                                && x.is_active == Convert.ToSByte(true)).ToList();

                            List<int> ProjectedAllowanceIDs = new List<int>();
                            List<int> OnceOffAllowanceIDs = new List<int>();
                           

                            if (ProjectedAllowancelist.Count > 0)
                            {
                                ProjectedAllowanceIDs = ProjectedAllowancelist.AsEnumerable().Select(x => x.allowance_name_id).ToList();
                                OnceOffAllowanceIDs = salallList.Where(x => !ProjectedAllowanceIDs.Contains(x.allowanceid)).Select(y => y.allowanceid).ToList();
                            }


                            foreach (var a in salallList)
                            {
                                DateTime fiscalYrStart = Convert.ToDateTime(s_date);
                                DateTime fiscalYrEnd = Convert.ToDateTime((fiscalYrStart.Year + 1) + "-" + 6 + "-" + 30);

                                if (a.allowancename.Contains("House Rent Allowance"))
                                {
                                    
                                    decimal HRexmpOnBasic = yearlyBasic * (taxDetail.max_house_rent_percentage.Value / 100);
                                    decimal monthlyHRexmp = 0;
                                    decimal HRexemOnLimit = taxDetail.house_rent_not_exceding.Value;

                                    if (Utility.CommonDateClass.MonthYearIsInRange(item.joining_date, fiscalYrStart, fiscalYrEnd))
                                    {
                                        int calculated_month = Utility.DateUtility.getTaxTotalMonthForNewEmp(item.joining_date.Month);
                                        monthlyHRexmp = Math.Min((HRexmpOnBasic / calculated_month), (HRexemOnLimit / 12));
                                        actualHouseExemption = monthlyHRexmp * calculated_month;
                                        totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                                    }
                                    else
                                    {
                                        //if employee inactive

                                        var discontinued_empD = dataContext.prl_employee_discontinue.FirstOrDefault(d => d.discontinue_date >= fiscalYrStart && d.discontinue_date <= fiscalYrEnd && d.emp_id == item.id);
                                        if (discontinued_empD != null)
                                        {
                                            int calculated_month = Utility.DateUtility.getTaxTotalMonthForSeparatedEmp(discontinued_empD.discontinue_date.Month);
                                            monthlyHRexmp = Math.Min((HRexmpOnBasic / calculated_month), (HRexemOnLimit / 12));
                                            actualHouseExemption = monthlyHRexmp * calculated_month;
                                            totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                                        }
                                        else
                                        {
                                            actualHouseExemption = Math.Min(HRexmpOnBasic, taxDetail.house_rent_not_exceding.Value);
                                            totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                                        }
                                    }

                                    if (totalTaxableHouse < 0)
                                    {
                                        totalTaxableHouse = 0;
                                        actualHouseExemption = a.yearly_amount;
                                    }

                                    taxableIncome += totalTaxableHouse;
                                }
                                else if (a.allowancename.Contains("Car/MC Allowance"))
                                {
                                    decimal medicalExemOnBasic = yearlyBasic * (taxDetail.medical_exemption_percentage.Value / 100);
                                    decimal medicalExemOnLimit = taxDetail.medical_not_exceding.Value;
                                    actualMedicalExemption = Math.Min(medicalExemOnBasic, medicalExemOnLimit);

                                    totalTaxableMedical = a.yearly_amount - actualMedicalExemption;

                                    if (totalTaxableMedical < 0)
                                    {
                                        totalTaxableMedical = 0;
                                        actualMedicalExemption = a.yearly_amount;
                                    }
                                    taxableIncome += totalTaxableMedical;
                                }

                                else if (a.allowancename.Contains("Conveyance Allowance"))
                                {

                                    totalTaxableConveyance = a.yearly_amount - taxDetail.max_conveyance_allowance.Value;
                                    actualConveyanceExemption = taxDetail.max_conveyance_allowance.Value;
                                    if (totalTaxableConveyance < 0)
                                    {
                                        totalTaxableConveyance = 0;
                                        actualConveyanceExemption = a.yearly_amount;
                                    }

                                    taxableIncome += totalTaxableConveyance;
                                }
                                else if (a.allowancename == "LFA")
                                {
                                    //decimal medicalExemOnBasic = yearlyBasic * (taxDetail.lfa_exemption_percentage.Value / 100);
                                    //decimal medicalExemOnLimit = taxDetail.medical_not_exceding.Value;
                                    //actualLFAExemption = Math.Min(medicalExemOnBasic, medicalExemOnLimit);

                                    actualLFAExemption = a.yearly_amount * (taxDetail.lfa_exemption_percentage.Value / 100);
                                    totalTaxableLFA = a.yearly_amount - actualLFAExemption;

                                    if (totalTaxableLFA < 0)
                                    {
                                        totalTaxableLFA = 0;
                                        actualLFAExemption = a.yearly_amount;
                                    }
                                    taxableIncome += totalTaxableLFA;
                                    totalAnnualIncomeProjected += totalTaxableLFA;
                                }
                                else
                                {
                                    taxableIncome += a.yearly_amount;
                                    
                                }


                                if (OnceOffAllowanceIDs.Count > 0)
                                {
                                    foreach (var allId in OnceOffAllowanceIDs)
                                    {
                                        if (allId == a.allowanceid)
                                        {
                                            currentMonthOnceOffAmount += a.current_amount;
                                        }
                                    }
                                }
                            }

                            totalAnnualIncomeAll = taxableIncome;
                            totalAnnualIncomeWithoutOnceOff = totalAnnualIncomeAll - currentMonthOnceOffAmount;
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

                            var totalAnnualIncomeList = new List<decimal> { totalAnnualIncomeWithoutOnceOff, totalAnnualIncomeProjected, totalAnnualIncomeAll };
                            var totalTaxLiabilityList = new List<decimal>();
                            var netTaxPayableList = new List<decimal>();
                            int flag = 1;
                            decimal TaxPayableAmount = 0;

                            foreach (var totalAmount in totalAnnualIncomeList)
                            {
                                List<prl_employee_tax_slab> taxItemList = new List<prl_employee_tax_slab>();

                                List<prl_income_tax_parameter> taxSlab = new List<prl_income_tax_parameter>();
                                taxSlab = dataContext.prl_income_tax_parameter.Where(objID => (objID.fiscal_year_id == pFiscalYear && objID.gender == item.gender)).ToList();
                                int maxNumberItem = taxSlab.Count;

                                decimal lastSlabAmount = (taxSlab[maxNumberItem - 1] != null ? taxSlab[maxNumberItem - 1].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 2] != null ? taxSlab[maxNumberItem - 2].slab_maximum_amount : 0);
                                decimal lastSlabPercentage = (taxSlab[maxNumberItem - 1] != null ? taxSlab[maxNumberItem - 1].slab_percentage : 0);
                                decimal secondlastSlabAmount = (taxSlab[maxNumberItem - 2] != null ? taxSlab[maxNumberItem - 2].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 3] != null ? taxSlab[maxNumberItem - 3].slab_maximum_amount : 0);
                                decimal secondlastSlabPercentage = (taxSlab[maxNumberItem - 2] != null ? taxSlab[maxNumberItem - 2].slab_percentage : 0);
                                decimal thirdlastSlabAmount = (taxSlab[maxNumberItem - 3] != null ? taxSlab[maxNumberItem - 3].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 4] != null ? taxSlab[maxNumberItem - 4].slab_maximum_amount : 0);
                                decimal thirdlastSlabPercentage = (taxSlab[maxNumberItem - 3] != null ? taxSlab[maxNumberItem - 3].slab_percentage : 0);
                                decimal forthlastSlabAmount = (taxSlab[maxNumberItem - 4] != null ? taxSlab[maxNumberItem - 4].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 5] != null ? taxSlab[maxNumberItem - 5].slab_maximum_amount : 0);
                                decimal forthlastSlabPercentage = (taxSlab[maxNumberItem - 4] != null ? taxSlab[maxNumberItem - 4].slab_percentage : 0);
                                decimal fifthlastSlabAmount = (taxSlab[maxNumberItem - 5] != null ? taxSlab[maxNumberItem - 5].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 6] != null ? taxSlab[maxNumberItem - 6].slab_maximum_amount : 0);
                                decimal fifthlastSlabPercentage = (taxSlab[maxNumberItem - 5] != null ? taxSlab[maxNumberItem - 5].slab_percentage : 0);
                                //lukman
                                decimal sixthlastSlabAmount = (taxSlab[maxNumberItem - 6] != null ? taxSlab[maxNumberItem - 6].slab_maximum_amount : 0);
                                decimal sixthlastSlabPercentage = (taxSlab[maxNumberItem - 6] != null ? taxSlab[maxNumberItem - 6].slab_percentage : 0);

                                decimal _TaxableIncome = totalAmount; //taxableIncome; //Rakib


                                decimal firstSlabAmount = 0;
                                decimal secondSlabAmount = 0;
                                decimal thirdSlabAmount = 0;
                                decimal forthSlabAmount = 0;
                                decimal fifthSlabAmount = 0;

                                prl_employee_tax_slab taxItem;

                                if (_TaxableIncome <= sixthlastSlabAmount)
                                {
                                    TaxPayableAmount = 0;
                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, sixthlastSlabAmount, sixthlastSlabPercentage, _TaxableIncome, TaxPayableAmount);
                                    taxItemList.Add(taxItem);
                                }
                                if (_TaxableIncome > sixthlastSlabAmount)
                                {
                                    decimal netTaxPayableAmount = _TaxableIncome - sixthlastSlabAmount;

                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, sixthlastSlabAmount, sixthlastSlabPercentage, sixthlastSlabAmount, 0);
                                    taxItemList.Add(taxItem);

                                    if (netTaxPayableAmount <= fifthlastSlabAmount)
                                    {
                                        TaxPayableAmount = (netTaxPayableAmount * fifthlastSlabPercentage) / 100;
                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, fifthlastSlabAmount, fifthlastSlabPercentage, netTaxPayableAmount, TaxPayableAmount);
                                        taxItemList.Add(taxItem);
                                    }

                                    if (netTaxPayableAmount > fifthlastSlabAmount)
                                    {
                                        decimal reminderAmount = netTaxPayableAmount - fifthlastSlabAmount;
                                        firstSlabAmount = (fifthlastSlabAmount * fifthlastSlabPercentage) / 100;

                                        taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, fifthlastSlabAmount, fifthlastSlabPercentage, fifthlastSlabAmount, firstSlabAmount);
                                        taxItemList.Add(taxItem);

                                        if (reminderAmount <= forthlastSlabAmount)
                                        {
                                            secondSlabAmount = (reminderAmount * forthlastSlabPercentage) / 100;
                                            taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, forthlastSlabAmount, forthlastSlabPercentage, reminderAmount, secondSlabAmount);
                                            taxItemList.Add(taxItem);
                                        }

                                        if (reminderAmount > forthlastSlabAmount)
                                        {
                                            decimal secondReminderAmount = reminderAmount - forthlastSlabAmount;

                                            secondSlabAmount = (forthlastSlabAmount * forthlastSlabPercentage) / 100;
                                            taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, forthlastSlabAmount, forthlastSlabPercentage, forthlastSlabAmount, secondSlabAmount);
                                            taxItemList.Add(taxItem);

                                            if (secondReminderAmount <= thirdlastSlabAmount)
                                            {
                                                thirdSlabAmount = (secondReminderAmount * thirdlastSlabPercentage) / 100;

                                                taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, thirdlastSlabAmount, thirdlastSlabPercentage, secondReminderAmount, thirdSlabAmount);
                                                taxItemList.Add(taxItem);
                                            }

                                            if (secondReminderAmount > thirdlastSlabAmount)
                                            {
                                                decimal thirdReminderAmount = secondReminderAmount - thirdlastSlabAmount;
                                                thirdSlabAmount = (thirdlastSlabAmount * thirdlastSlabPercentage) / 100;

                                                taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, thirdlastSlabAmount, thirdlastSlabPercentage, thirdlastSlabAmount, thirdSlabAmount);
                                                taxItemList.Add(taxItem);

                                                if (thirdReminderAmount <= secondlastSlabAmount)
                                                {
                                                    forthSlabAmount = (thirdReminderAmount * secondlastSlabPercentage) / 100;
                                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, secondlastSlabAmount, secondlastSlabPercentage, thirdReminderAmount, forthSlabAmount);
                                                    taxItemList.Add(taxItem);
                                                }

                                                if (thirdReminderAmount > secondlastSlabAmount)
                                                {
                                                    forthSlabAmount = (secondlastSlabAmount * secondlastSlabPercentage) / 100;
                                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, secondlastSlabAmount, secondlastSlabPercentage, secondlastSlabAmount, forthSlabAmount);
                                                    taxItemList.Add(taxItem);

                                                    decimal fourthReminder = (thirdReminderAmount - secondlastSlabAmount);
                                                    fifthSlabAmount = (fourthReminder * lastSlabPercentage) / 100;

                                                    taxItem = GetTaxCertificateSlabWiseItem(item.id, pFiscalYear, salaryMonth, lastSlabAmount, lastSlabPercentage, fourthReminder, fifthSlabAmount);
                                                    taxItemList.Add(taxItem);
                                                }
                                            }
                                        }

                                        TaxPayableAmount = firstSlabAmount + secondSlabAmount + thirdSlabAmount + forthSlabAmount + fifthSlabAmount;
                                    }
                                }

                                TaxPayableAmount = Math.Round(TaxPayableAmount, 0, MidpointRounding.AwayFromZero);
                                totalTaxLiabilityList.Add(TaxPayableAmount);
                                //}


                                //Tax Slab

                                // Investment Rebate
                                decimal otherInvestment = 0;
                                decimal totalRebate = 0;


                                decimal actual_investment_total = 0;

                                var yearlyInvestmentTotal = dataContext.prl_employee_yearly_investment.SingleOrDefault(x => x.emp_id == item.id && x.fiscal_year_id == fsId);

                                if (yearlyInvestmentTotal != null)
                                {
                                    actual_investment_total = (decimal?)yearlyInvestmentTotal.invested_amount ?? 0;
                                }
                                else
                                {
                                    actual_investment_total = totalAmount * taxDetail.max_investment_percentage.Value / 100; //taxableIncome
                                }

                                otherInvestment = actual_investment_total - (yearlyPF * 2); // OtherInvetment

                                decimal maxInvPercentRebate = taxDetail.max_inv_exempted_percentage.Value;
                                decimal minInvPercentRebate = taxDetail.min_inv_exempted_percentage.Value;
                                decimal maxAmountForMaxExemptionPercent = taxDetail.max_amount_for_max_exemption_percent.Value;

                                if (TaxPayableAmount == 0)
                                {
                                    totalRebate = 0;
                                }
                                else if (actual_investment_total > max_investment_allowed)
                                {
                                    totalRebate = (max_investment_allowed * minInvPercentRebate) / 100;
                                }
                                else
                                {
                                    if (_TaxableIncome <= maxAmountForMaxExemptionPercent) //taxableIncome
                                    {
                                        totalRebate = (actual_investment_total * maxInvPercentRebate) / 100;
                                    }
                                    else
                                    {
                                        totalRebate = (actual_investment_total * minInvPercentRebate) / 100;
                                    }
                                }


                                //  ToDo:: Other Investment Should be incorporated

                                //

                                // Investment Rebate

                                // yearly Income Tax


                                double yearlyIncomeTax = 0;


                                if (TaxPayableAmount == 0)
                                {
                                    yearlyIncomeTax = 0;
                                }

                                else if (totalRebate > TaxPayableAmount)
                                {
                                    yearlyIncomeTax = double.Parse(taxDetail.min_tax_amount.ToString());
                                }

                                else
                                {
                                    if ((TaxPayableAmount - totalRebate) <= taxDetail.min_tax_amount)
                                    {
                                        yearlyIncomeTax = double.Parse(taxDetail.min_tax_amount.ToString());
                                    }
                                    else
                                    {
                                        yearlyIncomeTax = double.Parse((TaxPayableAmount - totalRebate).ToString());
                                    }

                                }


                                netTaxPayableList.Add(Convert.ToDecimal(yearlyIncomeTax));

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
                                //double YearlyLiabilities = 0;
                                try
                                {
                                    _previousTax = dataContext.prl_salary_process_detail.Where(e => e.salary_month >= frmDate && e.salary_month <= toDate && e.emp_id == item.id).Sum(q => (decimal?)q.total_monthly_tax) ?? 0;

                                }
                                catch
                                {
                                    _previousTax = 0;
                                }

                                double remainingYearlyTax = 0;
                                remainingYearlyTax = yearlyIncomeTax - double.Parse(_previousTax.ToString());

                                //Monthly Tax
                                double MonthlyTax = 0;
                                decimal netTaxPayableWithoutOnceOff = Math.Round(netTaxPayableList[0], 1, MidpointRounding.AwayFromZero);//totalAnnualIncomeWithoutOnceOff
                                decimal netTaxPayableAll = Math.Round(netTaxPayableList[2], 1, MidpointRounding.AwayFromZero); //totalAnnualIncomeAll 
                                decimal netTaxPayableProjected = Math.Round(netTaxPayableList[1], 1, MidpointRounding.AwayFromZero);//totalAnnualIncomeProjected

                                //Checking for AIT
                                var advanceIncomeTax = dataContext.prl_certificate_upload.Where(x => x.emp_id == item.id && x.certificat_type == "AIT" && x.income_year == fsYear).FirstOrDefault();
                                if (advanceIncomeTax != null)
                                {
                                    netTaxPayableProjected -= advanceIncomeTax.amount;
                                }

                                decimal thisMonthOnceOffTax = netTaxPayableAll - netTaxPayableWithoutOnceOff;



                                var previousProjectedTaxAmount = dataContext.prl_employee_tax_process.Where(x => x.emp_id == SpDetails.emp_id).ToList().Sum(x => x.projected_tax);

                                var thisMonthProjectedTax = (netTaxPayableProjected - previousProjectedTaxAmount) / (_reminingMonth + 1); //((netTaxPayableProjected - previous all netTaxPayableProjected)/ (_reminingMonth+1))

                                thisMonthProjectedTax = Math.Round(thisMonthProjectedTax, 2, MidpointRounding.AwayFromZero);

                                decimal thisMonthTax = thisMonthOnceOffTax + thisMonthProjectedTax;
                                //thisMonthTax = Math.Round(thisMonthTax,0,MidpointRounding.AwayFromZero);

                                MonthlyTax = Convert.ToDouble(thisMonthTax);

                                double total_paid = 0;

                                total_paid = double.Parse(_previousTax.ToString()) + MonthlyTax;

                                e_id = item.id;
                                #region Save Data

                                tran = mySqlConnection.BeginTransaction();
                                int taxProcessId = 0;
                                mySqlCommand.Parameters.Clear();
                                var taxprocessText = @"INSERT INTO prl_employee_tax_process
                        (emp_id,salary_process_id,fiscal_year_id,salary_month, yearly_taxable_income, total_tax_payable, inv_rebate, yearly_tax, total_paid, monthly_tax,  projected_tax, onceoff_tax, created_by,created_date)
                        VALUES (?emp_id,?salary_process_id,?fiscal_year_id,?salary_month,?yearly_taxable_income, ?total_tax_payable,?inv_rebate,?yearly_tax, ?total_paid, ?monthly_tax, ?projected_tax, ?onceoff_tax, ?created_by,?created_date);";

                                mySqlCommand.Connection = mySqlConnection;
                                mySqlCommand.CommandText = taxprocessText;
                                mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
                                mySqlCommand.Parameters.AddWithValue("?fiscal_year_id", pFiscalYear);
                                mySqlCommand.Parameters.AddWithValue("?salary_month", salaryMonth);
                                mySqlCommand.Parameters.AddWithValue("?yearly_taxable_income", decimal.Parse(taxableIncome.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?total_tax_payable", decimal.Parse(TaxPayableAmount.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?inv_rebate", decimal.Parse(totalRebate.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?yearly_tax", decimal.Parse(yearlyIncomeTax.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?total_paid", decimal.Parse(total_paid.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?monthly_tax", decimal.Parse(MonthlyTax.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?projected_tax", decimal.Parse(thisMonthProjectedTax.ToString()));
                                mySqlCommand.Parameters.AddWithValue("?onceoff_tax", decimal.Parse(thisMonthOnceOffTax.ToString()));
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
                                                (tax_process_id,salary_process_id,emp_id,tax_item, till_date_income, current_month_income, projected_income, gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item, ?till_date_income, ?current_month_income, ?projected_income, ?gross_annual_income,?less_exempted,?total_taxable_income);";

                                mySqlCommand.Connection = mySqlConnection;
                                mySqlCommand.CommandText = taxprocessDetText;
                                mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                                mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
                                mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                mySqlCommand.Parameters.AddWithValue("?tax_item", "Basic Salary");
                                mySqlCommand.Parameters.AddWithValue("?till_date_income", previousBasic);
                                mySqlCommand.Parameters.AddWithValue("?current_month_income", thisMonthBasic);
                                mySqlCommand.Parameters.AddWithValue("?projected_income", projectedBasic);
                                mySqlCommand.Parameters.AddWithValue("?gross_annual_income", yearlyBasic);
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", yearlyBasic);
                                mySqlCommand.ExecuteNonQuery();

                                //tran2.Commit();

                                //All Allowances
                                foreach (var sal in salallList)
                                {
                                    if (sal.allowancename != "Basic Salary")
                                    {

                                        mySqlCommand.Parameters.Clear();
                                        taxprocessDetText = "";
                                        taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                                (tax_process_id,salary_process_id,emp_id,tax_item, till_date_income, current_month_income, projected_income, gross_annual_income,less_exempted,total_taxable_income)
                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item, ?till_date_income, ?current_month_income, ?projected_income, ?gross_annual_income,?less_exempted,?total_taxable_income);";

                                        mySqlCommand.Connection = mySqlConnection;
                                        mySqlCommand.CommandText = taxprocessDetText;

                                        mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                                        mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
                                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);

                                        mySqlCommand.Parameters.AddWithValue("?tax_item", sal.allowancename);

                                        mySqlCommand.Parameters.AddWithValue("?till_date_income", sal.actual_amount);
                                        mySqlCommand.Parameters.AddWithValue("?current_month_income", sal.this_month_amount);
                                        mySqlCommand.Parameters.AddWithValue("?projected_income", sal.projected_amount);

                                        mySqlCommand.Parameters.AddWithValue("?gross_annual_income", sal.yearly_amount);

                                        if (sal.allowancename.Contains("House Rent Allowance"))
                                        {
                                            mySqlCommand.Parameters.AddWithValue("?less_exempted", actualHouseExemption);
                                            mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableHouse);
                                        }

                                        else if (sal.allowancename.Contains("Conveyance Allowance"))
                                        {
                                            mySqlCommand.Parameters.AddWithValue("?less_exempted", actualConveyanceExemption);
                                            mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableConveyance);
                                        }

                                        else if (sal.allowancename.Contains("Car/MC Allowance"))
                                        {
                                            mySqlCommand.Parameters.AddWithValue("?less_exempted", actualMedicalExemption);
                                            mySqlCommand.Parameters.AddWithValue("?total_taxable_income", totalTaxableMedical);
                                        }

                                        else
                                        {
                                            mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                                            mySqlCommand.Parameters.AddWithValue("?total_taxable_income", sal.yearly_amount);
                                        }
                                        mySqlCommand.ExecuteNonQuery();
                                    }
                                }

                                //PF
                                mySqlCommand.Parameters.Clear();
                                taxprocessDetText = "";
                                taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                        (tax_process_id,salary_process_id,emp_id,tax_item, till_date_income, current_month_income, projected_income, gross_annual_income,less_exempted,total_taxable_income)
                        VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item, ?till_date_income, ?current_month_income, ?projected_income, ?gross_annual_income,?less_exempted,?total_taxable_income);";

                                mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                                mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
                                mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                mySqlCommand.Parameters.AddWithValue("?tax_item", "Own Contribution to PF");

                                mySqlCommand.Parameters.AddWithValue("?till_date_income", previousPF);
                                mySqlCommand.Parameters.AddWithValue("?current_month_income", currentPF);
                                mySqlCommand.Parameters.AddWithValue("?projected_income", projectedPF);

                                mySqlCommand.Parameters.AddWithValue("?gross_annual_income", yearlyPF);
                                mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                                mySqlCommand.Parameters.AddWithValue("?total_taxable_income", yearlyPF);
                                mySqlCommand.ExecuteNonQuery();

                                //                        # region Festival Bonus Insert

                                //                        mySqlCommand.Parameters.Clear();
                                //                        taxprocessDetText = "";
                                //                        taxprocessDetText = @"INSERT INTO prl_employee_tax_process_detail
                                //                                                (tax_process_id,salary_process_id,emp_id, tax_item, till_date_income, current_month_income, projected_income, gross_annual_income,less_exempted,total_taxable_income)
                                //                                            VALUES (?tax_process_id,?salary_process_id,?emp_id,?tax_item, ?till_date_income, ?current_month_income, ?projected_income, ?gross_annual_income,?less_exempted,?total_taxable_income);";

                                //                        mySqlCommand.Connection = mySqlConnection;
                                //                        mySqlCommand.CommandText = taxprocessDetText;

                                //                        mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                                //                        mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
                                //                        mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                //                        mySqlCommand.Parameters.AddWithValue("?tax_item", "Festival Bonus");

                                //                        mySqlCommand.Parameters.AddWithValue("?till_date_income", actualFestival);
                                //                        mySqlCommand.Parameters.AddWithValue("?current_month_income", currentFestival);
                                //                        mySqlCommand.Parameters.AddWithValue("?projected_income", projectedFestival);
                                //                        mySqlCommand.Parameters.AddWithValue("?gross_annual_income", yearlyFestival);
                                //                        mySqlCommand.Parameters.AddWithValue("?less_exempted", 0);
                                //                        mySqlCommand.Parameters.AddWithValue("?total_taxable_income", yearlyFestival);
                                //                        mySqlCommand.ExecuteNonQuery();
                                //                        #endregion

                                //Tax Slab
                                foreach (var _tax in taxItemList)
                                {
                                    mySqlCommand.Parameters.Clear();
                                    string taxprocessSlab = "";
                                    //                          
                                    taxprocessSlab = @"INSERT INTO prl_employee_tax_slab
                                                    (emp_id,tax_process_id,fiscal_year_id,salary_date,salary_month,salary_year,current_rate,parameter,
                                                        taxable_income,tax_liability,created_by,created_date)
                                                    VALUES (?emp_id,?tax_process_id,?fiscal_year_id,?salary_date,?salary_month,?salary_year,?current_rate,?parameter,
                                                        ?taxable_income,?tax_liability,?created_by,?created_date);";

                                    mySqlCommand.Connection = mySqlConnection;
                                    mySqlCommand.CommandText = taxprocessSlab;

                                    mySqlCommand.Parameters.AddWithValue("?emp_id", item.id);
                                    mySqlCommand.Parameters.AddWithValue("?tax_process_id", taxProcessId);
                                    //mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
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
                                    mySqlCommand.Parameters.AddWithValue("?salary_process_id", _salaryPrss.id);
                                    mySqlCommand.Parameters.AddWithValue("?total_monthly_tax", decimal.Parse(MonthlyTax.ToString()));
                                    mySqlCommand.ExecuteNonQuery();
                                }

                                tran2.Commit();
                                #endregion
                            }

                            flag++;
                        }
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

        public int FindFiscalYear(DateTime processDate)
        {
            int fiscalyR = 0;
            int _month = processDate.Month;
            int _yr = processDate.Year;
            string curYear = string.Empty;
            //string prevYear = string.Empty;

            if (_month <= 6)
            {
                //prevYear = (_yr - 2).ToString() + "-" + (_yr - 1).ToString();
                curYear = (_yr - 1).ToString() + "-" + _yr.ToString();
            }
            else if (_month > 6)
            {
                curYear = _yr.ToString() + "-" + (_yr + 1).ToString();
                //prevYear = (_yr - 1).ToString() + "-" + _yr.ToString();
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

        private static prl_employee_tax_slab GetTaxCertificateSlabWiseItem(int empid, int pFiscalYrID, DateTime salaryMonth, decimal onNextAmount, decimal currentPercentage, decimal TaxableIncome, decimal taxLiability)
        {
            prl_employee_tax_slab taxItem;
            taxItem = new prl_employee_tax_slab();
            taxItem.emp_id = empid;
            taxItem.fiscal_year_id = pFiscalYrID;
            taxItem.current_rate = currentPercentage;
            taxItem.taxable_income = TaxableIncome;
            taxItem.tax_liability = taxLiability;
            if (onNextAmount > 4800000)
            {
                taxItem.parameter = "On remaining balance"; 
            }
            else
            {
              taxItem.parameter = "On the Next BDT-" + Convert.ToString(Math.Round(onNextAmount));
            }
            taxItem.salary_month = salaryMonth.Month;
            taxItem.salary_year = salaryMonth.Year;
            taxItem.salary_date = salaryMonth;
            return taxItem;
        }

        public static int FindProjectedMonth(int _month)
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

        public static int FindActualMonth(int _month)
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