using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.linde.DataContext;
using com.linde.Model;
using PayrollWeb.CustomSecurity;

namespace PayrollWeb.Controllers
{
    public class HomeController : Controller
    {

        private readonly payroll_systemContext dataContext;

        public HomeController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult Index()
        {
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //string s = User.Identity.Name;

            return View();
        }

        public ActionResult TotalEmployeeStatusSummary()
        {
            try
            {
                var currentMonthYear = DateTime.Now.Date;
                //var srCount = 0;
                var curDate = DateTime.Now.Date;

                var totalActive = dataContext.prl_employee.AsEnumerable().Count(x => x.is_active.Value == Convert.ToSByte(true));
                var totalNew = dataContext.prl_employee.AsEnumerable().Count(x => x.joining_date.ToString("yyyy-MM") == currentMonthYear.ToString("yyyy-MM"));
                //var totalSalaryReview =dataContext.prl_salary_review.AsEnumerable().Where(x => x.is_arrear_calculated == "No");
                //if (totalSalaryReview != null)
                //{
                //    srCount = totalSalaryReview.Count();
                //}

                var noDiscontinue = dataContext.prl_employee_discontinue.AsEnumerable().Count(x => x.discontinue_date != null && x.discontinue_date.Date.ToString("yyyy-MM") == curDate.ToString("yyyy-MM"));

                return Json(new { totalActive = totalActive, totalNew = totalNew, discontinueCount = noDiscontinue }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public ActionResult OverallPayrollStatus()
        {
            var salaryProcessedEmp = 0;
            decimal totalTaxAmount = 0;

            var lastProcess = "No salary processed";
            var processMonth = dataContext.prl_salary_process.DefaultIfEmpty().Max(x => x.salary_month);
            if (processMonth != null)
            {
                lastProcess = processMonth.ToString("yyyy-MM");
                salaryProcessedEmp = dataContext.prl_salary_process_detail.AsEnumerable().Where(x => x.salary_month.ToString("yyyy-MM") == lastProcess).Distinct().Count();
                totalTaxAmount = dataContext.prl_employee_tax_process.AsEnumerable().Where(x => x.salary_month.ToString("yyyy-MM") == lastProcess).Sum(x => x.monthly_tax);
                totalTaxAmount = decimal.Round(totalTaxAmount, MidpointRounding.AwayFromZero);
            }

            return Json(new {processMonth = lastProcess, totalEmployee = salaryProcessedEmp, totalTax = totalTaxAmount},JsonRequestBehavior.AllowGet);
        }

        public ActionResult MiscellaneousInfo()
        {
            var curDate = DateTime.Now.Date;
            var startDate = new DateTime(curDate.Year, curDate.Month, 1);
            var endDate = new DateTime(curDate.Year, curDate.Month, DateTime.DaysInMonth(curDate.Year, curDate.Month));

            var noLWP = dataContext.prl_employee_leave_without_pay.AsEnumerable().Count(x => (x.strat_date.Value.Date <= startDate && x.end_date.Value.Date <= endDate) || (x.strat_date.Value > x.strat_date.Value && x.end_date.Value >= endDate));
            
            var childAlw =dataContext.prl_employee_children_allowance.AsEnumerable().Count(x => x.is_active != null && x.is_active.Value == Convert.ToSByte(true));

            return Json(new {lwpCount = noLWP, childAlwCount=childAlw}, JsonRequestBehavior.AllowGet);

        }
    }
}
