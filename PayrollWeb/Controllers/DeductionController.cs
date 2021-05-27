using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using com.linde.DataContext;
using com.linde.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using PagedList;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Service;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;
using PayrollWeb.ViewModels.Utility;
using Microsoft.Reporting.WebForms;


namespace PayrollWeb.Controllers
{
    public class DeductionController : Controller
    {
        private readonly payroll_systemContext dataContext;
        //
        // GET: /Deduction/

        public DeductionController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult DeductionMain(string menuName)
        {

            return View();
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            var lstDedHead = dataContext.prl_deduction_head.ToList();
            return View(Mapper.Map<List<DeductionHead>>(lstDedHead));
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            var dedHead = new DeductionHead();
            return View(dedHead);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(DeductionHead item)
        {


            var res = new OperationResult();
            try
            {
                var dedHead = Mapper.Map<prl_deduction_head>(item);
                dataContext.prl_deduction_head.Add(dedHead);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = dedHead.name + " created. ";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch
            {

                return View();
            }
        }

        public ActionResult Paging(int? page)
        {
            int pageSize = 25;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var _loanEmployees = dataContext.prl_loan_entry;
            var loanList = Mapper.Map<List<LoanEntry>>(_loanEmployees);
            var pglst = loanList.ToPagedList(pageIndex, pageSize);
            return View("LoanEntryGrid", pglst);
        }

        [PayrollAuthorize]
        public ActionResult LoanEntryGrid(int? empid, FormCollection collection, string sButton)
        {
            var lists = new List<LoanEntry>().ToPagedList(1, 1);
            if (sButton == null)
            {
                var lstEmp = dataContext.prl_loan_entry;
                lists = Mapper.Map<List<LoanEntry>>(lstEmp).ToPagedList(1, 25);
            }
            else
            {
                if (empid == null)
                {
                    var lstEmp = dataContext.prl_loan_entry;
                    lists = Mapper.Map<List<LoanEntry>>(lstEmp).ToPagedList(1, 25);
                    ModelState.AddModelError("", "Please select an employee or put employee ID");
                }
                else
                {
                    var _emp = dataContext.prl_loan_entry.Where(x => x.emp_id == empid);
                    if (_emp.Count() > 0)
                    {
                        lists = Mapper.Map<List<LoanEntry>>(_emp).ToPagedList(1, 1);
                    }
                    else
                    {
                        var lstEmp = dataContext.prl_loan_entry;
                        lists = Mapper.Map<List<LoanEntry>>(lstEmp).ToPagedList(1, 25);
                        ModelState.AddModelError("", "Threre is no information for the given employee ID");
                    }
                }
            }
            return View(lists);
        }

        [PayrollAuthorize]
        public ActionResult LoanEntry()
        {
            var loanTypesList = dataContext.prl_deduction_name.Where(x => x.deduction_head_id == 1).ToList(); //1 is Loan Deductions
            ViewBag.loanTypes = loanTypesList;

            LoanEntry LsEntryView = new LoanEntry();
            DateTime nowDateTime = DateTime.Now;
            DateTime StartDate = new DateTime(nowDateTime.Year, nowDateTime.Month, 1);
            DateTime EndDate = StartDate.AddMonths(12).AddDays(-1);

            LsEntryView.loan_start_date = StartDate;
            LsEntryView.loan_end_date = EndDate;

            return View(LsEntryView);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult LoanEntry(int? empid, LoanEntry item)
        {
            var res = new OperationResult();
            try
            {
                if (empid == null)
                {
                    ModelState.AddModelError("", "Please select an employee.");
                }
                else
                {
                    int EmpId = (int)empid;

                    if (ModelState.IsValid)
                    {
                        decimal monthlyInstallment = item.principal_amount / Math.Round(Convert.ToDecimal((item.loan_end_date.Subtract(item.loan_start_date).Days / (365 / 12)).ToString("0.00")), 2);
                        var loanEntry = new prl_loan_entry();
                        loanEntry.emp_id = EmpId;
                        loanEntry.deduction_name_id = item.deduction_name_id;
                        loanEntry.loan_start_date = item.loan_start_date;
                        loanEntry.loan_end_date = item.loan_end_date;
                        loanEntry.principal_amount = item.principal_amount;
                        loanEntry.monthly_installment = monthlyInstallment;

                        dataContext.prl_loan_entry.Add(loanEntry);
                        dataContext.SaveChanges();

                        var emp = dataContext.prl_employee.FirstOrDefault(x => x.id == empid);

                        res.IsSuccessful = true;
                        res.Message = "A Loan Entry for " + emp.name + " Successfully Saved. ";
                        TempData.Add("msg", res);

                        return RedirectToAction("LoanEntryGrid");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var loanTypesList = dataContext.prl_deduction_name.Where(x => x.deduction_head_id == 1).ToList(); //1 is Loan Deductions
            ViewBag.loanTypes = loanTypesList;
            return View();
        }

        [PayrollAuthorize]
        public ActionResult EditLoanEntry(int id)
        {
            var loanTypesList = dataContext.prl_deduction_name.Where(x => x.deduction_head_id == 1).ToList(); //1 is Loan Deductions
            ViewBag.loanTypes = loanTypesList;

            var lstloanEntry = dataContext.prl_loan_entry.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<LoanEntry>(lstloanEntry));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EditLoanEntry(LoanEntry Le)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var editLE = dataContext.prl_loan_entry.SingleOrDefault(x => x.id == Le.id);

                    decimal monthlyInstallment = Le.principal_amount / Math.Round(Convert.ToDecimal((Le.loan_end_date.Subtract(Le.loan_start_date).Days / (365 / 12)).ToString("0.00")), 2);

                    editLE.emp_id = Le.emp_Id;
                    editLE.deduction_name_id = Le.deduction_name_id;
                    editLE.loan_start_date = Le.loan_start_date;
                    editLE.loan_end_date = Le.loan_end_date;
                    editLE.principal_amount = Le.principal_amount;
                    editLE.monthly_installment = monthlyInstallment;

                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = "The information has been updated successfully.";
                    TempData.Add("msg", res);

                    return RedirectToAction("LoanEntryGrid");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return View("LoanEntryGrid");
        }


        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var dedHead = dataContext.prl_deduction_head.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<DeductionHead>(dedHead));
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult LoanSummaryReport()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult LoanSummaryReport(int? empid, FormCollection collection, string sButton, ReportLoanSummary rLS)
        {
            bool errorFound = false;
            var res = new OperationResult();
            try
            {
                if (sButton != null)
                {
                    if (sButton == "Download")
                    {
                        if (empid == null)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Please select an employee or employee no.");
                        }
                        else
                        {
                            var Emp = new Employee();
                            if (empid != null)
                            {
                                var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                                Emp = Mapper.Map<Employee>(_empD);
                            }

                            var loanPS = (from le in dataContext.prl_loan_entry
                                          join lps in dataContext.prl_loan_payment_summary on le.id equals lps.loan_entry_id
                                          join emp in dataContext.prl_employee on le.emp_id equals emp.id
                                          join ded in dataContext.prl_deduction_name on le.deduction_name_id equals ded.id
                                          //join sp in dataContext.prl_salary_process on lps.salary_process_id equals sp.id
                                          where le.emp_id == empid
                                          select new
                                          {
                                              le.id,
                                              le.emp_id,
                                              emp.emp_no,
                                              emp.name,
                                              ded.deduction_name,
                                              le.loan_start_date,
                                              le.loan_end_date,
                                              le.principal_amount,
                                              lps.salary_month_year,
                                              lps.this_month_paid,
                                              lps.loan_realized,
                                              lps.loan_balance
                                          }
                                 ).OrderByDescending(x => x.id).ToList();

                            if (loanPS.Count() == 0)
                            {
                                errorFound = true;
                                ModelState.AddModelError("", "No Record Found for the employee.");
                            }

                            if (!errorFound)
                            {
                                var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                                /****************/
                                string reportType = "PDF";

                                LocalReport lr = new LocalReport();
                                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "LoanSummary.rdlc");
                                if (System.IO.File.Exists(path))
                                {
                                    lr.ReportPath = path;
                                }
                                else
                                {
                                    ViewBag.Years = DateUtility.GetYears();
                                    ViewBag.Months = DateUtility.GetMonths();
                                    return View("Index");
                                }


                                var empDlist = new List<ReportLoanSummary>();

                                foreach (var item in loanPS)
                                {
                                    var reportData = new ReportLoanSummary();

                                    reportData.emp_Id = Emp.id;
                                    reportData.empNo = Emp.emp_no;
                                    reportData.empName = Emp.name;
                                    reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";
                                    reportData.category = empD.employee_category;
                                    reportData.joining_date = Emp.joining_date;

                                    reportData.loan_type_name = item.deduction_name;
                                    reportData.loan_start_date = item.loan_start_date;
                                    reportData.loan_end_date = item.loan_end_date;
                                    reportData.principal_amount = item.loan_balance;
                                    reportData.this_month_installment = item.this_month_paid;
                                    reportData.salary_month = DateUtility.MonthName(item.salary_month_year.Month);
                                    reportData.salary_year = item.salary_month_year.Year;
                                    reportData.loan_realized = item.loan_realized;
                                    reportData.loan_balance = item.loan_balance;

                                    empDlist.Add(reportData);
                                }

                                ReportDataSource rd = new ReportDataSource("DataSet1", empDlist);
                                lr.DataSources.Add(rd);

                                string mimeType;
                                string encoding;
                                string fileNameExtension;

                                string deviceInfo =
                                "<DeviceInfo>" +
                                "<OutputFormat>PDF</OutputFormat>" +
                                "</DeviceInfo>";

                                Warning[] warnings;
                                string[] streams;
                                byte[] renderedBytes;

                                renderedBytes = lr.Render(
                                    reportType,
                                    deviceInfo,
                                    out mimeType,
                                    out encoding,
                                    out fileNameExtension,
                                    out streams,
                                    out warnings);

                                return File(renderedBytes, mimeType);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View();
        }



        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, DeductionHead item)
        {
            var res = new OperationResult();
            try
            {
                var dedHead = dataContext.prl_deduction_head.SingleOrDefault(x => x.id == item.id);
                dedHead.name = item.name;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = dedHead.name + " edited. ";
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
                var dedHead = dataContext.prl_deduction_head.SingleOrDefault(x => x.id == id);
                if (dedHead == null)
                {
                    return HttpNotFound();
                }
                name = dedHead.name;
                dataContext.prl_deduction_head.Remove(dedHead);
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

        [PayrollAuthorize]
        public ActionResult CreateDeductionName()
        {
            var dn = new DeductionName();
            var lst = dataContext.prl_deduction_head.ToList();
            var lst2 = Mapper.Map<List<DeductionHead>>(lst);
            ViewBag.DeductionHeads = lst2;
            return View(dn);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult CreateDeductionName(DeductionName dn)
        {

            var res = new OperationResult();

            if (ModelState.IsValid)
            {
                try
                {
                    var nwName = Mapper.Map<prl_deduction_name>(dn);
                    dataContext.prl_deduction_name.Add(nwName);
                    dataContext.SaveChanges();
                    res.IsSuccessful = true;
                    res.Message = nwName.deduction_name + " created.";
                    TempData.Add("msg", res);
                    return RedirectToAction("DeductionNames");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }
            var lst = dataContext.prl_deduction_head.ToList();
            var lst2 = Mapper.Map<List<DeductionHead>>(lst);
            ViewBag.DeductionHeads = lst2;
            return View();
        }

        [PayrollAuthorize]
        public ActionResult EditDeductionName(int id)
        {
            var prlDn = dataContext.prl_deduction_name.SingleOrDefault(x => x.id == id);
            var dn = Mapper.Map<DeductionName>(prlDn);
            var lstHeads = dataContext.prl_deduction_head.ToList();
            ViewBag.DeductionHeads = Mapper.Map<List<DeductionHead>>(lstHeads);
            return View(dn);
        }

        [HttpPost]
        public ActionResult EditDeductionName(DeductionName dn)
        {
            var res = new OperationResult();
            if (ModelState.IsValid)
            {
                try
                {
                    var name = dataContext.prl_deduction_name.SingleOrDefault(x => x.id == dn.id);
                    name.deduction_name = dn.deduction_name;

                    name.deduction_head_id = dn.deduction_head_id;
                    dataContext.SaveChanges();
                    res.IsSuccessful = true;
                    res.Message = dn.deduction_name + " edited.";
                    TempData.Add("msg", res);
                    return RedirectToAction("DeductionNames");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }
            var lst = dataContext.prl_deduction_head.ToList();
            ViewBag.DeductionHeads = Mapper.Map<List<DeductionHead>>(lst);
            return View();
        }

        [PayrollAuthorize]
        public ActionResult DeleteDeductionName(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var deductionName = dataContext.prl_deduction_name.SingleOrDefault(x => x.id == id);
                if (deductionName == null)
                {
                    return HttpNotFound();
                }
                name = deductionName.deduction_name;
                dataContext.prl_deduction_name.Remove(deductionName);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = name + " deleted.";
                TempData.Add("msg", res);
                return RedirectToAction("DeductionNames");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = name + " could not delete.";
                TempData.Add("msg", res);
                return RedirectToAction("DeductionNames");
            }
        }

        [PayrollAuthorize]
        public ActionResult DeductionNames()
        {
            var list = dataContext.prl_deduction_name.ToList();
            var vwList = Mapper.Map<List<DeductionName>>(list).AsEnumerable();

            return View(vwList);
        }

        [PayrollAuthorize]
        public ActionResult ConfigureDeduction(int dnid = 0)
        {

            var prlGrds = dataContext.prl_grade.ToList();
            var grades = Mapper.Map<List<Grade>>(prlGrds);
            DeductionConfiguration dc;
            if (dnid == 0)
                dc = new DeductionConfiguration();
            else
            {
                var dbVal = dataContext.prl_deduction_configuration.SingleOrDefault(x => x.deduction_name_id == dnid);
                if (dbVal == null)
                {
                    dbVal = new prl_deduction_configuration();
                }
                dc = Mapper.Map<DeductionConfiguration>(dbVal);
                dc.deduction_name_id = dnid;
            }

            dc.Grades = grades;

            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            ViewBag.DeductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            return View(dc);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult ConfigureDeduction(DeductionConfiguration dc)
        {
            bool errorFound = false;
            var operationResult = new OperationResult();


            if (ModelState.IsValid)
            {
                try
                {
                    if (dc.flat_amount == null && dc.percent_amount == null)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Enter flat or percentage amount.");
                    }
                    if (dc.flat_amount <= 0 || dc.percent_amount <= 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Flat or percentage amount should be greater than zero.");
                    }
                    if (dc.deactivation_date != null)
                    {
                        var k = ((DateTime)dc.deactivation_date).Subtract((DateTime)dc.activation_date);
                        if (k.Days <= 0)
                        {
                            errorFound = true;
                            ModelState.AddModelError("deactivation_date",
                                "Deactivation date should be greater than activation date");
                        }
                    }
                    if (!errorFound)
                    {
                        var prlConf = Mapper.Map<prl_deduction_configuration>(dc);
                        prlConf.prl_deduction_name =
                            dataContext.prl_deduction_name.SingleOrDefault(x => x.id == dc.deduction_name_id);

                        DeductionService ds = new DeductionService(dataContext);
                        operationResult.IsSuccessful = ds.CreateConfiguration(prlConf);
                        operationResult.Message = "Deduction saved successfully.";
                    }

                    if (!errorFound && operationResult.IsSuccessful)
                    {
                        operationResult.IsSuccessful = true;
                        operationResult.Message = "Configuration saved.";
                        TempData.Add("msg", operationResult);
                        return RedirectToAction("ConfigureDeduction", new { dnid = 0 });
                    }

                }
                catch (Exception ex)
                {
                    operationResult.IsSuccessful = false;
                    operationResult.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData.Add("msg", operationResult);
                }
            }


            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            ViewBag.DeductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            return View(dc);
        }

        [PayrollAuthorize]
        public ActionResult IndividualDeduction(int eid = 0)
        {

            return View();
        }

        public JsonResult GetEmployeeSearch(string query)
        {
            var lst =
                dataContext.prl_employee.AsEnumerable()
                    .Where(x => x.name.ToLower().Contains(query.ToLower()) || x.emp_no.Contains(query))
                    .Select(x => new SearchEmployeeData() { id = x.id, name = x.name + " (" + x.emp_no + ")" })
                    .ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeDeductions(int empid)
        {
            var res = new OperationResult();
            if (empid != 0)
            {
                ViewBag.EmpId = empid;

                var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                ViewBag.Employee = Mapper.Map<Employee>(emp);

                var lst = dataContext.prl_employee_individual_deduction.Where(x => x.emp_id == empid).ToList();
                return View("IndvDeduction", Mapper.Map<List<EmployeeIndividualDeduction>>(lst));
            }
            else
            {
                res.IsSuccessful = false;
                res.Message = "Please select an employee after search.";
                TempData.Add("msg", res);
                return View("IndividualDeduction");
            }
        }

        public ActionResult EmployeeDeductionDetails(int edi, int empid)
        {
            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            ViewBag.DeductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);

            if (edi == 0)
            {
                var a = new EmployeeIndividualDeduction() { emp_id = empid };
                return View("EmpDedcution", Mapper.Map<EmployeeIndividualDeduction>(a));
            }

            var obj = dataContext.prl_employee_individual_deduction.SingleOrDefault(x => x.id == edi);

            return View("EmpDedcution", Mapper.Map<EmployeeIndividualDeduction>(obj));
        }

        [HttpPost]
        public ActionResult ChangeEmployeeDeduction(EmployeeIndividualDeduction eidObj)
        {
            OperationResult operationResult = new OperationResult();
            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == eidObj.emp_id);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            ViewBag.DeductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            if (ModelState.IsValid)
            {
                try
                {
                    var newOb = Mapper.Map<prl_employee_individual_deduction>(eidObj);

                    if (eidObj.id == 0)
                    {
                        dataContext.prl_employee_individual_deduction.Add(newOb);
                    }
                    else
                    {
                        var extOb = dataContext.prl_employee_individual_deduction.SingleOrDefault(x => x.id == eidObj.id);
                        var entry = dataContext.Entry(extOb);
                        entry.Property(x => x.id).IsModified = false;
                        entry.CurrentValues.SetValues(newOb);
                        entry.State = System.Data.EntityState.Modified;
                    }
                    dataContext.SaveChanges();
                    operationResult.IsSuccessful = true;
                    operationResult.Message = "Saved successfully.";
                    TempData["msg"] = operationResult;
                }
                catch (Exception ex)
                {
                    operationResult.IsSuccessful = true;
                    operationResult.Message = ex.Message;
                    TempData["msg"] = operationResult;
                }
            }
            else
            {
                return View("EmpDedcution", eidObj);
            }

            return RedirectToAction("GetEmployeeDeductions", new { empid = eidObj.emp_id });
        }

        [PayrollAuthorize]
        public ActionResult DeleteEmployeeDeduction(int id, int empid)
        {
            OperationResult operationResult = new OperationResult();

            if (ModelState.IsValid)
            {
                try
                {
                    var extOb = dataContext.prl_employee_individual_deduction.SingleOrDefault(x => x.id == id);
                    dataContext.prl_employee_individual_deduction.Remove(extOb);
                    dataContext.SaveChanges();
                    operationResult.IsSuccessful = true;
                    operationResult.Message = "Saved successfully.";
                    TempData["msg"] = operationResult;
                }
                catch (Exception ex)
                {
                    operationResult.IsSuccessful = false;
                    operationResult.Message = ex.Message;
                    TempData["msg"] = operationResult;
                }
            }
            return RedirectToAction("GetEmployeeDeductions", new { empid = empid });
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult UploadDeduction()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            var prlDeductionNames = dataContext.prl_deduction_name.ToList();
            var deductionNames = Mapper.Map<List<DeductionName>>(prlDeductionNames);
            DeductionUploadView up = new DeductionUploadView();
            up.DeductionNames = deductionNames;
            return View(up);
        }

        [HttpGet]
        public PartialViewResult UploadForm()
        {
            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            var deductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            DeductionUploadView up = new DeductionUploadView();
            up.DeductionNames = deductionNames;
            return PartialView("_DeductionUploadForm", up);
        }

        [HttpPost]
        public ActionResult UploadDeduction(HttpPostedFileBase deductionFile, DeductionUploadView dcv)
        {
            var dateTime = new DateTime(Convert.ToInt32(dcv.Year), Convert.ToInt32(dcv.Month), 1);

            var DedD = dataContext.prl_deduction_name.SingleOrDefault(x => x.id == dcv.DeductionName);

            // Allow any decimal, negetive, parentheses while parse an amount.
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands |
                                 NumberStyles.AllowParentheses | NumberStyles.Number;

            string filefullpath = string.Empty;
            var res = new OperationResult();
            try
            {
                if (deductionFile != null)
                {
                    var file = deductionFile;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);
                        //do stuff with the bytes
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Request.PhysicalApplicationPath, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileBytes);

                        //File Uploaded
                        XSSFWorkbook xssfWorkbook;

                        filefullpath = filePath;

                        //StreamReader streamReader = new StreamReader(model.ImportFile.InputStream);

                        using (FileStream fileStream = new FileStream(filefullpath, FileMode.Open, FileAccess.Read))
                        {
                            xssfWorkbook = new XSSFWorkbook(fileStream);
                        }

                        var deductionXlsViewModelList = new List<DeductionXlsViewModel>();

                        //the columns
                        var properties = new string[] {
                            "Id",
                            "DeductionAmount",
                            "remarks",
                        };

                        ISheet sheet = xssfWorkbook.GetSheetAt(0);

                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            // Row Not Null
                            if (sheet.GetRow(row) != null)
                            {
                                ICell firstCell = sheet.GetRow(row).GetCell(0);

                                // If First Cell Null and Cell not Contain any style others format
                                if (firstCell != null && sheet.GetRow(row) != null && sheet.GetRow(row).Cells.Count > 0) //null is when the row only contains empty cells 
                                {
                                    string Id = GetRowCellValue(sheet, row, properties, "Id", "StringCellValue");
                                    string DeductionAmount = GetRowCellValue(sheet, row, properties, "DeductionAmount", "NumericCellValue");
                                    string remarks = GetRowCellValue(sheet, row, properties, "remarks", "StringCellValue");

                                    var deductionXlsViewModel = new DeductionXlsViewModel
                                    {
                                        Id = Id.ToString(),
                                        DeductionAmount = decimal.Parse(string.IsNullOrEmpty(DeductionAmount) ? "0" : DeductionAmount, style),
                                        DeductionName = DedD.deduction_name,
                                        remarks = remarks.ToString()
                                    };

                                    deductionXlsViewModelList.Add(deductionXlsViewModel);
                                }
                                //else
                                //{
                                //    res.IsSuccessful = false;
                                //    res.Messages.Add("The amount cell of " + row + "th row is empty. Please check the uploading file.");
                                //    TempData.Add("msg", res);
                                //}
                            }
                        }

                        ////////// duplicate check
                        var lstEmpNo = deductionXlsViewModelList.AsEnumerable().Select(x => x.Id).ToList();
                        var lstSysEmpId = dataContext.prl_employee.AsEnumerable().Where(x => lstEmpNo.Contains(x.emp_no)).ToList();
                        var existingUploadedData = dataContext.prl_upload_deduction.Include("prl_deduction_name").AsEnumerable()
                                .Where(x => x.salary_month_year.Value.ToString("yyyy-MM") == dateTime.ToString("yyyy-MM"))
                                .ToList();
                        /////////////////////////////

                        #region Insert To Database

                        int returnSaveChanges = 0;
                        var dnames = dataContext.prl_deduction_name.ToList();
                        foreach (var v in deductionXlsViewModelList)
                        {
                            var i = new prl_upload_deduction();
                            var prlDeductionName = dnames.SingleOrDefault(x => x.deduction_name.ToLower() == v.DeductionName.ToLower());
                            if (prlDeductionName == null)
                            {
                                res.HasPartialError = true;
                                res.Messages.Add(" Could not find deduction name " + v.DeductionName);
                                continue;
                            }
                            i.deduction_name_id = prlDeductionName.id;
                            var singleOrDefault = lstSysEmpId.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.Id.ToLower());
                            if (singleOrDefault == null)
                            {
                                res.HasPartialError = true;
                                res.Messages.Add(" Could not find employee number " + v.Id);
                                continue;
                            }

                            var duplicateData = existingUploadedData.AsEnumerable().FirstOrDefault(x => x.emp_id == singleOrDefault.id && x.deduction_name_id == prlDeductionName.id);
                            if (duplicateData != null)
                            {
                                res.HasPartialError = true;
                                res.Messages.Add(" Employee " + v.Id + " deduction already exist in the system. ");
                                continue;
                            }

                            i.emp_id = singleOrDefault.id;
                            i.salary_month_year = dateTime;
                            i.amount = v.DeductionAmount;
                            i.remarks = v.remarks;
                            i.created_by = User.Identity.Name;
                            i.created_date = DateTime.Now;
                            dataContext.prl_upload_deduction.Add(i);
                        }

                        returnSaveChanges = dataContext.SaveChanges();
                        #endregion

                        if (returnSaveChanges > 0)
                        {
                            res.IsSuccessful = true;
                            res.Messages.Add(DedD.deduction_name + " data uploaded successfully.");
                            TempData.Add("msg", res);
                        }

                        else
                        {
                            res.IsSuccessful = false;
                            res.Messages.Add(DedD.deduction_name + " data do not uploaded successfully.");
                            TempData.Add("msg", res);
                        }

                        return RedirectToAction("UploadDeduction", "Deduction");

                    }
                    else
                    {
                        res.IsSuccessful = false;
                        res.Messages.Add("Upload can not be empty.");
                        TempData.Add("msg", res);

                        return RedirectToAction("UploadDeduction", "Deduction");
                    }
                }
                else
                {
                    //Upload file Null Message
                    res.IsSuccessful = false;
                    res.Messages.Add("Upload can not be empty.");
                    TempData.Add("msg", res);

                    return RedirectToAction("UploadDeduction", "Deduction");
                }

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Messages.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                TempData.Add("msg", res);

                return RedirectToAction("UploadDeduction", "Deduction");
            }
            finally
            {

                if (System.IO.File.Exists(filefullpath))
                {
                    System.IO.File.Delete(filefullpath);
                }
            }

        }

        //[HttpPost]
        //public ActionResult UploadDeduction(DeductionUploadView dcv,HttpPostedFileBase fileupload)
        //{
        //    var lstDat = new List<DeductionUploadData>();
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            fileupload.InputStream.Position = 0;
        //            using (var package = new ExcelPackage(fileupload.InputStream))
        //            {
        //                var ws = package.Workbook.Worksheets.First();
        //                var startRow =  2;

        //                var firstColumPos = ws.Cells.FirstOrDefault(x => x.Value.ToString().Trim() == "ID Number");
        //                startRow = firstColumPos.Start.Row + 1;

        //                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        //                {
        //                    var d = new DeductionUploadData();

        //                    if (ws.Cells[rowNum, 1].Value == null)
        //                    {
        //                        d.ErrorMsg.Add("Row "+rowNum + "does not have an employee ID");
        //                    }
        //                    else
        //                    {
        //                        d.EmployeeID = ws.Cells[rowNum, 1].Value.ToString();
        //                    }

        //                    if (ws.Cells[rowNum, 2].Value == null)
        //                    {
        //                        d.ErrorMsg.Add("Row " + rowNum + "does not have amount");
        //                    }
        //                    else
        //                    {
        //                        decimal val = 0;
        //                        if (decimal.TryParse(ws.Cells[rowNum, 2].Value.ToString(), out val))
        //                        {
        //                            d.amount = val;
        //                        }
        //                        else
        //                        {
        //                            d.ErrorMsg.Add("Row " + rowNum + " amount column should have decimal value");
        //                        }
        //                    }
        //                    if (ws.Cells[rowNum, 3].Value == null)
        //                    {
        //                        d.ErrorMsg.Add("Row " + rowNum + " does not have deduction name");
        //                    }
        //                    else
        //                    {
        //                        d.DeductionNameString = ws.Cells[rowNum, 3].Value.ToString();
        //                    }

        //                    lstDat.Add(d);
        //                }
        //            }
        //            HttpContext.Cache.Insert("currentDeductionUploadInfo", dcv, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
        //            HttpContext.Cache.Insert("currentDeductionUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
        //        }
        //        catch (Exception ex)
        //        {
        //            var d = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        return View(dcv);
        //    }
        //    return Json(new { isUploaded = true, message = "hello" }, "text/html");
        //}

        private string GetRowCellValue(ISheet sheet, int row, string[] properties, string propertyName, string propertyType)
        {
            string cellValue = string.Empty;

            if (sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)) != null)
            {
                CellType cellType = sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).CellType;
                if (propertyType == "NumericCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).NumericCellValue);
                }
                else if (propertyType == "BooleanCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).BooleanCellValue);
                }
                else if (propertyType == "DateCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).DateCellValue);
                }
                else if (propertyType == "StringCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName))).Trim();
                }
                else if (propertyType == "ErrorCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).ErrorCellValue);
                }
            }

            return cellValue;
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    //return i + 1; //excel indexes start from 1
                    return i; //excel indexes start from 0
            return 0;
        }
        public PartialViewResult LoadUploadedData(int? page)
        {
            int pageSize = 30;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<DeductionUploadData> products = null;


            var lst = new List<DeductionUploadData>();
            lst = (List<DeductionUploadData>)HttpContext.Cache["currentDeductionUpload"];

            var pglst = lst.ToPagedList(pageIndex, pageSize);

            return PartialView("_DeductionUploadedData", pglst);
        }

        public ActionResult SaveUploadedData()
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var lst = new List<DeductionUploadData>();
                lst = (List<DeductionUploadData>)HttpContext.Cache["currentDeductionUpload"];
                var dcv = (DeductionUploadView)HttpContext.Cache["currentDeductionUploadInfo"];
                var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month.ToString()), 1);
                var dnames = dataContext.prl_deduction_name.ToList();

                ////////// duplicate check
                var lstEmpNo = lst.AsEnumerable().Select(x => x.EmployeeID).ToList();
                var lstSysEmpId = dataContext.prl_employee.AsEnumerable().Where(x => lstEmpNo.Contains(x.emp_no)).ToList();
                var existingUploadedData = dataContext.prl_upload_deduction.Include("prl_deduction_name").AsEnumerable()
                        .Where(x => x.salary_month_year.Value.ToString("yyyy-MM") == salmon.ToString("yyyy-MM"))
                        .ToList();
                /////////////////////////////

                var notFoundMsg = "";

                foreach (var v in lst)
                {
                    var i = new prl_upload_deduction();
                    var prlDeductionName = dnames.SingleOrDefault(x => x.deduction_name.ToLower() == v.DeductionNameString.ToLower());
                    if (prlDeductionName == null)
                    {
                        operationResult.HasPartialError = true;
                        operationResult.Messages.Add("Could not find deduction name " + v.DeductionNameString);
                        continue;
                    }
                    i.deduction_name_id = prlDeductionName.id;
                    var singleOrDefault = lstSysEmpId.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower());
                    if (singleOrDefault == null)
                    {
                        operationResult.HasPartialError = true;
                        operationResult.Messages.Add("Could not find employee number " + v.EmployeeID);
                        continue;
                    }

                    var duplicateData = existingUploadedData.AsEnumerable().FirstOrDefault(x => x.emp_id == singleOrDefault.id && x.deduction_name_id == prlDeductionName.id);
                    if (duplicateData != null)
                    {
                        operationResult.HasPartialError = true;
                        operationResult.Messages.Add(" Employee " + v.EmployeeID + " deduction already exist in the system. ");
                        continue;
                    }

                    i.emp_id = singleOrDefault.id;
                    i.amount = v.amount;
                    i.salary_month_year = salmon;
                    i.created_by = "russell";
                    i.created_date = DateTime.Now;
                    dataContext.prl_upload_deduction.Add(i);
                }
                dataContext.SaveChanges();
                operationResult.IsSuccessful = true;
                operationResult.Message = "Deduction saved. " + notFoundMsg;
                TempData.Add("msg", operationResult);
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
                TempData.Add("msg", operationResult);
            }
            return RedirectToAction("UploadDeduction");
        }

        public ActionResult EditUploadedDeduction()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult GetDeductionDataSelection()
        {
            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            var deductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            DeductionUploadView up = new DeductionUploadView();
            up.DeductionNames = deductionNames;

            return PartialView("_GetDeductionDataSelection", up);
        }


        //[HttpGet]
        //public PartialViewResult GetIndividualDeductionEntry()
        //{
        //    var prlDeducNames = dataContext.prl_deduction_name.ToList();
        //    var deductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
        //    DeductionUploadView up = new DeductionUploadView();
        //    up.DeductionNames = deductionNames;

        //    return PartialView("_GetIndividualDeductionEntry", up);
        //}


        public ActionResult IndividualDeductionEntry()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            var deductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            DeductionUploadView up = new DeductionUploadView();
            up.DeductionNames = deductionNames;
            return View(up);
        }

        [HttpPost]
        public ActionResult IndividualDeductionEntry(int? empid, DeductionUploadView dcv)
        {
            try
            {
                bool IsSuccess = false;
                var res = new OperationResult();


                if (empid == null || empid == 0)
                {
                    ModelState.AddModelError("", "Please select an employee or employee no.");
                }
                else
                {
                    var empNoIsExist = dataContext.prl_employee.SingleOrDefault(x => x.id == empid);

                    if (empNoIsExist == null)
                    {
                        ModelState.AddModelError("", "Wrong Employee Id which is not existed.");
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            var alreadyUploaded = dataContext.prl_upload_deduction.SingleOrDefault(x => x.deduction_name_id == dcv.DeductionName && x.salary_month_year.Value.Year == dcv.Year && x.salary_month_year.Value.Month == dcv.Month && x.emp_id == empid);

                            if (alreadyUploaded != null)
                            {
                                ModelState.AddModelError("", "Already entried this employee's data.");
                            }
                            else
                            {
                                DateTime salary_month_year = new DateTime(dcv.Year, dcv.Month, 1);

                                prl_upload_deduction uploadDeduction = new prl_upload_deduction
                                {
                                    deduction_name_id = dcv.DeductionName, //id
                                    emp_id = dcv.empid,
                                    salary_month_year = salary_month_year,
                                    amount = dcv.amount,
                                    remarks = dcv.remarks,
                                    created_by = User.Identity.Name,
                                    created_date = DateTime.Now
                                };

                                dataContext.prl_upload_deduction.Add(uploadDeduction);
                            }

                            IsSuccess = dataContext.SaveChanges() > 0;
                        }
                    }
                }

                var prlDeductionName = dataContext.prl_deduction_name.SingleOrDefault(x => x.id == dcv.DeductionName).deduction_name;
                var monthYear = Utility.DateUtility.MonthName(dcv.Month) + "-" + dcv.Year;

                if (IsSuccess == true)
                {
                    var empNo = dataContext.prl_employee.SingleOrDefault(x => x.id == empid).emp_no;

                    res.IsSuccessful = true;
                    res.Message = "The " + prlDeductionName + " of " + empNo + " has been added for " + monthYear;
                    TempData.Add("msg", res);
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message = "Error Found, The " + prlDeductionName + " has not been added.";
                    TempData.Add("msg", res);
                }
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }


            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            var deductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            DeductionUploadView up = new DeductionUploadView();
            up.DeductionNames = deductionNames;
            return View(up);
        }


        [HttpPost]
        public PartialViewResult GgetDeductionDataSelection(DeductionUploadView dcv)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                var dateToSearch = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month), 1);
                ViewBag.did = dcv.DeductionName;
                ViewBag.dt = dateToSearch;
                var lst = dataContext.prl_upload_deduction.Include("prl_deduction_name").Include("prl_employee").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.deduction_name_id == dcv.DeductionName).OrderByDescending(x => x.id);
                var kk = Mapper.Map<List<DeductionUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedDeductions", kk);
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        [HttpPost]
        public JsonResult UpdateRecord(HttpRequestMessage request, string name, string pk, string value)
        {
            try
            {
                int primKey = 0;
                decimal amnt = 0;
                if (Int32.TryParse(pk, out primKey) && decimal.TryParse(value, out amnt))
                {
                    var original = dataContext.prl_upload_deduction.SingleOrDefault(x => x.id == primKey);
                    original.amount = amnt;
                    original.updated_by = User.Identity.Name;
                    original.updated_date = DateTime.Now;
                    dataContext.SaveChanges();
                    request.CreateResponse(HttpStatusCode.OK);
                    return Json(new { status = "success", msg = "Successfully updated" }, "json", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    request.CreateResponse(HttpStatusCode.OK);
                    return Json(new { status = "error", msg = "Amount must be a decimal value" }, "json", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                request.CreateResponse(HttpStatusCode.OK);
                return Json(new { status = "error", msg = "Sorry could not save!" }, "json", JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult EditDataPaging(int did, DateTime dt, int? page)
        {
            int pageSize = 30;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.did = did;
            ViewBag.dt = dt;
            var lst = dataContext.prl_upload_deduction.Include("prl_deduction_name").Include("prl_employee").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.deduction_name_id == did);
            var kk = Mapper.Map<List<DeductionUploadData>>(lst).ToPagedList(pageIndex, pageSize);

            return PartialView("_UploadedDeductions", kk);
        }


        public FileResult GetDeductionUploadSample()
        {
            var fileName = "DeductionUploadFormat.xlsx";
            //string FileURL = "D:/1. Lukman_Official/Sample of Employees Data Import.xlsx";
            //string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/Sample of Salery Review Data Import.xlsx";
            string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/DeductionUploadFormat.xlsx";
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);

            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(FileBytes, "application/xlsx", fileName); GetSaleryReviewUplodSample
        }


    }
}
