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
    public class FiscalYrController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public FiscalYrController(payroll_systemContext context)
        {
            this.dataContext = context;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            

            var lstFYr = dataContext.prl_fiscal_year.ToList().OrderByDescending(p=>p.fiscal_year);
            return View(Mapper.Map<List<FiscalYr>>(lstFYr));
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            

            FiscalYr fYr = new FiscalYr();
            return View(fYr);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(FiscalYr fisYr)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var fYr = new prl_fiscal_year();
                    fYr.fiscal_year = fisYr.fiscal_year;
                    fYr.assesment_year = fisYr.assesment_year;

                    dataContext.prl_fiscal_year.Add(fYr);
                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = fYr.fiscal_year + " created successfully.";
                    TempData.Add("msg", res);

                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            

            return View();
        }

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var desig = dataContext.prl_fiscal_year.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<FiscalYr>(desig));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(FiscalYr fYr)
        {
            var res = new OperationResult();
            try
            {
                var editFyr = dataContext.prl_fiscal_year.SingleOrDefault(x => x.id == fYr.id);
                editFyr.fiscal_year = fYr.fiscal_year;
                editFyr.assesment_year = fYr.assesment_year;

                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Information of " + editFyr.fiscal_year + " updated successfully.";
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
                var fisFyr = dataContext.prl_fiscal_year.SingleOrDefault(x => x.id == id);
                if (fisFyr == null)
                {
                    return HttpNotFound();
                }
                name = fisFyr.fiscal_year;
                dataContext.prl_fiscal_year.Remove(fisFyr);
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
