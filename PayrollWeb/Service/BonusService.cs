/*********************************************************************************************
*   Company             :   Grameenphone IT Ltd.
*   Project Name        :   GPIT Profident Payroll System
*   Location            :   PayrollWeb  
*   Type                :   Business class file
*   Version             :   1.0.0   
*
*   Created By          :   Md. Khurshidur Rahman
*   Updated By          :   Md. Khurshidur Rahman
*   Updated Date        :   04-Sep-2013
*   Reviewer            :   Md. Khurshidur Rahman
**********************************************************************************************/
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

namespace PayrollWeb.Service
{
    public class BonusService
    {
        private payroll_systemContext dataContext;

        public BonusService(payroll_systemContext context)
        {
            this.dataContext = context;
        }

        public BonusService()
        {

        }

        ~BonusService()
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

        public bool CreateConfiguration(prl_bonus_configuration bonusConfiguration)
        {
            bool _result = false;
            try
            {
                if (bonusConfiguration.id == 0)
                {
                    dataContext.prl_bonus_configuration.Add(bonusConfiguration);
                    dataContext.SaveChanges();
                    _result = true;
                }
                else
                {
                    prl_bonus_configuration bn = new prl_bonus_configuration();
                    bn = dataContext.prl_bonus_configuration.Where(q => q.id == bonusConfiguration.id).FirstOrDefault();

                    bn.is_festival = bonusConfiguration.is_festival;
                    bn.bonus_name_id = bonusConfiguration.bonus_name_id;
                    bn.basic_of_days = bonusConfiguration.basic_of_days;
                    bn.confirmed_emp = bonusConfiguration.confirmed_emp;
                    bn.flat_amount = bonusConfiguration.flat_amount;
                    bn.is_taxable = bonusConfiguration.is_taxable;
                    bn.number_of_basic = bonusConfiguration.number_of_basic;
                    bn.percentage_of_basic = bonusConfiguration.percentage_of_basic;
                    bn.effective_from = bonusConfiguration.effective_from;
                    bn.effective_to = bonusConfiguration.effective_to;
                    bn.updated_by = bonusConfiguration.updated_by;
                    bn.updated_date = DateTime.Now;
                    dataContext.SaveChanges();
                    _result = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return _result;
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

        public int ProcessBonus(BonusProcess _bonus, string _UserName)
        {
            var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            int _prcs = 0;
            prl_bonus_configuration _bnsConfig = new prl_bonus_configuration();

            try
            {
                objectContext.Connection.Open();
                using (var _context = new payroll_systemContext())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                    {
                        //Selected Bonus Amount
                        if (_bonus.bonus_name_id > 0)
                        {
                            _bnsConfig = _context.prl_bonus_configuration.FirstOrDefault(x => x.bonus_name_id == _bonus.bonus_name_id);

                            if (_bnsConfig == null)
                            {
                                _prcs = -909;
                                return _prcs;
                            }
                        }

                        int fsId = FindFiscalYear(_bnsConfig.effective_from);
                        

                        DateTime blankDate = Convert.ToDateTime("0001-01-01"); 
                        //dataContext.Database.Connection.Open();
                        List<prl_employee_details> empList = new List<prl_employee_details>();
                        empList = _context.prl_employee_details.Include("prl_employee").Where(p => p.prl_employee.is_active == 1 && p.prl_employee.confirmation_date != blankDate && p.prl_employee.is_confirmed != 0 && p.prl_employee.confirmation_date <= _bnsConfig.effective_from).OrderBy(k => k.id).ToList();

                        //Filter By Department 
                        if (_bonus.department_id > 0)
                        {
                            //empList = dataContext.prl_employee_details.Include("prl_employee").Where(p => p.department_id == _bonus.department_id).ToList();
                            empList = empList.Where(p => p.department_id == _bonus.department_id).ToList();
                        }

                        //For Save Employee Bonus
                        //using (var scope = dataContext.Database.Connection.BeginTransaction())
                        //{

                        if (empList != null)
                        {
                            prl_bonus_process _process = new prl_bonus_process();
                            _process = _context.prl_bonus_process.FirstOrDefault(q => q.festival_date.Month == _bonus.festival_date.Month && q.festival_date.Year == _bonus.festival_date.Year
                                 && q.department_id == _bonus.department_id);
                            if (_process == null)
                            {
                                _process = new prl_bonus_process();
                                _process.bonus_name_id = _bonus.bonus_name_id;
                                _process.fiscal_year_id = fsId;
                                _process.month = _bonus.process_date.Month.ToString();
                                _process.year = _bonus.process_date.Year.ToString();
                                _process.batch_no = BatchNumberGenerator.generateBonusBatchNumber("Bonus", _bonus.festival_date, _bonus.bonus_name_id, _bnsConfig.is_festival, _bonus.is_pay_with_salary);
                                _process.process_date = _bonus.process_date;
                                _process.festival_date = _bonus.festival_date;
                                _process.is_festival = _bnsConfig.is_festival;
                                //_process.company_id = _bonus.company_id;

                                _process.department_id = _bonus.department_id;
                                
                                _process.is_pay_with_salary = _bonus.is_pay_with_salary;
                                _process.is_available_in_payslip = "NO";
                                _process.created_by = _UserName;
                                _process.created_date = DateTime.Now;
                                _context.prl_bonus_process.Add(_process);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _prcs = -101;
                                return _prcs;
                            }

                            int no_of_basic = 0;

                            foreach (var item in empList)
                            {
                                //no_of_basic = _context.prl_religion.FirstOrDefault(e => e.id == item.prl_employee.religion_id).no_of_bonus.Value;

                                prl_bonus_process_detail _processDet = new prl_bonus_process_detail();
                                _processDet.bonus_process_id = _process.id;
                                _processDet.process_date = _bonus.process_date;
                                _processDet.emp_id = item.emp_id;
                                _processDet.batch_no = BatchNumberGenerator.generateBonusBatchNumber("Bonus", _bonus.festival_date, _bonus.bonus_name_id, _bnsConfig.is_festival, _bonus.is_pay_with_salary);
                                if (_bnsConfig.percentage_of_basic > 0)
                                {
                                    _processDet.amount = Math.Round(getBonusAmount(item.prl_employee.joining_date, item.basic_salary, no_of_basic, _bnsConfig.percentage_of_basic.Value, _bonus.festival_date));
                                }
                                else
                                {
                                    _processDet.amount = _bnsConfig.flat_amount.Value;
                                }

                                if (_bnsConfig.is_festival == "YES")
                                    _processDet.bonus_tax_amount = 0;
                                else
                                    _processDet.bonus_tax_amount = 0; //ToDo::

                                _processDet.created_by = _UserName;
                                _processDet.created_date = DateTime.Now;
                                _context.prl_bonus_process_detail.Add(_processDet);
                            }
                            _context.SaveChanges();
                            ts.Complete();
                            _prcs = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
            finally
            {
                try { objectContext.Connection.Close(); }
                catch { }
            }
            return _prcs;
        }

        public int DisburseProcess(BonusProcess _bonus, string _UserName, int fiscalYr)
        {
            int _prcs = 0;
            prl_bonus_configuration _bnsConfig = new prl_bonus_configuration();
            try
            {

                prl_bonus_process _bProcess = dataContext.prl_bonus_process.FirstOrDefault(x => x.batch_no == _bonus.batch_no && x.is_available_in_payslip == "NO");
                if (_bProcess != null)
                {
                    _bProcess.is_available_in_payslip = "YES";
                    dataContext.SaveChanges();
                    _prcs = 1;
                }
                else
                {
                    _prcs = -909;
                }
            }
            catch
            { 
                
            }
            finally
            {
                
            }
            return _prcs;
        }

        private decimal getBonusAmount(DateTime join_date, decimal basic_salary, int no_of_basic, decimal percentage_of_basic, DateTime festival_date)
        {
            
            decimal amount = 0;
            
            int year = join_date.Year;

            int festivalYear = festival_date.Year;

            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(festivalYear, 12, 31);

            amount = basic_salary * (percentage_of_basic / 100);

            //if (year == festivalYear)
            //{
            //    number_of_days = int.Parse(CommonDateClass.DateDifference(join_date, lastDay).ToString());
            //}
            //else
            //{
            //    number_of_days = total_days_of_year;
            //}

            //total_bonus_amount = basic_salary * 2 / total_days_of_year * number_of_days;


            //if (percentage_of_basic == 100)
            //{
            //    amount = total_bonus_amount / 2;
            //}
            //else
            //{
            //    amount = total_bonus_amount / 1;
            //}
            //if (year < DateTime.Now.Year)
            //{
            //    amount = basic_salary * (percentage_of_basic / (100 * 2));
            //}
            //else
            //{
            //    number_of_days = total_days_of_year - number_of_days;
            //    amount = ((basic_salary * (percentage_of_basic / 100)) / total_days_of_year * number_of_days) / no_of_basic;
            //}

            return amount;
        }

        public bool RollbackProcess(BonusProcess _bonus, string _UserName)
        {
            bool _result = false;
            string batch_number = "";
            prl_bonus_configuration _bnsConfig = new prl_bonus_configuration();
            try
            {
                using (var dataContext = new payroll_systemContext())
                {
                    if (_bonus.bonus_name_id > 0)
                    {
                        _bnsConfig = dataContext.prl_bonus_configuration.FirstOrDefault(x => x.bonus_name_id == _bonus.bonus_name_id);

                        if (_bnsConfig != null)
                        {
                            //batch_number = BatchNumberGenerator.generateBonusBatchNumber("Bonus", _bonus.festival_date.Value, _bonus.bonus_name_id, _bnsConfig.is_festival, _bonus.is_pay_with_salary);
                            try
                            {
                                batch_number = dataContext.prl_bonus_process.AsEnumerable().FirstOrDefault(x => x.bonus_name_id == _bonus.bonus_name_id && x.is_available_in_payslip == "NO" && x.month == _bonus.festival_date.Month.ToString() && x.year == _bonus.festival_date.Year.ToString()).batch_no;
                            }
                            catch (Exception)
                            {
                                batch_number = "";
                            }
                            
                        }
                    }

                    if (batch_number != "")
                    {
                        List<prl_bonus_process_detail> processDetail = new List<prl_bonus_process_detail>();
                        processDetail = dataContext.prl_bonus_process_detail.Where(b => b.batch_no == batch_number).ToList();
                        if (processDetail != null)
                        {
                            foreach (var item in processDetail)
                            {
                                dataContext.prl_bonus_process_detail.Remove(item);
                            }

                            prl_bonus_process process = new prl_bonus_process();
                            process = dataContext.prl_bonus_process.FirstOrDefault(p => p.batch_no == batch_number);
                            if (process != null)
                            {
                                dataContext.prl_bonus_process.Remove(process);
                            }
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        dataContext.SaveChanges();
                        _result = true;
                    }
                    else
                    {
                        _result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return _result;
        }
    }
}