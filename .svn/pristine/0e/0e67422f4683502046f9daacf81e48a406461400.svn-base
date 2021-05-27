using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Transactions;
using com.gpit.DataContext;
using com.gpit.Model;
using Jace;
using PayrollWeb.Utility;

namespace PayrollWeb.Service
{
    public class AllowanceService
    {
        private payroll_systemContext dataContext;
        private IIdentity userIdentity;

        public AllowanceService(payroll_systemContext context)
        {
            this.dataContext = context;
            userIdentity = Thread.CurrentPrincipal.Identity;
        }

        ~AllowanceService()
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

        public bool CreateConfiguration(prl_allowance_configuration allowanceConfiguration)
        {
            try
            {
                if (allowanceConfiguration.id == 0)
                {
                    allowanceConfiguration.created_by = userIdentity.Name;
                    allowanceConfiguration.created_date = DateTime.Now;
                    dataContext.prl_allowance_configuration.Add(allowanceConfiguration);
                }
                else
                {
                    var extOb = dataContext.prl_allowance_configuration.Include("prl_allowance_name").SingleOrDefault(x => x.id == allowanceConfiguration.id);
                    extOb.prl_allowance_name.prl_grade = allowanceConfiguration.prl_allowance_name.prl_grade;
                    extOb.updated_by = userIdentity.Name;
                    extOb.updated_date = DateTime.Now;
                    var entry = dataContext.Entry(extOb);
                    entry.Property(x => x.id).IsModified = false;
                    entry.CurrentValues.SetValues(allowanceConfiguration);
                    entry.State = EntityState.Modified;
                }
                dataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public bool SaveTimeCardEntry(List<prl_upload_time_card_entry> lsTimeCardEntries )
        {
            var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            
            try
            {
                objectContext.Connection.Open();
                using (var k = new payroll_systemContext())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                    {
                        lsTimeCardEntries.ForEach(x => { k.prl_upload_time_card_entry.Add(x); });
                        k.SaveChanges();
                        
                        var onOverTime = CalculateOvertime(lsTimeCardEntries, k);
                        var lstShift = CalculateShiftBase(lsTimeCardEntries,k);
                        var transport = CalculateTransportBase(lsTimeCardEntries,k);
                        var lsOncall = CalculateOncallBase(lsTimeCardEntries,k);
                        
                        onOverTime.ForEach(x => { k.prl_over_time_amount.Add(x); });
                        lstShift.ForEach(x => { k.prl_over_time_amount.Add(x); });
                        transport.ForEach(x => { k.prl_over_time_amount.Add(x); });
                        lsOncall.ForEach(x => { k.prl_over_time_amount.Add(x); });

                        k.SaveChanges();
                        ts.Complete();
                        return true;
                    }
                }

            }
            finally
            {
              try{objectContext.Connection.Close();}catch{}
            }
        }
        private List<prl_over_time_amount> CalculateOvertime(List<prl_upload_time_card_entry> lsTimeCardEntries,payroll_systemContext k)
        {
            prl_over_time ot = k.prl_over_time.Include("prl_over_time_configuration").SingleOrDefault(x => x.name.ToLower().Contains("over time"));

            var lstAmounts = new List<prl_over_time_amount>();
            
            foreach (var entry in lsTimeCardEntries.Where(x=>x.double_rate_hour>0 || x.triple_rate_hour >0 || x.four_times_rate_hour>0))
            {
                bool isReviewFound = false;
                decimal basicAmountToAdjust = 0;

                var ldt = new DateTime(entry.submission_date.Value.Year, entry.submission_date.Value.Month,DateTime.DaysInMonth(entry.submission_date.Value.Year, entry.submission_date.Value.Month));

                var empInfo = k.prl_employee.Join(
                    k.prl_employee_details, e => e.id,d => d.emp_id, (d, e) => new {Details = e, Emp = d})
                    .Where(y => y.Emp.emp_no == entry.emp_no)
                    .OrderByDescending(o => o.Details.id).SingleOrDefault();

                var sr = k.prl_salary_review.AsEnumerable().SingleOrDefault(r => r.emp_id == empInfo.Emp.id && r.is_arrear_calculated == "No" && r.effective_from.Value.Date <= ldt);
                if ( sr != null)
                {
                    basicAmountToAdjust = sr.new_basic - empInfo.Details.basic_salary;
                    isReviewFound = true;
                }

                var conf = ot.prl_over_time_configuration.SingleOrDefault(x => x.name.ToLower().Contains("double rate"));
                //calculate double rate hour
                if (entry.double_rate_hour > 0)
                {

                    var prlOverTimeAmount = CalaculateDoubleRate(entry,conf,empInfo.Emp,empInfo.Details.basic_salary);
                    if (isReviewFound)
                    {
                       decimal amt =ArrearAmountForDoubleRate(sr.effective_from.Value.Date, ldt, conf.formula, empInfo.Emp.emp_no,
                            basicAmountToAdjust,k);
                        prlOverTimeAmount.arrear_amount = amt;
                    }
                    lstAmounts.Add(prlOverTimeAmount);
                }
                //calculate triple rate hour
                conf = ot.prl_over_time_configuration.SingleOrDefault(x => x.name.ToLower().Contains("triple rate"));
                
                if (entry.double_rate_hour > 0)
                {
                    var prlOverTimeAmount = CalaculateTripleRate(entry, conf, empInfo.Emp, empInfo.Details.basic_salary);
                    if (isReviewFound)
                    {
                        decimal amt = ArrearAmountForTripleRate(sr.effective_from.Value.Date, ldt, conf.formula, empInfo.Emp.emp_no,
                             basicAmountToAdjust, k);
                        prlOverTimeAmount.arrear_amount = amt;
                    }
                    lstAmounts.Add(prlOverTimeAmount);
                    lstAmounts.Add(prlOverTimeAmount);
                }
                //calculate triple rate hour
                conf = ot.prl_over_time_configuration.SingleOrDefault(x => x.name.ToLower().Contains("four times rate"));
                if (entry.four_times_rate_hour > 0)
                {
                    var prlOverTimeAmount = CalaculateFourTimesRate(entry, conf, empInfo.Emp, empInfo.Details.basic_salary);
                    if (isReviewFound)
                    {
                        decimal amt = ArrearAmountForFourTimesRate(sr.effective_from.Value.Date, ldt, conf.formula, empInfo.Emp.emp_no,
                             basicAmountToAdjust, k);
                        prlOverTimeAmount.arrear_amount = amt;
                    }
                    lstAmounts.Add(prlOverTimeAmount);
                    lstAmounts.Add(prlOverTimeAmount);
                }
            }
            return lstAmounts;
        }
        private List<prl_over_time_amount> CalculateShiftBase(List<prl_upload_time_card_entry> lsTimeCardEntries,payroll_systemContext k)
        {
            var engine = new CalculationEngine();
            Dictionary<string, double> variables;
            prl_over_time ot = k.prl_over_time.Include("prl_over_time_configuration").SingleOrDefault(x => x.name.ToLower().Contains("shift allowance"));
            var lstAmounts = new List<prl_over_time_amount>();
            
            foreach (var entry in lsTimeCardEntries.Where(x => x.night_shift > 0 || x.evening_shift > 0 || x.holiday> 0))
            {
                decimal sum = 0;
                prl_over_time_amount overTimeAmount=null, overTimeAmount2=null, overTimeAmount3=null;
                var empInfo = k.prl_employee.Join(k.prl_employee_details, e => e.id, d => d.emp_id,
                    (d, e) => new { Details = e, Emp = d })
                    .Where(y => y.Emp.emp_no == entry.emp_no)
                    .OrderBy(o => o.Details.id).SingleOrDefault();

               var conf = k.prl_over_time_configuration.AsEnumerable().SingleOrDefault(x => x.name.ToLower().Contains("night shift"));
                //calculate night shift 
                if (entry.night_shift > 0)
                {
                    variables = new Dictionary<string, double>();
                    variables.Add("HRS", (double)entry.night_shift);
                    double result = engine.Calculate(conf.formula, variables);
                    sum += (decimal) result;

                    overTimeAmount = new prl_over_time_amount();
                    overTimeAmount.time_card_upload_id = entry.id;
                    overTimeAmount.over_time_config_id = conf.id;
                    overTimeAmount.amount = (decimal?)result;
                    overTimeAmount.emp_id = empInfo.Emp.id;
                    overTimeAmount.created_by = entry.created_by;
                    overTimeAmount.created_date = entry.created_date;
                    overTimeAmount.month_year = (DateTime)entry.for_month;
                    lstAmounts.Add(overTimeAmount);
                }

                conf =k.prl_over_time_configuration.AsEnumerable().SingleOrDefault(x => x.name.ToLower().Contains("evening shift"));
                //calculate evening shift
                if (entry.evening_shift > 0)
                {
                    variables = new Dictionary<string, double>();
                    variables.Add("HRS", (double)entry.evening_shift);

                    double result = engine.Calculate(conf.formula, variables);
                    sum += (decimal) result;

                    overTimeAmount2 = new prl_over_time_amount();
                    overTimeAmount2.over_time_config_id = conf.id;
                    overTimeAmount2.time_card_upload_id = entry.id;
                    overTimeAmount2.amount = (decimal?)result;
                    overTimeAmount2.emp_id = empInfo.Emp.id;
                    overTimeAmount2.created_by = entry.created_by;
                    overTimeAmount2.created_date = entry.created_date;
                    overTimeAmount2.month_year = (DateTime)entry.for_month;
                    lstAmounts.Add(overTimeAmount2);
                }
                conf =k.prl_over_time_configuration.AsEnumerable().SingleOrDefault(x => x.name.ToLower().Contains("weekend"));
                //calculate weekend shift
                if (entry.weekend > 0)
                {
                    variables = new Dictionary<string, double>();
                    variables.Add("HRS", (double)entry.weekend);
                    
                    double result = engine.Calculate(conf.formula, variables);
                    sum += (decimal) result;

                    overTimeAmount3 = new prl_over_time_amount();
                    overTimeAmount3.over_time_config_id = conf.id;
                    overTimeAmount3.time_card_upload_id = entry.id;
                    overTimeAmount3.amount = (decimal?)result;
                    overTimeAmount3.emp_id = empInfo.Emp.id;
                    overTimeAmount3.created_by = entry.created_by;
                    overTimeAmount3.created_date = entry.created_date;
                    overTimeAmount3.month_year = (DateTime)entry.for_month;
                    lstAmounts.Add(overTimeAmount3);
                }

                decimal tempSum = 0;

                if (overTimeAmount != null)
                {
                    overTimeAmount.actual_total = sum;
                }
                if (overTimeAmount2 != null)
                {
                    overTimeAmount2.actual_total = sum;
                }
                if (overTimeAmount3 != null)
                {
                    overTimeAmount3.actual_total = sum;
                }

                if (ot.max_value < sum)
                {
                    if (overTimeAmount != null)
                    {
                        overTimeAmount.pay_total = ot.max_value;
                    }
                    if (overTimeAmount2 != null)
                    {
                        overTimeAmount2.actual_total = ot.max_value;
                    }
                    if (overTimeAmount3 != null)
                    {
                        overTimeAmount3.actual_total = ot.max_value;
                    }
                }
            }
            return lstAmounts;
        }
        private List<prl_over_time_amount> CalculateOncallBase(List<prl_upload_time_card_entry> lsTimeCardEntries, payroll_systemContext k)
        {
            var engine = new CalculationEngine();
            Dictionary<string, double> variables;
            prl_over_time ot = k.prl_over_time.Include("prl_over_time_configuration").SingleOrDefault(x => x.name.ToLower().Contains("on call"));
            var lstAmounts = new List<prl_over_time_amount>();
            foreach (var entry in lsTimeCardEntries.Where(x => x.night_shift > 0 || x.evening_shift > 0 || x.weekend > 0))
            {
                decimal sum = 0;
                prl_over_time_amount overTimeAmount = null;
                var empInfo = k.prl_employee.Join(
                    k.prl_employee_details,
                    e => e.id,
                    d => d.emp_id,
                    (d, e) => new { Details = e, Emp = d })
                    .Where(y => y.Emp.emp_no == entry.emp_no)
                    .OrderBy(o => o.Details.id).SingleOrDefault();

                var conf =k.prl_over_time_configuration.AsEnumerable().SingleOrDefault(x => x.name.ToLower().Contains("on call"));
                //calculate normal day
                if (entry.on_call_days > 0)
                {
                    variables = new Dictionary<string, double>();
                    variables.Add("DAYS", (double)entry.on_call_days);
                    double result = engine.Calculate(conf.formula, variables);

                    overTimeAmount = new prl_over_time_amount();
                    overTimeAmount.over_time_config_id = conf.id;
                    overTimeAmount.time_card_upload_id = entry.id;
                    overTimeAmount.amount = (decimal?)result;
                    overTimeAmount.emp_id = empInfo.Emp.id;
                    overTimeAmount.created_by = entry.created_by;
                    overTimeAmount.created_date = entry.created_date;
                    overTimeAmount.month_year = (DateTime)entry.for_month;
                    lstAmounts.Add(overTimeAmount);
                    if (ot.max_value < entry.on_call_days)
                    {
                        overTimeAmount.actual_total = overTimeAmount.amount;
                        variables = new Dictionary<string, double>();
                        variables.Add("DAYS", (double)ot.max_value);
                        overTimeAmount.pay_total = (decimal?) engine.Calculate(conf.formula, variables);
                    }
                }
            }
            return lstAmounts;
        }
        private List<prl_over_time_amount> CalculateTransportBase(List<prl_upload_time_card_entry> lsTimeCardEntries,payroll_systemContext k)
        {
            var engine = new CalculationEngine();
            Dictionary<string, double> variables;
            var lstAmounts = new List<prl_over_time_amount>();
            prl_over_time ot = k.prl_over_time.Include("prl_over_time_configuration").SingleOrDefault(x => x.name.ToLower().Contains("transport allowance"));

            foreach (var entry in lsTimeCardEntries.Where(x => x.two_way > 0 || x.one_way > 0 ))
            {
                var empInfo = k.prl_employee.Join(
                    k.prl_employee_details,
                    e => e.id,
                    d => d.emp_id,
                    (d, e) => new { Details = e, Emp = d })
                    .Where(y => y.Emp.emp_no == entry.emp_no)
                    .OrderBy(o => o.Details.id).SingleOrDefault();

                
                var conf = ot.prl_over_time_configuration.SingleOrDefault(x => x.name.ToLower().Contains("one way"));

                //calculate one way
                if (entry.one_way > 0)
                {
                    variables = new Dictionary<string, double>();

                    variables.Add("TIMES", (double)entry.one_way);

                    double result = engine.Calculate(conf.formula, variables);
                    var overTimeAmount = new prl_over_time_amount();
                    overTimeAmount.over_time_config_id = conf.id;
                    overTimeAmount.time_card_upload_id = entry.id;
                    overTimeAmount.amount = (decimal?)result;
                    overTimeAmount.emp_id = empInfo.Emp.id;
                    overTimeAmount.created_by = entry.created_by;
                    overTimeAmount.created_date = entry.created_date;
                    overTimeAmount.month_year = (DateTime)entry.for_month;
                    overTimeAmount.actual_total = overTimeAmount.amount;
                    overTimeAmount.pay_total = overTimeAmount.amount;
                    lstAmounts.Add(overTimeAmount);
                }
                conf = k.prl_over_time_configuration.AsEnumerable().SingleOrDefault(x => x.name.ToLower().Contains("two way"));
                //calculate two way
                if (entry.two_way > 0)
                {
                    variables = new Dictionary<string, double>();
                    variables.Add("TIMES", (double)entry.two_way);
                    double result = engine.Calculate(conf.formula, variables);

                    var overTimeAmount = new prl_over_time_amount();
                    overTimeAmount.amount = (decimal?)result;
                    overTimeAmount.over_time_config_id = conf.id;
                    overTimeAmount.time_card_upload_id = entry.id;
                    overTimeAmount.emp_id = empInfo.Emp.id;
                    overTimeAmount.created_by = entry.created_by;
                    overTimeAmount.created_date = entry.created_date;
                    overTimeAmount.month_year = (DateTime)entry.for_month;
                    overTimeAmount.actual_total = overTimeAmount.amount;
                    overTimeAmount.pay_total = overTimeAmount.amount;
                    lstAmounts.Add(overTimeAmount);
                }
            }
            return lstAmounts;
        }
        private prl_over_time_amount CalaculateDoubleRate(prl_upload_time_card_entry ent, prl_over_time_configuration conf, prl_employee emp, decimal basic)
        {
            var result = CalcOTAmount(conf.formula, basic, (decimal)ent.double_rate_hour);

            var overTimeAmount = new prl_over_time_amount();
            overTimeAmount.time_card_upload_id = ent.id;
            overTimeAmount.over_time_config_id = conf.id;
            overTimeAmount.amount = (decimal?)result;
            overTimeAmount.emp_id = emp.id;
            overTimeAmount.created_by = ent.created_by;
            overTimeAmount.created_date = ent.created_date;
            overTimeAmount.month_year = (DateTime)ent.for_month;
            overTimeAmount.actual_total = overTimeAmount.amount;
            overTimeAmount.pay_total = overTimeAmount.amount;
            return overTimeAmount;
        }
        private prl_over_time_amount CalaculateTripleRate(prl_upload_time_card_entry ent, prl_over_time_configuration conf, prl_employee emp, decimal basic)
        {
            var result = CalcOTAmount(conf.formula, basic, (decimal)ent.triple_rate_hour);

            var overTimeAmount = new prl_over_time_amount();
            overTimeAmount.time_card_upload_id = ent.id;
            overTimeAmount.over_time_config_id = conf.id;
            overTimeAmount.amount = (decimal?)result;
            overTimeAmount.emp_id = emp.id;
            overTimeAmount.created_by = ent.created_by;
            overTimeAmount.created_date = ent.created_date;
            overTimeAmount.month_year = (DateTime)ent.for_month;
            overTimeAmount.actual_total = overTimeAmount.amount;
            overTimeAmount.pay_total = overTimeAmount.amount;
            return overTimeAmount;
        }
        private prl_over_time_amount CalaculateFourTimesRate(prl_upload_time_card_entry ent, prl_over_time_configuration conf, prl_employee emp, decimal basic)
        {
            var result = CalcOTAmount(conf.formula, basic, (decimal)ent.four_times_rate_hour);

            var overTimeAmount = new prl_over_time_amount();
            overTimeAmount.time_card_upload_id = ent.id;
            overTimeAmount.over_time_config_id = conf.id;
            overTimeAmount.amount = (decimal?)result;
            overTimeAmount.emp_id = emp.id;
            overTimeAmount.created_by = ent.created_by;
            overTimeAmount.created_date = ent.created_date;
            overTimeAmount.month_year = (DateTime)ent.for_month;
            overTimeAmount.actual_total = overTimeAmount.amount;
            overTimeAmount.pay_total = overTimeAmount.amount;
            return overTimeAmount;
        }
        private double CalcOTAmount(string formula, decimal basic, decimal hrs)
        {
            var variables = new Dictionary<string, double>();
            variables.Add("HRS", (double)hrs);
            variables.Add("Basic", (double)basic);
            var engine = new CalculationEngine();
            return engine.Calculate(formula, variables);
        }

        private decimal ArrearAmountForDoubleRate(DateTime effectiveFromDate,DateTime calculationTillDate,string formula,string emp_no,decimal amountToAdjustBasic,payroll_systemContext contxt)
        {
            var totalHrsToAdjust = contxt.prl_upload_time_card_entry.AsEnumerable().
                Where(
                        x => x.emp_no == emp_no &&
                             x.for_month.Value.Date >= effectiveFromDate.Date &&
                             x.for_month.Value.Date <= calculationTillDate.Date
                    ).Sum(x => x.double_rate_hour);
            if (totalHrsToAdjust !=null)
                return (decimal) CalcOTAmount(formula, amountToAdjustBasic, totalHrsToAdjust.Value);
            return 0;
        }
        private decimal ArrearAmountForTripleRate(DateTime effectiveFromDate, DateTime calculationTillDate, string formula, string emp_no, decimal amountToAdjustBasic, payroll_systemContext contxt)
        {
            var totalHrsToAdjust = contxt.prl_upload_time_card_entry.AsEnumerable().
                Where(
                        x => x.emp_no == emp_no &&
                             x.for_month.Value.Date >= effectiveFromDate.Date &&
                             x.for_month.Value.Date <= calculationTillDate.Date
                    ).Sum(x => x.triple_rate_hour);
            if (totalHrsToAdjust != null)
                return (decimal)CalcOTAmount(formula, amountToAdjustBasic, totalHrsToAdjust.Value);
            return 0;
        }
        private decimal ArrearAmountForFourTimesRate(DateTime effectiveFromDate, DateTime calculationTillDate, string formula, string emp_no, decimal amountToAdjustBasic, payroll_systemContext contxt)
        {
            var totalHrsToAdjust = contxt.prl_upload_time_card_entry.AsEnumerable().
                Where(
                        x => x.emp_no == emp_no &&
                             x.for_month.Value.Date >= effectiveFromDate.Date &&
                             x.for_month.Value.Date <= calculationTillDate.Date
                    ).Sum(x => x.four_times_rate_hour);
            if (totalHrsToAdjust != null)
                return (decimal)CalcOTAmount(formula, amountToAdjustBasic, totalHrsToAdjust.Value);
            return 0;
        }
        
        public int CreateAllowanceService(prl_employee_children_allowance cAll)
        {
            int _Result = 0;
            var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            try
            {
                objectContext.Connection.Open();
                using (var _context = new payroll_systemContext())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                    {
                        var c_allw = new prl_employee_children_allowance();
                        c_allw = _context.prl_employee_children_allowance.FirstOrDefault(e => e.emp_id == cAll.emp_id && e.is_active == 1);
                        if (c_allw != null)
                        {
                            c_allw.is_active = 0;

                        }
                        else
                        {
                            c_allw.emp_id = cAll.emp_id;
                            c_allw.no_of_children = cAll.no_of_children;
                            c_allw.amount = cAll.amount;
                            c_allw.is_active = 1;
                            c_allw.effective_from = cAll.effective_from;
                            dataContext.prl_employee_children_allowance.Add(c_allw);
                        }
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                _Result = -99;
                throw;
            }
            finally
            {
                try { objectContext.Connection.Close(); }
                catch { }
            }
            return _Result;
        }
        public int DeleteEmployeeChildAllowance(int cId)
        {
            int _Result = 0;
            var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            try
            {
                objectContext.Connection.Open();
                using (var _context = new payroll_systemContext())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                    {
                        var c_allow = _context.prl_employee_children_allowance.FirstOrDefault(c => c.id == cId);
                        if (c_allow != null)
                        {
                            var child_all = _context.prl_employee_children_allowance.OrderByDescending(x => x.id).FirstOrDefault(q => q.emp_id == c_allow.emp_id);
                            if (child_all != null)
                            {
                                child_all.is_active = 1;
                            }
                            _context.prl_employee_children_allowance.Remove(c_allow);
                            _context.SaveChanges();
                            _Result = 1;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                _Result = -99;
                throw ex;
            }
            finally
            {
                try { objectContext.Connection.Close(); }
                catch { }
            }
            return _Result;
        }
        public int CreateChildrenAllowance(prl_employee_children_allowance cAll)
        {
            int _Result = 0;
            var objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            try
            {
                objectContext.Connection.Open();
                using (var _context = new payroll_systemContext())
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                    {
                        var c_allw = new prl_employee_children_allowance();
                        c_allw = _context.prl_employee_children_allowance.FirstOrDefault(e => e.emp_id == cAll.emp_id && e.is_active == 1);
                        if (c_allw != null)
                        {
                            var cAllw = _context.prl_employee_children_allowance.FirstOrDefault(a => a.id == c_allw.id);
                            cAllw.is_active = 0;

                            var child_allw = new prl_employee_children_allowance();
                            child_allw.emp_id = cAll.emp_id;
                            child_allw.no_of_children = cAll.no_of_children;
                            child_allw.amount = cAll.amount;
                            child_allw.is_active = 1;
                            child_allw.effective_from = cAll.effective_from;
                            _context.prl_employee_children_allowance.Add(child_allw);
                        }
                        else
                        {
                            c_allw = new prl_employee_children_allowance();
                            c_allw.emp_id = cAll.emp_id;
                            c_allw.no_of_children = cAll.no_of_children;
                            c_allw.amount = cAll.amount;
                            c_allw.is_active = 1;
                            c_allw.effective_from = cAll.effective_from;
                            _context.prl_employee_children_allowance.Add(c_allw);
                        }
                        _Result = _context.SaveChanges();
                        ts.Complete();
                    }
                }
            }
            catch (Exception)
            {
                _Result = -99;
                throw;
            }
            finally
            {
                try { objectContext.Connection.Close(); }
                catch { }
            }
            return _Result;
        }

    }
}