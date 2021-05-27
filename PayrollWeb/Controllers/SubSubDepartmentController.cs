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
    public class SubSubDepartmentController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public SubSubDepartmentController (payroll_systemContext context)
        {
            this.dataContext = context;
        }

        //
        // GET: /SubSubDepartment/

        public ActionResult Index()
        {
            var list = dataContext.prl_sub_sub_department.OrderBy(x => x.id).ToList();
            var vwList = Mapper.Map<List<SubSubDepartment>>(list).AsEnumerable();
            return View(vwList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var lstDept = dataContext.prl_department.ToList();
            ViewBag.Departments = lstDept;

            ViewBag.SubDepartments = "";
            
            return View();
        }

        [HttpPost]
        public ActionResult Create(string SelectedValue, SubSubDepartment item)
        {
            var res = new OperationResult();
            try
            {
                var subSubDept = Mapper.Map<prl_sub_sub_department>(item);
                dataContext.prl_sub_sub_department.Add(subSubDept);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = subSubDept.name + " created.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var lstDept = dataContext.prl_department.ToList();
            ViewBag.Departments = lstDept;

            ViewBag.SubDepartments = "";

            return View();
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var lstDept = dataContext.prl_sub_sub_department.SingleOrDefault(x => x.id==id).prl_sub_department.prl_department.id.ToString();
            ViewBag.Departments = lstDept;

            //var subDept = dataContext.prl_sub_sub_department.SingleOrDefault(x => x.id==id).sub_department_id;
            var lstSubDept = dataContext.prl_sub_sub_department.SingleOrDefault(x => x.id == id).prl_sub_department.id.ToString();
            ViewBag.SubDepartments = lstSubDept;

            var prlAll = dataContext.prl_sub_sub_department.SingleOrDefault(x => x.id == id);
            var dn = Mapper.Map<SubSubDepartment>(prlAll);
            return View(dn);
        }

        //
        // POST: /SubSubDepartment/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, SubSubDepartment item)
        {
            var res = new OperationResult();
            try
            {
                var subSubDept = dataContext.prl_sub_sub_department.SingleOrDefault(x => x.id == item.id);
                subSubDept.name = item.name;
                subSubDept.sub_department_id = item.sub_department_id;
                dataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            var lst = dataContext.prl_sub_department.ToList();
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
                var subSubDept = dataContext.prl_sub_sub_department.SingleOrDefault(x => x.id == id);
                if (subSubDept == null)
                {
                    return HttpNotFound();
                }
                name = subSubDept.name;
                dataContext.prl_sub_sub_department.Remove(subSubDept);
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
        // POST: /SubSubDepartment/Delete/5

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

        // Department Dropdown Load
        [HttpPost]
        public ActionResult Department()
        {
            var deptList = dataContext.prl_department.ToList();
            List<DropDown> ddlDept = new List<DropDown>();
            foreach (var item in deptList)
            {
                DropDown dropDown = new DropDown();
                dropDown.Value = item.id.ToString();
                dropDown.Text = item.name.ToString();
                ddlDept.Add(dropDown);
            }
            return Json(ddlDept);
        }

        // SubDepartment Dropdown Load

        [HttpPost]
        public ActionResult SubDepartment(int? deptId)
        {
            var subList = dataContext.prl_sub_department.Where(sub => sub.department_id == deptId).ToList();

            List<DropDown> ddlSub = new List<DropDown>();
            foreach (var item in subList)
            {
                DropDown dropDown = new DropDown();
                dropDown.Value = item.id.ToString();
                dropDown.Text = item.name.ToString();
                ddlSub.Add(dropDown);
            }
            return Json(ddlSub);
        }

    }
}
