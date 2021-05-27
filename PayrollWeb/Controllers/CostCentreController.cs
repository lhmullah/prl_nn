using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.linde.DataContext;
using AutoMapper;
using com.linde.Model;
using Ninject.Infrastructure.Language;
using PayrollWeb.CustomSecurity;
using PayrollWeb.ViewModels;
using PayrollWeb.Utility;
using System.Web.Security;

namespace PayrollWeb.Controllers
{
    public class CostCentreController : Controller
    {
        private readonly payroll_systemContext dataContext;
        //
        // GET: /CostCentre/

        public CostCentreController(payroll_systemContext cont)
        {
            this.dataContext = cont;

        }

        [PayrollAuthorize]
        public ActionResult Index()
        {

            var lstLoc = dataContext.prl_cost_centre.ToList().OrderBy(p=>p.cost_centre_name);
            return View(Mapper.Map<List<CostCentre>>(lstLoc));
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            var _div = new CostCentre();
            return View(_div);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(CostCentre item)
        {
            var res = new OperationResult();
            try
            {
                // TODO: Add insert logic here
                var _div = Mapper.Map<prl_cost_centre>(item);
                dataContext.prl_cost_centre.Add(_div);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = _div.cost_centre_name + " created. ";
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
            var _div = dataContext.prl_cost_centre.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<CostCentre>(_div));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, CostCentre item)
        {
            var res = new OperationResult();
            try
            {
                // TODO: Add update logic here
                var extBank = dataContext.prl_cost_centre.SingleOrDefault(x => x.id == item.id);
                extBank.cost_centre_name = item.cost_centre_name;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = extBank.cost_centre_name + " edited. ";
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
            string cost_centre_name = "";
            var res = new OperationResult();

            try
            {
                var item = dataContext.prl_cost_centre.SingleOrDefault(x => x.id == id);
                if (item == null)
                {
                    return HttpNotFound();
                }
                cost_centre_name = item.cost_centre_name;
                dataContext.prl_cost_centre.Remove(item);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = cost_centre_name + " deleted.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = cost_centre_name + " could not delete.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /CostCentre/Delete/5

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
