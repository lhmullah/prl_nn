using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.gpit.DataContext;
using AutoMapper;
using com.gpit.Model;
using Ninject.Infrastructure.Language;
using PayrollWeb.ViewModels;
using System.Web.Security;
using PayrollWeb.Utility;

namespace PayrollWeb.Controllers
{
    public class SetupController : Controller
    {
        //
        // GET: /Setup/
        private readonly payroll_systemContext dataContext;

        public SetupController(payroll_systemContext context)
        {
            this.dataContext = context;
        }

        public ActionResult Index()
        {
            return View();
        }


        //public PartialViewResult GetMenuForUser()
        //{
        //    var model = _securityLayer.GetUrlForUser(HttpContext.User.Identity.Name);

        //    return PartialView("_UserMenu", model);
        //}

        public ActionResult CreateEmployee()
        {
            var ev = new EmployeeView();
            ev.account_no = "786";
            
            return View(ev);
        }

        [HttpGet]
        public ActionResult CreateDivision()
        {
            var ev = new Division();
            return View(ev);
        }
        [HttpPost]
        public ActionResult CreateDivision(Division d)
        {
            if(ModelState.IsValid)
            {
                var ev = new prl_division();
                ev.name = d.name;

                dataContext.prl_division.Add(ev);
                dataContext.SaveChanges();
                return RedirectToAction("CreateEmployee");
            }
            return View();
        }
        [HttpGet]
        public ActionResult CreateDepartment()
        {
            Department dp = new Department();
            return View(dp);
        }
        [HttpPost]
        public ActionResult CreateDepartment(Department d)
        {
            if (ModelState.IsValid)
            {
                var ev = new prl_department();
                ev.name = d.name;
                ev.created_by = "test";
                ev.created_date = DateTime.Now;

                dataContext.prl_department.Add(ev);
                dataContext.SaveChanges();
                return RedirectToAction("CreateEmployee");
            }
            return View();
        }
    }
}
