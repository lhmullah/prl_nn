using System;
using System.Collections.Generic;
using System.IO;
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
    public class CompanyController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public CompanyController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult Index()
        {

            var lstCompanies = dataContext.prl_company.ToList().OrderBy(p=>p.name);
            return View(Mapper.Map<List<Company>>(lstCompanies));
        }

        [PayrollAuthorize]
        public ActionResult Details(int id)
        {
            var lstCom = dataContext.prl_company.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Company>(lstCom));
        }

        public ActionResult Create()
        {
            Company _Company = new Company();
            return View(_Company);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Logo")]Company comp, HttpPostedFileBase Logo)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var _comp = new prl_company();
                    if (Logo != null && Logo.ContentLength > 0)
                    {
                        var logoName = Path.GetFileName(Logo.FileName);
                        if (System.IO.File.Exists(Server.MapPath("~/Images/CompanyLogo/"+logoName)))
                        {
                            logoName = 1+logoName;
                        }
                        var path = Path.Combine(Server.MapPath("~/Images/CompanyLogo"), logoName);
                        Logo.SaveAs(path);
                        _comp.logo = logoName;
                    }
                    _comp.name = comp.name;
                    _comp.description = comp.description;
                    _comp.address = comp.address;
                    _comp.primary_phone = comp.primary_phone;
                    _comp.secondary_phone = comp.secondary_phone;
                    _comp.email = comp.email;
                    _comp.web = comp.web;
                    _comp.created_by = User.Identity.Name;
                    _comp.created_date = DateTime.Now;

                    dataContext.prl_company.Add(_comp);
                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = "Company named " + comp.name + " created successfully.";
                    TempData.Add("msg", res);

                    return RedirectToAction("Index");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        

        public ActionResult Edit(int id)
        {
            var _company = dataContext.prl_company.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Company>(_company));
        }

        [HttpPost]
        public ActionResult Edit(int id,Company comp, HttpPostedFileBase cLogo)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var editCompany = dataContext.prl_company.SingleOrDefault(x => x.id == comp.id);

                    if (cLogo != null && cLogo.ContentLength > 0)
                    {
                        if (editCompany.logo != null)
                        {
                            System.IO.File.Delete(Server.MapPath("~/Images/CompanyLogo/" + editCompany.logo));
                        }
                        var logoName = Path.GetFileName(cLogo.FileName);
                        if (System.IO.File.Exists(Server.MapPath("~/Images/CompanyLogo/" + logoName)))
                        {
                            logoName = 1+logoName;
                        }
                        var path = Path.Combine(Server.MapPath("~/Images/CompanyLogo"), logoName);
                        cLogo.SaveAs(path);
                        editCompany.logo = logoName;
                    }
                    editCompany.name = comp.name;
                    editCompany.description = comp.description;
                    editCompany.address = comp.address;
                    editCompany.primary_phone = comp.primary_phone;
                    editCompany.secondary_phone = comp.secondary_phone;
                    editCompany.email = comp.email;
                    editCompany.web = comp.web;
                    editCompany.updated_by = User.Identity.Name;
                    editCompany.updated_date = DateTime.Now;

                    res.IsSuccessful = true;
                    res.Message = "Information of " + editCompany.name + " updated successfully.";
                    TempData.Add("msg", res);

                    dataContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                
            }

            return View();
        }

        
        public ActionResult Delete(int id)
        {
            string name = "";
            string logo = "";
            var res = new OperationResult();
            try
            {
                var theData = dataContext.prl_company.SingleOrDefault(x => x.id == id);
                if (theData == null)
                {
                    return HttpNotFound();
                }
                name = theData.name;
                logo = theData.logo;

                dataContext.prl_company.Remove(theData);
                dataContext.SaveChanges();

                System.IO.File.Delete(Server.MapPath("~/Images/CompanyLogo/" + logo));

                res.IsSuccessful = true;
                res.Message = name + " deleted successfully.";
                TempData.Add("msg", res);
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = name + " could not delete.";
                TempData.Add("msg", res);
            }

            return RedirectToAction("Index");
        }
    }
}
