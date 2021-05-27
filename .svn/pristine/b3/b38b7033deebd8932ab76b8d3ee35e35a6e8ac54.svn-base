using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.gpit.DataContext;
using AutoMapper;
using com.gpit.Model;
using Ninject.Infrastructure.Language;
using PayrollWeb.CustomSecurity;
using PayrollWeb.ViewModels;
using PayrollWeb.Utility;
using System.Web.Security;

namespace PayrollWeb.Controllers
{
    public class DivisionController : Controller
    {
        private readonly payroll_systemContext dataContext;
        //
        // GET: /Division/

        public DivisionController(payroll_systemContext cont)
        {
            this.dataContext = cont;

        }

        [PayrollAuthorize]
        public ActionResult Index()
        {

            var lstDiv = dataContext.prl_division.ToList().OrderBy(p=>p.name);
            return View(Mapper.Map<List<Division>>(lstDiv));
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            var _div = new Division();
            return View(_div);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(Division item)
        {
            var res = new OperationResult();
            try
            {
                // TODO: Add insert logic here
                var _div = Mapper.Map<prl_division>(item);
                dataContext.prl_division.Add(_div);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = _div.name + " created. ";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return View();
        }

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var _div = dataContext.prl_division.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Division>(_div));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, Division item)
        {
            var res = new OperationResult();
            try
            {
                // TODO: Add update logic here
                var extBank = dataContext.prl_division.SingleOrDefault(x => x.id == item.id);
                extBank.name = item.name;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = extBank.name + " edited. ";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch
            {

                return View();
            }
        }

        [PayrollAuthorize]
        public ActionResult Delete(int id)
        {
            string name = "";
            var res = new OperationResult();

            try
            {
                var item = dataContext.prl_division.SingleOrDefault(x => x.id == id);
                if (item == null)
                {
                    return HttpNotFound();
                }
                name = item.name;
                dataContext.prl_division.Remove(item);
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

        //
        // POST: /Division/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
