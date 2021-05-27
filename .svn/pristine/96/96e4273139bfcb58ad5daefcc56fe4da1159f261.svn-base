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
    public class BankController : Controller
    {
        private readonly payroll_systemContext dataContext;
        //
        // GET: /Bank/

        public BankController(payroll_systemContext cont)
        {
            this.dataContext = cont;

        }

        [PayrollAuthorize]
        public ActionResult Index(string menuName)
        {
            var lstBank = dataContext.prl_bank.ToList().OrderBy(p=>p.bank_name);
            return View(Mapper.Map<List<Bank>>(lstBank));
        }

        //
        // GET: /Bank/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {

            var _bank = new Bank();
            return View(_bank);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(Bank b)
        {
            var res = new OperationResult();
            try
            {
                var _bank = Mapper.Map<prl_bank>(b);
                dataContext.prl_bank.Add(_bank);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = _bank.bank_name + " created. ";
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
            var _bank = dataContext.prl_bank.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Bank>(_bank));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, Bank _bank)
        {
            var res = new OperationResult();
            try
            {
                var extBank = dataContext.prl_bank.SingleOrDefault(x => x.id == _bank.id);
                extBank.bank_name = _bank.bank_name;
                extBank.bank_code = _bank.bank_code;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = extBank.bank_name + " edited. ";
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
                var _bank = dataContext.prl_bank.SingleOrDefault(x => x.id == id);
                if (_bank == null)
                {
                    return HttpNotFound();
                }
                name = _bank.bank_name;
                dataContext.prl_bank.Remove(_bank);
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
