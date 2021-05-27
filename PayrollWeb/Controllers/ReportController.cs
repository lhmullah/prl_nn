using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.linde.DataContext;
using AutoMapper;
using com.linde.Model;
using Ninject.Infrastructure.Language;
using PayrollWeb.CustomSecurity;
using PayrollWeb.ViewModels;
using PayrollWeb.Service;
using System.Collections.ObjectModel;
using PayrollWeb.Utility;
using OfficeOpenXml;
using System.Web.Caching;
using PagedList;
using System.Net.Http;
using System.Net;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.IO.Compression;

namespace PayrollWeb.Controllers
{
    public class ReportController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public ReportController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult PaySlip()
        {

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Allowance = new List<AllowanceDeduction>();
            ViewBag.Deduction = new List<AllowanceDeduction>();
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult PaySlip(int? empid, FormCollection collection, string sButton, ReportPayslip rp)
        {

            bool errorFound = false;
            var res = new OperationResult();
            var payslipInfo = new ReportPayslip();

            try
            {
                if (sButton == "Search")
                {
                    if (empid == null && string.IsNullOrEmpty(collection["Emp_No"]))
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Please select an employee or put employee no.");
                    }
                    else
                    {
                        var Emp = new Employee();
                        if (empid != null)
                        {
                            var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                            Emp = Mapper.Map<Employee>(_empD);
                        }
                        else
                        {
                            var _empD = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"]);
                            if (_empD == null)
                            {
                                errorFound = true;
                                ModelState.AddModelError("", "Threre is no information for the given employee no.");
                            }
                            else
                            {
                                Emp = Mapper.Map<Employee>(_empD);
                            }
                        }
                        var salaryPD = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month);
                        if (salaryPD == null)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Salary has not been processed for the given data.");
                        }

                        if (!errorFound)
                        {
                            var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                            var salaryProcess = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rp.Year && x.salary_month.Month == rp.Month);

                            var allowances = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == salaryProcess.salary_process_id
                                && x.emp_id == salaryProcess.emp_id && x.salary_month.Month == salaryProcess.salary_month.Month).Select(p => new AllowanceDeduction
                                {
                                    head = p.prl_allowance_name.prl_allowance_head.name,
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

                            var reportData = new ReportPayslip();
                            var empDlist = new List<ReportPayslip>();
                            reportData.eId = Emp.id;
                            reportData.empNo = Emp.emp_no;
                            reportData.empName = Emp.name;
                            reportData.processId = salaryPD.salary_process_id;
                            reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";
                            reportData.department = empD.department_id != 0 ? empD.prl_department.name : " ";

                            reportData.job_level = empD.job_level_id != 0 ? empD.prl_job_level.title : " ";
                            reportData.cost_centre = empD.cost_centre_id != 0 ? empD.prl_cost_centre.cost_centre_name : " ";

                            reportData.joining_date = Emp.joining_date;
                            reportData.no_of_days_in_month = DateTime.DaysInMonth(salaryProcess.salary_month.Year, salaryProcess.salary_month.Month);
                            reportData.no_of_working_days = salaryPD.calculation_for_days;


                            reportData.salary_date = salaryProcess.salary_month;
                            reportData.basicSalary = Math.Round(Convert.ToDecimal(empD.basic_salary), 2, MidpointRounding.AwayFromZero);
                            reportData.totalEarnings = Math.Round(salaryPD.total_allowance, 2, MidpointRounding.AwayFromZero);
                            reportData.totalDeduction = Math.Round(Convert.ToDecimal(salaryPD.total_deduction), 2, MidpointRounding.AwayFromZero);
                            reportData.netPay = Math.Round(Convert.ToDecimal(salaryPD.net_pay), 2, MidpointRounding.AwayFromZero);



                            reportData.tax = Math.Round(Convert.ToDecimal(salaryPD.total_monthly_tax), 2, MidpointRounding.AwayFromZero);
                            if (reportData.tax > 0)
                            {
                                reportData.totalDeduction = Math.Round(Convert.ToDecimal(reportData.totalDeduction + reportData.tax), 2, MidpointRounding.AwayFromZero);
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
                            lr.DataSources.Add(rd);
                            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);

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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Allowance = new List<AllowanceDeduction>();
            ViewBag.Deduction = new List<AllowanceDeduction>();

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
                var allowances = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == pId
                                && x.emp_id == eId).Select(p => new AllowanceDeduction
                                {
                                    head = p.prl_allowance_name.allowance_name,
                                    value = p.amount
                                }).ToList();

                AllowanceDeduction AD = new AllowanceDeduction();
                AD.head = "Basic Salary";
                AD.value = Convert.ToDecimal(e.Parameters["basicSlr"].Values[0]);
                allowances.Insert(0, AD);

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
                    int i = 2;
                    foreach (var item in bonus)
                    {
                        AllowanceDeduction AD3 = new AllowanceDeduction();

                        AD3.head = item.head;
                        AD3.value = item.value;

                        allowances.Insert(i, AD3);
                    }
                }

                e.DataSources.Add(new ReportDataSource("DataSet1", allowances));
            }
            else
            {
                var deductions = dataContext.prl_salary_deductions.Where(x => x.salary_process_id == pId
                            && x.emp_id == eId).Select(p => new AllowanceDeduction
                            {
                                head = p.prl_deduction_name.deduction_name,
                                value = p.amount
                            }).ToList();
                decimal tax = Convert.ToDecimal(e.Parameters["tax"].Values[0]);
                if (tax > 0)
                {
                    AllowanceDeduction AD = new AllowanceDeduction();
                    AD.head = "Income Tax";
                    AD.value = tax;
                    deductions.Insert(0, AD);
                }

                
                e.DataSources.Add(new ReportDataSource("DataSet1", deductions));
            }

        }

        [PayrollAuthorize]
        public ActionResult BankAdvice()
        {
            ReportBankAdvice ba = new ReportBankAdvice();

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            var _bankInfo = dataContext.prl_bank.ToList();
            ViewBag.Banks = _bankInfo;
            return View(ba);
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

                int bnkId = Convert.ToInt32(collection["bnkId"]);
                int mnth = Convert.ToInt32(collection["mnth"]);
                int yr = Convert.ToInt32(collection["yr"]);
                var EmpList = (from emp in dataContext.prl_employee
                               join spd in dataContext.prl_salary_process_detail on emp.id equals spd.emp_id
                               join bank in dataContext.prl_bank on emp.bank_id equals bank.id
                               where bank.id == bnkId && spd.salary_month.Month == mnth && spd.salary_month.Year == yr
                               select new
                               {
                                   emp.emp_no,
                                   emp.name,
                                   emp.official_contact_no,
                               }).ToList();


                int totalRecords = EmpList.Count();
                var employees = EmpList.Skip(pageIndex).Take(pageSize);

                var aaData = employees.Select(x => new string[] { x.emp_no, x.name, "department", x.official_contact_no });

                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }

        //[PayrollAuthorize]
        //[HttpPost]
        //public ActionResult BankAdvice(ReportBankAdvice ba, FormCollection collection)
        //{

        //    var empNumber = new List<string>();
        //    var empIds = new List<int>();
        //    var EmpList = new List<ReportModel>();
        //    if (collection.Get("empGroup") == "all")
        //    {
        //        EmpList = (from emp in dataContext.prl_employee
        //                   join spd in dataContext.prl_salary_process_detail on emp.id equals spd.emp_id
        //                   where spd.salary_month.Year == ba.Year && spd.salary_month.Month == ba.Month
        //                   select new ReportModel
        //                   {
        //                       empNo = emp.emp_no,
        //                       name = emp.name,
        //                       accNo = emp.account_no,
        //                       netPay = spd.net_pay
        //                   }).ToList();
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrWhiteSpace(ba.SelectedEmployees))
        //        {
        //            empNumber = ba.SelectedEmployees.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //            empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
        //                    .Select(x => x.id).ToList();

        //            EmpList = (from emp in dataContext.prl_employee
        //                       join spd in dataContext.prl_salary_process_detail on emp.id equals spd.emp_id
        //                       where empNumber.Contains(emp.emp_no) && spd.salary_month.Year == ba.Year && spd.salary_month.Month == ba.Month
        //                       select new ReportModel
        //                       {
        //                           empNo = emp.emp_no,
        //                           name = emp.name,
        //                           accNo = emp.account_no,
        //                           netPay = spd.net_pay
        //                       }).ToList();
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "No employee selected.");
        //        }
        //    }

        //    if (EmpList.Count > 0)
        //    {
        //        LocalReport lr = new LocalReport();
        //        string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "BankAdvice.rdlc");
        //        if (System.IO.File.Exists(path))
        //        {
        //            lr.ReportPath = path;
        //        }
        //        else
        //        {
        //            return View("Index");
        //        }

        //        ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
        //        lr.DataSources.Add(rd);
        //        lr.SetParameters(new ReportParameter("monthYr", DateTime.Today.ToString("MM,yyy")));
        //        string reportType = "PDF";
        //        string mimeType;
        //        string encoding;
        //        string fileNameExtension;



        //        string deviceInfo =

        //                    "<DeviceInfo>" +

        //        "  <OutputFormat>reportType</OutputFormat>" +

        //        "  <PageWidth>8.5in</PageWidth>" +

        //        "  <PageHeight>11in</PageHeight>" +

        //        "  <MarginTop>0.5in</MarginTop>" +

        //        "  <MarginLeft>1in</MarginLeft>" +

        //        "  <MarginRight>1in</MarginRight>" +

        //        "  <MarginBottom>0.5in</MarginBottom>" +

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


        //        return File(renderedBytes, mimeType);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "No information found");
        //    }

        //    ViewBag.Years = DateUtility.GetYears();
        //    ViewBag.Months = DateUtility.GetMonths();

        //    var _bankInfo = dataContext.prl_bank.ToList();
        //    ViewBag.Banks = _bankInfo;
        //    return View();
        //}

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

                var aaData = employees.Select(x => new string[] { x.empNo, x.empName, "cost_center", x.email });

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
        public ActionResult SalarySheet(ReportSalarySheet SS, FormCollection collection, string sButton)
        {
            var empNumber = new List<string>();
            var empIds = new List<int>();
            var EmpList = new List<ReportSalarySheet>();
            
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
                               no_of_days_lwp = esd.no_of_days_lwp != 0 ? esd.no_of_days_lwp : 0,
                               loan_start_date = esd.loan_start_date,
                               loan_end_date = esd.loan_end_date,
                               principal_amount = esd.principal_amount != 0 ? esd.principal_amount : 0,
                               no_of_installment = esd.no_of_installment != 0 ? esd.no_of_installment : 0,
                               cost_centre_name = esd.cost_centre_name != null ? esd.cost_centre_name : "",

                               basic_salary = esd.basic_salary,
                               bta = esd.bta == null ? 0 : esd.bta,
                               car_mc_allowance = esd.car_mc_allowance == null ? 0 : esd.car_mc_allowance,
                               pf_cc_amount = esd.pf_cc_amount,
                               conveyance = esd.conveyance == null ? 0 : esd.conveyance,
                               houseR = esd.house == null ? 0 : esd.house,
                               special_allowance = esd.special_allowance != null ? esd.special_allowance : 0,

                               incentive_q1 = esd.incentive_q1 == null ? 0 : esd.incentive_q1,
                               incentive_q2 = esd.incentive_q2 == null ? 0 : esd.incentive_q2,
                               incentive_q3 = esd.incentive_q3 == null ? 0 : esd.incentive_q3,
                               incentive_q4 = esd.incentive_q4 == null ? 0 : esd.incentive_q4,
                               sti = esd.sti != null ? esd.sti : 0,
                               tax_paid_by_company = esd.tax_paid_by_company != null ? esd.tax_paid_by_company : 0,
                               festival_bonus = esd.festival_bonus == null ? 0 : esd.festival_bonus,
                               total_arrear_allowance = esd.total_arrear_allowance == null ? 0 : esd.total_arrear_allowance,
                               is_discontinued = esd.is_discontinued,
                               totalA = esd.total_allowance == null ? 0 : esd.total_allowance,


                               ipad_or_mobile_bill = esd.ipad_or_mobile_bill == null ? 0 : esd.ipad_or_mobile_bill,

                               lunch_support = esd.lunch_support == null ? 0 : esd.lunch_support,
                               pf_co_amount = esd.pf_co_amount == null ? 0 : esd.pf_co_amount,
                               monthly_tax = esd.monthly_tax == null ? 0 : esd.monthly_tax,
                               income_tax = esd.income_tax == null ? 0 : esd.income_tax, // if upload tax
                               totalD = esd.total_deduction == null ? 0 : esd.total_deduction,
                               netPay = esd.net_pay == null ? 0 : esd.net_pay

                           }).ToList();

            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();

                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "SalarySheetReport.rdlc");
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
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View();
        }

        [PayrollAuthorize]
        public ActionResult SalaryChange()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult SalaryChange(ReportModel reportModel, string sButton)
        {
            var slrChange = (from slr in dataContext.prl_salary_review
                             join emp in dataContext.prl_employee on slr.emp_id equals emp.id
                             where slr.created_date.Value.Month == reportModel.Month && slr.created_date.Value.Year == reportModel.Year
                             select new ReportSalaryChange
                             {
                                 eId = emp.id,
                                 empId = emp.emp_no,
                                 empName = emp.name,
                                 grade = "",
                                 entrydate = slr.created_date.Value,
                                 effectiveFrom = slr.effective_from.Value,
                                 oldbasic = slr.current_basic,
                                 newBasic = slr.new_basic,
                                 reason = slr.increment_reason
                             }).ToList();

            if (slrChange.Count > 0)
            {
                var data = new List<ReportSalaryChange>();
                foreach (var item in slrChange)
                {
                    //item.grade = dataContext.prl_employee_details.Where(p => p.emp_id == item.eId).OrderByDescending(x => x.id).First().prl_grade.grade;
                    data.Add(item);
                }

                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "SalaryChange.rdlc");
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

                DateTime dt = new DateTime(reportModel.Year, reportModel.Month, 1);

                ReportDataSource rd = new ReportDataSource("DataSet1", slrChange);
                lr.DataSources.Add(rd);
                lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                if (sButton == "To Excel")
                {
                    reportType = "Excel";
                    //fileNameExtension = string.Format("{0}.{1}", "ExportToExcel", "xlsx");
                }


                string deviceInfo =

                            "<DeviceInfo>" +

                "  <OutputFormat>reportType</OutputFormat>" +

                "  <PageWidth>11in</PageWidth>" +

                "  <PageHeight>8.5in</PageHeight>" +

                "  <MarginTop>0.5in</MarginTop>" +

                "  <MarginLeft>1in</MarginLeft>" +

                "  <MarginRight>1in</MarginRight>" +

                "  <MarginBottom>0.5in</MarginBottom>" +

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
            else
            {
                ModelState.AddModelError("", "No information found");
            }

            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(reportModel);
        }
        /*
                [PayrollAuthorize]
                public ActionResult Bonus(string MnthAndYr)
                {
                    var Rb = new ReportBonus();
            
                    var years = DateUtility.GetYears();
                    string yrs = Convert.ToString(years[0]);
                    ViewBag.Years = years;
                    ViewBag.Months = DateUtility.GetMonths();

                    var bonus = new List<ReportBonus>() ;
                    if (MnthAndYr != null)
                    {
                        string[] MnYr = MnthAndYr.Split(',');

                        string yr = MnYr[0];
                        string mnth = MnYr[1];

                        bonus = (from bp in dataContext.prl_bonus_process
                                 join bn in dataContext.prl_bonus_name on bp.bonus_name_id equals bn.id
                                 where bp.month == mnth && bp.year == yr
                                 select new ReportBonus
                                 {
                                     BonusId = bp.bonus_name_id,
                                     BonusName = bn.name
                                 }).Distinct().ToList();
                        Rb.Month = mnth;
                        Rb.Year = yr;
                    }
                    else
                    {
                        bonus = (from bp in dataContext.prl_bonus_process
                                 join bn in dataContext.prl_bonus_name on bp.bonus_name_id equals bn.id
                                 where bp.month == "1" && bp.year == yrs
                                 select new ReportBonus
                                 {
                                     BonusId = bp.bonus_name_id,
                                     BonusName = bn.name
                                 }).Distinct().ToList();
                    }
                    if (bonus.Count == 0)
                    {
                        ReportBonus rb = new ReportBonus();
                        rb.BonusId = -1;
                        rb.BonusName = "--- No Bonus ---";

                        bonus.Add(rb);
                    }
                    else
                    {
                        ReportBonus rb = new ReportBonus();
                        rb.BonusId = 0;
                        rb.BonusName = "All";

                        bonus.Add(rb);
                    }
                    ViewBag.BonusName = bonus;

                    return View(Rb);
                }*/

        /*
                [PayrollAuthorize]
                [HttpPost]
                public ActionResult Bonus(ReportBonus RB)
                {
                    var bonus = new List<ReportBonus>();

                    if (RB.BonusId == -1)
                    {
                        ModelState.AddModelError("", "No information found");
                    }
                    else if (RB.BonusId == 0) // All
                    {
                        bonus = (from bp in dataContext.prl_bonus_process
                                 join bn in dataContext.prl_bonus_name on bp.bonus_name_id equals bn.id
                                 join bpd in dataContext.prl_bonus_process_detail on bp.id equals bpd.bonus_process_id
                                 join emp in dataContext.prl_employee on bpd.emp_id equals emp.id
                                 where bp.month == RB.Month && bp.year == RB.Year
                                 select new ReportBonus
                                 {
                                     eId = emp.id,
                                     empId = emp.emp_no,
                                     empName = emp.name,
                                     grade = "",
                                     newBasic = 0,
                                     BonusName = bn.name,
                                     bonus = bpd.amount
                                 }).ToList();
                    }
                    else
                    {
                        bonus = (from bp in dataContext.prl_bonus_process
                                 join bn in dataContext.prl_bonus_name on bp.bonus_name_id equals bn.id
                                 join bpd in dataContext.prl_bonus_process_detail on bp.id equals bpd.bonus_process_id
                                 join emp in dataContext.prl_employee on bpd.emp_id equals emp.id
                                 where bp.month == RB.Month && bp.year == RB.Year && bn.id == RB.BonusId
                                 select new ReportBonus
                                 {
                                     eId=emp.id,
                                     empId = emp.emp_no,
                                     empName = emp.name,
                                     grade = "",
                                     newBasic = 0,
                                     BonusName = bn.name,
                                     bonus = bpd.amount
                                 }).ToList();
                    }

                    if (bonus.Count > 0)
                    {
                        var data = new List<ReportBonus>();
                        foreach (var item in bonus)
                        {
                            item.grade = dataContext.prl_employee_details.Where(p => p.emp_id == item.eId).OrderByDescending(x => x.id).First().prl_grade.grade;
                            item.bonus = dataContext.prl_employee_details.Where(p => p.emp_id == item.eId).OrderByDescending(x => x.id).First().basic_salary;
                            data.Add(item);
                        }

                        LocalReport lr = new LocalReport();
                        string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "Bonus.rdlc");
                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                        else
                        {
                            return View("Index");
                        }
                        ReportDataSource rd = new ReportDataSource("DataSet1", data);
                        lr.DataSources.Add(rd);

                        string reportType = "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;
                        //if (sButton == "To Excel")
                        //{
                        //    reportType = "Excel";
                        //    //fileNameExtension = string.Format("{0}.{1}", "ExportToExcel", "xlsx");
                        //}

                        string deviceInfo =

                                    "<DeviceInfo>" +

                        "  <OutputFormat>reportType</OutputFormat>" +

                        "  <PageWidth>11in</PageWidth>" +

                        "  <PageHeight>8.5in</PageHeight>" +

                        "  <MarginTop>0.5in</MarginTop>" +

                        "  <MarginLeft>1in</MarginLeft>" +

                        "  <MarginRight>1in</MarginRight>" +

                        "  <MarginBottom>0.5in</MarginBottom>" +

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
                    else
                    {
                        ModelState.AddModelError("", "No information found");
                    }


                    ViewBag.Years = DateUtility.GetYears();
                    ViewBag.Months = DateUtility.GetMonths();
                    if (bonus.Count == 0)
                    {
                        ReportBonus rb = new ReportBonus();
                        rb.BonusId = -1;
                        rb.BonusName = "--- No Bonus ---";

                        bonus.Add(rb);
                    }
                    else
                    {
                        ReportBonus rb = new ReportBonus();
                        rb.BonusId = -1;
                        rb.BonusName = "All";

                        bonus.Add(rb);
                    }
                    ViewBag.BonusName = bonus;
                    return View(RB);
                }*/

        [HttpPost]
        public ActionResult GetEmployeesForAIT(FormCollection collection)
        {
            try
            {
                //string incomeYr = "2019-2020";
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

                if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = "";
                }

                dataContext.prl_certificate_upload.Where(x => x.file_path.Contains("AIT")).ToList();
                var employees = (
                                from emp in dataContext.prl_employee
                                join cu in dataContext.prl_certificate_upload
                                on emp.id equals cu.emp_id into eGroup2
                                from e in eGroup2.DefaultIfEmpty()
                                where e.file_path.Contains("AIT")
                                && emp.emp_no.Contains(searchText)
                                select new { emp.id, emp.emp_no, emp.name, emp.email }).ToList().Distinct();

                int totalRecords = employees.Count();

                var aaData = employees.Select(x => new string[] { x.emp_no, x.name, x.email });

                //  
                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);
            }

            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }

        [HttpPost]
        public ActionResult GetEmployeesForTRA(FormCollection collection)
        {
            try
            {
                //string incomeYr = "2019-2020";
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

                if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = "";
                }

                dataContext.prl_certificate_upload.Where(x => x.file_path.Contains("TRA")).ToList();
                var employees = (
                                from emp in dataContext.prl_employee
                                join cu in dataContext.prl_certificate_upload
                                on emp.id equals cu.emp_id into eGroup2
                                from e in eGroup2.DefaultIfEmpty()
                                where e.file_path.Contains("TRA")
                                && emp.emp_no.Contains(searchText)
                                select new { emp.id, emp.emp_no, emp.name, emp.email }).ToList().Distinct();

                int totalRecords = employees.Count();

                var aaData = employees.Select(x => new string[] { x.emp_no, x.name, x.email });

                //  
                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);
            }

            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }

        public ActionResult AIT()
        {
            ViewBag.loadIncomeYear = dataContext.prl_fiscal_year.ToList();
            return View();
        }

        public ActionResult GetAITCertificateInfo(int selectYr)
        {
            var incomeYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == selectYr).fiscal_year;
            var res = new OperationResult();
            ViewBag.loadIncomeYear = dataContext.prl_fiscal_year.ToList();
            var resultList = new List<CertificateUpload>();

            try
            {
                resultList = (from emp in dataContext.prl_employee
                              join cu in dataContext.prl_certificate_upload
                              on emp.id equals cu.emp_id into eGroup2
                              from e in eGroup2.DefaultIfEmpty()
                              where e.file_path.Contains("AIT") && e.income_year == incomeYr
                              select new CertificateUpload
                              {
                                  ID = e.id,
                                  Emp_ID = emp.emp_no,
                                  Name = emp.name,
                                  Email = emp.email,
                                  No_of_Cars = e.number_of_car,
                                  Is_Appropved = e.is_approved == null || e.is_approved == false ? "No" : "Yes",
                                  Amount = e.amount
                              }).Distinct().ToList();


                if (resultList.Count > 0)
                {

                    var json = JsonConvert.SerializeObject(resultList);

                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //return RedirectToAction("AIT");
                    return Json("");
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult AITDownload(FormCollection model, Certificates vmc, string btn)
        {
            bool errorFound = false;
            //var incomeYr = selectYr;
            //var incomeYr = model["Income Year"];
            var incomeYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == vmc.fiscalYr_id).fiscal_year;
            var res = new OperationResult();

            try
            {
                if (btn == "Download in Excel")
                {
                    var resultList = new List<CertificateUpload>();


                    resultList = (from emp in dataContext.prl_employee
                                  join cu in dataContext.prl_certificate_upload
                                  on emp.id equals cu.emp_id into eGroup2
                                  from e in eGroup2.DefaultIfEmpty()
                                  where e.file_path.Contains("AIT") && e.income_year == incomeYr
                                  select new CertificateUpload
                                  {
                                      ID = e.id,
                                      Emp_ID = emp.emp_no,
                                      Name = emp.name,
                                      Email = emp.email,
                                      No_of_Cars = e.number_of_car,
                                      Is_Appropved = e.is_approved == null || e.is_approved == false ? "No" : "Yes",
                                      Amount = e.amount,
                                      Submission_Date = e.created_date
                                  }).Distinct().ToList();


                    if (resultList.Count > 0)
                    {
                        LocalReport lr = new LocalReport();

                        string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "AITSheetReport.rdlc");
                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                        else
                        {
                            return View("AIT");
                        }

                        //DateTime dt = new DateTime(SS.Year, SS.Month, 1);

                        ReportDataSource rd = new ReportDataSource("DataSet1", resultList);
                        lr.DataSources.Add(rd);
                        lr.SetParameters(new ReportParameter("incomeYr", incomeYr));
                        string reportType = "EXCELOPENXML";
                        //string contentType = "application/vnd.ms-excel";
                        string mimeType;
                        string encoding;
                        //string fileNameExtension = "xlsx";
                        string fileNameExtension = string.Format("{0}.{1}", "AITSheet", "xlsx");

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

                        ViewBag.loadIncomeYear = dataContext.prl_fiscal_year.ToList();

                        return File(renderedBytes, mimeType, "AITSheet.xlsx");
                    }
                    else
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No data found, Please select accurate fiscal year.");
                    }
                }

                else if (btn == "Certificate Download")
                {
                    var AllEmployee = false;
                    var empNumber = new List<string>();
                    var empIds = new List<int>();
                    List<string> FileUrls = new List<string>();
                    var certificateLists = new List<CertificateUploadVM>();

                    if (string.IsNullOrWhiteSpace(vmc.SelectedEmployeesOnly))
                    {
                        AllEmployee = true;
                    }

                    if (AllEmployee == false)
                    {
                        empNumber = vmc.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                                .Select(x => x.id).ToList();

                        var lstEmp = dataContext.prl_certificate_upload
                                 .Where(x => empIds.Contains(x.emp_id) && x.income_year == incomeYr && x.certificat_type == "AIT").ToList();
                        certificateLists = Mapper.Map<List<CertificateUploadVM>>(lstEmp);
                    }
                    else
                    {
                        var lstEmp = dataContext.prl_certificate_upload
                                .Where(x => x.income_year == incomeYr && x.certificat_type == "AIT").ToList();
                        certificateLists = Mapper.Map<List<CertificateUploadVM>>(lstEmp);

                    }

                    if (certificateLists.Count > 0)
                    {
                        foreach (var item in certificateLists)
                        {
                            FileUrls.Add(item.file_path);
                            //FileUrl= FileUrl.Replace(@"\", "/");
                            //byte[] FileBytes = System.IO.File.ReadAllBytes(FileUrl);

                            //return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
                            //return File(certificates.file_path, "application/pdf");
                        }

                        var filesCol = FileUrls;

                        using (var memoryStream = new MemoryStream())
                        {
                            using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            {
                                for (int i = 0; i < filesCol.Count; i++)
                                {
                                    string fileName = filesCol[i].Split('\\').Last();
                                    ziparchive.CreateEntryFromFile(filesCol[i], fileName);
                                }
                            }

                            return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
                        }
                    }
                }

                else if (btn == "Approved All")
                {
                    var certificatList = (from c in dataContext.prl_certificate_upload
                                          where c.income_year == incomeYr && c.certificat_type == "AIT" && c.is_approved == false
                                          select c).ToList();

                    if (certificatList.Count > 0)
                    {
                        foreach (var item in certificatList)
                        {
                            item.is_approved = true;
                            item.updated_by = User.Identity.Name;
                            item.updated_date = DateTime.Now;
                        }

                        if (dataContext.SaveChanges() > 0)
                        {
                            res.IsSuccessful = true;
                            res.Message = "Successfully Approved All Applied AIT, will be effected in next tax process";
                            TempData.Add("msg", res);
                            return RedirectToAction("AIT");
                        }
                    }
                    else
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No data found, Please select accurate fiscal year.");
                    }

                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return RedirectToAction("AIT");
        }

        public ActionResult TaxReturn()
        {
            ViewBag.loadIncomeYear = dataContext.prl_fiscal_year.ToList();
            return View();
        }

        public ActionResult GetTRACertificateInfo(int selectYr)
        {
            var incomeYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == selectYr).fiscal_year;
            var res = new OperationResult();
            ViewBag.loadIncomeYear = dataContext.prl_fiscal_year.ToList();

            var resultList = new List<CertificateUpload>();


            resultList = (from emp in dataContext.prl_employee
                          join cu in dataContext.prl_certificate_upload
                          on emp.id equals cu.emp_id into eGroup2
                          from e in eGroup2.DefaultIfEmpty()
                          where e.file_path.Contains("TRA") && e.income_year == incomeYr
                          select new CertificateUpload
                          {
                              ID = e.id,
                              Emp_ID = emp.emp_no,
                              Name = emp.name,
                              Email = emp.email,
                              Amount = e.amount,
                              Is_Appropved = e.is_approved == null || e.is_approved == false ? "No" : "Yes",
                          }).Distinct().ToList();


            if (resultList.Count > 0)
            {

                var json = JsonConvert.SerializeObject(resultList);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //return RedirectToAction("AIT");
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult TRADownload(FormCollection model, Certificates vmc, string btn)
        {
            bool errorFound = false;
            //var incomeYr = selectYr;
            //var incomeYr = model["Income Year"];
            var incomeYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == vmc.fiscalYr_id).fiscal_year;
            var res = new OperationResult();

            try
            {
                if (btn == "Download in Excel")
                {
                    var resultList = new List<CertificateUpload>();


                    resultList = (from emp in dataContext.prl_employee
                                  join cu in dataContext.prl_certificate_upload
                                  on emp.id equals cu.emp_id into eGroup2
                                  from e in eGroup2.DefaultIfEmpty()
                                  where e.file_path.Contains("TRA") && e.income_year == incomeYr
                                  select new CertificateUpload
                                  {
                                      ID = e.id,
                                      Emp_ID = emp.emp_no,
                                      Name = emp.name,
                                      Email = emp.email,
                                      No_of_Cars = e.number_of_car,
                                      Amount = e.amount,
                                      Is_Appropved = e.is_approved == null || e.is_approved == false ? "No" : "Yes",
                                      Submission_Date = e.created_date
                                  }).Distinct().ToList();


                    if (resultList.Count > 0)
                    {

                        LocalReport lr = new LocalReport();

                        string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "TRASheetReport.rdlc");
                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                        else
                        {
                            return View("TaxReturn");
                        }

                        //DateTime dt = new DateTime(SS.Year, SS.Month, 1);

                        ReportDataSource rd = new ReportDataSource("DataSet1", resultList);
                        lr.DataSources.Add(rd);
                        lr.SetParameters(new ReportParameter("incomeYr", incomeYr));
                        string reportType = "EXCELOPENXML";
                        //string contentType = "application/vnd.ms-excel";
                        string mimeType;
                        string encoding;
                        //string fileNameExtension = "xlsx";
                        string fileNameExtension = string.Format("{0}.{1}", "TRASheet", "xlsx");

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

                        ViewBag.loadIncomeYear = dataContext.prl_fiscal_year.ToList();

                        return File(renderedBytes, mimeType, "Tax_Return_Acknowledge_Sheet.xlsx");
                    }
                    else
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No data found, Please select accurate fiscal year.");
                    }
                }

                else if (btn == "Certificate Download")
                {
                    var AllEmployee = false;
                    var empNumber = new List<string>();
                    var empIds = new List<int>();
                    List<string> FileUrls = new List<string>();
                    var certificateLists = new List<CertificateUploadVM>();

                    if (string.IsNullOrWhiteSpace(vmc.SelectedEmployeesOnly))
                    {
                        AllEmployee = true;
                    }

                    if (AllEmployee == false)
                    {
                        empNumber = vmc.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                                .Select(x => x.id).ToList();

                        var lstEmp = dataContext.prl_certificate_upload
                                 .Where(x => empIds.Contains(x.emp_id) && x.income_year == incomeYr && x.certificat_type == "TRA").ToList();
                        certificateLists = Mapper.Map<List<CertificateUploadVM>>(lstEmp);
                    }
                    else
                    {
                        var lstEmp = dataContext.prl_certificate_upload
                                .Where(x => x.income_year == incomeYr && x.certificat_type == "TRA").ToList();
                        certificateLists = Mapper.Map<List<CertificateUploadVM>>(lstEmp);

                    }

                    if (certificateLists.Count > 0)
                    {
                        foreach (var item in certificateLists)
                        {
                            FileUrls.Add(item.file_path);
                            //FileUrl= FileUrl.Replace(@"\", "/");
                            //byte[] FileBytes = System.IO.File.ReadAllBytes(FileUrl);

                            //return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
                            //return File(certificates.file_path, "application/pdf");
                        }

                        var filesCol = FileUrls;

                        using (var memoryStream = new MemoryStream())
                        {
                            using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            {
                                for (int i = 0; i < filesCol.Count; i++)
                                {
                                    string fileName = filesCol[i].Split('\\').Last();
                                    ziparchive.CreateEntryFromFile(filesCol[i], fileName);
                                }
                            }

                            return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
                        }
                    }

                    //else
                    //{
                    //    //res.IsSuccessful = false;
                    //    //res.Message = "No Data Found";
                    //    //TempData.Add("msg", res);
                    //    return RedirectToAction("AIT");
                    //}

                }
                else if (btn == "Approved All")
                {
                    var certificatList = (from c in dataContext.prl_certificate_upload
                                          where c.income_year == incomeYr && c.certificat_type == "TRA" && c.is_approved == false
                                          select c).ToList();

                    if (certificatList.Count > 0)
                    {
                        foreach (var item in certificatList)
                        {
                            item.is_approved = true;
                            item.updated_by = User.Identity.Name;
                            item.updated_date = DateTime.Now;
                        }

                        //dataContext.SaveChanges();

                        if (dataContext.SaveChanges() > 0)
                        {
                            res.IsSuccessful = true;
                            res.Message = "Approved all applied Tax Return Acknoledgement, will be effected in next tax process";
                            TempData.Add("msg", res);
                            return RedirectToAction("TaxReturn");
                        }
                    }
                    else
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No data found, Please select accurate fiscal year.");
                    }
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return RedirectToAction("TaxReturn");
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
                    var original = dataContext.prl_certificate_upload.SingleOrDefault(x => x.id == primKey);
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

        [HttpPost]
        public JsonResult UpdateApproval(HttpRequestMessage request, string pk, string value)
        {
            try
            {
                int primKey = 0;
                decimal amnt = 0;

                if (value.ToLower() == "yes" || value.ToLower() == "no")
                {
                    bool isApproved = value.ToLower() == "yes" ? true : false;

                    if (Int32.TryParse(pk, out primKey)) //&& Boolean.TryParse(isApproved, out amnt)
                    {
                        var original = dataContext.prl_certificate_upload.SingleOrDefault(x => x.id == primKey);
                        original.is_approved = isApproved;
                        original.updated_by = User.Identity.Name;
                        original.updated_date = DateTime.Now;
                        dataContext.SaveChanges();
                        request.CreateResponse(HttpStatusCode.OK);
                        return Json(new { status = "success", msg = "Successfully updated" }, "json", JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        request.CreateResponse(HttpStatusCode.OK);
                        return Json(new { status = "error", msg = "It must be boolean value" }, "json", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    request.CreateResponse(HttpStatusCode.OK);
                    return Json(new { status = "error", msg = "Please write Yes or No" }, "json", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                request.CreateResponse(HttpStatusCode.OK);
                return Json(new { status = "error", msg = "Sorry could not save!" }, "json", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
