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
    public class AllowanceNameController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public AllowanceNameController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult Index()
        {

            var list = dataContext.prl_allowance_name.ToList();
            var vwList = Mapper.Map<List<AllowanceName>>(list).AsEnumerable();
            return View(vwList);
        }

        //
        // GET: /AllowanceName/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {

            var all = new AllowanceName();
            var lst = dataContext.prl_allowance_head.ToList();
            var lst2 = Mapper.Map<List<AllowanceHead>>(lst);
            ViewBag.AllAllowanceHead = lst2;
            return View(all);
        }

        //
        // POST: /AllowanceName/Create

        [HttpPost]
        public ActionResult Create(string SelectedValue, AllowanceName item)
        {

            var res = new OperationResult();
            try
            {
                var allName = Mapper.Map<prl_allowance_name>(item);
                dataContext.prl_allowance_name.Add(allName);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = allName.allowance_name + " created.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            var lst = dataContext.prl_allowance_head.ToList();
            var lst2 = Mapper.Map<List<AllowanceHead>>(lst);
            ViewBag.AllAllowanceHead = lst2;
            return View();
        }

        

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var prlAll = dataContext.prl_allowance_name.SingleOrDefault(x => x.id == id);
            var dn = Mapper.Map<AllowanceName>(prlAll);
            var lstHeads = dataContext.prl_allowance_head.ToList();
            ViewBag.AllAllowanceHead = Mapper.Map<List<AllowanceHead>>(lstHeads);
            return View(dn);
        }

        //
        // POST: /AllowanceName/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, AllowanceName item)
        {
            var res = new OperationResult();
            try
            {
                var allName = dataContext.prl_allowance_name.SingleOrDefault(x => x.id == item.id);
                allName.allowance_name = item.allowance_name;
                allName.allowance_head_id = item.allowance_head_id;
                allName.gl_code = item.gl_code;
                dataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            var lst = dataContext.prl_allowance_head.ToList();
            ViewBag.AllAllowanceHead = Mapper.Map<List<AllowanceHead>>(lst);
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Delete(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var allName = dataContext.prl_allowance_name.SingleOrDefault(x => x.id == id);
                if (allName == null)
                {
                    return HttpNotFound();
                }
                name = allName.allowance_name;
                dataContext.prl_allowance_name.Remove(allName);
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
        // POST: /AllowanceName/Delete/5

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


        public ActionResult AllowanceConfiguration()
        {
            return View();
        }

        public ActionResult ConfiguredAllowance()
        {
            var all = new AllowanceConfiguration();
            var lst = dataContext.prl_allowance_configuration.ToList();
            var lst2 = Mapper.Map<List<AllowanceName>>(lst);
            ViewBag.AllowanceConfiguration = lst2;
            return View(all);
        }
    }
}
