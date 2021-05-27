using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using com.linde.DataContext;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Service;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;
using Microsoft.Reporting.WebForms;
using System.IO;
using PayrollWeb.Models;
using System.Web;
using System.Globalization;
using NPOI.XSSF.UserModel;
using PayrollWeb.ViewModels.Utility;
using NPOI.SS.UserModel;
using com.linde.Model;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Objects.SqlClient;


namespace PayrollWeb.Controllers
{
    public class SalaryController : Controller
    {
        private IProcessResult result;
        private IProcessResult worker;

        private payroll_systemContext dataContext;

        public SalaryController(payroll_systemContext context)
        {
            this.dataContext = context;
            result = new SalaryProcessResult(ProcessType.SALARY);
            worker = new SalaryProcessResult(ProcessType.SALARY);
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            SalaryProcessModel spm = new SalaryProcessModel();
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            DateTime nowDateTime = DateTime.Now;
            DateTime salaryProcessDate = new DateTime(nowDateTime.Year, nowDateTime.Month, 23);

            int lastMonth = Convert.ToInt16(salaryProcessDate.AddMonths(-1).Month);

            DateTime salaryPaymentDate = new DateTime(nowDateTime.Year, nowDateTime.Month, 25);
            spm.SalaryProcessDate = salaryProcessDate;
            spm.SalaryPaymentDate = salaryPaymentDate;

            return View(spm);
        }


        [HttpPost]
        public ActionResult GetEmployees(FormCollection collection)
        {
            try
            {
                int pageIndex = 0;
                var iDisplayStrart = collection.Get("iDisplayStart");
                if (!string.IsNullOrWhiteSpace(iDisplayStrart))
                {
                    pageIndex = Convert.ToInt32(iDisplayStrart) > 0 ? Convert.ToInt32(iDisplayStrart) - 1 : Convert.ToInt32(iDisplayStrart);
                }
                int pageSize = 30;
                if (!string.IsNullOrWhiteSpace(collection.Get("iDisplayLength")))
                {
                    pageSize = Convert.ToInt32(collection.Get("iDisplayLength"));
                }

                var searchText = collection.Get("sSearch");
                int totalRecords = dataContext.prl_employee.AsEnumerable().Count(x => x.is_active == Convert.ToSByte(true));
                var employees = dataContext.prl_employee.AsEnumerable().Where(x => x.is_active == Convert.ToSByte(true) && x.emp_no.Contains(searchText)).Skip(pageIndex).Take(pageSize);

                //int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
                //var returnEmployees = employees.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                // this is test

                var aaData = employees.Select(x => new string[] { x.emp_no, x.name, x.official_contact_no });

                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);


            }
            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }


        // Not Necessary For the software
        [HttpPost]
        public ActionResult GetEmployeesForMS(FormCollection collection)
        {
            try
            {
                int pageIndex = 0;
                var iDisplayStrart = collection.Get("iDisplayStart");
                if (!string.IsNullOrWhiteSpace(iDisplayStrart))
                {
                    pageIndex = Convert.ToInt32(iDisplayStrart) > 0 ? Convert.ToInt32(iDisplayStrart) - 1 : Convert.ToInt32(iDisplayStrart);
                }
                int pageSize = 30;

                if (!string.IsNullOrWhiteSpace(collection.Get("iDisplayLength")))
                {
                    pageSize = Convert.ToInt32(collection.Get("iDisplayLength"));
                }
                var searchText = collection.Get("sSearch");

                var employees = (from emp in dataContext.prl_employee
                                 join empD in dataContext.prl_employee_details
                                 on emp.id equals empD.emp_id
                                 where emp.is_active == 1 && emp.emp_no.Contains(searchText)
                                 select new { emp.id, emp.emp_no, emp.name, emp.official_contact_no }).ToList();

                int totalRecords = employees.Count();

                var aaData = employees.Select(x => new string[] { x.emp_no, x.name, "department", x.official_contact_no });

                //  
                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);
            }

            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult SProcess(SalaryProcessModel sal)
        {
            bool errorFound = false;

            var salService = new SalaryService(dataContext);
            var processAllEmployee = false;
            var empNumber = new List<string>();
            var empIds = new List<int>();


            DateTime salaryMonth = new DateTime(sal.Year, sal.Month, 1).AddMonths(1).AddDays(-1);

            if (string.IsNullOrWhiteSpace(sal.SelectedEmployeesOnly))
            {
                processAllEmployee = true;
            }
            else
            {
                empNumber = sal.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                                    .Select(x => x.id).ToList();
            }

            var spDetails = (from sp in dataContext.prl_salary_process
                             join spd in dataContext.prl_salary_process_detail on sp.id equals spd.salary_process_id
                             where sp.salary_month.Year == sal.Year && sp.salary_month.Month == sal.Month
                             select new { sp, spd }).ToList();

            var lastSP_info = (from sp in dataContext.prl_salary_process
                               select new { sp.id, sp.batch_no, sp.salary_month, sp.process_date, sp.payment_date }
                                 ).OrderByDescending(x => x.id).FirstOrDefault();

            string toBeProcessMonthYear = "";

            if (lastSP_info != null)
            {
                toBeProcessMonthYear = lastSP_info.salary_month.AddMonths(1).ToString("yyyy-MM");
            }
            string _salaryMonth = salaryMonth.ToString("yyyy-MM");

            if (spDetails.Count > 0)
            {
                var isIdsContained = spDetails.Where(x => empIds.Contains(x.spd.emp_id)).Select(x => x.spd.emp_id);

                if (processAllEmployee == true || lastSP_info.salary_month > salaryMonth || isIdsContained.Count() > 0)
                {
                    string MonthName = DateUtility.MonthName(sal.Month);
                    result.ErrorOccured = true;
                    result.AddToErrorList(MonthName + " Salary Process has already been done.");
                }
                else
                {
                    result = salService.ProcessSalary(processAllEmployee, empIds, sal.Department, salaryMonth, sal.SalaryProcessDate, sal.SalaryPaymentDate);
                }
            }
            else
            {
                if (lastSP_info != null && _salaryMonth != toBeProcessMonthYear)
                {
                    string MissingMonth = DateUtility.MonthName(lastSP_info.salary_month.AddMonths(1).Month);
                    int MissingYear = lastSP_info.salary_month.AddMonths(1).Year;
                    result.ErrorOccured = true;
                    result.AddToErrorList("Wrong Selection! You can't process other month before completing of " + MissingMonth + "-" + MissingYear + " Salary Process.");
                }
                else
                {
                    result = salService.ProcessSalary(processAllEmployee, empIds, sal.Department, salaryMonth, sal.SalaryProcessDate, sal.SalaryPaymentDate);
                }
            }

            return Json(new { success = !result.ErrorOccured, errList = result.GetErrors, msg = "Salary could not be processed for some employees." });
        }


        [HttpPost]
        public ActionResult GetDateByMonthYear(int? year, int? month)
        {
            string nd = DateTime.Now.ToString("MM/dd/yyyy");
            if (year != null && month != null)
                if (year > 0 && month > 0)
                {
                    var dt = new DateTime((int)year, (int)month, 25);
                    nd = dt.ToString("MM/dd/yyyy");
                }

            return Json(new { nd = nd }, JsonRequestBehavior.DenyGet);
        }

        [PayrollAuthorize]
        public ActionResult UndoSalaryProcess()
        {
            SalaryProcessModel spm = new SalaryProcessModel();
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View(spm);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult UndoSalaryProcess(SalaryProcessModel salProcess)
        {
            var res = new OperationResult();
            int _result = 0;

            if (ModelState.IsValid)
            {

                var sp = dataContext.prl_salary_process.SingleOrDefault(x => x.salary_month.Year == salProcess.Year && x.salary_month.Month == salProcess.Month);

                if (sp != null)
                {
                    if (sp.is_disbursed == "N")
                    {
                        try
                        {
                            bool errorFound = false;
                            var processAllEmployee = false;
                            var empNumber = new List<string>();
                            var empIds = new List<int>();


                            DateTime salaryMonth = new DateTime(salProcess.Year, salProcess.Month, 1).AddMonths(1).AddDays(-1);

                            if (string.IsNullOrWhiteSpace(salProcess.SelectedEmployeesOnly))
                            {
                                processAllEmployee = true;
                            }

                            if (!string.IsNullOrWhiteSpace(salProcess.SelectedEmployeesOnly))
                            {
                                empNumber = salProcess.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                                        .Select(x => x.id).ToList();
                            }

                            string userName = User.Identity.Name;
                            SalaryService _service = new SalaryService(dataContext);
                            _result = _service.salaryRollbacked(processAllEmployee, empIds, salProcess, userName, salProcess.Month, salProcess.Year);

                            if (_result > 0)
                            {
                                res.IsSuccessful = true;
                                res.Message = "Salary has been rollbacked.";
                                TempData.Add("msg", res);
                                return RedirectToAction("UndoSalaryProcess");
                            }
                            else if (_result == -909)
                            {
                                res.IsSuccessful = false;
                                res.Message = "Salary already rollbacked.";
                                TempData.Add("msg", res);
                            }
                            else
                            {
                                res.IsSuccessful = false;
                                res.Message = "Salary has not been rollbacked.";
                                TempData.Add("msg", res);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                            res.IsSuccessful = false;
                            res.Message = "Something Error found. Not possible to rollback.";
                            TempData.Add("msg", res);
                        }
                    }
                    else
                    {
                        res.IsSuccessful = false;
                        res.Message = "You can't undo this salary. Because This month's salary distribution has been completed.";
                        TempData.Add("msg", res);
                    }
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message = "No Salary has been found for the selected month.";
                    TempData.Add("msg", res);
                }
            }

            SalaryProcessModel spm = new SalaryProcessModel();
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(spm);
        }

        [PayrollAuthorize]
        public ActionResult DisburseSalary()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View();
        }

        public ActionResult GetUndisbursedBatch(int y, int m)
        {
            if (y == 0 || m == 0)
            {
                return Json(new { isError = true, msg = "You must select a valid year and month." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dt = new DateTime(y, m, 1);
                var procList = dataContext.prl_salary_process.AsEnumerable().
                    Where(x => x.salary_month.ToString("yyyy-MM") == dt.ToString("yyyy-MM") && x.is_disbursed.ToLower() == "n").
                    Select(x =>
                            new
                            {
                                id = x.id,
                                batch_no = x.batch_no,
                                salary_month = x.salary_month,
                                process_date = x.process_date,
                                payment_date = x.payment_date,
                                is_disbursed = x.is_disbursed
                            }).ToList();
                return Json(new { isError = false, msg = "Data found", procList = procList }, JsonRequestBehavior.AllowGet);
            }
        }



        [PayrollAuthorize]
        public ActionResult Disburse(int d)
        {
            var res = new OperationResult();
            try
            {
                var sp = dataContext.prl_salary_process.Where(x => x.id == d).First();

                var updateCmd = @"update prl_salary_process set is_disbursed='Y' where id=" + d + ";";
                int r = dataContext.Database.ExecuteSqlCommand(updateCmd);

                //Permanently Inactive Which Employees Discontinued

                var empIdList_discontinued = dataContext.prl_employee_discontinue.Where(x => x.discontinue_date.Month <= sp.salary_month.Month && x.discontinue_date.Year == sp.salary_month.Year && x.is_active == "Y").Select(x => x.emp_id).ToList();
                var empList_discontinued = dataContext.prl_employee.Where(x => empIdList_discontinued.Contains(x.id)).ToList();

                if (empList_discontinued.Count > 0)
                {
                    foreach (var item in empList_discontinued)
                    {
                        var updateDiscontinuedTbl = @"update prl_employee_discontinue set is_active='N' where emp_id =" + item.id + ";";
                        dataContext.Database.ExecuteSqlCommand(updateDiscontinuedTbl);

                        var _updatingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == item.id);
                        _updatingEmp.is_active = 0;
                        dataContext.SaveChanges();
                    }
                }

                //Sending Email to Employee

                var empIdList_spd = dataContext.prl_salary_process_detail.Where(x => x.salary_process_id == sp.id).Select(x => x.emp_id).ToList();

                var empList = dataContext.prl_employee.Where(x => empIdList_spd.Contains(x.id)).ToList();

                int empTotal = 0;
                string monthYear = DateUtility.MonthName(sp.salary_month.Month) + " - " + sp.salary_month.Year.ToString();


                string senderEmail = "info@recombd.com";
                string senderPassword = "Recom@2021#";
                //string senderEmail_1 = "bprophr1@gmail.com";
                //string senderPassword_1 = "Bprop@HR12"; 

                if (empList.Count > 0)
                {
                    foreach (var emp in empList)
                    {
                        if (emp.email != "N/A" && !String.IsNullOrEmpty(emp.email.Trim()) && emp.email != "" && emp.is_active == 1)
                        {
                            if (Utility.CommonFunctions.IsValidEmail(emp.email.Trim()) == true)
                            {
                                SendPayslipNotification(emp.name, emp.email.Trim(), senderEmail, senderPassword, "Payslip", monthYear);
                                empTotal++;
                            }

                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No active employee found with corporate email.");
                }

                res.IsSuccessful = true;
                res.Message = "Successfully disburse salary and send email to " + empTotal + " employees about payslip.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                res.IsSuccessful = false;
                res.Message = "Could not disburse salary";
            }

            TempData.Add("msg", res);
            return RedirectToAction("DisburseSalary");
        }

        [NonAction]
        public void SendPayslipNotification(string employeeName, string emailID, string senderEmail, string senderPassword, string emailFor, string monthYear)
        {
            try
            {
                //var verifyUrl = "/User/" + emailFor + "/" + activationCode;
                //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var link = "www.recombd.com/self-services";
                string linkName = "Click Here to Login";
                var fromEmail = new MailAddress(senderEmail, "Recom Consulting Limited");
                var fromEmailPassword = senderPassword; // Replace with actual password

                var toEmail = new MailAddress(emailID);

                string subject = "";
                string body = "";
                if (emailFor == "Payslip")
                {
                    // For Testing Purpose

                    //subject = "Test Email " + monthYear;
                    //body = "<br/><br/><b>Dear " + employeeName + ",</b>" +
                    //    "<br/> Sorry have disturbed you. This is a Test mail :) :)" +


                    //    "<br/><br/>Best Wishes with Regards" +
                    //    "<br/><b>Novo Nordisk Pharma (Pvt.) Ltd</b>" +
                    //    "<br/>Dhaka, Bangladesh" +

                    //    "<br/><br/><b>Note: This is a system generated email, do not reply directly to this email id.</b>";

                    subject = "Payslip for " + monthYear;

                    body = "<br/><br/><b>Dear " + employeeName + ",</b>" +
                        "<br/>This is to inform you that your Payslip for <b>" + monthYear + "</b> has been successfully published. " +
                        "<br/>You are requested to login your payroll account to check Payslip." +
                        " <br/><br/><a href='http://" + link + "'>" + linkName + "</a> " +

                        "<br/><br/>Best wishes with regards" +
                        "<br/><b>Recom Consulting Ltd.</b>" +
                        "<br/>House 18 (Flat B2), Road 1/A, Block J" +
                        "<br/>Baridhara, Dhaka 1212" +

                        "<br/><br/><b>Note: This is a system generated email, do not reply directly to this email id.</b>";
                }

                //else if (emailFor == "ResetPassword")
                //{
                //    subject = "Reset Password";
                //    //body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                //    //    "<br/><br/><a href=" + link + ">Reset Password link</a>";
                //    body = "Hi,<br/><br/>We got a request to reset your HR payroll account password." +
                //    " Your New Password is: " + newPassword + "";
                //}


                var smtp = new SmtpClient
                {
                    Host = "mail.recombd.com",
                    //Port = 465,
                    Port = 25,
                    //EnableSsl = true,

                    //Host = "smtp.gmail.com",
                    //Port = 587,
                    //EnableSsl = true,

                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    //BodyEncoding = UTF8Encoding.UTF8,
                    //DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                })

                    smtp.Send(message);

            }
            catch (Exception ex)
            {
                var error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }


        [PayrollAuthorize]
        [HttpGet]
        public ActionResult PaySlip()
        {

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Allowance = new List<AllowanceDeduction>();
            ViewBag.Deduction = new List<AllowanceDeduction>();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult PaySlip(int? empid, string department, FormCollection collection, string sButton, ReportPayslip rp)
        {
            bool errorFound = false;
            var res = new OperationResult();
            var payslipInfo = new ReportPayslip();
            var EmpList = new List<ReportPayslip>();

            try
            {
                if (sButton != null)
                {
                    if (sButton == "Download")
                    {
                        if (empid == null)
                        {
                            errorFound = true;
                            ViewBag.Allowance = new List<AllowanceDeduction>();
                            ViewBag.Deduction = new List<AllowanceDeduction>();
                            ModelState.AddModelError("", "Please select an employee ID");
                        }
                        else
                        {
                            var Emp = new Employee();
                            if (empid != null)
                            {
                                var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                                Emp = Mapper.Map<Employee>(_empD);
                            }

                            var salaryPD = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month);
                            if (salaryPD == null)
                            {
                                errorFound = true;
                                ModelState.AddModelError("", "No Record Found for the employee for the selected Month.");
                            }

                            if (!errorFound)
                            {
                                var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                                var salaryProcess = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month);

                                var allowances = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == salaryProcess.salary_process_id
                                    && x.emp_id == salaryProcess.emp_id && x.salary_month.Month == salaryProcess.salary_month.Month).Select(p => new AllowanceDeduction
                                    {
                                        head = p.prl_allowance_name.allowance_name,
                                        value = p.amount
                                    }).ToList();

                                //Allowance = allowances;
                                var deductions = dataContext.prl_salary_deductions.Where(x => x.salary_process_id == salaryProcess.salary_process_id
                                    && x.emp_id == salaryProcess.emp_id && x.salary_month.Month == salaryProcess.salary_month.Month).Select(p => new AllowanceDeduction
                                    {
                                        head = p.prl_deduction_name.deduction_name,
                                        value = p.amount
                                    }).ToList();

                                //deduction = deductions;

                                /****************/
                                string reportType = "PDF";

                                LocalReport lr = new LocalReport();
                                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "PaySlip.rdlc");
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

                                var sp = dataContext.prl_salary_process.SingleOrDefault(x => x.id == salaryProcess.salary_process_id);

                                var reportData = new ReportPayslip();
                                var empDlist = new List<ReportPayslip>();
                                reportData.eId = Emp.id;
                                reportData.empNo = Emp.emp_no;
                                reportData.empName = Emp.name;

                                reportData.processId = salaryPD.salary_process_id;
                                reportData.cost_centre = empD.cost_centre_id != 0 ? empD.prl_cost_centre.cost_centre_name : " ";
                                reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";

                                reportData.joining_date = Emp.joining_date;
                                reportData.no_of_days_in_month = DateTime.DaysInMonth(rp.Year, rp.Month);
                                reportData.no_of_working_days = salaryPD.calculation_for_days;
                                reportData.MonthName = DateUtility.MonthName(rp.Month);
                                reportData.Year = rp.Year;

                                reportData.salary_date = sp.payment_date;
                               
                                reportData.this_month_basic = Math.Round(Convert.ToDecimal(salaryPD.this_month_basic), 0, MidpointRounding.AwayFromZero);

                                reportData.basicSalary = Math.Round(Convert.ToDecimal(salaryPD.current_basic), 0, MidpointRounding.AwayFromZero);

                                reportData.totalEarnings = Math.Round(salaryPD.total_allowance + (reportData.this_month_basic ?? 0) + (salaryPD.totla_arrear_allowance ?? 0), 0, MidpointRounding.AwayFromZero);

                                reportData.totalDeduction = Math.Round(Convert.ToDecimal(salaryPD.total_deduction), 2, MidpointRounding.AwayFromZero);
                                reportData.netPay = Math.Round(Convert.ToDecimal(salaryPD.net_pay), 2, MidpointRounding.AwayFromZero);

                                reportData.tax = Math.Round(Convert.ToDecimal(salaryPD.total_monthly_tax), 0, MidpointRounding.AwayFromZero);

                                if (reportData.tax > 0)
                                {
                                    var isTaxUploaded = deductions.Where(x => x.head == "Income Tax").FirstOrDefault();
                                    if (isTaxUploaded != null)
                                    {
                                        reportData.totalDeduction = reportData.totalDeduction - isTaxUploaded.value;
                                    }

                                    reportData.totalDeduction = Math.Round(Convert.ToDecimal(reportData.totalDeduction + reportData.tax), 0, MidpointRounding.AwayFromZero);
                                }

                                if (salaryPD.pf_arrear > 0)
                                {
                                    reportData.pf = Math.Round(Convert.ToDecimal(salaryPD.pf_amount + salaryPD.pf_arrear), 0, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    reportData.pf = Math.Round(salaryPD.pf_amount, 0, MidpointRounding.AwayFromZero);
                                }

                                if (reportData.pf > 0)
                                {
                                    reportData.totalEarnings = Math.Round(Convert.ToDecimal(reportData.totalEarnings + reportData.pf), 0, MidpointRounding.AwayFromZero);
                                    reportData.totalDeduction = Math.Round(Convert.ToDecimal(reportData.totalDeduction + reportData.pf * 2), 0, MidpointRounding.AwayFromZero);
                                }

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

                                //Bonus
                                string mnth = Convert.ToString(reportData.salary_date.Month);
                                string yr = Convert.ToString(reportData.salary_date.Year);
                                var bonus = (from bp in dataContext.prl_bonus_process
                                             join bn in dataContext.prl_bonus_name on bp.bonus_name_id equals bn.id
                                             join bpd in dataContext.prl_bonus_process_detail on bp.id equals bpd.bonus_process_id
                                             where bpd.emp_id == reportData.eId && bp.month == mnth && bp.year == yr && bp.is_pay_with_salary == "YES"
                                             select new AllowanceDeduction
                                             {
                                                 value = bpd.amount
                                             }).ToList();

                                if (bonus.Count > 0)
                                {
                                    decimal totlbonus = 0;
                                    foreach (var item in bonus)
                                    {
                                        totlbonus += Math.Round(Convert.ToDecimal(item.value), 2, MidpointRounding.AwayFromZero);
                                    }
                                    reportData.totalEarnings = Math.Round(Convert.ToDecimal(reportData.totalEarnings + totlbonus), 2, MidpointRounding.AwayFromZero);
                                }

                                empDlist.Add(reportData);

                                ReportDataSource rd = new ReportDataSource("DataSet1", empDlist);

                                lr.DataSources.Clear();

                                lr.DataSources.Add(rd);

                                lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);

                                lr.Refresh();

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
                ViewBag.Allowance = new List<AllowanceDeduction>();
                ViewBag.Deduction = new List<AllowanceDeduction>();
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            payslipInfo.Month = rp.Month;
            payslipInfo.Year = rp.Year;
            payslipInfo.MonthName = DateUtility.MonthName(rp.Month);

            return View(payslipInfo);
        }

        void lr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int eId = Convert.ToInt32(e.Parameters["eId"].Values[0]);
            int pId = Convert.ToInt32(e.Parameters["procesId"].Values[0]);

            string pth = e.ReportPath;

            if (pth == "PaySlipChildAllowance")
            {
                int i = 0;

                var spDetails = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == eId && x.salary_process_id == pId);

                var allowances = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == pId
                                && x.emp_id == eId).Select(p => new AllowanceDeduction
                                {
                                    head = p.remarks != null && p.remarks != "" ? p.prl_allowance_name.allowance_name
                                                + " (" + p.remarks + ")" : p.prl_allowance_name.allowance_name,
                                    value = p.amount
                                }).ToList();


                decimal basicAmount = Convert.ToDecimal(e.Parameters["basicSlr"].Values[0]);
                decimal arrearBasic = 0;
                AllowanceDeduction AD = new AllowanceDeduction();

                if (spDetails.this_month_basic > spDetails.current_basic)
                {
                    basicAmount = spDetails.current_basic;
                    arrearBasic = (spDetails.this_month_basic ?? 0) - spDetails.current_basic;

                    AD.head = "Basic Salary";
                    AD.value = basicAmount;
                    allowances.Insert(i, AD);
                    i++;

                    AllowanceDeduction AD1 = new AllowanceDeduction();
                    AD1.head = "Arrear Basic";
                    AD1.value = arrearBasic;
                    allowances.Insert(i, AD1);
                    i++;
                }
                else
                {
                    AD.head = "Basic Salary";
                    AD.value = basicAmount;
                    allowances.Insert(i, AD);
                    i++;
                }

                AllowanceDeduction AD2 = new AllowanceDeduction();

                var pf = dataContext.prl_salary_process_detail.Where(x => x.salary_process_id == pId
                                 && x.emp_id == eId).SingleOrDefault();

                if (pf != null)
                {
                    if (pf.pf_amount > 0)
                    {
                        AD2.head = "Provident Fund Company Contribution";
                        AD2.value = pf.pf_amount + (pf.pf_arrear != null ? pf.pf_arrear.Value : 0);
                        allowances.Insert(i, AD2);
                        i++;
                    }
                }


                //AllowanceDeduction AD = new AllowanceDeduction();
                //AD.head = "Basic Salary";
                //AD.value = Convert.ToDecimal(e.Parameters["basicSlr"].Values[0]);
                //allowances.Insert(0, AD);

                //AllowanceDeduction AD2 = new AllowanceDeduction();
                //decimal cf = Convert.ToDecimal(e.Parameters["cf"].Values[0]);
                //if (cf > 0)
                //{
                //    AD2.head = "Provident Fund Company Contribution";
                //    AD2.value = cf;
                //    allowances.Insert(1, AD2);
                //}

                DateTime dt = Convert.ToDateTime(e.Parameters["pDate"].Values[0]);
                string mnth = Convert.ToString(dt.Month);
                string yr = Convert.ToString(dt.Year);
                var bonus = (from bp in dataContext.prl_bonus_process
                             join bn in dataContext.prl_bonus_name on bp.bonus_name_id equals bn.id
                             join bpd in dataContext.prl_bonus_process_detail on bp.id equals bpd.bonus_process_id
                             where bpd.emp_id == eId && bp.month == mnth && bp.year == yr && bp.is_pay_with_salary == "YES"
                             select new AllowanceDeduction
                             {
                                 head = bn.name,
                                 value = bpd.amount
                             }).ToList();


                if (bonus.Count > 0)
                {

                    foreach (var item in bonus)
                    {
                        AllowanceDeduction AD3 = new AllowanceDeduction();

                        AD3.head = "Festival Bonus (" + item.head + ")";
                        AD3.value = item.value;

                        allowances.Insert(i, AD3);
                        i++;
                    }
                }

                var arrearLists = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == pId
                                && x.emp_id == eId && (x.arrear_amount ?? 0) != 0)
                                .Select(p => new AllowanceDeduction
                                {
                                    head = "Arrear " + p.prl_allowance_name.allowance_name,
                                    value = p.arrear_amount ?? 0
                                })
                                .ToList();

                if (arrearLists.Count > 0)
                {
                    foreach (var item in arrearLists)
                    {
                        AllowanceDeduction AD4 = new AllowanceDeduction();

                        AD4.head = item.head;
                        AD4.value = item.value;
                        allowances.Insert(i, AD4);
                        i++;
                    }

                }

                e.DataSources.Add(new ReportDataSource("DataSet1", allowances));
            }
            else
            {
                var deductions = dataContext.prl_salary_deductions.Where(x => x.id == 0).Select(p => new AllowanceDeduction
                {
                    head = p.remarks != null && p.remarks != "" ? p.prl_deduction_name.deduction_name
                                    + " (" + p.remarks + ")" : p.prl_deduction_name.deduction_name,
                    value = p.amount + ((decimal?)p.arrear_amount ?? 0)
                }).ToList();

                decimal tax = Convert.ToDecimal(e.Parameters["tax"].Values[0]);

                if (tax > 0)
                {
                    AllowanceDeduction AD = new AllowanceDeduction();
                    AD.head = "Income Tax";
                    AD.value = tax;

                    deductions = dataContext.prl_salary_deductions.Where(x => x.salary_process_id == pId
                           && x.emp_id == eId && x.prl_deduction_name.deduction_name != "Income Tax").Select(p => new AllowanceDeduction
                           {
                               head = p.remarks != null && p.remarks != "" ? p.prl_deduction_name.deduction_name
                                               + " (" + p.remarks + ")" : p.prl_deduction_name.deduction_name,
                               value = p.amount + ((decimal?)p.arrear_amount ?? 0)
                           }).ToList();

                    deductions.Insert(0, AD);
                }
                else
                {
                    deductions = dataContext.prl_salary_deductions.Where(x => x.salary_process_id == pId
                           && x.emp_id == eId).Select(p => new AllowanceDeduction
                           {
                               head = p.remarks != null && p.remarks != "" ? p.prl_deduction_name.deduction_name
                                               + " (" + p.remarks + ")" : p.prl_deduction_name.deduction_name,
                               value = p.amount + ((decimal?)p.arrear_amount ?? 0)
                           }).ToList();
                }

               

                var pf = dataContext.prl_salary_process_detail.Where(x => x.salary_process_id == pId
                                && x.emp_id == eId).SingleOrDefault();

                if (pf != null)
                {
                    if (pf.pf_amount > 0)
                    {
                        AllowanceDeduction AD = new AllowanceDeduction();
                        AD.head = "Provident Fund (Company+ Own)";
                        AD.value = (pf.pf_amount + ((decimal?)pf.pf_arrear ?? 0)) * 2;
                        deductions.Insert(0, AD);
                    }
                }

                e.DataSources.Add(new ReportDataSource("DataSet1", deductions));
            }
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult SalarySheet()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View(new ReportSalarySheet
            {
                //RType. = true
            });
        }


        [PayrollAuthorize]
        [HttpPost]
        public ActionResult SalarySheet(ReportSalarySheet SS, FormCollection collection, string sButton)
        {
            var empNumber = new List<string>();

            var EmpList = new List<ReportSalarySheet>();
            string year = SS.Year.ToString();

            EmpList = (from esd in dataContext.vw_empsalaryprocessdetails
                       where esd.salary_month.Year == SS.Year && esd.salary_month.Month == SS.month_no
                       select new ReportSalarySheet
                       {
                           empId = esd.id,
                           empNo = esd.emp_no,
                           empName = esd.empName,
                           designation = esd.designation,

                           joining_date = esd.joining_date,
                           accNo = esd.account_no,
                           routing_no = esd.routing_no,
                           month_name = esd.month_name,
                           month_no = esd.month_no,
                           Year = esd.salary_month.Year,
                           calendar_days = esd.calendar_days,
                           payment_date = esd.payment_date,
                           is_new_employee = (esd.joining_date.Month == esd.salary_month.Month && esd.joining_date.Year == esd.salary_month.Year) ? "Yes" : "No",

                           working_days = esd.working_days,

                           remarks = esd.remarks_Sa + " " + esd.remarks_Sd,
                           //remarks = esd.festival_bonus != 0 ? "MONTHLY STAFF SALARY & FESTIVAL BONUS PAYMENT" + " - " + esd.month_name + " " + year + "" : "MONTHLY STAFF SALARY" + " - " + esd.month_name + " " + year + "",

                           last_working_date = esd.last_working_date,
                           discontinued_reason = esd.discontinued_reason != null ? esd.discontinued_reason : " ",
                           bank_name = esd.bank_name != null ? esd.bank_name : " ",
                           
                           loan_start_date = esd.loan_start_date,
                           loan_end_date = esd.loan_end_date,
                           principal_amount = esd.principal_amount != 0 ? esd.principal_amount : 0,
                           no_of_installment = esd.no_of_installment != 0 ? esd.no_of_installment : 0,
                           cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                           basic_salary = esd.basic_salary,
                           this_month_basic = esd.this_month_basic,
                           bta = esd.bta == null ? 0 : esd.bta,
                           car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                           pf_cc_amount = esd.pf_cc_amount,
                           conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                           houseR = esd.house == null ? 0 : esd.house,
                           special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

                           arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                           arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                           arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                           arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                           arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                           incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                           incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                           incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                           incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                           sti = esd.sti != null ? esd.sti : 0,
                           one_time = esd.one_time != null ? esd.one_time : 0,
                           training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                           gift = esd.gift != null ? esd.gift : 0,
                           pf_refund = esd.pf_refund != null ? esd.pf_refund : 0,
                           basic_arrear = esd.basic_arrear != null ? esd.basic_arrear : 0,

                           tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                           festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                           bonus = esd.bonus == null ? 0 : esd.bonus,
                           total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                           leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                           long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                           is_discontinued = esd.is_discontinued,
                           totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                           ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                           modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),
                           ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                           mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                           lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                           others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                           tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                           pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                           monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                           income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                           totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),
                           netPay = esd.net_pay == null ? 0 : esd.net_pay

                       }).ToList();

            if (EmpList.Count > 0)
            {

                LocalReport lr = new LocalReport();
                string path = "";

                path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "SalarySheetReport.rdlc");


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
                DateTime dt = new DateTime(SS.Year, SS.month_no, 1);

                ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                lr.DataSources.Add(rd);
                lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));
                string reportType = "EXCELOPENXML";
                string mimeType;
                string encoding;
                string fileNameExtension = "xlsx";

                //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>xlsx</OutputFormat>" +
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

                //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                return File(renderedBytes, mimeType);
            }
            else
            {
                ModelState.AddModelError("", "No information found");
            }

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View();
        }


        [PayrollAuthorize]
        [HttpGet]
        public ActionResult AggregateSalarySheet()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View(new ReportSalarySheet
            {
                //RType. = true
            });
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult AggregateSalarySheet(ReportSalarySheet SS, FormCollection collection, string sButton)
        {
            var empNumber = new List<string>();

            var EmpList = new List<ReportSalarySheet>();
            string year = SS.Year.ToString();

            EmpList = (from esd in dataContext.vw_empsalaryprocessdetails
                       where esd.salary_month.Year == SS.Year && esd.salary_month.Month == SS.month_no
                       select new ReportSalarySheet
                       {
                           empId = esd.id,
                           empNo = esd.emp_no,
                           empName = esd.empName,
                           designation = esd.designation,

                           joining_date = esd.joining_date,
                           accNo = esd.account_no,
                           routing_no = esd.routing_no,
                           month_name = esd.month_name,
                           month_no = esd.month_no,
                           Year = esd.salary_month.Year,
                           calendar_days = esd.calendar_days,
                           payment_date = esd.payment_date,

                           last_working_date = esd.last_working_date,
                           discontinued_reason = esd.discontinued_reason != null ? esd.discontinued_reason : " ",
                           bank_name = esd.bank_name != null ? esd.bank_name : " ",
                           //no_of_days_lwp = esd.no_of_days_lwp != 0 ? esd.no_of_days_lwp : 0,
                           loan_start_date = esd.loan_start_date,
                           loan_end_date = esd.loan_end_date,
                           principal_amount = esd.principal_amount != 0 ? esd.principal_amount : 0,
                           no_of_installment = esd.no_of_installment != 0 ? esd.no_of_installment : 0,
                           cost_centre_id = esd.cost_centre_id,
                           cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                           basic_salary = esd.basic_salary,
                           this_month_basic = esd.this_month_basic,
                           bta = esd.bta == null ? 0 : esd.bta,
                           car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                           pf_cc_amount = esd.pf_cc_amount,
                           conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                           houseR = esd.house == null ? 0 : esd.house,
                           special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

                           arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                           arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                           arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                           arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                           arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                           incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                           incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                           incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                           incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                           sti = esd.sti != null ? esd.sti : 0,
                           one_time = esd.one_time != null ? esd.one_time : 0,
                           training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                           gift = esd.gift != null ? esd.gift : 0,
                           pf_refund = esd.pf_refund != null ? esd.pf_refund : 0,
                           basic_arrear = esd.basic_arrear != null ? esd.basic_arrear : 0,

                           tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                           festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                           bonus = esd.bonus == null ? 0 : esd.bonus,
                           total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                           leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                           long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                           is_discontinued = esd.is_discontinued,
                           totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                           ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                           modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),
                           ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                           mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                           lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                           others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                           tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                           pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                           monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                           income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                           totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),

                           netPay = esd.net_pay == null ? 0 : esd.net_pay

                       }).ToList();



            if (EmpList.Count > 0)
            {

                LocalReport lr = new LocalReport();
                string path = "";

                path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "AggregateSalarySheetReport.rdlc");


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
                DateTime dt = new DateTime(SS.Year, SS.month_no, 1);

                ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                lr.DataSources.Add(rd);
                lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));
                string reportType = "EXCELOPENXML";
                string mimeType;
                string encoding;
                string fileNameExtension = "xlsx";

                //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>xlsx</OutputFormat>" +
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

                //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                return File(renderedBytes, mimeType);
            }
            else
            {
                ModelState.AddModelError("", "No information found");
            }

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View();
        }

        //[PayrollAuthorize]
        //[HttpGet]
        //public ActionResult AggregateSalarySheet()
        //{
        //    ViewBag.Years = DateUtility.GetYears();
        //    ViewBag.Months = DateUtility.GetMonths();
        //    ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

        //    return View(new ReportSalarySheet
        //    {
        //        //RType. = true
        //    });
        //}

        //[PayrollAuthorize]
        //[HttpPost]
        //public ActionResult AggregateSalarySheet(ReportSalarySheet SS, FormCollection collection, string sButton)
        //{
        //    var empNumber = new List<string>();

        //    var DataList = new List<AggregateReportSalarySheet>();
        //    string year = SS.Year.ToString();

        //    DataList = (from esd in dataContext.vw_aggregatesalarydetails
        //               where esd.salary_month.Year == SS.Year && esd.salary_month.Month == SS.month_no
        //               select new AggregateReportSalarySheet
        //               {
        //                   month_name = esd.month_name,
        //                   month_no = esd.month_no,
        //                   Year = esd.salary_month.Year,
        //                   CostcentreId = esd.CostcentreId,
        //                   cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

        //                   this_month_basic = esd.this_month_basic,
        //                   bta = esd.bta == null ? 0 : esd.bta,
        //                   car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
        //                   pf_cc_amount = esd.pf_cc_amount,
        //                   conveyance = esd.conveyance == null ? 0 : esd.conveyance,
        //                   houseR = esd.house == null ? 0 : esd.house,
        //                   special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

        //                   incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
        //                   incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
        //                   incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
        //                   incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
        //                   sti = esd.sti != null ? esd.sti : 0,
        //                   one_time = esd.one_time != null ? esd.one_time : 0,
        //                   training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
        //                   gift = esd.gift != null ? esd.gift : 0,
        //                   tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
        //                   festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
        //                   total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
        //                   leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
        //                   totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

        //                   ipad_or_mobile_bill = esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill,
        //                   modem_bill = esd.modem_bill == null ? 0 : esd.modem_bill,

        //                   lunch_support = esd.lunch_support == null ? 0 : esd.lunch_support,
        //                   tax_return_non_submission = esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission,
        //                   pf_co_amount = esd.pf_co_amount == null ? 0 : esd.pf_co_amount,
        //                   monthly_tax = esd.monthly_tax == null ? 0 : esd.monthly_tax,
        //                   income_tax = esd.income_tax == null ? 0 : esd.income_tax, // if upload tax
        //                   totalD = esd.total_deduction == null ? 0 : esd.total_deduction,
        //                   netPay = esd.net_pay == null ? 0 : esd.net_pay

        //               }).ToList();



        //    if (DataList.Count > 0)
        //    {

        //        LocalReport lr = new LocalReport();
        //        string path = "";

        //        path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "AggregateSalarySheetReport.rdlc");


        //        if (System.IO.File.Exists(path))
        //        {
        //            lr.ReportPath = path;
        //        }
        //        else
        //        {
        //            ViewBag.Years = DateUtility.GetYears();
        //            ViewBag.Months = DateUtility.GetMonths();
        //            return View("Index");
        //        }
        //        DateTime dt = new DateTime(SS.Year, SS.month_no, 1);

        //        ReportDataSource rd = new ReportDataSource("DataSet1", DataList);
        //        lr.DataSources.Add(rd);
        //        lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));
        //        string reportType = "EXCELOPENXML";
        //        string mimeType;
        //        string encoding;
        //        string fileNameExtension = "xlsx";

        //        //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

        //        string deviceInfo =
        //        "<DeviceInfo>" +
        //        "<OutputFormat>xlsx</OutputFormat>" +
        //        "</DeviceInfo>";

        //        Warning[] warnings;
        //        string[] streams;
        //        byte[] renderedBytes;

        //        renderedBytes = lr.Render(
        //            reportType,
        //            deviceInfo,
        //            out mimeType,
        //            out encoding,
        //            out fileNameExtension,
        //            out streams,
        //            out warnings);

        //        //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

        //        return File(renderedBytes, mimeType);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "No information found");
        //    }

        //    ViewBag.Years = DateUtility.GetYears();
        //    ViewBag.Months = DateUtility.GetMonths();
        //    ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

        //    return View();
        //}

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult EmployeeWiseSalarySheet()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(new ReportSalarySheet
            {
                //RType. = true
            });
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EmployeeWiseSalarySheet(ReportSalarySheet SS, string sButton)
        {
            var empNumber = new List<string>();

            var processAllEmployee = false;
            var empIds = new List<int>();
            var EmpList = new List<ReportSalarySheet>();
            DateTime fromDate = new DateTime(SS.Year, SS.frm_month_no, 1);
            DateTime toDate = new DateTime(SS.Year, SS.to_month_no, 1).AddMonths(1).AddDays(-1);

            string year = SS.Year.ToString();


            if (!string.IsNullOrWhiteSpace(SS.SelectedEmployeesOnly))
            {
                empNumber = SS.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                            .Select(x => x.id).ToList();
            }

            if (SS.to_month_no >= SS.frm_month_no)
            {
                if (empNumber.Count > 0)
                {
                    EmpList = (from esd in dataContext.vw_empsalaryprocessdetails
                               where empNumber.Contains(esd.emp_no)
                               && esd.salary_month >= fromDate && esd.salary_month <= toDate
                               select new ReportSalarySheet
                               {
                                   empId = esd.id,
                                   empNo = esd.emp_no,
                                   empName = esd.empName,
                                   designation = esd.designation,
                                   joining_date = esd.joining_date,
                                   accNo = esd.account_no,
                                   routing_no = esd.routing_no,
                                   month_name = esd.month_name,
                                   month_no = esd.month_no,
                                   Year = esd.salary_month.Year,
                                   calendar_days = esd.calendar_days,
                                   working_days = esd.working_days,
                                   basic_salary = esd.basic_salary,
                                   remarks = esd.remarks_Sa + " " + esd.remarks_Sd,
                                   //remarks = esd.festival_bonus != 0 ? "MONTHLY STAFF SALARY & FESTIVAL BONUS PAYMENT" + " - " + esd.month_name + " " + year + "" : "MONTHLY STAFF SALARY" + " - " + esd.month_name + " " + year + "",
                                   basic = esd.basic_salary == null ? 0 : esd.basic_salary,
                                   houseR = esd.house == null ? 0 : esd.house,
                                   conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                   car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                   payment_date = esd.payment_date,
                                   cost_centre_id = esd.cost_centre_id,


                                   //sales_commission = esd.sales_commission == null ? 0 : esd.sales_commission,
                                   //performance_incentive = esd.performance_incentive == null ? 0 : esd.performance_incentive,
                                   //others_allowance = esd.others_allowance == null ? 0 : esd.others_allowance,
                                   festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                   bonus = esd.bonus == null ? 0 : esd.bonus,
                                   total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,

                                   totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                   //unpaid_leave = esd.unpaid_leave == null ? 0 : esd.unpaid_leave,
                                   //late_attendance_deductions = esd.late_attendance_deductions == null ? 0 : esd.late_attendance_deductions,
                                   //advance_salary = esd.advance_salary == null ? 0 : esd.advance_salary,
                                   //exceed_mobile_bill = esd.exceed_mobile_bill == null ? 0 : esd.exceed_mobile_bill,
                                   //others_deduction = esd.others_deduction == null ? 0 : esd.others_deduction,
                                   //mobile_dl_deduction = esd.mobile_dl_deduction == null ? 0 : esd.mobile_dl_deduction,
                                   //laptop_dl_deduction = esd.laptop_dl_deduction == null ? 0 : esd.laptop_dl_deduction,

                                   monthly_tax = esd.monthly_tax == null ? 0 : esd.monthly_tax,

                                   totalD = esd.total_deduction == null ? 0 : esd.total_deduction,
                                   netPay = esd.net_pay == null ? 0 : esd.net_pay

                               }).ToList();
                }
                else
                {
                    EmpList = (from esd in dataContext.vw_empsalaryprocessdetails
                               where esd.salary_month >= fromDate && esd.salary_month <= toDate
                               select new ReportSalarySheet
                               {
                                   empId = esd.id,
                                   empNo = esd.emp_no,
                                   empName = esd.empName,
                                   designation = esd.designation,

                                   joining_date = esd.joining_date,
                                   accNo = esd.account_no,
                                   routing_no = esd.routing_no,
                                   month_name = esd.month_name,
                                   month_no = esd.month_no,
                                   Year = esd.salary_month.Year,
                                   calendar_days = esd.calendar_days,
                                   working_days = esd.working_days,
                                   basic_salary = esd.basic_salary,
                                   remarks = esd.festival_bonus != 0 ? "MONTHLY STAFF SALARY & FESTIVAL BONUS PAYMENT" + " - " + esd.month_name + " " + year + "" : "MONTHLY STAFF SALARY" + " - " + esd.month_name + " " + year + "",
                                   basic = esd.basic_salary == null ? 0 : esd.basic_salary,
                                   houseR = esd.house == null ? 0 : esd.house,
                                   conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                   car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                   payment_date = esd.payment_date,


                                   //sales_commission = esd.sales_commission == null ? 0 : esd.sales_commission,
                                   //performance_incentive = esd.performance_incentive == null ? 0 : esd.performance_incentive,
                                   //others_allowance = esd.others_allowance == null ? 0 : esd.others_allowance,
                                   festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                   bonus = esd.bonus == null ? 0 : esd.bonus,
                                   total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,

                                   totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                   //unpaid_leave = esd.unpaid_leave == null ? 0 : esd.unpaid_leave,
                                   //late_attendance_deductions = esd.late_attendance_deductions == null ? 0 : esd.late_attendance_deductions,
                                   //advance_salary = esd.advance_salary == null ? 0 : esd.advance_salary,
                                   //exceed_mobile_bill = esd.exceed_mobile_bill == null ? 0 : esd.exceed_mobile_bill,
                                   //others_deduction = esd.others_deduction == null ? 0 : esd.others_deduction,
                                   //mobile_dl_deduction = esd.mobile_dl_deduction == null ? 0 : esd.mobile_dl_deduction,
                                   //laptop_dl_deduction = esd.laptop_dl_deduction == null ? 0 : esd.laptop_dl_deduction,

                                   monthly_tax = esd.monthly_tax == null ? 0 : esd.monthly_tax,

                                   totalD = esd.total_deduction == null ? 0 : esd.total_deduction,
                                   netPay = esd.net_pay == null ? 0 : esd.net_pay

                               }).ToList();
                }

                if (EmpList.Count > 0)
                {
                    LocalReport lr = new LocalReport();

                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "EmpWiseSalarySheetReport.rdlc");
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

                    ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                    lr.DataSources.Add(rd);
                    string reportType = "EXCELOPENXML";
                    string mimeType;
                    string encoding;
                    string fileNameExtension = "xlsx";
                    //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

                    string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>xlsx</OutputFormat>" +
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

                    //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ModelState.AddModelError("", "No information found");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select accurate month range.");
            }

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View();
        }


        [HttpPost]
        public ActionResult GetEmps(FormCollection collection)
        {
            try
            {
                int pageIndex = 0;
                var iDisplayStrart = collection.Get("iDisplayStart");
                if (!string.IsNullOrWhiteSpace(iDisplayStrart))
                {
                    pageIndex = Convert.ToInt32(iDisplayStrart) > 0 ? Convert.ToInt32(iDisplayStrart) - 1 : Convert.ToInt32(iDisplayStrart);
                }
                int pageSize = 30;
                if (!string.IsNullOrWhiteSpace(collection.Get("iDisplayLength")))
                {
                    pageSize = Convert.ToInt32(collection.Get("iDisplayLength"));
                }

                int gradeId = Convert.ToInt32(collection["grdId"]);
                int divisionId = Convert.ToInt32(collection["diviId"]);
                int departId = Convert.ToInt32(collection["dptId"]);
                int mnth = Convert.ToInt32(collection["mnth"]);
                int yr = Convert.ToInt32(collection["yr"]);

                var EmpList = (from emp in dataContext.prl_employee
                               join spd in dataContext.prl_salary_process_detail on emp.id equals spd.emp_id
                               where spd.salary_month.Month == mnth && spd.salary_month.Year == yr
                               select new ReportSalarySheet
                               {
                                   empId = emp.id,
                                   empNo = emp.emp_no,
                                   empName = emp.name,
                                   email = emp.email

                               }).ToList();

                var distEmp = new List<ReportSalarySheet>();
                int flag = 0; //All
                if (EmpList.Count > 0)
                {
                    if (gradeId == 0 && divisionId == 0 && departId == 0)
                    { }
                    else
                    {
                        flag = 1;
                        foreach (ReportSalarySheet emp in EmpList)
                        {
                            var _empD = dataContext.prl_employee_details.Where(x => x.emp_id == emp.empId).OrderByDescending(p => p.id).First();
                            if (_empD.grade_id == gradeId)
                            {
                                distEmp.Add(emp);
                            }
                            else if (_empD.division_id == divisionId)
                            {
                                distEmp.Add(emp);
                            }
                            else if (_empD.department_id == departId)
                            {
                                distEmp.Add(emp);
                            }
                        }

                    }
                }

                if (flag == 1)
                {
                    EmpList.Clear();
                    EmpList = distEmp;
                }

                int totalRecords = EmpList.Count();
                var employees = EmpList.Skip(pageIndex).Take(pageSize);

                var aaData = employees.Select(x => new string[] { x.empNo, x.empName, "cost_centre", x.email });

                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }

        [PayrollAuthorize]
        public ActionResult SalaryBankAdvice()
        {
            ReportBankAdvice ba = new ReportBankAdvice();
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View(ba);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult SalaryBankAdvice(ReportBankAdvice BA, string employee_category, DateTime SelectDate, FormCollection collection, string sButton)
        {
            var empNumber = new List<string>();
            var empIds = new List<int>();
            var EmpList = new List<ReportBankAdvice>();
            BA.Day = SelectDate.Day;
            BA.Month = SelectDate.Month;
            BA.Year = SelectDate.Year;
            string year = BA.Year.ToString();
            BA.MonthName = DateUtility.MonthName(SelectDate.Month);
            DateTime dt = new DateTime(BA.Year, BA.Month, BA.Day);
            string SelectedDate = dt.ToString("MM/dd/yyyy");
            DateTime emptyDate = new DateTime();

            var discontinued_empIds = new List<int>();
            discontinued_empIds = dataContext.prl_employee_discontinue.Where(x => x.continution_date == emptyDate || x.continution_date == null).Select(x => x.emp_id).ToList();


            EmpList = (from spd in dataContext.prl_salary_process_detail
                       join sp in dataContext.prl_salary_process on spd.salary_process_id equals sp.id
                       join emp in dataContext.prl_employee on spd.emp_id equals emp.id
                       join empD in dataContext.prl_employee_details on emp.id equals empD.emp_id
                       join bpd in dataContext.prl_bonus_process_detail on
                        new { Key1 = emp.id, Key2 = spd.salary_month.Month, Key3 = spd.salary_month.Year } equals new { Key1 = bpd.emp_id, Key2 = bpd.process_date.Month, Key3 = bpd.process_date.Year } into bon
                       from bonus in bon.DefaultIfEmpty()
                       join bp in dataContext.prl_bonus_process on bonus.bonus_process_id equals bp.id into fbon
                       from fbonus in fbon.DefaultIfEmpty()
                       join bn in dataContext.prl_bonus_name on fbonus.bonus_name_id equals bn.id into fbName
                       from fbonName in fbName.DefaultIfEmpty()
                       join ed in dataContext.prl_employee_discontinue on emp.id equals ed.emp_id into empDis
                       from empDiscon in empDis.DefaultIfEmpty()
                       //join cmb in dataContext.prl_company_bank on empD.employee_category equals cmb.account_category
                       where spd.salary_month.Year == SelectDate.Year && spd.salary_month.Month == SelectDate.Month
                           //&& (!string.IsNullOrEmpty(emp.account_no) 
                           //&& !emp.account_no.Contains("N/A") 
                           //&& emp.account_no != "0" 
                        // && !discontinued_empIds.Contains(spd.emp_id)
                       //)
                       select
                       new ReportBankAdvice
                       {
                           emp_id = emp.id,
                           empNo = emp.emp_no,
                           empName = emp.name,
                           //bankId = (int)emp.bank_id,
                           bank = emp.prl_bank.bank_name,
                           accNo = (emp.account_no != null || emp.account_no != "N/A" || emp.account_no != "0") ? emp.account_no : "",
                           routing_no = (emp.routing_no != null || emp.routing_no != "0") ? emp.routing_no : "",
                           accType = emp.account_type,
                           //bankBranchId = (int)emp.bank_branch_id,
                           netPay = (Math.Round((decimal)spd.this_month_basic, 2) + Math.Round((decimal)spd.total_allowance, 2) + (bonus.amount != null ? Math.Round((decimal)bonus.amount, 2) : 0) + Math.Round((decimal)spd.totla_arrear_allowance, 2) + Math.Round((decimal)spd.pf_amount, 2) + (spd.pf_arrear != null ? Math.Round((decimal)spd.pf_arrear, 2) : 0)) - (Math.Round(spd.total_deduction, 2) + Math.Round(spd.total_monthly_tax, 2) + (Math.Round(spd.pf_amount, 2) * 2)),

                           remarks = (bonus.amount != null || bonus.amount != 0) ? "Salary for " + BA.MonthName + " & Festival Bonus ( " + fbonName.name + " ) " + year + "" : "Salary for " + BA.MonthName + "-" + year + "",
                           debitAccNo = "200061004",
                           email = emp.email,
                           Year = BA.Year,
                           Month = BA.Month,
                           MonthName = BA.MonthName,
                           monthYear = BA.Year,
                           //SelectDate = 
                       }).ToList();


            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();
                string path = "";

                path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "BankAdvice.rdlc");

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

                ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                lr.DataSources.Add(rd);

                //// Prepare Parameters

                //ReportParameter[] para = new ReportParameter[2];
                //para[0] = new ReportParameter("employee_category", employee_category);
                //para[1] = new ReportParameter("selectedDate", SelectedDate);

                //// Pass Parameters for Local Report

                //lr.SetParameters(para);


                lr.SetParameters(new ReportParameter("selectedDate", dt.ToString("MM,yyy")));

                string reportType = "EXCELOPENXML";
                string mimeType;
                string encoding;
                string fileNameExtension = "xlsx";
                //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>xlsx</OutputFormat>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    //"",
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                return File(renderedBytes, mimeType);
            }
            else
            {
                ModelState.AddModelError("", "No information found");
            }


            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View();
        }


        [PayrollAuthorize]
        [HttpGet]
        public ActionResult VarianceReport()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(new ReportSalarySheet
            {
                //RType. = true
            });
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult VarianceReport(ReportSalarySheet SS, string sButton)
        {
            var empNumber = new List<string>();

           // var processAllEmployee = false;
            var empIds = new List<int>();
            var EmpList = new List<ReportSalarySheet>();
            var DataListMax = new List<ReportSalarySheet>();
            var DataListMin = new List<ReportSalarySheet>();
            //DateTime fromDate = new DateTime(SS.Year, SS.frm_month_no, 1);
            //DateTime toDate = new DateTime(SS.Year, SS.to_month_no, 1).AddMonths(1).AddDays(-1);

            //string year = SS.Year.ToString();


            if (!string.IsNullOrWhiteSpace(SS.SelectedEmployeesOnly))
            {
                empNumber = SS.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                            .Select(x => x.id).ToList();
            }

            DateTime selectedfromDate = new DateTime(SS.frm_year, SS.frm_month_no, 1);
            DateTime selectedtoDate = new DateTime(SS.to_year, SS.to_month_no, 1);

            //string frm_month_year = salaryMonth.ToString("yyyy-MM");

            //finding max Month number using ternary operator
            var toDate = (selectedtoDate > selectedfromDate) ? selectedtoDate : selectedfromDate;

            //finding min Month number using ternary operator
            var frmDate = (selectedtoDate < selectedfromDate) ? selectedtoDate : selectedfromDate;


            if (toDate == frmDate)
            {
                ModelState.AddModelError("", "Please select accurate month range.");
            }
            else
            {

                string month_Year = Utility.DateUtility.MonthName(frmDate.Month) + "-" + frmDate.Year + " and " + Utility.DateUtility.MonthName(toDate.Month) + "-" + toDate.Year;
                if (empNumber.Count > 0)
                {
                    

                    DataListMax = (from esd in dataContext.vw_empsalaryprocessdetails
                                   where empNumber.Contains(esd.emp_no)
                               && esd.salary_month.Year == toDate.Year && esd.salary_month.Month == toDate.Month
                                   select new ReportSalarySheet
                                   {
                                       empId = esd.id,
                                       empNo = esd.emp_no,
                                       empName = esd.empName,
                                       designation = esd.designation,

                                       joining_date = esd.joining_date,
                                       accNo = esd.account_no,
                                       routing_no = esd.routing_no != null ? esd.routing_no : " ",
                                       month_name = month_Year,
                                       month_no = esd.month_no,
                                       frm_month_no = frmDate.Month,
                                       to_month_no = toDate.Month,
                                       Year = esd.salary_month.Year,
                                       calendar_days = esd.calendar_days,

                                       bank_name = esd.bank_name != null ? esd.bank_name : " ",

                                       cost_centre_id = esd.cost_centre_id,
                                       cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                                       basic_salary = esd.basic_salary,
                                       this_month_basic = esd.this_month_basic,
                                       bta = esd.bta == null ? 0 : esd.bta,
                                       car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                       pf_cc_amount = esd.pf_cc_amount,
                                       conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                       houseR = esd.house == null ? 0 : esd.house,
                                       special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

                                       arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                                       arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                                       arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                                       arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                                       arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                                       incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                                       incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                                       incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                                       incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                                       sti = esd.sti != null ? esd.sti : 0,
                                       one_time = esd.one_time != null ? esd.one_time : 0,
                                       training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,

                                       gift = esd.gift != null ? esd.gift : 0,
                                       pf_refund = esd.pf_refund != null ? esd.pf_refund : 0,
                                       basic_arrear = esd.basic_arrear != null ? esd.basic_arrear : 0,

                                       tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                                       festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                       bonus = esd.bonus == null ? 0 : esd.bonus,
                                       total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                                       leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                                       long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                                       totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                       ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                                       modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),

                                       ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                                       mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                                       lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                                       others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                                       tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                                       pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                                       monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                                       income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                                       totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),

                                       netPay = esd.net_pay == null ? 0 : esd.net_pay

                                   }).ToList();

                    DataListMin = (from esd in dataContext.vw_empsalaryprocessdetails
                                   where empNumber.Contains(esd.emp_no)
                                && esd.salary_month.Year == frmDate.Year && esd.salary_month.Month == frmDate.Month
                                   select new ReportSalarySheet
                                   {
                                       empId = esd.id,
                                       empNo = esd.emp_no,
                                       empName = esd.empName,
                                       designation = esd.designation,

                                       joining_date = esd.joining_date,
                                       accNo = esd.account_no,
                                       routing_no = esd.routing_no != null ? esd.routing_no : " ",
                                       month_name = month_Year,
                                       month_no = esd.month_no,
                                       frm_month_no = frmDate.Month,
                                       to_month_no = toDate.Month,
                                       Year = esd.salary_month.Year,
                                       calendar_days = esd.calendar_days,

                                       bank_name = esd.bank_name != null ? esd.bank_name : " ",
                                       cost_centre_id = esd.cost_centre_id,
                                       cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                                       basic_salary = esd.basic_salary,
                                       this_month_basic = esd.this_month_basic,
                                       bta = esd.bta == null ? 0 : esd.bta,
                                       car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                       pf_cc_amount = esd.pf_cc_amount,
                                       conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                       houseR = esd.house == null ? 0 : esd.house,
                                       special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,
                                       arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                                       arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                                       arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                                       arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                                       arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                                       incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                                       incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                                       incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                                       incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                                       sti = esd.sti != null ? esd.sti : 0,
                                       one_time = esd.one_time != null ? esd.one_time : 0,
                                       training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                                       gift = esd.gift != null ? esd.gift : 0,
                                       pf_refund = esd.pf_refund != null ? esd.pf_refund : 0,
                                       basic_arrear = esd.basic_arrear != null ? esd.basic_arrear : 0,

                                       tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                                       festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                       bonus = esd.bonus == null ? 0 : esd.bonus,
                                       total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                                       leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                                       long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                                       totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                       ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                                       modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),
                                       ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                                       mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                                       lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                                       others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                                       tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                                       pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                                       monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                                       income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                                       totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),

                                   }).ToList();

                    // Uncommon Rows
                    var lstEmpIdMax = new List<int>();
                    lstEmpIdMax = DataListMax.AsEnumerable().Select(x => x.empId).ToList();
                    var lstEmpIdMin = new List<int>();
                    lstEmpIdMin = DataListMin.AsEnumerable().Select(x => x.empId).ToList();
                    var lstEmpIdUncommon = new List<int>();

                    var ListMax = DataListMax.Where(x => !lstEmpIdMin.Contains(x.empId)).ToList();
                    //var ListMin = DataListMin.Where(x => !lstEmpIdMax.Contains(x.empId)).ToList();
                    var ListMin = (from min in DataListMin
                                   where !lstEmpIdMax.Contains(min.empId)
                                   select new ReportSalarySheet
                                   {
                                       empId = min.empId,
                                       empNo = min.empNo,
                                       empName = min.empName,
                                       designation = min.designation,

                                       joining_date = min.joining_date,
                                       accNo = min.accNo,
                                       routing_no = min.routing_no != null ? min.routing_no : " ",
                                       month_name = month_Year,
                                       month_no = min.month_no,
                                       frm_month_no = frmDate.Month,
                                       to_month_no = toDate.Month,
                                       Year = min.Year,
                                       calendar_days = min.calendar_days,
                                       bank_name = min.bank_name != null ? min.bank_name : " ",

                                       cost_centre_id = min.cost_centre_id,
                                       cost_centre_name = min.cost_centre_name != null ? min.cost_centre_name : "",

                                       basic_salary = decimal.Negate(min.basic_salary ?? 0),

                                       this_month_basic = decimal.Negate(min.this_month_basic ?? 0),

                                       bta = decimal.Negate(min.bta ?? 0),
                                       car_mc_allowance = decimal.Negate(min.car_mc_allowance ?? 0),
                                       pf_cc_amount = decimal.Negate(min.pf_cc_amount ?? 0),
                                       conveyance = decimal.Negate(min.conveyance ?? 0),
                                       houseR = decimal.Negate(min.houseR ?? 0),
                                       special_allowance = decimal.Negate(min.special_allowance ?? 0),

                                       arrear_basic = decimal.Negate(min.arrear_basic ?? 0),
                                       arrear_house = decimal.Negate(min.arrear_house ?? 0),
                                       arrear_conveyance = decimal.Negate(min.arrear_conveyance ?? 0),
                                       arrear_car_mc_allowance = decimal.Negate(min.arrear_car_mc_allowance ?? 0),
                                       arrear_leave_encashment = decimal.Negate(min.arrear_leave_encashment ?? 0),

                                       incentive_q1 = decimal.Negate(min.incentive_q1 ?? 0),

                                       incentive_q2 = decimal.Negate(min.incentive_q2 ?? 0),
                                       incentive_q3 = decimal.Negate(min.incentive_q3 ?? 0),
                                       incentive_q4 = decimal.Negate(min.incentive_q4 ?? 0),
                                       sti = decimal.Negate(min.sti ?? 0),
                                       one_time = decimal.Negate(min.one_time ?? 0),
                                       training_allowance = decimal.Negate(min.training_allowance ?? 0),
                                       gift = decimal.Negate(min.gift ?? 0),
                                       pf_refund = decimal.Negate(min.pf_refund ?? 0),
                                       basic_arrear = decimal.Negate(min.basic_arrear ?? 0),

                                       tax_paid_by_company = decimal.Negate(min.tax_paid_by_company ?? 0),
                                       festival_bonus = decimal.Negate(min.festival_bonus ?? 0),
                                       bonus = decimal.Negate(min.bonus ?? 0),
                                       total_arrear_allowance = decimal.Negate(min.total_arrear_allowance ?? 0),
                                       leave_encashment = decimal.Negate(min.leave_encashment ?? 0),
                                       long_service_award = decimal.Negate(min.long_service_award ?? 0),

                                       totalA = decimal.Negate(min.totalA ?? 0),

                                       ipad_or_mobile_bill = decimal.Negate(min.ipad_or_mobile_bill ?? 0),
                                       modem_bill = decimal.Negate(min.modem_bill ?? 0),

                                       ipad_bill = decimal.Negate(min.ipad_bill ?? 0),
                                       mobile_bill = decimal.Negate(min.mobile_bill ?? 0),

                                       lunch_support = decimal.Negate(min.lunch_support ?? 0),
                                       others_deduction = decimal.Negate(min.others_deduction ?? 0),
                                       tax_return_non_submission = decimal.Negate(min.tax_return_non_submission ?? 0),
                                       pf_co_amount = decimal.Negate(min.pf_co_amount ?? 0),
                                       monthly_tax = decimal.Negate(min.monthly_tax ?? 0),
                                       income_tax = decimal.Negate(min.income_tax ?? 0), // if upload tax
                                       totalD = decimal.Negate(min.totalD ?? 0),
                                       netPay = decimal.Negate(min.netPay ?? 0),

                                   }).ToList();

                    var UncommonDataList = new List<ReportSalarySheet>();
                    UncommonDataList.AddRange(ListMax);
                    UncommonDataList.AddRange(ListMin);
                    lstEmpIdUncommon = UncommonDataList.AsEnumerable().Select(x => x.empId).ToList();

                    EmpList =
                        (from max in DataListMax
                         join min in DataListMin on max.empId equals min.empId into match
                         from min in match.DefaultIfEmpty()
                         where !lstEmpIdUncommon.Contains(max.empId) && !lstEmpIdUncommon.Contains(min.empId)
                         select new ReportSalarySheet
                         {
                             empId = max.empId,
                             empNo = max.empNo,
                             empName = max.empName,
                             designation = max.designation,

                             joining_date = max.joining_date,
                             accNo = max.accNo,
                             routing_no = max.routing_no != null ? max.routing_no : " ",
                             month_name = month_Year,
                             month_no = max.month_no,
                             frm_month_no = frmDate.Month,
                             to_month_no = toDate.Month,
                             Year = max.Year,
                             calendar_days = max.calendar_days,
                             bank_name = max.bank_name != null ? max.bank_name : " ",

                             cost_centre_id = max.cost_centre_id,
                             cost_centre_name = max.cost_centre_name != null ? max.cost_centre_name : "",

                             basic_salary = max.basic_salary - ((min != null || min.basic_salary != 0) ? min.basic_salary : 0),
                             this_month_basic = max.this_month_basic - ((min != null || min.this_month_basic != 0) ? min.this_month_basic : 0),
                             bta = max.bta - ((min != null || min.bta != 0) ? min.bta : 0),

                             car_mc_allowance = max.car_mc_allowance - ((min != null || min.car_mc_allowance != 0) ? min.car_mc_allowance : 0),
                             pf_cc_amount = max.pf_cc_amount - ((min != null || min.pf_cc_amount != 0) ? min.pf_cc_amount : 0),

                             conveyance = max.conveyance - ((min != null || min.conveyance != 0) ? min.conveyance : 0),

                             houseR = max.houseR - ((min != null || min.houseR != 0) ? min.houseR : 0),

                             special_allowance = max.special_allowance - ((min != null || min.special_allowance != 0) ? min.special_allowance : 0),

                             arrear_basic = max.arrear_basic - ((min != null || min.arrear_basic != 0) ? min.arrear_basic : 0),
                             arrear_house = max.arrear_house - ((min != null || min.arrear_house != 0) ? min.arrear_house : 0),
                             arrear_conveyance = max.arrear_conveyance - ((min != null || min.arrear_conveyance != 0) ? min.arrear_conveyance : 0),
                             arrear_car_mc_allowance = max.arrear_car_mc_allowance - ((min != null || min.arrear_car_mc_allowance != 0) ? min.arrear_car_mc_allowance : 0),
                             arrear_leave_encashment = max.arrear_leave_encashment - ((min != null || min.arrear_leave_encashment != 0) ? min.arrear_leave_encashment : 0),

                             incentive_q1 = max.incentive_q1 - ((min != null || min.incentive_q1 != 0) ? min.incentive_q1 : 0),
                             incentive_q2 = max.incentive_q2 - ((min != null || min.incentive_q2 != 0) ? min.incentive_q2 : 0),

                             incentive_q3 = max.incentive_q3 - ((min != null || min.incentive_q3 != 0) ? min.incentive_q3 : 0),

                             incentive_q4 = max.incentive_q4 - ((min != null || min.incentive_q4 != 0) ? min.incentive_q4 : 0),

                             sti = max.sti - ((min != null || min.sti != 0) ? min.sti : 0),

                             one_time = max.one_time - ((min != null || min.one_time != 0) ? min.one_time : 0),
                             training_allowance = max.training_allowance - ((min != null || min.training_allowance != 0) ? min.training_allowance : 0),

                             gift = max.gift - ((min != null || min.gift != 0) ? min.gift : 0),
                             pf_refund = max.pf_refund - ((min != null || min.pf_refund != 0) ? min.pf_refund : 0),
                             basic_arrear = max.basic_arrear - ((min != null || min.basic_arrear != 0) ? min.basic_arrear : 0),

                             tax_paid_by_company = max.tax_paid_by_company - ((min != null || min.tax_paid_by_company != 0) ? min.tax_paid_by_company : 0),

                             festival_bonus = max.festival_bonus - ((min != null || min.festival_bonus != 0) ? min.festival_bonus : 0),
                             bonus = max.bonus - ((min != null || min.bonus != 0) ? min.bonus : 0),

                             total_arrear_allowance = max.total_arrear_allowance - ((min != null || min.total_arrear_allowance != 0) ? min.total_arrear_allowance : 0),

                             leave_encashment = max.leave_encashment - ((min != null || min.leave_encashment != 0) ? min.leave_encashment : 0),
                             long_service_award = max.long_service_award - ((min != null || min.long_service_award != 0) ? min.long_service_award : 0),

                             totalA = max.totalA - ((min != null || min.totalA != 0) ? min.totalA : 0),

                             ipad_or_mobile_bill = max.ipad_or_mobile_bill - ((min != null || min.ipad_or_mobile_bill != 0) ? min.ipad_or_mobile_bill : 0),

                             modem_bill = max.modem_bill - ((min != null || min.modem_bill != 0) ? min.modem_bill : 0),
                             ipad_bill = max.ipad_bill - ((min != null || min.ipad_bill != 0) ? min.ipad_bill : 0),
                             mobile_bill = max.mobile_bill - ((min != null || min.mobile_bill != 0) ? min.mobile_bill : 0),

                             lunch_support = max.lunch_support - ((min != null || min.lunch_support != 0) ? min.lunch_support : 0),
                             others_deduction = max.others_deduction - ((min != null || min.others_deduction != 0) ? min.others_deduction : 0),

                             tax_return_non_submission = max.tax_return_non_submission - ((min != null || min.tax_return_non_submission != 0) ? min.tax_return_non_submission : 0),

                             pf_co_amount = max.pf_co_amount - ((min != null || min.pf_co_amount != 0) ? min.pf_co_amount : 0),

                             monthly_tax = max.monthly_tax - ((min != null || min.monthly_tax != 0) ? min.monthly_tax : 0),
                             // if upload tax
                             income_tax = max.income_tax - ((min != null || min.income_tax != 0) ? min.income_tax : 0),
                             totalD = max.totalD - ((min != null || min.totalD != 0) ? min.totalD : 0),

                             netPay = max.netPay - ((min != null || min.netPay != 0) ? min.netPay : 0),

                             selectedfromDate = frmDate,
                             selectedtoDate = toDate

                         }).ToList();

                    if (UncommonDataList.Count > 0)
                    {
                        EmpList.AddRange(UncommonDataList);
                    }

                }
                else
                {
                    DataListMax = (from esd in dataContext.vw_empsalaryprocessdetails
                                   where esd.salary_month.Year == toDate.Year && esd.salary_month.Month == toDate.Month
                                   select new ReportSalarySheet
                                   {
                                       empId = esd.id,
                                       empNo = esd.emp_no,
                                       empName = esd.empName,
                                       designation = esd.designation,

                                       joining_date = esd.joining_date,
                                       accNo = esd.account_no,
                                       routing_no = esd.routing_no != null ? esd.routing_no : " ",
                                       month_name = month_Year,
                                       month_no = esd.month_no,
                                       frm_month_no = frmDate.Month,
                                       to_month_no = toDate.Month,
                                       Year = esd.salary_month.Year,
                                       calendar_days = esd.calendar_days,

                                       bank_name = esd.bank_name != null ? esd.bank_name : " ",

                                       cost_centre_id = esd.cost_centre_id,
                                       cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                                       basic_salary = esd.basic_salary,
                                       this_month_basic = esd.this_month_basic,
                                       bta = esd.bta == null ? 0 : esd.bta,
                                       car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                       pf_cc_amount = esd.pf_cc_amount,
                                       conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                       houseR = esd.house == null ? 0 : esd.house,
                                       special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,
                                       arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                                       arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                                       arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                                       arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                                       arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                                       incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                                       incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                                       incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                                       incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                                       sti = esd.sti != null ? esd.sti : 0,
                                       one_time = esd.one_time != null ? esd.one_time : 0,
                                       training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                                       gift = esd.gift != null ? esd.gift : 0,

                                       pf_refund = esd.pf_refund != null ? esd.pf_refund : 0,
                                       basic_arrear = esd.basic_arrear != null ? esd.basic_arrear : 0,

                                       tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                                       festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                       bonus = esd.bonus == null ? 0 : esd.bonus,
                                       total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                                       leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                                       long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                                       totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                       ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                                       modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),
                                       ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                                       mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                                       lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                                       others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                                       tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                                       pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                                       monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                                       income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                                       totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),

                                       netPay = esd.net_pay == null ? 0 : esd.net_pay

                                   }).ToList();

                    DataListMin = (from esd in dataContext.vw_empsalaryprocessdetails
                                   where esd.salary_month.Year == frmDate.Year && esd.salary_month.Month == frmDate.Month

                                   select new ReportSalarySheet
                                   {
                                       empId = esd.id,
                                       empNo = esd.emp_no,
                                       empName = esd.empName,
                                       designation = esd.designation,

                                       joining_date = esd.joining_date,
                                       accNo = esd.account_no,
                                       routing_no = esd.routing_no != null ? esd.routing_no : " ",
                                       month_name = month_Year,
                                       month_no = esd.month_no,
                                       frm_month_no = frmDate.Month,
                                       to_month_no = toDate.Month,
                                       Year = esd.salary_month.Year,
                                       calendar_days = esd.calendar_days,
                                       payment_date = esd.payment_date,

                                       bank_name = esd.bank_name != null ? esd.bank_name : " ",
                                       cost_centre_id = esd.cost_centre_id,
                                       cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                                       basic_salary = esd.basic_salary,
                                       this_month_basic = esd.this_month_basic,
                                       bta = esd.bta == null ? 0 : esd.bta,
                                       car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                       pf_cc_amount = esd.pf_cc_amount,
                                       conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                       houseR = esd.house == null ? 0 : esd.house,
                                       special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,
                                       arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                                       arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                                       arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                                       arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                                       arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                                       incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                                       incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                                       incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                                       incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                                       sti = esd.sti != null ? esd.sti : 0,
                                       one_time = esd.one_time != null ? esd.one_time : 0,
                                       training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                                       gift = esd.gift != null ? esd.gift : 0,
                                       pf_refund = esd.pf_refund != null ? esd.pf_refund : 0,
                                       basic_arrear = esd.basic_arrear != null ? esd.basic_arrear : 0,

                                       tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                                       festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                       bonus = esd.bonus == null ? 0 : esd.bonus,
                                       total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                                       leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                                       long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                                       totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                       ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                                       modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),
                                       ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                                       mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                                       lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                                       others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                                       tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                                       pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                                       monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                                       income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                                       totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),
                                       netPay = esd.net_pay == null ? 0 : esd.net_pay

                                   }).ToList();

                    // Uncommon Rows
                    var lstEmpIdMax = new List<int>();
                    lstEmpIdMax = DataListMax.AsEnumerable().Select(x => x.empId).ToList();
                    var lstEmpIdMin = new List<int>();
                    lstEmpIdMin = DataListMin.AsEnumerable().Select(x => x.empId).ToList();
                    var lstEmpIdUncommon = new List<int>();

                    var ListMax = DataListMax.Where(x => !lstEmpIdMin.Contains(x.empId)).ToList();
                    //var ListMin = DataListMin.Where(x => !lstEmpIdMax.Contains(x.empId)).ToList();
                    var ListMin = (from min in DataListMin
                                   where !lstEmpIdMax.Contains(min.empId)
                                   select new ReportSalarySheet
                                   {
                                       empId = min.empId,
                                       empNo = min.empNo,
                                       empName = min.empName,
                                       designation = min.designation,

                                       joining_date = min.joining_date,
                                       accNo = min.accNo,
                                       routing_no = min.routing_no != null ? min.routing_no : " ",
                                       month_name = month_Year,
                                       month_no = min.month_no,
                                       frm_month_no = frmDate.Month,
                                       to_month_no = toDate.Month,
                                       Year = min.Year,
                                       calendar_days = min.calendar_days,
                                       bank_name = min.bank_name != null ? min.bank_name : " ",

                                       cost_centre_id = min.cost_centre_id,
                                       cost_centre_name = min.cost_centre_name != null ? min.cost_centre_name : "",

                                       basic_salary = decimal.Negate(min.basic_salary ?? 0),

                                       this_month_basic = decimal.Negate(min.this_month_basic ?? 0),

                                       bta = decimal.Negate(min.bta ?? 0),
                                       car_mc_allowance = decimal.Negate(min.car_mc_allowance ?? 0),
                                       pf_cc_amount = decimal.Negate(min.pf_cc_amount ?? 0),
                                       conveyance = decimal.Negate(min.conveyance ?? 0),
                                       houseR = decimal.Negate(min.houseR ?? 0),
                                       special_allowance = decimal.Negate(min.special_allowance ?? 0),

                                       arrear_basic = decimal.Negate(min.arrear_basic ?? 0),
                                       arrear_house = decimal.Negate(min.arrear_house ?? 0),
                                       arrear_conveyance = decimal.Negate(min.arrear_conveyance ?? 0),
                                       arrear_car_mc_allowance = decimal.Negate(min.arrear_car_mc_allowance ?? 0),
                                       arrear_leave_encashment = decimal.Negate(min.arrear_leave_encashment ?? 0),

                                       incentive_q1 = decimal.Negate(min.incentive_q1 ?? 0),

                                       incentive_q2 = decimal.Negate(min.incentive_q2 ?? 0),
                                       incentive_q3 = decimal.Negate(min.incentive_q3 ?? 0),
                                       incentive_q4 = decimal.Negate(min.incentive_q4 ?? 0),
                                       sti = decimal.Negate(min.sti ?? 0),
                                       one_time = decimal.Negate(min.one_time ?? 0),
                                       training_allowance = decimal.Negate(min.training_allowance ?? 0),
                                       gift = decimal.Negate(min.gift ?? 0),
                                       pf_refund = decimal.Negate(min.pf_refund ?? 0),
                                       basic_arrear = decimal.Negate(min.basic_arrear ?? 0),

                                       tax_paid_by_company = decimal.Negate(min.tax_paid_by_company ?? 0),
                                       festival_bonus = decimal.Negate(min.festival_bonus ?? 0),
                                       bonus = decimal.Negate(min.bonus ?? 0),
                                       total_arrear_allowance = decimal.Negate(min.total_arrear_allowance ?? 0),
                                       leave_encashment = decimal.Negate(min.leave_encashment ?? 0),
                                       long_service_award = decimal.Negate(min.long_service_award ?? 0),

                                       totalA = decimal.Negate(min.totalA ?? 0),

                                       ipad_or_mobile_bill = decimal.Negate(min.ipad_or_mobile_bill ?? 0),
                                       modem_bill = decimal.Negate(min.modem_bill ?? 0),

                                       ipad_bill = decimal.Negate(min.ipad_bill ?? 0),
                                       mobile_bill = decimal.Negate(min.mobile_bill ?? 0),

                                       lunch_support = decimal.Negate(min.lunch_support ?? 0),
                                       others_deduction = decimal.Negate(min.others_deduction ?? 0),
                                       tax_return_non_submission = decimal.Negate(min.tax_return_non_submission ?? 0),
                                       pf_co_amount = decimal.Negate(min.pf_co_amount ?? 0),
                                       monthly_tax = decimal.Negate(min.monthly_tax ?? 0),
                                       income_tax = decimal.Negate(min.income_tax ?? 0), // if upload tax
                                       totalD = decimal.Negate(min.totalD ?? 0),
                                       netPay = decimal.Negate(min.netPay ?? 0)

                                   }).ToList();

                    var UncommonDataList = new List<ReportSalarySheet>();
                    UncommonDataList.AddRange(ListMax);
                    UncommonDataList.AddRange(ListMin);
                    lstEmpIdUncommon = UncommonDataList.AsEnumerable().Select(x => x.empId).ToList();

                    EmpList =
                        (from max in DataListMax
                         join min in DataListMin on max.empId equals min.empId into match
                         from min in match.DefaultIfEmpty()
                         where !lstEmpIdUncommon.Contains(max.empId) && !lstEmpIdUncommon.Contains(min.empId)
                         select new ReportSalarySheet
                         {
                             empId = max.empId,
                             empNo = max.empNo,
                             empName = max.empName,
                             designation = max.designation,

                             joining_date = max.joining_date,
                             accNo = max.accNo,
                             routing_no = max.routing_no != null ? max.routing_no : " ",
                             month_name = month_Year,
                             month_no = max.month_no,
                             frm_month_no = frmDate.Month,
                             to_month_no = toDate.Month,
                             Year = max.Year,
                             calendar_days = max.calendar_days,
                             bank_name = max.bank_name != null ? max.bank_name : " ",

                             cost_centre_id = max.cost_centre_id,
                             cost_centre_name = max.cost_centre_name != null ? max.cost_centre_name : "",

                             basic_salary = max.basic_salary - ((min != null || min.basic_salary != 0) ? min.basic_salary : 0),
                             this_month_basic = max.this_month_basic - ((min != null || min.this_month_basic != 0) ? min.this_month_basic : 0),
                             bta = max.bta - ((min != null || min.bta != 0) ? min.bta : 0),

                             car_mc_allowance = max.car_mc_allowance - ((min != null || min.car_mc_allowance != 0) ? min.car_mc_allowance : 0),
                             pf_cc_amount = max.pf_cc_amount - ((min != null || min.pf_cc_amount != 0) ? min.pf_cc_amount : 0),

                             conveyance = max.conveyance - ((min != null || min.conveyance != 0) ? min.conveyance : 0),

                             houseR = max.houseR - ((min != null || min.houseR != 0) ? min.houseR : 0),

                             special_allowance = max.special_allowance - ((min != null || min.special_allowance != 0) ? min.special_allowance : 0),
                             arrear_basic = max.arrear_basic - ((min != null || min.arrear_basic != 0) ? min.arrear_basic : 0),
                             arrear_house = max.arrear_house - ((min != null || min.arrear_house != 0) ? min.arrear_house : 0),
                             arrear_conveyance = max.arrear_conveyance - ((min != null || min.arrear_conveyance != 0) ? min.arrear_conveyance : 0),
                             arrear_car_mc_allowance = max.arrear_car_mc_allowance - ((min != null || min.arrear_car_mc_allowance != 0) ? min.arrear_car_mc_allowance : 0),
                             arrear_leave_encashment = max.arrear_leave_encashment - ((min != null || min.arrear_leave_encashment != 0) ? min.arrear_leave_encashment : 0),


                             incentive_q1 = max.incentive_q1 - ((min != null || min.incentive_q1 != 0) ? min.incentive_q1 : 0),
                             incentive_q2 = max.incentive_q2 - ((min != null || min.incentive_q2 != 0) ? min.incentive_q2 : 0),

                             incentive_q3 = max.incentive_q3 - ((min != null || min.incentive_q3 != 0) ? min.incentive_q3 : 0),

                             incentive_q4 = max.incentive_q4 - ((min != null || min.incentive_q4 != 0) ? min.incentive_q4 : 0),

                             sti = max.sti - ((min != null || min.sti != 0) ? min.sti : 0),

                             one_time = max.one_time - ((min != null || min.one_time != 0) ? min.one_time : 0),
                             training_allowance = max.training_allowance - ((min != null || min.training_allowance != 0) ? min.training_allowance : 0),

                             gift = max.gift - ((min != null || min.gift != 0) ? min.gift : 0),

                             pf_refund = max.pf_refund - ((min != null || min.pf_refund != 0) ? min.pf_refund : 0),
                             basic_arrear = max.basic_arrear - ((min != null || min.basic_arrear != 0) ? min.basic_arrear : 0),

                             tax_paid_by_company = max.tax_paid_by_company - ((min != null || min.tax_paid_by_company != 0) ? min.tax_paid_by_company : 0),

                             festival_bonus = max.festival_bonus - ((min != null || min.festival_bonus != 0) ? min.festival_bonus : 0),
                             bonus = max.bonus - ((min != null || min.bonus != 0) ? min.bonus : 0),

                             total_arrear_allowance = max.total_arrear_allowance - ((min != null || min.total_arrear_allowance != 0) ? min.total_arrear_allowance : 0),

                             leave_encashment = max.leave_encashment - ((min != null || min.leave_encashment != 0) ? min.leave_encashment : 0),
                             long_service_award = max.long_service_award - ((min != null || min.long_service_award != 0) ? min.long_service_award : 0),

                             totalA = max.totalA - ((min != null || min.totalA != 0) ? min.totalA : 0),

                             ipad_or_mobile_bill = ( max.ipad_or_mobile_bill - ((min != null || min.ipad_or_mobile_bill != 0) ? min.ipad_or_mobile_bill : 0)),

                             modem_bill = ( max.modem_bill - ((min != null || min.modem_bill != 0) ? min.modem_bill : 0)),

                             ipad_bill = (max.ipad_bill - ((min != null || min.ipad_bill != 0) ? min.ipad_bill : 0)),
                             mobile_bill = (max.mobile_bill - ((min != null || min.mobile_bill != 0) ? min.mobile_bill : 0)),


                             lunch_support = ( max.lunch_support - ((min != null || min.lunch_support != 0) ? min.lunch_support : 0)),
                             others_deduction = (max.others_deduction - ((min != null || min.others_deduction != 0) ? min.others_deduction : 0)),

                             tax_return_non_submission = ( max.tax_return_non_submission - ((min != null || min.tax_return_non_submission != 0) ? min.tax_return_non_submission : 0)),

                             pf_co_amount = ( max.pf_co_amount - ((min != null || min.pf_co_amount != 0) ? min.pf_co_amount : 0)),

                             monthly_tax = ( max.monthly_tax - ((min != null || min.monthly_tax != 0) ? min.monthly_tax : 0)),
                             // if upload tax
                             income_tax = ( max.income_tax - ((min != null || min.income_tax != 0) ? min.income_tax : 0)),
                             totalD = ( max.totalD - ((min != null || min.totalD != 0) ? min.totalD : 0)),

                             netPay = max.netPay - ((min != null || min.netPay != 0) ? min.netPay : 0),

                             selectedfromDate = frmDate,
                             selectedtoDate = toDate

                         }).ToList();

                    if (UncommonDataList.Count > 0)
                    {
                        EmpList.AddRange(UncommonDataList);
                    }

                }

                if (EmpList.Count > 0)
                {
                    LocalReport lr = new LocalReport();

                    
                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "VarianceAnlysisReport.rdlc");
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

                    ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                    lr.DataSources.Clear();
                    lr.DataSources.Add(rd);

                    lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_vr_SubreportProcessing);

                    lr.Refresh();

                    string reportType = "EXCELOPENXML";
                    string mimeType;
                    string encoding;
                    string fileNameExtension = "xlsx";
                    //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

                    string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>xlsx</OutputFormat>" +
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

                    //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                    return File(renderedBytes, mimeType);
                }
                else
                {
                    ModelState.AddModelError("", "No information found");
                }
            }

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View();
        }

        void lr_vr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            string month_name = e.Parameters["month_name"].Values[0];
            int frm_month_no = Convert.ToInt32(e.Parameters["frm_month_no"].Values[0]);
            int to_month_no = Convert.ToInt32(e.Parameters["to_month_no"].Values[0]);
            int Year = Convert.ToInt32(e.Parameters["Year"].Values[0]);

            DateTime selectedfromDate = Convert.ToDateTime(e.Parameters["selectedfromDate"].Values[0]);
            DateTime selectedtoDate = Convert.ToDateTime(e.Parameters["selectedtoDate"].Values[0]);

            string pth = e.ReportPath;

            if (pth == "PreviousMonthSalaryTotal")
            {
                //.GroupBy(x => new { x.Year, x.Month }, (key, group) => new
                //{
                //    yr = key.Year,
                //    mnth = key.Month,
                //    tCharge = group.Sum(k => k.TotalCharge)
                //}).ToList();
                string this_month_name = Utility.DateUtility.MonthName(frm_month_no);

                var DataListMin = (from esd in dataContext.vw_empsalaryprocessdetails
                                   where esd.salary_month.Year == selectedfromDate.Year && esd.salary_month.Month == selectedfromDate.Month
                                   select new MonthlySalaryTotal
                                   {
                                       month_name = month_name,
                                       month_no = esd.month_no,
                                       Year = esd.salary_month.Year,
                                       cost_centre_id = esd.cost_centre_id,
                                       cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",
                                       this_month_name = this_month_name,
                                       basic_salary = esd.basic_salary,
                                       this_month_basic = esd.this_month_basic,
                                       bta = esd.bta == null ? 0 : esd.bta,
                                       car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                       pf_cc_amount = esd.pf_cc_amount,
                                       conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                       houseR = esd.house == null ? 0 : esd.house,
                                       special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

                                       arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                                       arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                                       arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                                       arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                                       arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                                       incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                                       incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                                       incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                                       incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                                       sti = esd.sti != null ? esd.sti : 0,
                                       one_time = esd.one_time != null ? esd.one_time : 0,
                                       training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                                       gift = esd.gift != null ? esd.gift : 0,
                                       tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                                       festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                       bonus = esd.bonus == null ? 0 : esd.bonus,
                                       total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                                       leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                                       long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                                       totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                       ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                                       modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),

                                       ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                                       mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                                       lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                                       others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),

                                       tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                                       pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                                       monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                                       income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                                       totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),

                                       netPay = esd.net_pay == null ? 0 : esd.net_pay

                                   }).GroupBy(x => x.month_no).Select( g => new {
                                       this_month_name = this_month_name,
                                       basic_salary = g.Sum(x => x.basic_salary),
                                       this_month_basic = g.Sum(x => x.this_month_basic),
                                       bta = g.Sum(x => x.bta),
                                       car_mc_allowance = g.Sum(x => x.car_mc_allowance),
                                       pf_cc_amount = g.Sum(x => x.pf_cc_amount),
                                       conveyance = g.Sum(x => x.conveyance),
                                       houseR = g.Sum(x => x.houseR),
                                       special_allowance = g.Sum(x => x.special_allowance),

                                       arrear_basic = g.Sum(x => x.arrear_basic),
                                       arrear_house = g.Sum(x => x.arrear_house),
                                       arrear_conveyance = g.Sum(x => x.arrear_conveyance),
                                       arrear_car_mc_allowance = g.Sum(x => x.arrear_car_mc_allowance),
                                       arrear_leave_encashment = g.Sum(x => x.arrear_leave_encashment),

                                       incentive_q1 = g.Sum(x => x.incentive_q1),
                                       incentive_q2 = g.Sum(x => x.incentive_q2),
                                       incentive_q3 = g.Sum(x => x.incentive_q3),
                                       incentive_q4 = g.Sum(x => x.incentive_q4),
                                       sti = g.Sum(x => x.sti),
                                       one_time = g.Sum(x => x.one_time),
                                       training_allowance = g.Sum(x => x.training_allowance),
                                       gift = g.Sum(x => x.gift),
                                       tax_paid_by_company = g.Sum(x => x.tax_paid_by_company),
                                       festival_bonus = g.Sum(x => x.festival_bonus),
                                       bonus = g.Sum(x=> x.bonus),
                                       total_arrear_allowance = g.Sum(x => x.total_arrear_allowance),
                                       leave_encashment = g.Sum(x => x.leave_encashment),
                                       long_service_award = g.Sum(x => x.long_service_award),

                                       totalA = g.Sum(x => x.totalA),

                                       ipad_or_mobile_bill = g.Sum(x => x.ipad_or_mobile_bill),
                                       modem_bill = g.Sum(x => x.modem_bill),

                                       ipad_bill = g.Sum(x => x.ipad_bill),
                                       mobile_bill = g.Sum(x => x.mobile_bill),

                                       lunch_support = g.Sum(x => x.lunch_support),
                                       others_deduction = g.Sum(x => x.others_deduction),
                                       tax_return_non_submission = g.Sum(x => x.tax_return_non_submission),
                                       pf_co_amount = g.Sum(x => x.pf_co_amount),
                                       monthly_tax = g.Sum(x => x.monthly_tax),
                                       income_tax = g.Sum(x => x.income_tax), // if upload tax
                                       totalD = g.Sum(x => x.totalD),
                                       netPay = g.Sum(x => x.netPay),
                                   
                                   }).ToList();

                e.DataSources.Add(new ReportDataSource("DataSet1", DataListMin));
            }
            else
            {
                string this_month_name = Utility.DateUtility.MonthName(to_month_no);
                var DataListMax = (from esd in dataContext.vw_empsalaryprocessdetails
                                   where esd.salary_month.Year == selectedtoDate.Year && esd.salary_month.Month == selectedtoDate.Month
                                   select new MonthlySalaryTotal
                                   {
                                       month_name = month_name,
                                       month_no = esd.month_no,
                                       Year = esd.salary_month.Year,
                                       cost_centre_id = esd.cost_centre_id,
                                       cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",
                                       this_month_name = this_month_name,
                                       basic_salary = esd.basic_salary,
                                       this_month_basic = esd.this_month_basic,
                                       bta = esd.bta == null ? 0 : esd.bta,
                                       car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                                       pf_cc_amount = esd.pf_cc_amount,
                                       conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                                       houseR = esd.house == null ? 0 : esd.house,
                                       special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

                                       arrear_basic = esd.arrear_basic != null ? esd.arrear_basic : 0,
                                       arrear_house = esd.arrear_house != null ? esd.arrear_house : 0,
                                       arrear_conveyance = esd.arrear_conveyance != null ? esd.arrear_conveyance : 0,
                                       arrear_car_mc_allowance = esd.arrear_car_mc_allowance != null ? esd.arrear_car_mc_allowance : 0,
                                       arrear_leave_encashment = esd.arrear_leave_encashment != null ? esd.arrear_leave_encashment : 0,

                                       incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                                       incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                                       incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                                       incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                                       sti = esd.sti != null ? esd.sti : 0,
                                       one_time = esd.one_time != null ? esd.one_time : 0,
                                       training_allowance = esd.training_allowance != null ? esd.training_allowance : 0,
                                       gift = esd.gift != null ? esd.gift : 0,
                                       tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                                       festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                                       bonus = esd.bonus == null ? 0 : esd.bonus,
                                       total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                                       leave_encashment = esd.leave_encashment == null ? 0 : esd.leave_encashment,
                                       long_service_award = esd.long_service_award == null ? 0 : esd.long_service_award,

                                       totalA = esd.total_allowance == null ? 0 : esd.total_allowance,

                                       ipad_or_mobile_bill = (esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill) * (-1),
                                       modem_bill = (esd.modem_bill == null ? 0 : esd.modem_bill) * (-1),

                                       ipad_bill = (esd.ipad_bill == null ? 0 : esd.ipad_bill) * (-1),
                                       mobile_bill = (esd.mobile_bill == null ? 0 : esd.mobile_bill) * (-1),

                                       lunch_support = (esd.lunch_support == null ? 0 : esd.lunch_support) * (-1),
                                       others_deduction = (esd.others_deduction == null ? 0 : esd.others_deduction) * (-1),
                                       tax_return_non_submission = (esd.tax_return_non_submission == null ? 0 : esd.tax_return_non_submission) * (-1),
                                       pf_co_amount = (esd.pf_co_amount == null ? 0 : esd.pf_co_amount) * (-1),
                                       monthly_tax = (esd.monthly_tax == null ? 0 : esd.monthly_tax) * (-1),
                                       income_tax = (esd.income_tax == null ? 0 : esd.income_tax) * (-1), // if upload tax
                                       totalD = (esd.total_deduction == null ? 0 : esd.total_deduction) * (-1),

                                       netPay = esd.net_pay == null ? 0 : esd.net_pay

                                   }).GroupBy(x => x.month_no).Select(g => new
                                   {
                                       this_month_name = this_month_name,
                                       basic_salary = g.Sum(x => x.basic_salary),
                                       this_month_basic = g.Sum(x => x.this_month_basic),
                                       bta = g.Sum(x => x.bta),
                                       car_mc_allowance = g.Sum(x => x.car_mc_allowance),
                                       pf_cc_amount = g.Sum(x => x.pf_cc_amount),
                                       conveyance = g.Sum(x => x.conveyance),
                                       houseR = g.Sum(x => x.houseR),
                                       special_allowance = g.Sum(x => x.special_allowance),


                                       arrear_basic = g.Sum(x => x.arrear_basic),
                                       arrear_house = g.Sum(x => x.arrear_house),
                                       arrear_conveyance = g.Sum(x => x.arrear_conveyance),
                                       arrear_car_mc_allowance = g.Sum(x => x.arrear_car_mc_allowance),
                                       arrear_leave_encashment = g.Sum(x => x.arrear_leave_encashment),

                                       incentive_q1 = g.Sum(x => x.incentive_q1),
                                       incentive_q2 = g.Sum(x => x.incentive_q2),
                                       incentive_q3 = g.Sum(x => x.incentive_q3),
                                       incentive_q4 = g.Sum(x => x.incentive_q4),
                                       sti = g.Sum(x => x.sti),
                                       one_time = g.Sum(x => x.one_time),
                                       training_allowance = g.Sum(x => x.training_allowance),
                                       gift = g.Sum(x => x.gift),
                                       tax_paid_by_company = g.Sum(x => x.tax_paid_by_company),
                                       festival_bonus = g.Sum(x => x.festival_bonus),
                                       bonus = g.Sum(x => x.bonus),
                                       total_arrear_allowance = g.Sum(x => x.total_arrear_allowance),
                                       leave_encashment = g.Sum(x => x.leave_encashment),
                                       long_service_award = g.Sum(x => x.long_service_award),

                                       totalA = g.Sum(x => x.totalA),

                                       ipad_or_mobile_bill = g.Sum(x => x.ipad_or_mobile_bill),
                                       modem_bill = g.Sum(x => x.modem_bill),

                                       ipad_bill = g.Sum(x => x.ipad_bill),
                                       mobile_bill = g.Sum(x => x.mobile_bill),

                                       lunch_support = g.Sum(x => x.lunch_support),
                                       others_deduction = g.Sum(x => x.others_deduction),
                                       tax_return_non_submission = g.Sum(x => x.tax_return_non_submission),
                                       pf_co_amount = g.Sum(x => x.pf_co_amount),
                                       monthly_tax = g.Sum(x => x.monthly_tax),
                                       income_tax = g.Sum(x => x.income_tax), // if upload tax
                                       totalD = g.Sum(x => x.totalD),

                                       netPay = g.Sum(x => x.netPay)

                                   }).ToList();

                e.DataSources.Add(new ReportDataSource("DataSet1", DataListMax));
            }
        }

        [PayrollAuthorize]
        public ActionResult CashSalaryReport()
        {
            ReportCashSalary ba = new ReportCashSalary();
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View(ba);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult CashSalaryReport(ReportCashSalary RC, string employee_category, DateTime SelectDate, FormCollection collection, string sButton)
        {
            var empNumber = new List<string>();
            var empIds = new List<int>();
            var EmpList = new List<ReportCashSalary>();
            RC.Day = SelectDate.Day;
            RC.Month = SelectDate.Month;
            RC.Year = SelectDate.Year;
            string year = RC.Year.ToString();
            RC.MonthName = DateUtility.MonthName(SelectDate.Month);
            DateTime dt = new DateTime(RC.Year, RC.Month, RC.Day);
            string SelectedDate = dt.ToString("MM/dd/yyyy");

            var discontinued_empIds = new List<int>();
            discontinued_empIds = dataContext.prl_employee_discontinue.Where(x => x.discontinue_date.Year == SelectDate.Year && x.discontinue_date.Month == SelectDate.Month && (x.with_salary == "Y" || x.without_salary == "N" || x.is_active == "Y")).Select(x => x.emp_id).ToList();

            EmpList = (from spd in dataContext.prl_salary_process_detail
                       join sp in dataContext.prl_salary_process on spd.salary_process_id equals sp.id
                       join emp in dataContext.prl_employee on spd.emp_id equals emp.id
                       join empD in dataContext.prl_employee_details on emp.id equals empD.emp_id
                       join bpd in dataContext.prl_bonus_process_detail on
                        new { Key1 = emp.id, Key2 = spd.salary_month.Month, Key3 = spd.salary_month.Year } equals new { Key1 = bpd.emp_id, Key2 = bpd.process_date.Month, Key3 = bpd.process_date.Year } into bon
                       from bonus in bon.DefaultIfEmpty()
                       join bp in dataContext.prl_bonus_process on bonus.bonus_process_id equals bp.id into fbon
                       from fbonus in fbon.DefaultIfEmpty()
                       join bn in dataContext.prl_bonus_name on fbonus.bonus_name_id equals bn.id into fbName
                       from fbonName in fbName.DefaultIfEmpty()
                       join ed in dataContext.prl_employee_discontinue on emp.id equals ed.emp_id into empDis
                       from empDiscon in empDis.DefaultIfEmpty()
                       //join cmb in dataContext.prl_company_bank on empD.employee_category equals cmb.account_category
                       where
                       spd.salary_month.Year == SelectDate.Year && spd.salary_month.Month == SelectDate.Month
                       && (string.IsNullOrEmpty(emp.account_no) || emp.account_no == "0" || discontinued_empIds.Contains(spd.emp_id))

                       select
                       new ReportCashSalary
                       {
                           empNo = emp.emp_no,
                           empName = emp.name,

                           department_id = empD.department_id,
                           sub_department_id = (int?)empD.sub_department_id ?? 0,
                           sub_sub_department_id = (int?)empD.sub_sub_department_id ?? 0,

                           department = (empD.department_id != null ? empD.department_id : 0) != 0 ? empD.prl_department.name : " ",
                           sub_department = (empD.sub_department_id != null ? empD.sub_department_id : 0) != 0 ? empD.prl_sub_department.name : " ",
                           sub_sub_department = (empD.sub_sub_department_id != null ? empD.sub_sub_department_id : 0) != 0 ? empD.prl_sub_sub_department.name : " ",

                           designation = empD.designation_id != 0 ? empD.prl_designation.name : " ",
                           salary_amount = (Math.Round((decimal)spd.total_allowance, 2) + (bonus.amount != null ? Math.Round((decimal)bonus.amount, 2) : 0) + Math.Round((decimal)spd.totla_arrear_allowance, 2)) - (Math.Round(spd.total_deduction, 2) + Math.Round(spd.total_monthly_tax, 2)),
                           heading = (bonus.amount != null || bonus.amount != 0) ? "Salary On Cash/ Cheque: " + RC.MonthName + " & Festival Bonus ( " + fbonName.name + " ) " + year + "" : "Salary On Cash/ Cheque: " + RC.MonthName + "-" + year + "",
                           remarks = "Salary on Cash/ Cheque",
                           payment_date = sp.payment_date,
                           Year = RC.Year,
                           Month = RC.Month,
                           MonthName = RC.MonthName,
                           monthYear = RC.Year,

                       }).ToList();

            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();
                string path = "";

                path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "CashSalaryReport.rdlc");

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

                ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                lr.DataSources.Add(rd);

                //// Prepare Parameters

                //ReportParameter[] para = new ReportParameter[2];
                //para[0] = new ReportParameter("employee_category", employee_category);
                //para[1] = new ReportParameter("selectedDate", SelectedDate);

                //// Pass Parameters for Local Report

                //lr.SetParameters(para);


                lr.SetParameters(new ReportParameter("selectedDate", dt.ToString("MM,yyy")));

                string reportType = "EXCELOPENXML";
                string mimeType;
                string encoding;
                string fileNameExtension = "xlsx";
                //string fileNameExtension = string.Format("{0}.{1}", "Report_SalarySheet", "xlsx");

                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>xlsx</OutputFormat>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    //"",
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                return File(renderedBytes, mimeType);
            }
            else
            {
                ModelState.AddModelError("", "No information found");
            }


            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View();
        }


        [PayrollAuthorize]
        public ActionResult MonthlyWorkerAllowances()
        {
            MonthlyWorkerAllowances mwa = new MonthlyWorkerAllowances();
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            DateTime nowDateTime = DateTime.Now;
            DateTime ProcessDate = new DateTime(nowDateTime.Year, nowDateTime.Month, 1);
            mwa.ProcessDate = ProcessDate;

            return View(mwa);
        }


        //[PayrollAuthorize]
        //public ActionResult CategoryWiseReport()
        //{
        //    MonthlyWorkerAllowances mwa = new MonthlyWorkerAllowances();
        //    ViewBag.Years = DateUtility.GetYears();
        //    ViewBag.Months = DateUtility.GetMonths();
        //    DateTime nowDateTime = DateTime.Now;
        //    DateTime ProcessDate = new DateTime(nowDateTime.Year, nowDateTime.Month, 1);
        //    mwa.ProcessDate = ProcessDate;

        //    return View(mwa);
        //}


        [PayrollAuthorize]
        public ActionResult MonthlyAllowanceReport()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult MonthlyAllowanceReport(ReportMonthlyAllowance rma, FormCollection collection, string sButton)
        {
            var monthlyAllowanceInfo = new ReportMonthlyAllowance();
            bool errorFound = false;
            var empIds = new List<int>();
            var ReportAllowanceList = new List<ReportMonthlyAllowance>();


            if (ModelState.IsValid)
            {
                var EmpList = (from ewa in dataContext.vw_empwisemonthlyallowances
                               where ewa.salary_month.Year == rma.Year && ewa.salary_month.Month == rma.Month
                               select new ReportMonthlyAllowance
                               {
                                   empId = ewa.id,
                                   empNo = ewa.emp_no,
                                   empName = ewa.name,
                                   designation = ewa.designation,
                                   office_staff_ot = ewa.office_staff_ot,
                                   factory_staff_ot = ewa.factory_staff_ot,
                                   shift_allowance = ewa.shift_allowance,
                                   officiating = ewa.officiating,
                                   mid_month_advance = ewa.advance,
                                   lda = ewa.lda,
                                   ramadan_allowance = ewa.ramadan_allowance,
                                   month_name = ewa.month_name,
                                   Month = ewa.salary_month.Month,
                                   Year = ewa.salary_month.Year,
                                   totalAllowance = ewa.office_staff_ot + ewa.factory_staff_ot + ewa.shift_allowance + ewa.officiating + ewa.advance + ewa.lda + ewa.ramadan_allowance
                               }).ToList();


                if (EmpList.Count > 0)
                {

                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "MonthlyAllowanceReport.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        ViewBag.Years = DateUtility.GetYears();
                        ViewBag.Months = DateUtility.GetMonths();
                        return View("MonthlyAllowanceReport");
                        //return View("Index");
                    }

                    DateTime dt = new DateTime(rma.Year, rma.Month, 1);

                    ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                    lr.DataSources.Add(rd);
                    lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));
                    string reportType = "EXCELOPENXML";
                    string mimeType;
                    string encoding;
                    string fileNameExtension = "xlsx";
                    //string fileNameExtension = string.Format("{0}.{1}", "Report_MonthlyAllowances", "xlsx");


                    string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>xlsx</OutputFormat>" +
                    "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        //"",
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);

                    //Response.AddHeader("content-disposition", "attachment; filename=Report_MonthlyAllowances.xlsx");

                    return File(renderedBytes, mimeType);

                }
                else
                {
                    errorFound = true;
                    ModelState.AddModelError("", "No information found");
                }
            }

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            //monthlyAllowanceInfo.Month = rma.Month;
            //monthlyAllowanceInfo.Year = rma.Year;
            //monthlyAllowanceInfo.month_name = DateUtility.MonthName(rma.Month);
            return View(monthlyAllowanceInfo);
        }


        //[PayrollAuthorize]
        //[HttpPost]
        //public ActionResult WorkerPay(int? empid, FormCollection collection, string sButton, ReportWorkerpay rp)
        //{
        //    bool errorFound = false;
        //    var res = new OperationResult();
        //    var workerpayInfo = new ReportWorkerpay();

        //    try
        //    {
        //        if (sButton == "Search")
        //        {
        //            if (empid == null && string.IsNullOrEmpty(collection["Emp_No"]))
        //            {
        //                errorFound = true;
        //                ModelState.AddModelError("", "Please select an employee or put employee no.");
        //            }
        //            else
        //            {
        //                var Emp = new Employee();
        //                if (empid != null)
        //                {
        //                    var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
        //                    Emp = Mapper.Map<Employee>(_empD);
        //                }
        //                else
        //                {
        //                    var _empD = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"]);
        //                    if (_empD == null)
        //                    {
        //                        errorFound = true;
        //                        ModelState.AddModelError("", "Threre is no information for the given employee no.");
        //                    }
        //                    else
        //                    {
        //                        Emp = Mapper.Map<Employee>(_empD);
        //                    }
        //                }

        //                var allowancePD = dataContext.prl_workers_allowances.FirstOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month);
        //                if (allowancePD == null)
        //                {
        //                    errorFound = true;
        //                    ModelState.AddModelError("", "Allowance has not been processed for the given data.");
        //                }


        //                if (!errorFound)
        //                {
        //                    var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

        //                    var allowances = dataContext.prl_workers_allowances.Where(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month).Select(p => new WorkerAllowances
        //                        {
        //                            head = p.prl_allowance_name.prl_allowance_head.name,
        //                            value = p.amount
        //                        }).ToList();
        //                    //ViewBag.Allowance = allowances;
        //                    var deductions = dataContext.prl_salary_deductions.Where(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month).Select(p => new AllowanceDeduction
        //                        {
        //                            head = p.prl_deduction_name.deduction_name,
        //                            value = p.amount
        //                        }).ToList();
        //                    //ViewBag.Deduction = deductions;

        //                    workerpayInfo.empName = Emp.name;
        //                    workerpayInfo.empNo = Emp.emp_no;
        //                    if (Emp.bank_id == null)
        //                    {
        //                        workerpayInfo.paymentMode = "Cash";
        //                    }
        //                    else
        //                    {
        //                        workerpayInfo.paymentMode = "Bank Transfer";
        //                        workerpayInfo.bank = Emp.prl_bank.bank_name;
        //                        workerpayInfo.accNo = Emp.account_no;
        //                    }
        //                    decimal total_earnings = this.dataContext.prl_workers_allowances.Where(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month).Sum(z => z.amount);
        //                    workerpayInfo.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";
        //                    workerpayInfo.department = empD.department_id != 0 ? empD.prl_department.name : " ";
        //                    workerpayInfo.basicSalary = Convert.ToDecimal(empD.basic_salary);
        //                    workerpayInfo.totalEarnings = total_earnings;
        //                    //workerpayInfo.totalDeduction = this.dataContext.prl_workers_allowances.Where(x =>  x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month).Sum(z => z.amount);
        //                    workerpayInfo.totalDeduction = 0;
        //                    workerpayInfo.netPay = 0;


        //                    /****************/
        //                    string reportType = "PDF";

        //                    LocalReport lr = new LocalReport();
        //                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "WorkerPay.rdlc");
        //                    if (System.IO.File.Exists(path))
        //                    {
        //                        lr.ReportPath = path;
        //                    }
        //                    else
        //                    {
        //                        return View("Index");
        //                    }

        //                    var reportData = new ReportWorkerpay();
        //                    var empDlist = new List<ReportWorkerpay>();
        //                    reportData.eId = Emp.id;
        //                    reportData.empNo = Emp.emp_no;
        //                    reportData.empName = Emp.name;
        //                    reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";
        //                    reportData.department = empD.department_id != 0 ? empD.prl_department.name : "N/A";
        //                    reportData.division = empD.division_id != 0 ? empD.prl_division.name : "N/A";
        //                    reportData.processId = allowancePD.allowance_process_id ;
        //                    reportData.basicSalary = Convert.ToDecimal(empD.basic_salary);
        //                    reportData.totalEarnings = total_earnings;
        //                    reportData.monthYear = allowancePD.salary_month;

        //                    if (Emp.bank_id == null)
        //                    {
        //                        reportData.paymentMode = "Cash";
        //                        reportData.bank = "";
        //                        reportData.accNo = "";
        //                    }
        //                    else
        //                    {
        //                        reportData.paymentMode = "Bank Transfer";
        //                        reportData.bank = Emp.prl_bank.bank_name;
        //                        reportData.accNo = Emp.account_no;
        //                    }

        //                    empDlist.Add(reportData);

        //                    ReportDataSource rd = new ReportDataSource("DataSet1", empDlist);
        //                    lr.DataSources.Add(rd);
        //                    lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_WpSubreportProcessing);

        //                    string mimeType;
        //                    string encoding;
        //                    string fileNameExtension;

        //                    string deviceInfo =
        //                    "<DeviceInfo>" +
        //                    "<OutputFormat>PDF</OutputFormat>" +
        //                    "</DeviceInfo>";

        //                    Warning[] warnings;
        //                    string[] streams;
        //                    byte[] renderedBytes;

        //                    renderedBytes = lr.Render(
        //                        reportType,
        //                        deviceInfo,
        //                        out mimeType,
        //                        out encoding,
        //                        out fileNameExtension,
        //                        out streams,
        //                        out warnings);

        //                    return File(renderedBytes, mimeType);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //    ViewBag.Years = DateUtility.GetYears();
        //    ViewBag.Months = DateUtility.GetMonths();
        //    ViewBag.Allowance = new List<AllowanceDeduction>();
        //    ViewBag.Deduction = new List<AllowanceDeduction>();

        //    workerpayInfo.Month = rp.Month;
        //    workerpayInfo.Year = rp.Year;
        //    workerpayInfo.MonthName = DateUtility.MonthName(rp.Month);

        //    return View(workerpayInfo);
        //}




        private string GetRowCellValue(ISheet sheet, int row, string[] properties, string propertyName, string propertyType)
        {
            string cellValue = string.Empty;

            if (sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)) != null)
            {
                CellType cellType = sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).CellType;
                if (propertyType == "NumericCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).NumericCellValue).Trim();
                }
                else if (propertyType == "BooleanCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).BooleanCellValue).Trim();
                }
                else if (propertyType == "DateCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).DateCellValue).Trim();
                }
                else if (propertyType == "StringCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName))).Trim();
                }
                else if (propertyType == "ErrorCellValue")
                {
                    cellValue = Convert.ToString(sheet.GetRow(row).GetCell(GetColumnIndex(properties, propertyName)).ErrorCellValue).Trim();
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

    }
}
