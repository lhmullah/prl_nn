using AutoMapper;
using com.linde.DataContext;
using PayrollWeb.CustomSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollWeb.ViewModels;
using com.linde.Model;
using PayrollWeb.Utility;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace PayrollWeb.Controllers
{
    public class GratuityFundController : Controller
    {
        private readonly payroll_systemContext dataContext;
        //
        // GET: /GratuityFund/

        public GratuityFundController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }
        

        public ActionResult GratuityFundMain(string menuName)
        {
            return View();
        }


        [PayrollAuthorize]
        public ActionResult Index()
        {
            var lstGFp = dataContext.prl_gf_setting.ToList();
            return View(Mapper.Map<List<GratuityFundParameter>>(lstGFp));
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            GratuityFundParameter gfP = new GratuityFundParameter();
            return View(gfP);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(GratuityFundParameter gfP)
        {
            var res = new OperationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    var gfPs = new prl_gf_setting();
                    gfPs.service_length_from = gfP.service_length_from;
                    gfPs.service_length_to = gfP.service_length_to;
                    gfPs.number_of_basic = gfP.number_of_basic;
                    gfPs.created_by = User.Identity.Name;
                    gfPs.created_date = DateTime.Now;

                    dataContext.prl_gf_setting.Add(gfPs);
                    dataContext.SaveChanges();

                    res.IsSuccessful = true;
                    res.Message = " New perameter setting successfully done.";
                    TempData.Add("msg", res);

                    return RedirectToAction("Index");
                }
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
            var gfP = dataContext.prl_gf_setting.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<GratuityFundParameter>(gfP));
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(GratuityFundParameter gfP)
        {
            var res = new OperationResult();
            try
            {
                var editGFp = dataContext.prl_gf_setting.SingleOrDefault(x => x.id == gfP.id);
                editGFp.service_length_from = gfP.service_length_from;
                editGFp.service_length_to = gfP.service_length_to;
                editGFp.number_of_basic = gfP.number_of_basic;

                editGFp.updated_by = User.Identity.Name;
                editGFp.updated_date = DateTime.Now;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Information updated successfully.";
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
        public ActionResult Delete(int id)
        {
           var res = new OperationResult();
            try
            {
                var gfPeram = dataContext.prl_gf_setting.SingleOrDefault(x => x.id == id);
                if (gfPeram == null)
                {
                    return HttpNotFound();
                }
               
                dataContext.prl_gf_setting.Remove(gfPeram);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "The Parameter has been deleted.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = " This Parameter could not delete.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult GratuityFundReport()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult GratuityFundReport(int? empid, FormCollection collection, string sButton, ReportGratuityFund rgf)
        {
            bool errorFound = false;
            var res = new OperationResult();
            decimal totalGFAmount = 0;

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

                            var salaryPD = dataContext.prl_salary_process_detail.Where(x => x.emp_id == Emp.id).OrderByDescending(x=> x.salary_month).FirstOrDefault();
                            if (salaryPD == null)
                            {
                                errorFound = true;
                                ModelState.AddModelError("", "No Record Found for the employee.");
                            }

                            if (!errorFound)
                            { 
                                var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                                decimal serviceLengthMonth = Convert.ToDecimal((salaryPD.salary_month.Subtract(Emp.joining_date).Days / (365 / 12)).ToString("0.00"));
                                decimal serviceLengthYear = Math.Round(Convert.ToDecimal(serviceLengthMonth / 12), 0, MidpointRounding.AwayFromZero);
                                decimal ageOfEmployee = 60; //Convert.ToDecimal((salaryPD.salary_month.Subtract(Emp.dob).Days / 365).ToString("0.00"));

                                var lstGFp = dataContext.prl_gf_setting.ToList();

                                if (serviceLengthMonth > 6)
                                {
                                    if (ageOfEmployee > 57)
                                    {
                                        decimal no_OfBasic = 2;
                                        totalGFAmount = Math.Round(Convert.ToDecimal(no_OfBasic * salaryPD.this_month_basic * serviceLengthYear), 0);
                                    }
                                    else
                                    {
                                        foreach (var item in lstGFp)
                                        {
                                            if (serviceLengthMonth > item.service_length_from && serviceLengthMonth < item.service_length_to)
                                            {
                                                totalGFAmount = Math.Round(Convert.ToDecimal(item.number_of_basic * salaryPD.this_month_basic * serviceLengthYear), 0);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    errorFound = true;
                                    ModelState.AddModelError("", "The employee's service period less than 6 month.");
                                }

                                /****************/
                                string reportType = "PDF";

                                LocalReport lr = new LocalReport();
                                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "GratuityFund.rdlc");
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

                                var reportData = new ReportGratuityFund();
                                var empDlist = new List<ReportGratuityFund>();
                                reportData.eId = Emp.id;
                                reportData.empNo = Emp.emp_no;
                                reportData.empName = Emp.name;
                                reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";
                                reportData.department = empD.department_id != 0 ? empD.prl_department.name : "N/A";
                                //reportData.division = empD.division_id != 0 ? empD.prl_division.name : "N/A";
                                reportData.category = empD.employee_category;
                                reportData.joining_date = Emp.joining_date;
                                reportData.serviceLength = Convert.ToInt16(serviceLengthYear);
                                reportData.age = Convert.ToInt16(ageOfEmployee);
                                reportData.basicSalary = Math.Round(Convert.ToDecimal(salaryPD.this_month_basic), 0, MidpointRounding.AwayFromZero);
                                reportData.netPay = totalGFAmount;
                                reportData.MonthName = DateUtility.MonthName(salaryPD.salary_month.Month);
                                reportData.Year = salaryPD.salary_month.Year;

                                if (Emp.bank_id == null)
                                {
                                    reportData.paymentMode = "Cash";
                                    reportData.bank = "";
                                    reportData.accNo = "";
                                    reportData.routing_no = "";
                                }
                                else
                                {
                                    reportData.paymentMode = "Bank Transfer";
                                    reportData.bank = Emp.prl_bank.bank_name;
                                    reportData.accNo = Emp.account_no;
                                    reportData.routing_no = Emp.routing_no;
                                }

                                empDlist.Add(reportData);

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

    }
}
