using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using com.gpit.DataContext;
using com.gpit.Model;
using Ninject.Activation.Caching;
using PayrollWeb.Models;
using PayrollWeb.Service;
using PayrollWeb.ViewModels;
using Cache = System.Web.Caching.Cache;

namespace PayrollWeb.Controllers
{
    public class SettlementController : Controller
    {
        private readonly payroll_systemContext dataContext;
        private static readonly object lockObject = new object();

        public SettlementController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult Index()
        {
            ViewBag.AllowanceNames =
                dataContext.prl_allowance_name.AsEnumerable()
                    .Select(x => new AllowanceName() {allowance_name = x.allowance_name, id = x.id})
                    .Distinct()
                    .ToList();
            ViewBag.DeductionNames =
                dataContext.prl_deduction_name.AsEnumerable()
                    .Select(x => new DeductionName() {deduction_name = x.deduction_name, id = x.id})
                    .Distinct()
                    .ToList();
            ViewBag.BonusNames =
                dataContext.prl_bonus_name.AsEnumerable()
                    .Select(x => new BonusName() {id = x.id, name = x.name})
                    .Distinct()
                    .ToList();
            ViewBag.OvertimeNames =
                dataContext.prl_over_time.AsEnumerable()
                    .Select(x => new Overtime() {id = x.id, name = x.name})
                    .Distinct()
                    .ToList();

            return View();
        }

        public ActionResult SettlementEmployeeSearch(string query)
        {
            var lst = dataContext.prl_employee_discontinue.AsEnumerable().Join(dataContext.prl_employee, ok => ok.emp_id, ik => ik.id, (ok, ik) => ik)
                .AsEnumerable().Where(x =>(x.name.ToLower().Contains(query.ToLower()) || x.emp_no.Contains(query)) &&
                        Convert.ToSByte(x.is_active.Value) == Convert.ToSByte(false)).Select(x => new SearchEmployeeData() { id = x.id, name = x.name, empno = x.emp_no })
                .ToList();

            lst = lst.Where(x => dataContext.prl_employee_settlement.AsEnumerable().All(y => y.emp_id != x.id)).ToList();
           
            //var lst = dataContext.prl_employee.AsEnumerable().Where(x =>(x.name.ToLower().Contains(query.ToLower()) || x.emp_no.Contains(query)) &&
            //            Convert.ToSByte(x.is_active.Value) == Convert.ToSByte(false))
            //    .Select(x => new SearchEmployeeData() {id = x.id, name = x.name, empno = x.emp_no})
            //    .ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SettementEmpInfo(string id)
        {
            var result = dataContext.prl_employee.AsEnumerable()
                .Where(x => x.id == Convert.ToInt32(id))
                .Join(dataContext.prl_employee_details.Include("prl_employee")
                    .Include("prl_division")
                    .Include("prl_department")
                    .Include("prl_grade")
                    .AsEnumerable(), ok => ok.id, ik => ik.emp_id, (ok, ik) => ik)
                .GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First())
                .Select(x => new
                             {
                                 joinDate = x.prl_employee.joining_date.ToString("yyyy-MM-dd"),
                                 grade = x.prl_grade.grade,
                                 designation = x.prl_designation.name
                             }).FirstOrDefault();

            if (result != null)
            {
                var discontinueDate = dataContext.prl_employee_discontinue.AsEnumerable().FirstOrDefault(x => x.emp_id == Convert.ToInt32(id)).discontinue_date.Value.Date.ToString("yyyy-MM-dd");

                return Json(new {empData = result,discontinueDate=discontinueDate}, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult CalculateSalaryEntiltled(string id, string fdate, string tdate)
        {
            IFormatProvider enUsDateFormat = new CultureInfo("en-US").DateTimeFormat;
            var dt1 = Convert.ToDateTime(fdate, enUsDateFormat);
            var dt2 = Convert.ToDateTime(tdate, enUsDateFormat);

            var emp = PrlEmployeeDetails(id);

            int numberOfDaysWorked;
            var basicSalaryAmount = BasicSalaryAmount(id, dt2, dt1, emp, out numberOfDaysWorked);

            var  ownpf = dataContext.prl_salary_process_detail.AsEnumerable()
                .Where(x => x.emp_id == Convert.ToInt32(id))
                .Select(x => new {pf = x.pf_amount, pfArrear = x.pf_arrear}).Sum(x => x.pf + x.pfArrear);

            var res = FractionateSalaryProcessResult(dt1, dt2, emp);

            var lst = new List<SettlementAllowance>();
            var lstAlws = res.GetCompletedResultObjects() as List<prl_salary_allowances>;
            var alwNames =dataContext.prl_allowance_name.AsEnumerable().Where(x => lstAlws.Select(y => y.allowance_name_id).ToList().Contains(x.id)).ToList();

            foreach (var eal in res.GetCompletedResultObjects() as List<prl_salary_allowances>)
            {
                var i = new SettlementAllowance();
                i.Id = eal.allowance_name_id;
                i.Amount = decimal.Round(eal.amount,MidpointRounding.AwayFromZero);
                i.Name = alwNames.FirstOrDefault(x => x.id == eal.allowance_name_id).allowance_name;
                lst.Add(i);
            }

            var summary = HttpContext.Cache.Get(id) as EmployeeEntitlement;
            if (summary == null)
            {
                summary = new EmployeeEntitlement();
                summary.EligibleForCompanyPF = IsEligibleForCPF(emp[0].prl_employee.joining_date.Date,GetDiscontinueDate(Convert.ToInt32(id)));
                summary.EntitledCalculationDays = numberOfDaysWorked;
                summary.EmployeeId = Convert.ToInt32(id);
                summary.EntitledAllowances = lst;
                summary.BasicSalary = decimal.Round(basicSalaryAmount,MidpointRounding.AwayFromZero);
                summary.ProvidentFund = (decimal) ownpf;
                summary.ProvidentFundCompany = summary.EligibleForCompanyPF ? summary.ProvidentFund : 0;
                HttpContext.Cache.Insert(id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                summary.EntitledCalculationDays = numberOfDaysWorked;
                summary.EligibleForCompanyPF = IsEligibleForCPF(emp[0].prl_employee.joining_date.Date, GetDiscontinueDate(Convert.ToInt32(id)));
                summary.EmployeeId = Convert.ToInt32(id);
                summary.EntitledAllowances = lst;
                summary.BasicSalary = decimal.Round(basicSalaryAmount, MidpointRounding.AwayFromZero);
                summary.ProvidentFund = summary.ProvidentFund = (decimal)ownpf;
                summary.ProvidentFundCompany = summary.EligibleForCompanyPF ? summary.ProvidentFund : 0;
                HttpContext.Cache.Insert(id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }

            return Json(new {status = "ok"},JsonRequestBehavior.AllowGet);
        }

        private static IProcessResult FractionateSalaryProcessResult(DateTime dt1, DateTime dt2, List<prl_employee_details> emp)
        {
            var salAl = new SalaryAllowanceProcess(dt1.Date, dt1.Date, dt2.Date, emp);
            var res = salAl.Process();
            return res;
        }

        public ActionResult CalculateSalaryDue(string id, string fdate, string tdate)
        {
            var test = id;
            IFormatProvider enUsDateFormat = new CultureInfo("en-US").DateTimeFormat;
            var dt1 = Convert.ToDateTime(fdate, enUsDateFormat);
            var dt2 = Convert.ToDateTime(tdate, enUsDateFormat);

            var emp = PrlEmployeeDetails(id);

            int numberOfDaysWorked;
            var basicSalaryAmount = BasicSalaryAmount(id, dt2, dt1, emp, out numberOfDaysWorked);

            var res = FractionateSalaryProcessResult(dt1, dt2, emp);

            var lst = new List<SettlementAllowance>();
            var lstAlws = res.GetCompletedResultObjects() as List<prl_salary_allowances>;
            var alwNames = dataContext.prl_allowance_name.AsEnumerable().Where(x => lstAlws.Select(y => y.allowance_name_id).ToList().Contains(x.id)).ToList();
            foreach (var eal in res.GetCompletedResultObjects() as List<prl_salary_allowances>)
            {
                var i = new SettlementAllowance();
                i.Id = eal.allowance_name_id;
                i.Amount = decimal.Round(eal.amount,MidpointRounding.AwayFromZero);
                i.Name = alwNames.FirstOrDefault(x => x.id == eal.allowance_name_id).allowance_name;
                lst.Add(i);
            }

            var summary = HttpContext.Cache.Get(id) as EmployeeEntitlement;
            if (summary == null)
            {
                summary = new EmployeeEntitlement();
                summary.DueCalculationDays = numberOfDaysWorked;
                summary.EmployeeId = Convert.ToInt32(id);
                summary.DueSalary = decimal.Round(basicSalaryAmount,MidpointRounding.AwayFromZero);
                summary.ProvidentFundDue = summary.DueSalary*(decimal) .10;
                summary.DueAllowances = lst;
                HttpContext.Cache.Insert(id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                summary.DueCalculationDays = numberOfDaysWorked;
                summary.EmployeeId = Convert.ToInt32(id);
                summary.DueSalary = decimal.Round(basicSalaryAmount, MidpointRounding.AwayFromZero);
                summary.DueAllowances = lst;
                HttpContext.Cache.Insert(id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        private List<prl_employee_details> PrlEmployeeDetails(string id)
        {
            var emp = dataContext.prl_employee.AsEnumerable()
                .Where(x => x.id == Convert.ToInt32(id))
                .Join(dataContext.prl_employee_details.Include("prl_employee")
                    .Include("prl_division")
                    .Include("prl_department")
                    .Include("prl_grade")
                    .AsEnumerable(), ok => ok.id, ik => ik.emp_id, (ok, ik) => ik)
                .GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();
            return emp;
        }

        private decimal BasicSalaryAmount(string id, DateTime dt2, DateTime dt1, List<prl_employee_details> emp, out int numberOfDaysWorked)
        {
            var discontinueConfig =
                dataContext.prl_employee_discontinue.AsEnumerable().FirstOrDefault(x => x.emp_id == Convert.ToInt32(id));

            decimal basicSalaryAmount = 0;
            numberOfDaysWorked = 0;
            if (discontinueConfig.with_salary == "Y")
            {
                numberOfDaysWorked = dt2.Subtract(dt1).Days + 1;
                int daysInMonth = DateTime.DaysInMonth(dt1.Year, dt1.Month);
                basicSalaryAmount = (emp[0].basic_salary/daysInMonth)*numberOfDaysWorked;
            }
            return basicSalaryAmount;
        }

        public ActionResult RemoveSalaryAllowanceDue(string emp_id, int alw_id)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {
                return null;
            }
            else
            {
                summary.DueAllowances.RemoveAll(x => x.Id == alw_id);
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSummaryView(string id)
        {
            var summary = HttpContext.Cache.Get(id) as EmployeeEntitlement;
            if (summary != null)
            {

                return Json(new {data = summary}, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult AddEntitledAllowance(string emp_id, int alw_id,string alw_name, decimal amount)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {

                summary = new EmployeeEntitlement();
                summary.EmployeeId = id;
                summary.EntitledAllowances = new List<SettlementAllowance>();
                summary.EntitledAllowances.Add(new SettlementAllowance() {Id = alw_id,Amount = amount,Name = alw_name});
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                summary.EntitledAllowances.Add(new SettlementAllowance(){Id = alw_id,Amount = amount,Name = alw_name});
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new {status = "ok"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveEntitledAllowance(string emp_id, int alw_id)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {
                return null;
            }
            else
            {
                summary.EntitledAllowances.RemoveAll(x => x.Id == alw_id);
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new {status = "ok"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEntitledBonus(string emp_id, int bonus_id, string bonus_name ,decimal amount)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {

                summary = new EmployeeEntitlement();
                summary.EmployeeId = id;
                summary.EntitledBonus = new List<BonusSettlement>();
                summary.EntitledBonus.Add(new BonusSettlement() {Id = bonus_id, Name = bonus_name,Amount = amount});
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                if (summary.EntitledBonus == null)
                {
                    summary.EntitledBonus = new List<BonusSettlement>();
                }
                summary.EntitledBonus.Add(new BonusSettlement() {Id = bonus_id, Name = bonus_name,Amount = amount});
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new {status = "ok"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveEntitledBonus(string emp_id, int bonus_id)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {
                return null;
            }
            else
            {
                summary.EntitledBonus.RemoveAll(x => x.Id == bonus_id);
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new {status = "ok"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEntitledOT(string emp_id, int ot_id, string ot_name ,decimal amount)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {

                summary = new EmployeeEntitlement();
                summary.EmployeeId = id;
                summary.EntitledOT = new List<OTSettlement>();
                summary.EntitledOT.Add(new OTSettlement() {Id = ot_id, Name = ot_name,Amount = amount});
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                if (summary.EntitledOT == null)
                {
                    summary.EntitledOT = new List<OTSettlement>();
                }
                summary.EntitledOT.Add(new OTSettlement() {Id = ot_id, Name = ot_name,Amount = amount});
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new {status = "ok"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveEntitledOT(string emp_id, int ot_id)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {
                return null;
            }
            else
            {
                summary.EntitledOT.RemoveAll(x => x.Id == ot_id);
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new {status = "ok"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDueDeduction(string emp_id, int ded_id, string ded_name, decimal amount)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {

                summary = new EmployeeEntitlement();
                summary.EmployeeId = id;
                summary.EntitledDeductions = new List<SettlementDeduction>();
                summary.EntitledDeductions.Add(new SettlementDeduction() { Id = ded_id, Amount = amount, Name = ded_name });
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                if (summary.EntitledDeductions == null)
                {
                    summary.EntitledDeductions = new List<SettlementDeduction>();
                }
                summary.EntitledDeductions.Add(new SettlementDeduction() { Id = ded_id, Amount = amount, Name = ded_name });
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveDueDeduction(string emp_id, int ded_id)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {
                return null;
            }
            else
            {
                summary.EntitledDeductions.RemoveAll(x => x.Id == ded_id);
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDueBonus(string emp_id, int bonus_id, string bonus_name, decimal amount)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {

                summary = new EmployeeEntitlement();
                summary.EmployeeId = id;
                summary.BonusDue = new List<BonusSettlement>();
                summary.BonusDue.Add(new BonusSettlement() { Id = bonus_id, Name = bonus_name, Amount = amount });
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            else
            {
                if (summary.BonusDue == null)
                {
                    summary.BonusDue = new List<BonusSettlement>();
                }
                summary.BonusDue.Add(new BonusSettlement() { Id = bonus_id, Name = bonus_name, Amount = amount });
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveDueBonus(string emp_id, int bonus_id)
        {
            int id = 0;
            if (!int.TryParse(emp_id, out id))
            {
                return null;
            }
            var summary = HttpContext.Cache.Get(emp_id) as EmployeeEntitlement;
            if (summary == null)
            {
                return null;
            }
            else
            {
                summary.BonusDue.RemoveAll(x => x.Id == bonus_id);
                HttpContext.Cache.Insert(emp_id, summary, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        private bool IsEligibleForCPF(DateTime joinDate, DateTime discontinueDate)
        {

            return discontinueDate.Subtract(joinDate).TotalDays + 1 >= 1095;
        }

        private DateTime GetDiscontinueDate(int empid)
        {
           var discontObj = dataContext.prl_employee_discontinue.AsEnumerable().FirstOrDefault(x => x.emp_id == empid);
           return discontObj.discontinue_date.Value.Date;
        }

        [HttpPost]
        public ActionResult CompleteSettlement(FormCollection formCollection)
        {
            var empid = formCollection.GetValue("empId").AttemptedValue;
            var summary = HttpContext.Cache.Get(empid) as EmployeeEntitlement;
            var eid = Convert.ToInt32(formCollection.GetValue("empId").AttemptedValue);

            if (null == summary)
            {
                return Json(new { status = "failure" });
            }
            var lstAllowances = new List<prl_employee_settlement_allowance>();
            if(summary.EntitledAllowances !=null)
                summary.EntitledAllowances.ForEach(x=> lstAllowances.Add(new prl_employee_settlement_allowance()
                                                                         {
                                                                             allowance_id=x.Id,
                                                                             amount=x.Amount,
                                                                             due_amount=0
                                                                         }));

            if(summary.DueAllowances !=null)
                summary.DueAllowances.ForEach(x => lstAllowances.Add(new prl_employee_settlement_allowance()
                                                                     {
                                                                         due_allowance_id =  x.Id,
                                                                         amount = 0,
                                                                         due_amount = x.Amount
                                                                     }));


            var lstDeduction = new List<prl_employee_settlement_deduction>();
            if (summary.EntitledDeductions != null)
                summary.EntitledDeductions.ForEach(x => lstDeduction.Add(new prl_employee_settlement_deduction()
                                                                         {
                                                                             deduction_id = x.Id,
                                                                             amount = x.Amount
                                                                         }));

            var lstBonus = new List<prl_employee_settlement_detail>();
            if (summary.EntitledBonus != null)
                summary.EntitledBonus.ForEach(x => lstBonus.Add(new prl_employee_settlement_detail(){
                    bonus_name_id_earning = x.Id,
                   bonus_earning_amount =  x.Amount
                }));

            if (summary.BonusDue != null)
                summary.BonusDue.ForEach(x => lstBonus.Add(new prl_employee_settlement_detail()
                {
                    bonus_name_id_deductions = x.Id,
                    bonus_deduction_amount = x.Amount
                }));

            var lstOvertime = new List<prl_employee_settlement_over_time>();
            if (summary.EntitledOT != null)
                summary.EntitledOT.ForEach(x => lstOvertime.Add(new prl_employee_settlement_over_time()
                {
                   
                    overtime_id = x.Id,
                    amount = x.Amount
                }));

            var service = new SettlementService();
            service.SettleEmployee(summary.EmployeeId,summary.EntitledCalculationDays,summary.DueCalculationDays,summary.BasicSalary,summary.DueSalary,lstAllowances,lstDeduction,lstBonus,lstOvertime, summary.ProvidentFund, summary.ProvidentFundCompany);
            HttpContext.Cache.Remove(empid); 
            return Json(new {status = "ok"});
        }
    }

}
