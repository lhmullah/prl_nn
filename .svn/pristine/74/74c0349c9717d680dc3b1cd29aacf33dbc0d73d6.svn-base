using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using com.gpit.DataContext;
using AutoMapper;
using com.gpit.Model;
using PayrollWeb.CustomSecurity;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly payroll_systemContext dataContext;
        
        public DepartmentsController(payroll_systemContext cont)
        {
            this.dataContext = cont;

        }

        [PayrollAuthorize]
        public ActionResult Index()
        {

            var lstDept = dataContext.prl_department.ToList().OrderBy(p=>p.name);
            return View(Mapper.Map<List<Department>>(lstDept));
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            
            var dept = new Department();
            return View(dept);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(Department d)
        {
            var res = new OperationResult();
            try
            {
                var dept = Mapper.Map<prl_department>(d);
                dept.created_by = User.Identity.Name;
                dept.created_date = DateTime.Now;
                dataContext.prl_department.Add(dept);
                dataContext.SaveChanges();
                res.IsSuccessful = true;
                res.Message = dept.name + " created. ";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch
            {
                
                return View();
            }
        }

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {

           var dept =  dataContext.prl_department.SingleOrDefault(x => x.id == id);
           return View(Mapper.Map<Department>(dept));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, Department dept)
        {
            var res = new OperationResult();
            try
            {
                var extDept =  dataContext.prl_department.SingleOrDefault(x => x.id == dept.id);
                extDept.name = dept.name;
                dataContext.SaveChanges();
                res.IsSuccessful = true;
                res.Message = extDept.name + " edited. ";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var d = dataContext.prl_department.SingleOrDefault(x => x.id == id);
                name = d.name;
                dataContext.prl_department.Remove(d);
                dataContext.SaveChanges();
                
                res.IsSuccessful = true;
                res.Message = name+" deleted.";
                TempData.Add("msg",res);
                return RedirectToAction("Index");
                // return Json(new{ result="true",mag=name+" is deleted."});
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = name + " could not delete.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
        }
        
    }
}
