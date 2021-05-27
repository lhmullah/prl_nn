using AutoMapper;
using com.linde.DataContext;
using com.linde.Model;
using PayrollWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.Controllers
{
    public class SubDepartmentController : Controller
    {

        private readonly payroll_systemContext dataContext;

        public SubDepartmentController (payroll_systemContext context)
        {
            this.dataContext = context;
        }
        //
        // GET: /SubDepartment/

        public ActionResult Index()
        {
            var list = dataContext.prl_sub_department.OrderBy(x => x.id).ToList();
            var vwList = Mapper.Map<List<SubDepartment>>(list).AsEnumerable();
            return View(vwList);
        }

        // GET: /SubDepartment/Create

        [HttpGet]
        public ActionResult Create()
        {
            var all = new SubDepartment();
            var lst = dataContext.prl_department.ToList();
            var lst2 = Mapper.Map<List<Department>>(lst);
            ViewBag.AllDepartments = lst2;
            return View(all);
        }

        // POST: /SubDepartment/Create

        [HttpPost]
        public ActionResult Create(string SelectedValue, SubDepartment item)
        {

            var res = new OperationResult();
            try
            {
                var subDept = Mapper.Map<prl_sub_department>(item);
                dataContext.prl_sub_department.Add(subDept);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = subDept.name + " created.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            var lst = dataContext.prl_department.ToList();
            var lst2 = Mapper.Map<List<Department>>(lst);
            ViewBag.AllDepartment = lst2;
            return View();
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var prlAll = dataContext.prl_sub_department.SingleOrDefault(x => x.id == id);
            var dn = Mapper.Map<SubDepartment>(prlAll);
            var lstDept = dataContext.prl_department.ToList();
            ViewBag.AllDepartment = Mapper.Map<List<Department>>(lstDept);
            return View(dn);
        }

        //
        // POST: /SubDepartment/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, SubDepartment item)
        {
            var res = new OperationResult();
            try
            {
                var subDept = dataContext.prl_sub_department.SingleOrDefault(x => x.id == item.id);
                subDept.name = item.name;
                subDept.department_id = item.department_id;
                dataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            var lst = dataContext.prl_department.ToList();
            ViewBag.AllDepartment = Mapper.Map<List<Department>>(lst);
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var subDept = dataContext.prl_sub_department.SingleOrDefault(x => x.id == id);
                if (subDept == null)
                {
                    return HttpNotFound();
                }
                name = subDept.name;
                dataContext.prl_sub_department.Remove(subDept);
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
        // POST: /SubDepartment/Delete/5

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
