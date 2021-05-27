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
    public class BankBranchController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public BankBranchController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            var lstAllBankBranch = dataContext.prl_bank_branch.ToList().OrderBy(p=>p.prl_bank.bank_name);
            return View(Mapper.Map<List<BankBranch>>(lstAllBankBranch));
        }

        //
        // GET: /BankBranch/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {

            var _BankBranch = new BankBranch();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var t in dataContext.prl_bank)
            {
                SelectListItem s = new SelectListItem();
                s.Text = t.id.ToString();
                s.Value = t.bank_name;
                items.Add(s);
            }
            ViewBag.AllBank = items;
            return View(_BankBranch);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(string SelectedValue, BankBranch branch)
        {
            var res = new OperationResult();
            if (ModelState.IsValid)
            {
                try
                {
                    var _branch = new prl_bank_branch();
                    _branch.bank_id = Convert.ToInt16(SelectedValue);
                    _branch.branch_name = branch.branch_name;
                    _branch.branch_code = branch.branch_code;

                    dataContext.prl_bank_branch.Add(_branch);
                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = branch.branch_name + " created successfully.";
                    TempData.Add("msg", res);

                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }


            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var t in dataContext.prl_bank)
            {
                SelectListItem s = new SelectListItem();
                s.Text = t.id.ToString();
                s.Value = t.bank_name;
                items.Add(s);
            }
            ViewBag.AllBank = items;
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var bankBranch = dataContext.prl_bank_branch.SingleOrDefault(x=>x.id==id);
            ViewBag.Banks = dataContext.prl_bank.ToList();
            return View(Mapper.Map<BankBranch>(bankBranch));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(BankBranch bb)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var editBranch = dataContext.prl_bank_branch.SingleOrDefault(x => x.id == bb.id);
                    editBranch.bank_id = bb.bank_id;
                    editBranch.branch_name = bb.branch_name;
                    editBranch.branch_code = bb.branch_code;

                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = "Information of " + editBranch.branch_name + " updated successfully.";
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
        public ActionResult Delete(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var _branch = dataContext.prl_bank_branch.SingleOrDefault(x => x.id == id);
                if (_branch == null)
                {
                    return HttpNotFound();
                }
                name = _branch.branch_name;
                dataContext.prl_bank_branch.Remove(_branch);
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
