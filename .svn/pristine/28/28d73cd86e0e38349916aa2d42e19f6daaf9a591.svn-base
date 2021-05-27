using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using com.gpit.DataContext;
using com.gpit.Model;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Controllers
{
    public class DesignationController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public DesignationController(payroll_systemContext context)
        {
            this.dataContext = context;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            var lstDesignation = dataContext.prl_designation.ToList().OrderBy(p=>p.name);
            return View(Mapper.Map<List<Designation>>(lstDesignation));
        }

        
        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult CreateDesignation()
        {
            Designation design = new Designation();
            return View(design);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult CreateDesignation(Designation d)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var desig = new prl_designation();
                    desig.name = d.name;

                    dataContext.prl_designation.Add(desig);
                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = desig.name + " created successfully.";
                    TempData.Add("msg", res);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }


            return View();
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var desig = dataContext.prl_designation.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Designation>(desig));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, Designation desig)
        {
            var res = new OperationResult();
            try
            {
                var editDesig = dataContext.prl_designation.SingleOrDefault(x => x.id == desig.id);
                editDesig.name = desig.name;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Information of " + editDesig.name + " updated successfully.";
                TempData.Add("msg", res);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return View();
        }

        [PayrollAuthorize]
        public ActionResult Delete(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var Desig = dataContext.prl_designation.SingleOrDefault(x => x.id == id);
                if (Desig == null)
                {
                    return HttpNotFound();
                }
                name = Desig.name;
                dataContext.prl_designation.Remove(Desig);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = name + " deleted.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
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
