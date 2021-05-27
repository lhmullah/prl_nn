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
    public class GradesController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public GradesController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {

            var lstDept = dataContext.prl_grade.ToList().OrderBy(p=>p.grade);
            return View(Mapper.Map<List<Grade>>(lstDept));
        }


        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {

            Grade grd = new Grade();
            return View(grd);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(Grade grd)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var _grd = new prl_grade();
                    _grd.grade = grd.grade;
                    _grd.upper_basic = grd.upper_basic;
                    _grd.lower_basic = grd.lower_basic;

                    dataContext.prl_grade.Add(_grd);
                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = "Grade named " + _grd.grade + " created successfully.";
                    TempData.Add("msg", res);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                
            }

            

            return View();
        }

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var grd = dataContext.prl_grade.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Grade>(grd));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, Grade grd)
        {
            var res = new OperationResult();
            try
            {
                var editGrades = dataContext.prl_grade.SingleOrDefault(x => x.id == grd.id);
                editGrades.grade = grd.grade;
                editGrades.lower_basic = grd.lower_basic;
                editGrades.upper_basic = grd.upper_basic;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message ="grade updated successfully.";
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
                var _grd = dataContext.prl_grade.SingleOrDefault(x => x.id == id);
                if (_grd == null)
                {
                    return HttpNotFound();
                }
                name = _grd.grade;
                dataContext.prl_grade.Remove(_grd);
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
