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
using NPOI.SS.UserModel;
using System.Globalization;
using NPOI.XSSF.UserModel;
using PayrollWeb.ViewModels.Utility;

namespace PayrollWeb.Controllers
{
    public class IncomeTaxController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public IncomeTaxController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {

            var prlFiscalYears = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYears);

            //var list = dataContext.prl_income_tax_parameter.ToList();
            //var vwList = Mapper.Map<List<IncomeTaxParameter>>(list).AsEnumerable();
            return View();
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

                var employees = (from spd in dataContext.prl_salary_process_detail
                                 join emp in dataContext.prl_employee on spd.emp_id equals emp.id
                                 join empD in dataContext.prl_employee_details
                                 on emp.id equals empD.emp_id
                                 where emp.is_active == 1 && emp.emp_no.Contains(searchText) && spd.total_monthly_tax>0
                                 select new { emp.id, emp.emp_no, emp.name, emp.official_contact_no }).Distinct().ToList();

                int totalRecords = employees.Count();

                var aaData = employees.Select(x => new string[] { x.emp_no, x.name, x.official_contact_no });

                //  
                return Json(new { sEcho = collection.Get("sEcho"), aaData = aaData, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords }, JsonRequestBehavior.DenyGet);
            }

            catch (Exception ex)
            {
                var k = ex.Message;
                throw;
            }
        }

        public ActionResult EmployeeWiseMonthlyTax()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(new ReportSalarySheet
            {
                //RType. = true
            });
        }

        [HttpPost]
        public ActionResult EmployeeWiseMonthlyTax(ReportSalarySheet SS, string sButton)
        {
            var empNumber = new List<string>();
            var empIds = new List<int>();
            var EmpList = new List<ReportSalarySheet>();
            var month = SS.month_no;
            var year = SS.Year;

            if (!string.IsNullOrWhiteSpace(SS.SelectedEmployeesOnly))
            {
                empNumber = SS.SelectedEmployeesOnly.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                empIds = dataContext.prl_employee.AsEnumerable().Where(x => empNumber.Contains(x.emp_no))
                            .Select(x => x.id).ToList();

                if (empIds.Count>0)
                {
                    EmpList = (from spd in dataContext.prl_salary_process_detail
                               join emp in dataContext.prl_employee on spd.emp_id equals emp.id into empGroup
                               from eG in empGroup.DefaultIfEmpty()

                               join empD in dataContext.prl_employee_details on eG.id equals empD.emp_id into empDGroup
                               from eDG in empDGroup.DefaultIfEmpty()

                               join dpt in dataContext.prl_department on eDG.department_id equals dpt.id into dGroup
                               from d in dGroup.DefaultIfEmpty()

                               join sd in dataContext.prl_sub_department on eDG.sub_department_id equals sd.id into sGroup
                               from s in sGroup.DefaultIfEmpty()

                               join ssd in dataContext.prl_sub_sub_department on eDG.sub_department_id equals ssd.id into ssdGroup
                               from ss in ssdGroup.DefaultIfEmpty()

                               where empIds.Contains(spd.emp_id) && spd.salary_month.Month == month && spd.salary_month.Year == year && spd.total_monthly_tax > 0

                               select new ReportSalarySheet
                               {
                                   empNo = eG.emp_no,
                                   monthly_tax = spd.total_monthly_tax
                               }).ToList();

                    if (EmpList.Count > 0)
                    {
                        LocalReport lr = new LocalReport();

                        string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "EmpWiseTaxSheetReport.rdlc");
                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                        else
                        {
                            ViewBag.Years = DateUtility.GetYears();
                            ViewBag.Months = DateUtility.GetMonths();
                            return View("EmployeeWiseMonthlyTax");
                        }

                        ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                        lr.DataSources.Add(rd);
                        lr.SetParameters(new ReportParameter("month", DateUtility.MonthName(month).ToString()));
                        lr.SetParameters(new ReportParameter("year", year.ToString()));
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
                    ModelState.AddModelError("", "No information found");
                }


            }


            else
            {
                EmpList = (from spd in dataContext.prl_salary_process_detail
                           join emp in dataContext.prl_employee on spd.emp_id equals emp.id into empGroup
                           from eG in empGroup.DefaultIfEmpty()

                           join empD in dataContext.prl_employee_details on eG.id equals empD.emp_id into empDGroup
                           from eDG in empDGroup.DefaultIfEmpty()

                           join dpt in dataContext.prl_department on eDG.department_id equals dpt.id into dGroup
                           from d in dGroup.DefaultIfEmpty()

                           join sd in dataContext.prl_sub_department on eDG.sub_department_id equals sd.id into sGroup
                           from s in sGroup.DefaultIfEmpty()

                           join ssd in dataContext.prl_sub_sub_department on eDG.sub_department_id equals ssd.id into ssdGroup
                           from ss in ssdGroup.DefaultIfEmpty()

                           where spd.salary_month.Month == month && spd.salary_month.Year == year && spd.total_monthly_tax > 0

                           select new ReportSalarySheet
                           {
                               empNo = eG.emp_no,
                               monthly_tax = spd.total_monthly_tax
                           }).ToList();

                if (EmpList.Count > 0)
                {
                    LocalReport lr = new LocalReport();

                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "EmpWiseTaxSheetReport.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        ViewBag.Years = DateUtility.GetYears();
                        ViewBag.Months = DateUtility.GetMonths();
                        return View("EmployeeWiseMonthlyTax");
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                    lr.DataSources.Add(rd);
                    lr.SetParameters(new ReportParameter("month", DateUtility.MonthName(month).ToString()));
                    lr.SetParameters(new ReportParameter("year", year.ToString()));
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
            return View("EmployeeWiseMonthlyTax");
        }

        public PartialViewResult SearchTaxParameterResult(int f = 0, string g = "")
        {
            var list = new List<prl_income_tax_parameter>();
            if (g == "Regardless")
            {
                list = dataContext.prl_income_tax_parameter.Where(x => x.fiscal_year_id == f).ToList();
            }
            else
            {
                list = dataContext.prl_income_tax_parameter.Where(x => x.fiscal_year_id == f && x.gender == g).ToList();
            }
            var vwList = Mapper.Map<List<IncomeTaxParameter>>(list).AsEnumerable();
            return PartialView("_TaxParameterResult", vwList);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult IncomeTaxParameter()
        {
            var prlFiscalYears = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYears);

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult IncomeTaxParameter(IncomeTaxParameter _taxParam)
        {
            bool errorFound = false;
            var operationResult = new OperationResult();
            try
            {
                var _param = new prl_income_tax_parameter();
                _param.fiscal_year_id = _taxParam.fiscal_year_id;
                _param.assessment_year = _taxParam.assessment_year;
                _param.slab_mininum_amount = _taxParam.slab_mininum_amount;
                _param.slab_maximum_amount = _taxParam.slab_maximum_amount;
                _param.slab_percentage = _taxParam.slab_percentage;
                _param.gender = _taxParam.gender;
                _param.created_by = User.Identity.Name;
                _param.created_date = DateTime.Now;
                dataContext.prl_income_tax_parameter.Add(_param);
                dataContext.SaveChanges();

                operationResult.IsSuccessful = true;
                operationResult.Message = "Tax parameter save successfully.";
                TempData.Add("msg", operationResult);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", operationResult);
            }
            var prlFiscalYears = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYears);
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var prlAll = dataContext.prl_income_tax_parameter.SingleOrDefault(x => x.id == id);
            var dn = Mapper.Map<IncomeTaxParameter>(prlAll);

            var lstFis = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(lstFis);
            return View(dn);
        }

        //
        // POST: /IncomeTax/Edit/5

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, IncomeTaxParameter item)
        {
            var res = new OperationResult();
            try
            {
                var _param = dataContext.prl_income_tax_parameter.SingleOrDefault(x => x.id == item.id);
                _param.fiscal_year_id = item.fiscal_year_id;
                _param.assessment_year = item.assessment_year;
                _param.slab_mininum_amount = item.slab_mininum_amount;
                _param.slab_maximum_amount = item.slab_maximum_amount;
                _param.slab_percentage = item.slab_percentage;
                _param.gender = item.gender;
                _param.updated_by = User.Identity.Name;
                _param.updated_date = DateTime.Now;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Parameter setting updated successfully.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            var lstFis = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(lstFis);
            return View();
        }

        //
        // GET: /IncomeTax/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /IncomeTax/Delete/5

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

        [PayrollAuthorize]
        public ActionResult SearchTaxParameterDetail()
        {
            var prlTaxDet = dataContext.prl_income_tax_parameter_details.ToList();
            var detList = Mapper.Map<List<IncomeTaxParameterDetail>>(prlTaxDet);
            return View(detList);
        }

        [PayrollAuthorize]
        public ActionResult IncomeTaxParameterDetail(string _val = "")
        {
            var operationResult = new OperationResult();
            string[] result = { };
            int _fs = 0;
            string _gender = "";
            var _paramDet = new prl_income_tax_parameter_details();
            if (_val != "")
            {
                result = _val.Split('-');
                _fs = int.Parse(_val[0].ToString());
                _gender = _val[2].ToString();

                if (_gender == "M")
                    _gender = "Male";
                else if (_gender == "R")
                    _gender = "Regardless";
                else
                    _gender = "Female";

                _paramDet = dataContext.prl_income_tax_parameter_details.FirstOrDefault(x => x.fiscal_year_id == _fs);
            }

            if (_paramDet == null)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = "No Data found.";
                TempData.Add("msg", operationResult);

            }

            var _detail = Mapper.Map<IncomeTaxParameterDetail>(_paramDet);

            var prlFiscalYears = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYears);

            return View(_detail);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult IncomeTaxParameterDetail(IncomeTaxParameterDetail _taxParamDet)
        {
            //bool errorFound = false;
            var operationResult = new OperationResult();
            try
            {
                var param = new prl_income_tax_parameter_details();
                if (_taxParamDet.id == 0)
                {
                    param.fiscal_year_id = _taxParamDet.fiscal_year_id;
                    param.assesment_year = _taxParamDet.assesment_year;
                    param.gender = _taxParamDet.gender;
                    param.max_tax_age = _taxParamDet.max_tax_age;
                    param.max_investment_amount = _taxParamDet.max_investment_amount;
                    param.max_investment_percentage = _taxParamDet.max_investment_percentage;
                    param.max_inv_exempted_percentage = _taxParamDet.max_inv_exempted_percentage;
                    param.max_amount_for_max_exemption_percent = _taxParamDet.max_amount_for_max_exemption_percent;
                    param.min_inv_exempted_percentage = _taxParamDet.min_inv_exempted_percentage;
                    param.min_tax_amount = _taxParamDet.min_tax_amount;
                    param.max_house_rent_percentage = _taxParamDet.max_house_rent_percentage;
                    param.house_rent_not_exceding = _taxParamDet.house_rent_not_exceding;
                    param.max_conveyance_allowance = _taxParamDet.max_conveyance_allowance;
                    param.medical_exemption_percentage = _taxParamDet.medical_exemption_percentage;
                    param.medical_not_exceding = _taxParamDet.medical_not_exceding;

                    dataContext.prl_income_tax_parameter_details.Add(param);
                }
                else
                {
                    param = dataContext.prl_income_tax_parameter_details.FirstOrDefault(p => p.id == _taxParamDet.id);
                    param.assesment_year = _taxParamDet.assesment_year;
                    param.gender = _taxParamDet.gender;
                    param.max_tax_age = _taxParamDet.max_tax_age;
                    param.max_investment_amount = _taxParamDet.max_investment_amount;
                    param.max_investment_percentage = _taxParamDet.max_investment_percentage;
                    param.max_inv_exempted_percentage = _taxParamDet.max_inv_exempted_percentage;
                    param.max_amount_for_max_exemption_percent = _taxParamDet.max_amount_for_max_exemption_percent;
                    param.min_inv_exempted_percentage = _taxParamDet.min_inv_exempted_percentage;
                    param.min_tax_amount = _taxParamDet.min_tax_amount;
                    param.max_house_rent_percentage = _taxParamDet.max_house_rent_percentage;
                    param.house_rent_not_exceding = _taxParamDet.house_rent_not_exceding;
                    param.max_conveyance_allowance = _taxParamDet.max_conveyance_allowance;
                    param.medical_exemption_percentage = _taxParamDet.medical_exemption_percentage;
                    param.medical_not_exceding = _taxParamDet.medical_not_exceding;
                }

                dataContext.SaveChanges();

                operationResult.IsSuccessful = true;
                operationResult.Message = "Tax parameter save successfully.";
                TempData.Add("msg", operationResult);
                return RedirectToAction("IncomeTaxParameterDetail");
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", operationResult);
            }
            var prlFiscalYears = dataContext.prl_fiscal_year.ToList();
            ViewBag.AllFiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYears);
            return View();
        }


        [PayrollAuthorize]
        [HttpGet]
        public ActionResult ReportIncomeTax()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(new ReportIncomeTax
            {
                //RType. = true
            });
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult ReportIncomeTax(ReportIncomeTax it, string sButton)
        {
            bool errorFound = false;
            var res = new OperationResult();
            var empNumber = new List<string>();
            var empIds = new List<int>();
            var employeeList = new List<prl_employee_details>();
            var EmpList = new List<ReportIncomeTax>();

            int Yr = it.Year;
            int currMonth = it.Month;
            int fiscalYearStart = 7;
            string s_date = "";
            string e_date = "";
            string end_date = "";

            string projected_s_date = "";
            string projected_e_date = "";

            if (sButton != null)
            {
                errorFound = true;
                ViewBag.Years = DateUtility.GetYears();
                ViewBag.Months = DateUtility.GetMonths();
                ModelState.AddModelError("", "Sorry.. This report has not been developed yet.");
                return View();
            }

            //TillDateRange Start
            if (currMonth > 6)
            {
                s_date = Yr.ToString() + "-" + fiscalYearStart + "-" + "1";
            }
            else
            {
                s_date = (Yr - 1).ToString() + "-" + fiscalYearStart + "-" + "1";
            }

            //TillDateRange End
            if (currMonth == 1)
            {
                e_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
                end_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
            }
            else if (currMonth == 7)
            {
                e_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
                end_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
            }
            else
            {
                DateTime endDT = new DateTime(Yr, currMonth - 1, 1).AddMonths(1).AddDays(-1);
                e_date = endDT.ToString("yyyy-MM-dd ");
                end_date = Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + 31;
            }

            //projectedDateRange

            if (currMonth == 6)
            {
                projected_s_date = "";
                projected_e_date = "";
            }
            else if (currMonth > 6 && currMonth < 12)
            {
                projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
                projected_e_date = (Yr + 1).ToString() + "-6-30";
            }
            else if (currMonth == 12)
            {
                projected_s_date = (Yr + 1).ToString() + "-1-31"; ;
                projected_e_date = (Yr + 1).ToString() + "-6-30";
            }
            else
            {
                projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
                projected_e_date = Yr.ToString() + "-6-30";
            }

            DateTime frmDate = Convert.ToDateTime(s_date);
            DateTime toDate = Convert.ToDateTime(e_date);

            var discontEmpIds = new List<int>();
            discontEmpIds = dataContext.prl_employee_discontinue.Where(ed => ed.discontinue_date < frmDate).Select(x => x.emp_id).Distinct().ToList();

            empIds = dataContext.prl_employee.Where(x => !discontEmpIds.Contains(x.id)).Select(x => x.id).Distinct().ToList();
            employeeList = dataContext.prl_employee_details.Include("prl_employee").AsEnumerable().Where(x => empIds.Contains(x.emp_id)).GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();

            int e_id = 0;
            MySqlCommand mySqlCommand = null;
            MySqlConnection mySqlConnection = null;

            mySqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
            mySqlCommand = new MySqlCommand();
            mySqlCommand.Connection = mySqlConnection;
            mySqlConnection.Open();


            int _result = 0;
            decimal taxAge = 0;
            decimal min_tax = 0;

            //For Finding yearly Basic
            decimal currentBasic = 0;
            decimal thisMonthBasic = 0;
            decimal projectedBasic = 0;
            decimal actualBasic = 0;
            decimal yearlyBasic = 0;
            decimal previousBasic = 0;
            //For Finding yearly Basic

            //Festival
            decimal projectedFestival = 0;
            decimal actualFestival = 0;
            decimal yearlyFestival = 0;
            //Festival

            decimal freeCar = 0;

            int _reminingMonth = 0;
            int _actualMonth = 0;

            //Tax
            decimal taxableIncome = 0;
            //Tax

            int fiscalYrStartMonth = 7;

            foreach (var item in employeeList)
            {
                List<prl_salary_allowances> allowList = new List<prl_salary_allowances>();
                List<prl_workers_allowances> WorkerAllowList = new List<prl_workers_allowances>();
                List<EmployeeSalaryAllowance> salallList = new List<EmployeeSalaryAllowance>();

                var Emp = new Employee();

                var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == item.emp_id);
                Emp = Mapper.Map<Employee>(_empD);

                var salaryPD = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == it.Year && x.salary_month.Month == it.Month);

                if (!errorFound)
                {
                    /****************/
                    int pFiscalYrID = FindFiscalYear(new DateTime(it.Year, it.Month, 1).AddMonths(1).AddDays(-1));
                    var fiscalYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == pFiscalYrID);
                    /****************/

                    var reportData = new ReportIncomeTax();
                    var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                    reportData.empId = Emp.id;
                    reportData.empNo = Emp.emp_no;
                    reportData.empName = Emp.name;
                    reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";

                    reportData.emp_category = empD.employee_category;
                    reportData.joining_date = Emp.joining_date.ToString("dd-MMM-yyyy");


                    reportData.gender = Emp.gender;
                    reportData.fiscal_year = fiscalYr.fiscal_year;
                    reportData.assesment_year = fiscalYr.assesment_year;
                    reportData.tin = Emp.tin;
                    reportData.bank = Emp.prl_bank.bank_name;
                    reportData.accNo = Emp.account_no;
                    reportData.routing_no = Emp.routing_no;

                    reportData.Year = it.Year;
                    reportData.month_name = DateUtility.MonthName(it.Month);

                    reportData.monthYear = new DateTime(it.Year, it.Month, 1).AddMonths(1).AddDays(-1);
                    //reportData.taxProcessId = empTax.id;


                    if (projected_e_date != "" && projected_s_date != "")
                    {
                        DateTime p_frmDate = Convert.ToDateTime(projected_s_date);
                        DateTime p_toDate = Convert.ToDateTime(projected_e_date);
                        reportData.projectedDateRange = DateUtility.MonthName(p_frmDate.Month).Substring(0, 3) + "'" + p_frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(p_toDate.Month).Substring(0, 3) + "'" + p_toDate.Year.ToString().Substring(2);
                    }
                    else
                    {
                        reportData.projectedDateRange = "";
                    }

                    reportData.tillDateRange = DateUtility.MonthName(frmDate.Month).Substring(0, 3) + "'" + frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(toDate.Month).Substring(0, 3) + "'" + toDate.Year.ToString().Substring(2);
                    reportData.currentMonth = DateUtility.MonthName(currMonth);

                    #region previous basic

                    mySqlCommand.Parameters.Clear();
                    var select_cmd = @"SELECT IFNULL(SUM(this_month_basic), 0) FROM prl_salary_process_detail WHERE salary_month BETWEEN ?s_datee AND ?e_datee AND emp_id = ?emp_iid;";
                    mySqlCommand.Connection = mySqlConnection;
                    mySqlCommand.CommandText = select_cmd;
                    mySqlCommand.Parameters.AddWithValue("?emp_iid", item.emp_id);
                    mySqlCommand.Parameters.AddWithValue("?s_datee", s_date);
                    mySqlCommand.Parameters.AddWithValue("?e_datee", e_date);

                    string resVal = "";
                    using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                    {
                        foreach (var dr in msReader)
                        {
                            if (System.DBNull.Value != null)
                            {
                                //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
                                resVal = msReader.GetString(0);
                            }
                        }
                    }

                    if (resVal != "")
                        previousBasic = decimal.Parse(resVal);
                    #endregion

                    #region basics and allowances

                    var SpDetails = new prl_salary_process_detail();

                    if (salaryPD != null)
                    {
                        SpDetails = dataContext.prl_salary_process_detail.FirstOrDefault(s => s.salary_process_id == salaryPD.salary_process_id && s.emp_id == item.emp_id);
                        _reminingMonth = IncomeTaxService.FindProjectedMonth(SpDetails.salary_month.Month);
                        _actualMonth = IncomeTaxService.FindActualMonth(SpDetails.salary_month.Month);
                        currentBasic = item.basic_salary;
                        currentBasic = SpDetails.current_basic; // Current Basic
                        thisMonthBasic = SpDetails.this_month_basic.Value; // This Month Basic 
                        projectedBasic = SpDetails.current_basic * _reminingMonth; // projected basic

                    }

                    yearlyBasic = previousBasic + thisMonthBasic + projectedBasic; // Chnge for getting previous payment in Basic Salary
                    //Basic Salary

                    reportData.basic = yearlyBasic;


                    //For Calculate All Allowances
                    DateTime EndDate = new DateTime(it.Year, it.Month, 1).AddMonths(1).AddDays(-1);
                    string allWName = "";

                    allowList = dataContext.prl_salary_allowances.Where(x => x.salary_process_id == SpDetails.salary_process_id && x.emp_id == item.emp_id).ToList();
                    if (allowList.Count > 0)
                    {
                        foreach (var allW in allowList)
                        {
                            var AllwConfig = dataContext.prl_allowance_configuration.FirstOrDefault(q => q.allowance_name_id == allW.allowance_name_id);
                            if (AllwConfig.is_taxable == 1)
                            {
                                allWName = dataContext.prl_allowance_name.FirstOrDefault(a => a.id == allW.allowance_name_id).allowance_name;
                                EmployeeSalaryAllowance salAllW = new EmployeeSalaryAllowance();
                                salAllW.allowanceid = allW.allowance_name_id;
                                salAllW.allowancename = allWName;
                                salAllW.this_month_amount = allW.amount;


                                #region previous Allowances

                                var allw_cmd = @"SELECT IFNULL(SUM(amount), 0)  + IFNULL(SUM(arrear_amount),0) FROM prl_salary_allowances WHERE salary_month BETWEEN ?s_date AND ?e_date AND emp_id = ?emp_id AND allowance_name_id = ?allw_name_id;";
                                mySqlCommand.Connection = mySqlConnection;
                                mySqlCommand.CommandText = allw_cmd;
                                mySqlCommand.Parameters.AddWithValue("?emp_id", item.emp_id);
                                mySqlCommand.Parameters.AddWithValue("?s_date", s_date);
                                mySqlCommand.Parameters.AddWithValue("?e_date", e_date);
                                mySqlCommand.Parameters.AddWithValue("?allw_name_id", allW.allowance_name_id);

                                string allwVal = "";
                                using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                                {
                                    foreach (var dr in msReader)
                                    {
                                        if (System.DBNull.Value != null)
                                        {
                                            //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
                                            allwVal = msReader.GetString(0);
                                        }
                                    }
                                }
                                mySqlCommand.Parameters.Clear();
                                if (allwVal != "")
                                    salAllW.actual_amount = decimal.Parse(allwVal);

                                #endregion


                                if (AllwConfig.percent_amount > 0)
                                {
                                    salAllW.current_amount = (currentBasic * AllwConfig.percent_amount / 100).Value;
                                }
                                else
                                {
                                    salAllW.current_amount = allW.amount;
                                    //salAllW.current_amount = AllwConfig.flat_amount.Value; //commented by Lukman
                                }

                                if (allWName.Contains("House Rent Allowance") || allWName.Contains("Medical Allowance") || allWName.Contains("Conveyance Allowance"))
                                {
                                    salAllW.projected_amount = salAllW.current_amount * _reminingMonth;
                                }

                                salAllW.yearly_amount = salAllW.actual_amount + salAllW.current_amount + salAllW.projected_amount;

                                salallList.Add(salAllW);
                            }
                        }
                    }

                    var lstAllowances = new List<int>();
                    var allAllowancesThisYr = new List<prl_salary_allowances>();
                    lstAllowances = salallList.AsEnumerable().Select(x => x.allowanceid).Distinct().ToList();
                    allAllowancesThisYr = dataContext.prl_salary_allowances.AsEnumerable().Where(x => x.salary_month >= frmDate && x.salary_month <= EndDate && x.emp_id == item.emp_id).GroupBy(x => x.allowance_name_id).Select(grp => grp.FirstOrDefault()).ToList();

                    foreach (var empAllow in allAllowancesThisYr) // To Get All Allowances of this fiscal year
                    {
                        if (!lstAllowances.Contains(empAllow.allowance_name_id))
                        {

                            var AllwConfig = dataContext.prl_allowance_configuration.FirstOrDefault(q => q.allowance_name_id == empAllow.allowance_name_id);
                            if (AllwConfig.is_taxable == 1)
                            {
                                allWName = dataContext.prl_allowance_name.FirstOrDefault(a => a.id == empAllow.allowance_name_id).allowance_name;
                                EmployeeSalaryAllowance salAllW = new EmployeeSalaryAllowance();
                                salAllW.allowanceid = empAllow.allowance_name_id;
                                salAllW.allowancename = allWName;
                                salAllW.this_month_amount = 0;


                                #region previous Allowances

                                var allw_cmd = @"SELECT IFNULL(SUM(amount), 0)  + IFNULL(SUM(arrear_amount),0) FROM prl_salary_allowances WHERE salary_month BETWEEN ?s_date AND ?e_date AND emp_id = ?emp_id AND allowance_name_id = ?allw_name_id;";
                                mySqlCommand.Connection = mySqlConnection;
                                mySqlCommand.CommandText = allw_cmd;
                                mySqlCommand.Parameters.AddWithValue("?emp_id", item.emp_id);
                                mySqlCommand.Parameters.AddWithValue("?s_date", s_date);
                                mySqlCommand.Parameters.AddWithValue("?e_date", e_date);
                                mySqlCommand.Parameters.AddWithValue("?allw_name_id", empAllow.allowance_name_id);

                                string allwVal = "";
                                using (MySqlDataReader msReader = mySqlCommand.ExecuteReader())
                                {
                                    foreach (var dr in msReader)
                                    {
                                        if (System.DBNull.Value != null)
                                        {
                                            //while (msReader.Read()) //commented because .Read() skip the first row whereas query execute 1 row.
                                            allwVal = msReader.GetString(0);
                                        }
                                    }
                                }
                                mySqlCommand.Parameters.Clear();
                                if (allwVal != "")
                                    salAllW.actual_amount = decimal.Parse(allwVal);

                                #endregion


                                if (AllwConfig.percent_amount > 0)
                                {
                                    salAllW.current_amount = (currentBasic * AllwConfig.percent_amount / 100).Value;
                                }
                                else
                                {
                                    salAllW.current_amount = 0;
                                    //salAllW.current_amount = AllwConfig.flat_amount.Value; //commented by Lukman
                                }

                                if (allWName.Contains("House Rent Allowance") || allWName.Contains("Medical Allowance") || allWName.Contains("Conveyance Allowance"))
                                {
                                    salAllW.projected_amount = salAllW.current_amount * _reminingMonth;
                                }

                                salAllW.yearly_amount = salAllW.actual_amount + salAllW.current_amount + salAllW.projected_amount;

                                //if (AllwConfig.exempted_amount > 0)
                                //    salAllW.exempted_amount = AllwConfig.exempted_amount.Value;

                                salallList.Add(salAllW);
                            }
                        }
                    }

                    #endregion

                    var taxDetail = new prl_income_tax_parameter_details();

                    //Free Carn

                    //Total Taxable Income
                    // Need apply changes which one is 100% taxable

                    //Commented By Lukman//
                    //taxableIncome = yearlyBasic + yearlyFestival + yearlyPF + freeCar + yearlyChildAllowance;

                    taxableIncome = yearlyBasic;

                    decimal totalTaxableConveyance = 0;
                    decimal actualConveyanceExemption = 0;

                    decimal totalTaxableHouse = 0;
                    decimal actualHouseExemption = 0;

                    decimal totalTaxableMedical = 0;
                    decimal actualMedicalExemption = 0;


                    foreach (var a in salallList)
                    {

                        DateTime fiscalYrStart = Convert.ToDateTime(s_date);
                        DateTime fiscalYrEnd = Convert.ToDateTime((fiscalYrStart.Year + 1) + "-" + 6 + "-" + 30);

                        if (a.allowancename.Contains("House Rent Allowance"))
                        {
                            decimal HRexmpOnBasic = yearlyBasic * (taxDetail.max_house_rent_percentage.Value / 100);
                            decimal monthlyHRexmp = 0;
                            decimal HRexemOnLimit = taxDetail.house_rent_not_exceding.Value;

                            if (Utility.CommonDateClass.MonthYearIsInRange(Emp.joining_date, fiscalYrStart, fiscalYrEnd))
                            {
                                int calculated_month = Utility.DateUtility.getTaxTotalMonthForNewEmp(Emp.joining_date.Month);
                                monthlyHRexmp = Math.Min((HRexmpOnBasic / calculated_month), (HRexemOnLimit / 12));
                                actualHouseExemption = monthlyHRexmp * calculated_month;
                                totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                            }
                            else
                            {
                                //if employee inactive

                                DateTime endDateFsYr = Convert.ToDateTime(frmDate.Year + 1 + "-6-30");

                                var discontinued_empD = dataContext.prl_employee_discontinue.FirstOrDefault(d => d.discontinue_date >= fiscalYrStart && d.discontinue_date <= fiscalYrEnd && d.emp_id == item.emp_id);
                                if (discontinued_empD != null)
                                {
                                    int calculated_month = Utility.DateUtility.getTaxTotalMonthForSeparatedEmp(discontinued_empD.discontinue_date.Month);
                                    monthlyHRexmp = Math.Min((HRexmpOnBasic / calculated_month), (HRexemOnLimit / 12));
                                    actualHouseExemption = monthlyHRexmp * calculated_month;
                                    totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                                }
                                else
                                {
                                    actualHouseExemption = Math.Min(HRexmpOnBasic, taxDetail.house_rent_not_exceding.Value);
                                    totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                                }
                            }

                            if (totalTaxableHouse < 0)
                            {
                                totalTaxableHouse = 0;
                                actualHouseExemption = a.yearly_amount;
                            }
                            taxableIncome += totalTaxableHouse;

                            reportData.hr_allowance = a.yearly_amount;
                            reportData.exemption_hr = actualHouseExemption;
                            reportData.taxable_hr = totalTaxableHouse;
                        }

                        if (a.allowancename.Contains("House Rent Allowance"))
                        {
                            decimal HRexmpOnBasic = yearlyBasic * (taxDetail.max_house_rent_percentage.Value / 100);
                            decimal monthlyHRexmpOnBasic = 0;
                            decimal montlyHRexemOnLimit = taxDetail.house_rent_not_exceding.Value / 12;

                            if (Emp.joining_date > fiscalYrStart)
                            {
                                monthlyHRexmpOnBasic = HRexmpOnBasic / (Emp.joining_date.Month - fiscalYrStart.Month);
                                actualHouseExemption = Math.Min(monthlyHRexmpOnBasic, montlyHRexemOnLimit) * (Emp.joining_date.Month - fiscalYrStart.Month);
                                totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                            }
                            else
                            {
                                actualHouseExemption = Math.Min(HRexmpOnBasic, taxDetail.house_rent_not_exceding.Value);
                                totalTaxableHouse = a.yearly_amount - actualHouseExemption;
                            }

                            if (totalTaxableHouse < 0)
                            {
                                totalTaxableHouse = 0;
                                actualHouseExemption = a.yearly_amount;
                            }
                            taxableIncome += totalTaxableHouse;

                            reportData.hr_allowance = a.yearly_amount;
                            reportData.exemption_hr = actualHouseExemption;
                            reportData.taxable_hr = totalTaxableHouse;
                        }
                        else if (a.allowancename.Contains("Medical Allowance"))
                        {
                            decimal medicalExemOnBasic = yearlyBasic * (taxDetail.medical_exemption_percentage.Value / 100);
                            decimal medicalExemOnLimit = taxDetail.medical_not_exceding.Value;
                            actualMedicalExemption = Math.Min(medicalExemOnBasic, medicalExemOnLimit);

                            totalTaxableMedical = a.yearly_amount - actualMedicalExemption;

                            if (totalTaxableMedical < 0)
                            {
                                totalTaxableMedical = 0;
                                actualMedicalExemption = a.yearly_amount;
                            }
                            taxableIncome += totalTaxableMedical;

                            reportData.medical_allowance = a.yearly_amount;
                            reportData.exemption_medical = actualMedicalExemption;
                            reportData.taxable_medical = totalTaxableMedical;
                        }
                        else if (a.allowancename.Contains("Conveyance Allowance"))
                        {
                            totalTaxableConveyance = a.yearly_amount - taxDetail.max_conveyance_allowance.Value;
                            actualConveyanceExemption = taxDetail.max_conveyance_allowance.Value;
                            if (totalTaxableConveyance < 0)
                            {
                                totalTaxableConveyance = 0;
                                actualConveyanceExemption = a.yearly_amount;
                            }
                            taxableIncome += totalTaxableConveyance;

                            reportData.conveyance_allowance = a.yearly_amount;
                            reportData.exemption_conv = actualConveyanceExemption;
                            reportData.taxable_conv = totalTaxableConveyance;
                        }

                        else if (a.allowancename.Contains("Festival Bonus"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.festibal_bonus = a.yearly_amount;
                        }

                        else if (a.allowancename.Contains("Mid Month Advance"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.mid_month_advance = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("Others"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.other_allowance = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("Leave Encashment"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.leave_encashment = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("Long Service Award"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.long_service_award = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("Loan Refund"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.loan_refund = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("SIP"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.sip = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("KEB"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.keb = a.yearly_amount;
                        }

                        else if (a.allowancename.Contains("LTP"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.ltp = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("Ramadan Allowance"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.ramadan_allowance = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("STIP"))
                        {
                            taxableIncome += a.yearly_amount;
                            reportData.stip = a.yearly_amount;
                        }
                        else if (a.allowancename.Contains("Tax Refund"))
                        {
                            reportData.taxRefund = a.yearly_amount;
                        }
                        else
                        {
                            taxableIncome += a.yearly_amount;
                        }
                    }
                    //Tax Parameter Settings
                    if (taxDetail != null)
                    {

                        taxAge = taxDetail.max_tax_age.Value;
                        min_tax = taxDetail.min_tax_amount.Value;
                    }

                    //Tax Parameter Settings

                    //Tax Slab Wise data

                    //var empTaxSlabList = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).ToList();

                    //Tax Slab

                    List<prl_income_tax_parameter> taxSlab = new List<prl_income_tax_parameter>();

                    taxSlab = dataContext.prl_income_tax_parameter.Where(objID => (objID.fiscal_year_id == pFiscalYrID && objID.gender == Emp.gender)).ToList();
                    int maxNumberItem = taxSlab.Count;

                    decimal lastSlabAmount = (taxSlab[maxNumberItem - 1] != null ? taxSlab[maxNumberItem - 1].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 2] != null ? taxSlab[maxNumberItem - 2].slab_maximum_amount : 0);
                    decimal lastSlabPercentage = (taxSlab[maxNumberItem - 1] != null ? taxSlab[maxNumberItem - 1].slab_percentage : 0);
                    decimal secondlastSlabAmount = (taxSlab[maxNumberItem - 2] != null ? taxSlab[maxNumberItem - 2].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 3] != null ? taxSlab[maxNumberItem - 3].slab_maximum_amount : 0);
                    decimal secondlastSlabPercentage = (taxSlab[maxNumberItem - 2] != null ? taxSlab[maxNumberItem - 2].slab_percentage : 0);
                    decimal thirdlastSlabAmount = (taxSlab[maxNumberItem - 3] != null ? taxSlab[maxNumberItem - 3].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 4] != null ? taxSlab[maxNumberItem - 4].slab_maximum_amount : 0);
                    decimal thirdlastSlabPercentage = (taxSlab[maxNumberItem - 3] != null ? taxSlab[maxNumberItem - 3].slab_percentage : 0);
                    decimal forthlastSlabAmount = (taxSlab[maxNumberItem - 4] != null ? taxSlab[maxNumberItem - 4].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 5] != null ? taxSlab[maxNumberItem - 5].slab_maximum_amount : 0);
                    decimal forthlastSlabPercentage = (taxSlab[maxNumberItem - 4] != null ? taxSlab[maxNumberItem - 4].slab_percentage : 0);
                    decimal fifthlastSlabAmount = (taxSlab[maxNumberItem - 5] != null ? taxSlab[maxNumberItem - 5].slab_maximum_amount : 0) - (taxSlab[maxNumberItem - 6] != null ? taxSlab[maxNumberItem - 6].slab_maximum_amount : 0);
                    decimal fifthlastSlabPercentage = (taxSlab[maxNumberItem - 5] != null ? taxSlab[maxNumberItem - 5].slab_percentage : 0);
                    //lukman
                    decimal sixthlastSlabAmount = (taxSlab[maxNumberItem - 6] != null ? taxSlab[maxNumberItem - 6].slab_maximum_amount : 0);
                    decimal sixthlastSlabPercentage = (taxSlab[maxNumberItem - 6] != null ? taxSlab[maxNumberItem - 6].slab_percentage : 0);

                    decimal _TaxableIncome = taxableIncome;
                    decimal TaxPayableAmount = 0;

                    decimal firstSlabAmount = 0;
                    decimal secondSlabAmount = 0;
                    decimal thirdSlabAmount = 0;
                    decimal forthSlabAmount = 0;
                    decimal fifthSlabAmount = 0;


                    if (_TaxableIncome <= sixthlastSlabAmount)
                    {
                        TaxPayableAmount = 0;
                        reportData.incomeTaxableAmount_0 = _TaxableIncome;
                        reportData.individualTaxLiabilityAmount_0 = TaxPayableAmount;
                    }
                    if (_TaxableIncome > sixthlastSlabAmount)
                    {
                        decimal netTaxPayableAmount = _TaxableIncome - sixthlastSlabAmount;

                        reportData.incomeTaxableAmount_0 = sixthlastSlabAmount;
                        reportData.individualTaxLiabilityAmount_0 = TaxPayableAmount;

                        if (netTaxPayableAmount <= fifthlastSlabAmount)
                        {
                            TaxPayableAmount = (netTaxPayableAmount * fifthlastSlabPercentage) / 100;

                            reportData.incomeTaxableAmount_10 = netTaxPayableAmount;
                            reportData.individualTaxLiabilityAmount_10 = TaxPayableAmount;
                        }

                        if (netTaxPayableAmount > fifthlastSlabAmount)
                        {
                            decimal reminderAmount = netTaxPayableAmount - fifthlastSlabAmount;
                            firstSlabAmount = (fifthlastSlabAmount * fifthlastSlabPercentage) / 100;

                            reportData.incomeTaxableAmount_10 = fifthlastSlabAmount;
                            reportData.individualTaxLiabilityAmount_10 = firstSlabAmount;

                            if (reminderAmount <= forthlastSlabAmount)
                            {
                                secondSlabAmount = (reminderAmount * forthlastSlabPercentage) / 100;

                                reportData.incomeTaxableAmount_15 = reminderAmount;
                                reportData.individualTaxLiabilityAmount_15 = secondSlabAmount;
                            }

                            if (reminderAmount > forthlastSlabAmount)
                            {
                                decimal secondReminderAmount = reminderAmount - forthlastSlabAmount;

                                secondSlabAmount = (forthlastSlabAmount * forthlastSlabPercentage) / 100;
                                reportData.incomeTaxableAmount_15 = forthlastSlabAmount;
                                reportData.individualTaxLiabilityAmount_15 = secondSlabAmount;

                                if (secondReminderAmount <= thirdlastSlabAmount)
                                {
                                    thirdSlabAmount = (secondReminderAmount * thirdlastSlabPercentage) / 100;
                                    reportData.incomeTaxableAmount_20 = secondReminderAmount;
                                    reportData.individualTaxLiabilityAmount_20 = thirdSlabAmount;
                                }
                                if (secondReminderAmount > thirdlastSlabAmount)
                                {
                                    decimal thirdReminderAmount = secondReminderAmount - thirdlastSlabAmount;
                                    thirdSlabAmount = (thirdlastSlabAmount * thirdlastSlabPercentage) / 100;

                                    reportData.incomeTaxableAmount_20 = thirdlastSlabAmount;
                                    reportData.individualTaxLiabilityAmount_20 = thirdSlabAmount;

                                    if (thirdReminderAmount <= secondlastSlabAmount)
                                    {
                                        forthSlabAmount = (thirdReminderAmount * secondlastSlabPercentage) / 100;
                                        reportData.incomeTaxableAmount_25 = thirdReminderAmount;
                                        reportData.individualTaxLiabilityAmount_25 = forthSlabAmount;
                                    }

                                    if (thirdReminderAmount > secondlastSlabAmount)
                                    {
                                        forthSlabAmount = (secondlastSlabAmount * secondlastSlabPercentage) / 100;
                                        reportData.incomeTaxableAmount_25 = secondlastSlabAmount;
                                        reportData.individualTaxLiabilityAmount_25 = forthSlabAmount;

                                        decimal fourthReminder = (thirdReminderAmount - secondlastSlabAmount);
                                        fifthSlabAmount = (fourthReminder * lastSlabPercentage) / 100;

                                        reportData.incomeTaxableAmount_30 = fourthReminder;
                                        reportData.individualTaxLiabilityAmount_30 = fifthSlabAmount;
                                    }
                                }
                            }

                            TaxPayableAmount = firstSlabAmount + secondSlabAmount + thirdSlabAmount + forthSlabAmount + fifthSlabAmount;
                        }
                    }

                    //Grand Total Everything//

                    decimal totalAnnualTillDate = previousBasic;
                    decimal totalAnnualIncomeCurrentMonth = currentBasic;
                    decimal totalAnnualIncomeProjected = projectedBasic;
                    decimal totalLessExempted = 0;
                    decimal totalTaxableIncome = yearlyBasic;



                    foreach (var sal in salallList)
                    {
                        totalAnnualTillDate += sal.actual_amount;
                        totalAnnualIncomeCurrentMonth += sal.this_month_amount;
                        totalAnnualIncomeProjected += sal.projected_amount;

                        decimal exemption = 0;
                        decimal taxableEarnings = 0;

                        if (sal.allowancename.Contains("House Rent Allowance"))
                        {
                            exemption = actualHouseExemption;
                            taxableEarnings = totalTaxableHouse;
                        }

                        else if (sal.allowancename.Contains("Conveyance Allowance"))
                        {
                            exemption = actualConveyanceExemption;
                            taxableEarnings = totalTaxableConveyance;
                        }

                        else if (sal.allowancename.Contains("Medical Allowance"))
                        {
                            exemption = actualMedicalExemption;
                            taxableEarnings = totalTaxableMedical;
                        }

                        else
                        {
                            exemption = 0;
                            taxableEarnings = sal.yearly_amount;
                        }

                        totalLessExempted += exemption;
                        totalTaxableIncome += taxableEarnings;
                    }

                    reportData.totalAnnualIncomeTillDate = totalAnnualTillDate;
                    reportData.totalAnnualIncomeCurrentMonth = totalAnnualIncomeCurrentMonth;
                    reportData.totalAnnualIncomeProjected = totalAnnualIncomeProjected;
                    reportData.totalLessExempted = totalLessExempted;
                    reportData.totalTaxableIncome = totalTaxableIncome;


                    //decimal incomeTaxableAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => q.taxable_income);
                    //decimal individualTaxLiabilityAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => q.tax_liability);

                    reportData.incomeTaxableAmountTotal = reportData.incomeTaxableAmount_0 + reportData.incomeTaxableAmount_10 + reportData.incomeTaxableAmount_15 + reportData.incomeTaxableAmount_20 + reportData.incomeTaxableAmount_25 + reportData.incomeTaxableAmount_30;
                    reportData.individualTaxLiabilityAmountTotal = TaxPayableAmount;

                    //........ Constant Head........... //

                    var incomeTaxParamDetails = dataContext.prl_income_tax_parameter_details.FirstOrDefault(x => x.assesment_year == reportData.assesment_year);
                    var MaxInvPercentage = incomeTaxParamDetails.max_investment_percentage.Value;


                    decimal netTaxPayable = 0;

                    if (TaxPayableAmount == 0)
                    {
                        netTaxPayable = 0;
                    }
                    else
                    {
                        netTaxPayable = incomeTaxParamDetails.min_tax_amount.Value;
                    }


                    var totalTaxTillDate = dataContext.prl_salary_deductions.Where(x => x.emp_id == item.emp_id && x.deduction_name_id == 5 && x.salary_month >= frmDate && x.salary_month <= toDate).Sum(q => (decimal?)q.amount) ?? 0;
                    var taxDeductedThisMonth = dataContext.prl_salary_deductions.Where(x => x.emp_id == item.emp_id && x.deduction_name_id == 5 && x.salary_month.Month == SpDetails.salary_month.Month && x.salary_month.Year == SpDetails.salary_month.Year).Sum(q => (decimal?)q.amount) ?? 0;
                    var TaxToBeAdjusted = netTaxPayable - (totalTaxTillDate + taxDeductedThisMonth);

                    //************//
                    reportData.taxRefund = 0;
                    //************//
                    reportData.netTaxPayable = netTaxPayable;
                    reportData.totalTaxTillDate = totalTaxTillDate + taxDeductedThisMonth;
                    //reportData.taxDeductedThisMonth = taxDeductedThisMonth);
                    reportData.TaxToBeAdjusted = TaxToBeAdjusted - reportData.taxRefund;

                    EmpList.Add(reportData);
                }
            }
            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();

                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "IncomeTaxReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    ViewBag.Years = DateUtility.GetYears();
                    ViewBag.Months = DateUtility.GetMonths();
                    return View("ReportIncomeTax");
                }
                DateTime dt = new DateTime(it.Year, it.Month, 1);

                ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                lr.DataSources.Add(rd);
                //lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));
                string reportType = "EXCELOPENXML";
                string mimeType;
                string encoding;
                string fileNameExtension = "xlsx";
                //string fileNameExtension = string.Format("{0}.{1}", "IncomeTaxReport", "xlsx");

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
        public ActionResult TaxCardReport()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult TaxCardReport(int? empid, FormCollection collection, string sButton, ReportTaxCard rt)
        {
            bool errorFound = false;
            var res = new OperationResult();
            var TaxCard = new ReportTaxCard();

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

                            var salaryPD = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rt.Year && x.salary_month.Month == rt.Month);
                            if (salaryPD == null)
                            {
                                errorFound = true;
                                ModelState.AddModelError("", "No Record Found for the employee for the selected Month.");
                            }
                            else
                            {
                                var empTax = dataContext.prl_employee_tax_process.SingleOrDefault(x => x.emp_id == empid && x.salary_process_id == salaryPD.salary_process_id);
                                if (empTax == null)
                                {
                                    errorFound = true;
                                    ModelState.AddModelError("", "Tax has not been found for the employee for the selected Month Please select another month.");
                                }

                                if (!errorFound)
                                {
                                    /****************/

                                    var fiscalYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == empTax.fiscal_year_id);

                                    /****************/
                                    string reportType = "PDF";

                                    LocalReport lr = new LocalReport();
                                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "TaxCard.rdlc");
                                    if (System.IO.File.Exists(path))
                                    {
                                        lr.ReportPath = path;
                                    }
                                    else
                                    {
                                        ViewBag.Years = DateUtility.GetYears();
                                        ViewBag.Months = DateUtility.GetMonths();
                                        return View("TaxCardReport");
                                    }

                                    var reportData = new ReportTaxCard();
                                    var empDlist = new List<ReportTaxCard>();
                                    var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                                    reportData.eId = Emp.id;
                                    reportData.taxProcessId = empTax.id;
                                    reportData.empNo = Emp.emp_no;
                                    reportData.empName = Emp.name;
                                    reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";

                                    string dept = empD.department_id != 0 ? empD.prl_department.name : " ";
                                    string subDept = (empD.sub_department_id != null ? empD.sub_department_id : 0) != 0 ? empD.prl_sub_department.name : " ";
                                    string subSubDept = (empD.sub_sub_department_id != null ? empD.sub_sub_department_id : 0) != 0 ? empD.prl_sub_sub_department.name : " ";
                                    reportData.department = dept + " " + subDept + " " + subSubDept;
                                    reportData.category = empD.employee_category != null || empD.employee_category != "" ? empD.employee_category : " ";

                                    reportData.job_level = (empD.job_level_id != 0 && empD.job_level_id != null) ? empD.prl_job_level.title : " ";
                                    //reportData.job_location = empD.cost_centre_id != 0 ? empD.prl_cost_centre.cost_centre_name : " ";


                                    reportData.processId = salaryPD.salary_process_id;
                                    reportData.basicSalary = Convert.ToDecimal(salaryPD.this_month_basic);
                                    reportData.gender = Emp.gender;
                                    reportData.fiscal_year = fiscalYr.fiscal_year;
                                    reportData.assesment_year = fiscalYr.assesment_year;
                                    reportData.tin = Emp.tin != null || Emp.tin != "" ? Emp.tin : " ";

                                    reportData.bank = Emp.bank_id != null ? Emp.prl_bank.bank_name : " ";
                                    reportData.accNo = Emp.account_no != null || Emp.account_no != " " ? Emp.account_no : " ";
                                    reportData.routing_no = Emp.routing_no != null || Emp.routing_no != "" ? Emp.routing_no : " ";
                                    reportData.Year = rt.Year;
                                    reportData.MonthName = DateUtility.MonthName(rt.Month);

                                    reportData.monthYear = new DateTime(rt.Year, rt.Month, 1).AddMonths(1).AddDays(-1);

                                    //reportData.taxProcessId = empTax.id;

                                    int Yr = rt.Year;
                                    int currMonth = rt.Month;
                                    int fiscalYearStart = 7;
                                    string s_date = "";
                                    string e_date = "";
                                    string end_date = "";

                                    string projected_s_date = "";
                                    string projected_e_date = "";

                                    //TillDateRange Start
                                    if (currMonth > 6)
                                    {
                                        s_date = Yr.ToString() + "-" + fiscalYearStart + "-" + "1";
                                    }
                                    else
                                    {
                                        s_date = (Yr - 1).ToString() + "-" + fiscalYearStart + "-" + "1";
                                    }

                                    //TillDateRange End
                                    if (currMonth == 1)
                                    {
                                        e_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
                                        end_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
                                    }
                                    else if (currMonth == 7)
                                    {
                                        e_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
                                        end_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
                                    }
                                    else
                                    {
                                        DateTime endDT = new DateTime(Yr, currMonth - 1, 1).AddMonths(1).AddDays(-1);
                                        e_date = endDT.ToString("yyyy-MM-dd ");
                                        end_date = Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + 31;
                                    }

                                    //projectedDateRange

                                    if (currMonth == 6)
                                    {
                                        projected_s_date = "";
                                        projected_e_date = "";
                                    }

                                    else if (currMonth > 6 && currMonth < 12)
                                    {
                                        projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
                                        projected_e_date = (Yr + 1).ToString() + "-6-30";
                                    }
                                    else if (currMonth == 12)
                                    {
                                        projected_s_date = (Yr + 1).ToString() + "-1-31"; ;
                                        projected_e_date = (Yr + 1).ToString() + "-6-30";
                                    }
                                    else
                                    {
                                        projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
                                        projected_e_date = Yr.ToString() + "-6-30";
                                    }

                                    DateTime frmDate = Convert.ToDateTime(s_date);
                                    DateTime toDate = Convert.ToDateTime(e_date);

                                    if (projected_e_date != "" && projected_s_date != "")
                                    {
                                        DateTime p_frmDate = Convert.ToDateTime(projected_s_date);
                                        DateTime p_toDate = Convert.ToDateTime(projected_e_date);
                                        reportData.projectedDateRange = DateUtility.MonthName(p_frmDate.Month).Substring(0, 3) + "'" + p_frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(p_toDate.Month).Substring(0, 3) + "'" + p_toDate.Year.ToString().Substring(2);
                                    }
                                    else
                                    {
                                        reportData.projectedDateRange = "";
                                    }

                                    reportData.tillDateRange = DateUtility.MonthName(frmDate.Month).Substring(0, 3) + "'" + frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(toDate.Month).Substring(0, 3) + "'" + toDate.Year.ToString().Substring(2);
                                    reportData.currentMonth = DateUtility.MonthName(currMonth);

                                    //Slab Wise Tax
                                    var empTaxSlab = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).ToList();

                                    int row = 0;

                                    foreach (var item in empTaxSlab)
                                    {
                                        if (row == 0)
                                        {
                                            reportData.incomeTaxableAmount_0 = item.taxable_income;
                                            reportData.individualTaxLiabilityAmount_0 = item.tax_liability;
                                        }
                                        else if (row == 1)
                                        {
                                            reportData.incomeTaxableAmount_10 = item.taxable_income;
                                            reportData.individualTaxLiabilityAmount_10 = item.tax_liability;
                                        }
                                        else if (row == 2)
                                        {
                                            reportData.incomeTaxableAmount_15 = item.taxable_income;
                                            reportData.individualTaxLiabilityAmount_15 = item.tax_liability;
                                        }
                                        else if (row == 3)
                                        {
                                            reportData.incomeTaxableAmount_20 = item.taxable_income;
                                            reportData.individualTaxLiabilityAmount_20 = item.tax_liability;
                                        }
                                        else if (row == 4)
                                        {
                                            reportData.incomeTaxableAmount_25 = item.taxable_income;
                                            reportData.individualTaxLiabilityAmount_25 = item.tax_liability;
                                        }

                                        else if (row == 5)
                                        {
                                            reportData.incomeTaxableAmount_30 = item.taxable_income;
                                            reportData.individualTaxLiabilityAmount_30 = item.tax_liability;
                                        }

                                        row++;
                                    }

                                    //var _previousTax = dataContext.prl_employee_tax_process_detail.Where(e => e.fiscal_year_id == pFiscalYear && e.emp_id == empid).Sum(q => (decimal?)q.monthly_tax) ?? 0;
                                    decimal totalAnnualTillDate = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.till_date_income) ?? 0;
                                    decimal totalAnnualIncomeCurrentMonth = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.current_month_income) ?? 0;
                                    decimal totalAnnualIncomeProjected = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.projected_income) ?? 0;
                                    decimal totalLessExempted = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.less_exempted) ?? 0;
                                    decimal totalTaxableIncome = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.total_taxable_income) ?? 0;


                                    reportData.totalAnnualIncomeTillDate = totalAnnualTillDate;
                                    reportData.totalAnnualIncomeCurrentMonth = totalAnnualIncomeCurrentMonth;
                                    reportData.totalAnnualIncomeProjected = totalAnnualIncomeProjected;
                                    reportData.totalLessExempted = totalLessExempted;
                                    reportData.totalTaxableIncome = totalTaxableIncome;


                                    //decimal incomeTaxableAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => q.taxable_income);
                                    //decimal individualTaxLiabilityAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => q.tax_liability);

                                    decimal totalIncomeTaxAmountTotal = (decimal?)empTax.yearly_taxable_income ?? 0;
                                    //dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.taxable_income) ?? 0;
                                    decimal TaxPayableAmount = (decimal?)empTax.total_tax_payable ?? 0;
                                    //dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.tax_liability) ?? 0;

                                    reportData.incomeTaxableAmountTotal = totalIncomeTaxAmountTotal;
                                    reportData.individualTaxLiabilityAmountTotal = TaxPayableAmount;

                                    // PF

                                    decimal yearlyPF = 0;

                                    decimal PF_Contribution_Both_Parts = 0;
                                    var CompanyContributionToPF = yearlyPF;

                                    PF_Contribution_Both_Parts = CompanyContributionToPF * 2;

                                    //Investment
                                    var incomeTaxParamDetails = dataContext.prl_income_tax_parameter_details.FirstOrDefault(x => x.assesment_year == reportData.assesment_year);
                                    var MaxInvPercentage = incomeTaxParamDetails.max_investment_percentage.Value;

                                    decimal maxInvPercentRebate = incomeTaxParamDetails.max_inv_exempted_percentage.Value;
                                    decimal minInvPercentRebate = incomeTaxParamDetails.min_inv_exempted_percentage.Value;
                                    decimal maxAmountForMaxExemptionPercent = incomeTaxParamDetails.max_amount_for_max_exemption_percent.Value;
                                    decimal max_investment_allowed = incomeTaxParamDetails.max_investment_amount.Value;

                                    decimal totalRebate = 0;


                                    decimal ActualInvestementTotal = 0;

                                    var yearlyInvestmentTotal = dataContext.prl_employee_yearly_investment.SingleOrDefault(x => x.emp_id == empid && x.fiscal_year_id == empTax.fiscal_year_id);

                                    // is investment amount Edited or not?? // The investment amount only edited in last month (June) of the fiscal year. 

                                    if (yearlyInvestmentTotal != null && rt.Month == 6)
                                    {
                                        ActualInvestementTotal = (decimal?)yearlyInvestmentTotal.invested_amount ?? 0;
                                    }
                                    else
                                    {
                                        ActualInvestementTotal = totalTaxableIncome * MaxInvPercentage / 100;
                                    }

                                    var Other_Investment_except_PF = ActualInvestementTotal - PF_Contribution_Both_Parts;


                                    //rebate calculation
                                    #region Rebate Slab

                                    //if (TaxPayableAmount == 0)
                                    //{
                                    //    totalRebate = 0;
                                    //}
                                    //else if (ActualInvestementTotal > max_investment_allowed)
                                    //{
                                    //    totalRebate = (max_investment_allowed * minInvPercentRebate) / 100;
                                    //}
                                    //else
                                    //{
                                    //    if (totalTaxableIncome <= maxAmountForMaxExemptionPercent)
                                    //    {
                                    //        totalRebate = (ActualInvestementTotal * maxInvPercentRebate) / 100;
                                    //    }
                                    //    else
                                    //    {
                                    //        totalRebate = (ActualInvestementTotal * minInvPercentRebate) / 100;
                                    //    }
                                    //}


                                    //decimal firstRebateSlabAmount = 0;
                                    //decimal secondRebateSlabAmount = 0;
                                    //decimal thirdRebateSlabAmount = 0;

                                    decimal NetRebateAmount = (decimal?)empTax.inv_rebate ?? 0;



                                    // Old rebate policy//

                                    //if (ActualInvestementTotal > max_investment_allowed)
                                    //{
                                    //    decimal RemainingAmount = max_investment_allowed - 250000;
                                    //    firstRebateSlabAmount = (250000 * 15) / 100;  // Here MaxInvPerRebate = 15%

                                    //    secondRebateSlabAmount = (500000 * 12) / 100;
                                    //    decimal SecondRemainingAmount = RemainingAmount - 500000;

                                    //    thirdRebateSlabAmount = (SecondRemainingAmount * 10) / 100;

                                    //    NetRebateAmount = firstRebateSlabAmount + secondRebateSlabAmount + thirdRebateSlabAmount;
                                    //}
                                    //else
                                    //{
                                    //    if (ActualInvestementTotal <= 250000)
                                    //    {
                                    //        NetRebateAmount = ActualInvestementTotal * MaxInvPercentForRebate / 100; // On first Tk. 250,000 @15%
                                    //    }

                                    //    if (ActualInvestementTotal > 250000)
                                    //    {
                                    //        decimal RemainingAmount = ActualInvestementTotal - 250000;
                                    //        firstRebateSlabAmount = (250000 * 15) / 100;  // On first Tk. 250,000 @15%

                                    //        if (RemainingAmount <= 500000)
                                    //        {
                                    //            secondRebateSlabAmount = (RemainingAmount * 12) / 100; // On next Tk. 500,000 @12%
                                    //            NetRebateAmount = firstRebateSlabAmount + secondRebateSlabAmount + thirdRebateSlabAmount;
                                    //        }

                                    //        if (RemainingAmount > 500000)
                                    //        {
                                    //            secondRebateSlabAmount = (500000 * 12) / 100;  // On next Tk. 500,000 @12%

                                    //            decimal SecondRemainingAmount = RemainingAmount - 500000;

                                    //            thirdRebateSlabAmount = (SecondRemainingAmount * 10) / 100; //On balance amount @10%

                                    //            NetRebateAmount = firstRebateSlabAmount + secondRebateSlabAmount + thirdRebateSlabAmount;
                                    //        }
                                    //    }
                                    //}

                                    #endregion

                                    reportData.pf_Contribution_Both_Parts = PF_Contribution_Both_Parts;
                                    reportData.other_Investment_except_PF = Other_Investment_except_PF;
                                    reportData.actualInvestementTotal = ActualInvestementTotal;
                                    reportData.netRebateAmount = NetRebateAmount;

                                    decimal netTaxPayable = (decimal?)empTax.yearly_tax ?? 0;
                                    decimal paid_total = (decimal?)empTax.paid_total ?? 0;

                                    decimal totalTaxTillDate = dataContext.prl_employee_tax_process.Where(x => x.emp_id == empid && x.salary_month >= frmDate && x.salary_month <= toDate).Sum(q => (decimal?)q.monthly_tax) ?? 0;
                                    decimal taxDeductedThisMonth = (decimal?)empTax.monthly_tax ?? 0;

                                    decimal TaxToBeAdjusted = netTaxPayable - (totalTaxTillDate + taxDeductedThisMonth);

                                    reportData.netTaxPayable = netTaxPayable;
                                    reportData.taxPaidTotal = paid_total;

                                    reportData.totalTaxTillDate = totalTaxTillDate;
                                    reportData.taxDeductedThisMonth = taxDeductedThisMonth;
                                    reportData.TaxToBeAdjusted = TaxToBeAdjusted;

                                    empDlist.Add(reportData);

                                    ReportDataSource rd = new ReportDataSource("DataSet1", empDlist);
                                    lr.DataSources.Clear();

                                    lr.DataSources.Add(rd);

                                    lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_TCSubreportProcessing);

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
            }
            catch (Exception ex)
            {
                ViewBag.Allowance = new List<AllowanceDeduction>();
                ViewBag.Deduction = new List<AllowanceDeduction>();
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            TaxCard.Month = rt.Month;
            TaxCard.Year = rt.Year;
            TaxCard.MonthName = DateUtility.MonthName(rt.Month);

            return View(TaxCard);
        }

        void lr_TCSubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int taxProcessId = Convert.ToInt32(e.Parameters["taxProcessId"].Values[0]);
            int eId = Convert.ToInt32(e.Parameters["eId"].Values[0]);
            string tillDateRange = e.Parameters["tillDateRange"].Values[0];
            string projectedDateRange = e.Parameters["projectedDateRange"].Values[0];

            string pth = e.ReportPath;

            if (pth == "TaxCardIncomeHead")
            {
                var empTaxDetList = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == eId && x.tax_process_id == taxProcessId).ToList();

                var empTaxDlist = new List<ReportTaxCardIncomeHead>();

                foreach (var eTd in empTaxDetList)
                {
                    var TaxIH = new ReportTaxCardIncomeHead();

                    TaxIH.incomeAnnualGrossName = eTd.tax_item;
                    TaxIH.incomeTillDateAmount = (decimal?)eTd.till_date_income ?? 0;
                    TaxIH.incomeCurrentMonthAmount = (decimal?)eTd.current_month_income ?? 0;
                    TaxIH.incomeProjectedDateAmount = (decimal?)eTd.projected_income ?? 0;
                    TaxIH.exemptedLessAmount = (decimal?)eTd.less_exempted ?? 0;
                    TaxIH.incomeTotalTaxableAmount = (decimal?)eTd.total_taxable_income ?? 0;

                    empTaxDlist.Add(TaxIH);
                }

                e.DataSources.Add(new ReportDataSource("DataSet1", empTaxDlist));
            }
            else
            {

            }
        }

        //[PayrollAuthorize]
        //[HttpPost]
        //public ActionResult TaxCardReport(int? empid, FormCollection collection, string sButton, ReportTaxCard rt)
        //{
        //    bool errorFound = false;
        //    var res = new OperationResult();
        //    var TaxCard = new ReportTaxCard();

        //    try
        //    {
        //        if (sButton != null)
        //        {
        //            if (sButton == "Download")
        //            {
        //                if (empid == null)
        //                {
        //                    errorFound = true;
        //                    ViewBag.Allowance = new List<AllowanceDeduction>();
        //                    ViewBag.Deduction = new List<AllowanceDeduction>();
        //                    ModelState.AddModelError("", "Please select an employee or employee no.");
        //                }
        //                else
        //                {
        //                    var Emp = new Employee();
        //                    if (empid != null)
        //                    {
        //                        var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
        //                        Emp = Mapper.Map<Employee>(_empD);
        //                    }

        //                    var salaryPD = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == rt.Year && x.salary_month.Month == rt.Month);
        //                    if (salaryPD == null)
        //                    {
        //                        errorFound = true;
        //                        ModelState.AddModelError("", "No Record Found for the employee for the selected Month.");
        //                    }
        //                    else
        //                    {

        //                        var empTaxProcess = dataContext.prl_employee_tax_process.SingleOrDefault(x => x.emp_id == empid && x.salary_process_id == salaryPD.salary_process_id);
        //                        if (empTaxProcess == null)
        //                        {
        //                            errorFound = true;
        //                            ModelState.AddModelError("", "Tax has not been processed for the employee for the selected Month.");
        //                        }

        //                        if (!errorFound)
        //                        {
        //                            /****************/
        //                            string reportType = "PDF";

        //                            LocalReport lr = new LocalReport();
        //                            string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "TaxCard.rdlc");
        //                            if (System.IO.File.Exists(path))
        //                            {
        //                                lr.ReportPath = path;
        //                            }
        //                            else
        //                            {
        //                                ViewBag.Years = DateUtility.GetYears();
        //                                ViewBag.Months = DateUtility.GetMonths();
        //                                return View("TaxCardReport");
        //                            }

        //                            var reportData = new ReportTaxCard();
        //                            var empDlist = new List<ReportTaxCard>();
        //                            var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

        //                            reportData.eId = Emp.id;
        //                            reportData.empNo = Emp.emp_no;
        //                            reportData.empName = Emp.name;
        //                            reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";
        //                            reportData.division = empD.division_id != 0 ? empD.prl_division.name : "N/A";
        //                            reportData.processId = salaryPD.salary_process_id;
        //                            reportData.basicSalary = Convert.ToDecimal(salaryPD.this_month_basic);
        //                            reportData.gender = Emp.gender;
        //                            reportData.fiscal_year = empTaxProcess.prl_fiscal_year.fiscal_year;
        //                            reportData.assesment_year = empTaxProcess.prl_fiscal_year.assesment_year;
        //                            reportData.tin = Emp.tin;
        //                            reportData.bank = Emp.prl_bank.bank_name;
        //                            reportData.accNo = Emp.account_no;

        //                            reportData.Year = rt.Year;
        //                            reportData.MonthName = DateUtility.MonthName(rt.Month);

        //                            reportData.monthYear = empTaxProcess.salary_month;
        //                            reportData.taxProcessId = empTaxProcess.id;

        //                            int Yr = rt.Year;
        //                            int currMonth = rt.Month;
        //                            int fiscalYearStart = 7;
        //                            string s_date = "";
        //                            string e_date = "";
        //                            string end_date = "";

        //                            string projected_s_date = "";
        //                            string projected_e_date = "";

        //                            //TillDateRange Start
        //                            if (currMonth > 6)
        //                            {
        //                                s_date = Yr.ToString() + "-" + fiscalYearStart + "-" + "1";
        //                            }
        //                            else
        //                            {
        //                                s_date = (Yr - 1).ToString() + "-" + fiscalYearStart + "-" + "1";
        //                            }

        //                            //TillDateRange End
        //                            if (currMonth == 1)
        //                            {
        //                                e_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
        //                                end_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
        //                            }
        //                            else if (currMonth == 7)
        //                            {
        //                                e_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
        //                                end_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
        //                            }
        //                            else
        //                            {
        //                                e_date = Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + 28;
        //                                end_date = Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + 31;
        //                            }

        //                            //projectedDateRange

        //                            if (currMonth == 6)
        //                            {
        //                                projected_s_date = "";
        //                                projected_e_date = "";
        //                            }
        //                            else if (currMonth > 6 && currMonth < 12)
        //                            {
        //                                projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
        //                                projected_e_date = (Yr + 1).ToString() + "-6-30";
        //                            }
        //                            else if (currMonth == 12)
        //                            {
        //                                projected_s_date = (Yr + 1).ToString() + "-1-31"; ;
        //                                projected_e_date = (Yr + 1).ToString() + "-6-30";
        //                            }
        //                            else
        //                            {
        //                                projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
        //                                projected_e_date = Yr.ToString() + "-6-30";
        //                            }

        //                            DateTime frmDate = Convert.ToDateTime(s_date);
        //                            DateTime toDate = Convert.ToDateTime(e_date);


        //                            if (projected_e_date != "" && projected_s_date != "")
        //                            {
        //                                DateTime p_frmDate = Convert.ToDateTime(projected_s_date);
        //                                DateTime p_toDate = Convert.ToDateTime(projected_e_date);
        //                                reportData.projectedDateRange = DateUtility.MonthName(p_frmDate.Month).Substring(0, 3) + "'" + p_frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(p_toDate.Month).Substring(0, 3) + "'" + p_toDate.Year.ToString().Substring(2);
        //                            }
        //                            else
        //                            {
        //                                reportData.projectedDateRange = "";
        //                            }

        //                            reportData.tillDateRange = DateUtility.MonthName(frmDate.Month).Substring(0, 3) + "'" + frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(toDate.Month).Substring(0, 3) + "'" + toDate.Year.ToString().Substring(2);
        //                            reportData.currentMonth = DateUtility.MonthName(currMonth);

        //                            //Tax Slab Wise data

        //                            var empTaxSlabList = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).ToList();

        //                            foreach (var item in empTaxSlabList)
        //                            {
        //                                if (item.current_rate == 0)
        //                                {
        //                                    reportData.incomeTaxableAmount_0 = item.taxable_income);
        //                                    reportData.individualTaxLiabilityAmount_0 = item.tax_liability);
        //                                }

        //                                if (item.current_rate == 10)
        //                                {
        //                                    reportData.incomeTaxableAmount_10 = item.taxable_income);
        //                                    reportData.individualTaxLiabilityAmount_10 = item.tax_liability);
        //                                }

        //                                if (item.current_rate == 15)
        //                                {
        //                                    reportData.incomeTaxableAmount_15 = item.taxable_income);
        //                                    reportData.individualTaxLiabilityAmount_15 = item.tax_liability);
        //                                }

        //                                if (item.current_rate == 20)
        //                                {
        //                                    reportData.incomeTaxableAmount_20 = item.taxable_income);
        //                                    reportData.individualTaxLiabilityAmount_20 = item.tax_liability);
        //                                }

        //                                if (item.current_rate == 25)
        //                                {
        //                                    reportData.incomeTaxableAmount_25 = item.taxable_income);
        //                                    reportData.individualTaxLiabilityAmount_25 = item.tax_liability);
        //                                }

        //                                if (item.current_rate == 30)
        //                                {
        //                                    reportData.incomeTaxableAmount_30 = item.taxable_income);
        //                                    reportData.individualTaxLiabilityAmount_30 = item.tax_liability);
        //                                }

        //                            }

        //                            //Grand Total Everything//

        //                            var totalAnnualIncomeTillDate = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.till_date_income);
        //                            var totalAnnualIncomeCurrentMonth = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.current_month_income);
        //                            var totalAnnualIncomeProjected = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.projected_income);
        //                            var totalLessExempted = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.less_exempted);
        //                            var totalTaxableIncome = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.total_taxable_income);

        //                            reportData.totalAnnualIncomeTillDate = totalAnnualIncomeTillDate;
        //                            reportData.totalAnnualIncomeCurrentMonth = totalAnnualIncomeCurrentMonth;
        //                            reportData.totalAnnualIncomeProjected = totalAnnualIncomeProjected;
        //                            reportData.totalLessExempted = totalLessExempted;
        //                            reportData.totalTaxableIncome = totalTaxableIncome;


        //                            decimal incomeTaxableAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.taxable_income);
        //                            decimal individualTaxLiabilityAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id).Sum(q => q.tax_liability);

        //                            reportData.incomeTaxableAmountTotal = incomeTaxableAmountTotal;
        //                            reportData.individualTaxLiabilityAmountTotal = individualTaxLiabilityAmountTotal;



        //                            //........ Constant Head........... //
        //                            decimal PF_Contribution_Both_Parts = 0;

        //                            if (Emp.is_confirmed == true)
        //                            {
        //                                var CompanyContributionToPF = dataContext.prl_employee_tax_process_detail.FirstOrDefault(x => x.emp_id == empid && x.tax_process_id == empTaxProcess.id && x.tax_item == "Company Contribution to PF").total_taxable_income;
        //                                PF_Contribution_Both_Parts = CompanyContributionToPF * 2;
        //                            }


        //                            var incomeTaxParamDetails = dataContext.prl_income_tax_parameter_details.FirstOrDefault(x => x.fiscal_year_id == empTaxProcess.fiscal_year_id);
        //                            var MaxInvPercentage = incomeTaxParamDetails.max_investment_percentage.Value;

        //                            decimal ActualInvestementTotal = totalTaxableIncome * MaxInvPercentage / 100;
        //                            var Other_Investment_except_PF = ActualInvestementTotal - PF_Contribution_Both_Parts;

        //                            #region Rebate Slab
        //                            decimal MaxInvPercentForRebate = incomeTaxParamDetails.max_inv_exempted_percentage.Value;
        //                            decimal max_investment_allowed = incomeTaxParamDetails.max_investment_amount.Value;

        //                            decimal firstRebateSlabAmount = 0;
        //                            decimal secondRebateSlabAmount = 0;
        //                            decimal thirdRebateSlabAmount = 0;

        //                            decimal NetRebateAmount = 0;

        //                            if (ActualInvestementTotal > max_investment_allowed)
        //                            {
        //                                decimal RemainingAmount = max_investment_allowed - 250000;
        //                                firstRebateSlabAmount = (250000 * 15) / 100;  // Here MaxInvPerRebate = 15%

        //                                secondRebateSlabAmount = (500000 * 12) / 100;
        //                                decimal SecondRemainingAmount = RemainingAmount - 500000;

        //                                thirdRebateSlabAmount = (SecondRemainingAmount * 10) / 100;

        //                                NetRebateAmount = firstRebateSlabAmount + secondRebateSlabAmount + thirdRebateSlabAmount;
        //                            }
        //                            else
        //                            {
        //                                if (ActualInvestementTotal <= 250000)
        //                                {
        //                                    NetRebateAmount = ActualInvestementTotal * MaxInvPercentForRebate / 100; // On first Tk. 250,000 @15%
        //                                }

        //                                if (ActualInvestementTotal > 250000)
        //                                {
        //                                    decimal RemainingAmount = ActualInvestementTotal - 250000;
        //                                    firstRebateSlabAmount = (250000 * 15) / 100;  // On first Tk. 250,000 @15%

        //                                    if (RemainingAmount <= 500000)
        //                                    {
        //                                        secondRebateSlabAmount = (RemainingAmount * 12) / 100; // On next Tk. 500,000 @12%
        //                                        NetRebateAmount = firstRebateSlabAmount + secondRebateSlabAmount + thirdRebateSlabAmount;
        //                                    }

        //                                    if (RemainingAmount > 500000)
        //                                    {
        //                                        secondRebateSlabAmount = (500000 * 12) / 100;  // On next Tk. 500,000 @12%

        //                                        decimal SecondRemainingAmount = RemainingAmount - 500000;

        //                                        thirdRebateSlabAmount = (SecondRemainingAmount * 10) / 100; //On balance amount @10%

        //                                        NetRebateAmount = firstRebateSlabAmount + secondRebateSlabAmount + thirdRebateSlabAmount;
        //                                    }
        //                                }
        //                            }
        //                            #endregion

        //                            decimal netTaxPayable = 0;

        //                            if (individualTaxLiabilityAmountTotal == 0)
        //                            {
        //                                netTaxPayable = 0;
        //                            }
        //                            else if (NetRebateAmount > individualTaxLiabilityAmountTotal)
        //                            {
        //                                netTaxPayable = incomeTaxParamDetails.min_tax_amount.Value;
        //                            }
        //                            else
        //                            {
        //                                if ((individualTaxLiabilityAmountTotal - NetRebateAmount) <= incomeTaxParamDetails.min_tax_amount.Value)
        //                                {
        //                                    netTaxPayable = incomeTaxParamDetails.min_tax_amount.Value;
        //                                }
        //                                else
        //                                {
        //                                    netTaxPayable = individualTaxLiabilityAmountTotal - NetRebateAmount;
        //                                }
        //                            }

        //                            DateTime stratDate = Convert.ToDateTime(s_date);
        //                            DateTime endDate = Convert.ToDateTime(end_date);

        //                            var totalTaxTillDate = dataContext.prl_employee_tax_process.Where(e => e.fiscal_year_id == empTaxProcess.fiscal_year_id && e.emp_id == empid && e.salary_month >= stratDate && e.salary_month <= endDate).Sum(q => (decimal?)q.monthly_tax) ?? 0;
        //                            var taxDeductedThisMonth = dataContext.prl_employee_tax_process.FirstOrDefault(e => e.fiscal_year_id == empTaxProcess.fiscal_year_id && e.emp_id == empid && e.salary_process_id == salaryPD.salary_process_id).monthly_tax;
        //                            var TaxToBeAdjusted = netTaxPayable - (totalTaxTillDate + taxDeductedThisMonth);

        //                            reportData.pf_Contribution_Both_Parts = PF_Contribution_Both_Parts);
        //                            reportData.other_Investment_except_PF = Other_Investment_except_PF);
        //                            reportData.actualInvestementTotal = ActualInvestementTotal);
        //                            reportData.netRebateAmount = NetRebateAmount);
        //                            reportData.netTaxPayable = netTaxPayable);
        //                            reportData.totalTaxTillDate = totalTaxTillDate);
        //                            reportData.taxDeductedThisMonth = taxDeductedThisMonth);
        //                            reportData.TaxToBeAdjusted = TaxToBeAdjusted);

        //                            empDlist.Add(reportData);

        //                            ReportDataSource rd = new ReportDataSource("DataSet1", empDlist);
        //                            lr.DataSources.Add(rd);
        //                            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_TCSubreportProcessing);

        //                            string mimeType;
        //                            string encoding;
        //                            string fileNameExtension;

        //                            string deviceInfo =
        //                            "<DeviceInfo>" +
        //                            "<OutputFormat>PDF</OutputFormat>" +
        //                            "</DeviceInfo>";

        //                            Warning[] warnings;
        //                            string[] streams;
        //                            byte[] renderedBytes;

        //                            renderedBytes = lr.Render(
        //                                reportType,
        //                                deviceInfo,
        //                                out mimeType,
        //                                out encoding,
        //                                out fileNameExtension,
        //                                out streams,
        //                                out warnings);

        //                            return File(renderedBytes, mimeType);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Allowance = new List<AllowanceDeduction>();
        //        ViewBag.Deduction = new List<AllowanceDeduction>();
        //        ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //    ViewBag.Years = DateUtility.GetYears();
        //    ViewBag.Months = DateUtility.GetMonths();

        //    TaxCard.Month = rt.Month;
        //    TaxCard.Year = rt.Year;
        //    TaxCard.MonthName = DateUtility.MonthName(rt.Month);

        //    return View(TaxCard);
        //}

        //void lr_TCSubreportProcessing(object sender, SubreportProcessingEventArgs e)
        //{
        //    int eId = Convert.ToInt32(e.Parameters["eId"].Values[0]);
        //    int taxProcessId = Convert.ToInt32(e.Parameters["taxProcessId"].Values[0]);
        //    string tillDateRange = e.Parameters["tillDateRange"].Values[0];
        //    string projectedDateRange = e.Parameters["projectedDateRange"].Values[0];

        //    string pth = e.ReportPath;

        //    if (pth == "TaxCardIncomeHead")
        //    {
        //        var empTaxProcessDetails = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == eId && x.tax_process_id == taxProcessId).Select(x => new ReportTaxCardIncomeHead
        //        {
        //            incomeAnnualGrossName = x.tax_item,
        //            incomeTillDateAmount = x.till_date_income != 0 ? x.till_date_income : 0,
        //            incomeCurrentMonthAmount = x.current_month_income != 0 ? x.current_month_income : 0,
        //            incomeProjectedDateAmount = x.projected_income != 0 ? x.projected_income : 0,
        //            exemptedLessAmount = x.less_exempted != 0 ? x.less_exempted : 0,
        //            incomeTotalTaxableAmount =x.total_taxable_income != 0 ? x.total_taxable_income : 0,
        //            tillDateRange = tillDateRange,
        //            projectedDateRange = projectedDateRange,
        //        }).ToList();

        //        e.DataSources.Add(new ReportDataSource("DataSet1", empTaxProcessDetails));
        //    }
        //    else
        //    {

        //    }
        //}

        public int FindFiscalYear(DateTime processDate)
        {
            int fiscalyR = 0;
            int _month = processDate.Month;
            int _yr = processDate.Year;
            string curYear = string.Empty;
            //string prevYear = string.Empty;

            if (_month <= 6)
            {
                //prevYear = (_yr - 2).ToString() + "-" + (_yr - 1).ToString();
                curYear = (_yr - 1).ToString() + "-" + _yr.ToString();
            }
            else if (_month > 6)
            {
                curYear = _yr.ToString() + "-" + (_yr + 1).ToString();
                //prevYear = (_yr - 1).ToString() + "-" + _yr.ToString();
            }

            var fisYr = dataContext.prl_fiscal_year.Where(f => f.fiscal_year.Substring(0, 9) == curYear).ToList();

            if (fisYr != null)
            {
                if (fisYr.Count > 1)
                {
                    foreach (var item in fisYr)
                    {
                        if (curYear == item.fiscal_year)
                        {
                            fiscalyR = item.id;
                        }
                    }
                }
                else if (fisYr.Count == 1)
                {
                    fiscalyR = fisYr[0].id;
                }
                else
                {
                    throw new Exception();
                }
            }
            return fiscalyR;
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult Tax108Report()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();

            return View(new ReportIncomeTax108
            {
                //RType. = true
            });
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Tax108Report(ReportIncomeTax108 it, string sButton)
        {
            bool errorFound = false;
            var res = new OperationResult();
            var empNumber = new List<string>();
            var empIds = new List<int>();
            var employeeList = new List<prl_employee_details>();
            var EmpList = new List<ReportIncomeTax108>();

            int Yr = it.Year;
            int currMonth = it.Month;
            int fiscalYearStart = 7;
            string s_date = "";
            string e_date = "";
            string end_date = "";

            string projected_s_date = "";
            string projected_e_date = "";

            //TillDateRange Start
            if (currMonth > 6)
            {
                s_date = Yr.ToString() + "-" + fiscalYearStart + "-" + "1";
            }
            else
            {
                s_date = (Yr - 1).ToString() + "-" + fiscalYearStart + "-" + "1";
            }

            //TillDateRange End
            if (currMonth == 1)
            {
                e_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
                end_date = (Yr - 1).ToString() + "-" + 12 + "-" + 31;
            }
            else if (currMonth == 7)
            {
                e_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
                end_date = Yr.ToString() + "-" + currMonth.ToString() + "-" + 01;
            }
            else
            {
                DateTime endDT = new DateTime(Yr, currMonth - 1, 1).AddMonths(1).AddDays(-1);
                e_date = endDT.ToString("yyyy-MM-dd ");
                end_date = Yr.ToString() + "-" + (currMonth - 1).ToString() + "-" + 31;
            }

            //projectedDateRange

            if (currMonth == 6)
            {
                projected_s_date = "";
                projected_e_date = "";
            }
            else if (currMonth > 6 && currMonth < 12)
            {
                projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
                projected_e_date = (Yr + 1).ToString() + "-6-30";
            }
            else if (currMonth == 12)
            {
                projected_s_date = (Yr + 1).ToString() + "-1-31"; ;
                projected_e_date = (Yr + 1).ToString() + "-6-30";
            }
            else
            {
                projected_s_date = Yr.ToString() + "-" + (currMonth + 1) + "-" + "1";
                projected_e_date = Yr.ToString() + "-6-30";
            }

            DateTime frmDate = Convert.ToDateTime(s_date);
            DateTime toDate = Convert.ToDateTime(e_date);

            var discontEmpIds = new List<int>();
            discontEmpIds = dataContext.prl_employee_discontinue.Where(ed => ed.discontinue_date < frmDate).Select(x => x.emp_id).Distinct().ToList();
            var taxEmpIds = new List<int>();
            empIds = dataContext.prl_employee.Where(x => !discontEmpIds.Contains(x.id)).Select(x => x.id).Distinct().ToList();
            taxEmpIds = dataContext.prl_employee_tax_process.Where(x => empIds.Contains(x.emp_id) && x.salary_month.Year == it.Year && x.salary_month.Month == it.Month).Select(x => x.emp_id).Distinct().ToList();
            employeeList = dataContext.prl_employee_details.Include("prl_employee").AsEnumerable().Where(x => taxEmpIds.Contains(x.emp_id)).GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First()).ToList();

            foreach (var item in employeeList)
            {
                var Emp = new Employee();

                var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == item.emp_id);
                Emp = Mapper.Map<Employee>(_empD);

                var salaryPD = dataContext.prl_salary_process_detail.SingleOrDefault(x => x.emp_id == Emp.id && x.salary_month.Year == it.Year && x.salary_month.Month == it.Month);

                var empTax = dataContext.prl_employee_tax_process.SingleOrDefault(x => x.emp_id == item.emp_id && x.salary_process_id == salaryPD.salary_process_id);
                if (empTax == null)
                {
                    errorFound = true;
                    ModelState.AddModelError("", "Tax has not been found for the employee for the selected Month Please select another month.");
                }

                if (!errorFound)
                {
                    /****************/

                    var fiscalYr = dataContext.prl_fiscal_year.FirstOrDefault(x => x.id == empTax.fiscal_year_id);

                    /****************/
                    string reportType = "PDF";

                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "TaxCard.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        ViewBag.Years = DateUtility.GetYears();
                        ViewBag.Months = DateUtility.GetMonths();
                        return View("TaxCardReport");
                    }

                    var reportData = new ReportIncomeTax108();
                    var empDlist = new List<ReportIncomeTax108>();
                    var empD = Mapper.Map<EmployeeDetails>(Emp.prl_employee_details.OrderByDescending(x => x.id).First());

                    reportData.empId = Emp.id;

                    reportData.empNo = Emp.emp_no;
                    reportData.empName = Emp.name;
                    reportData.designation = empD.designation_id != 0 ? empD.prl_designation.name : " ";

                    string dept = empD.department_id != 0 ? empD.prl_department.name : " ";
                    string subDept = (empD.sub_department_id != null ? empD.sub_department_id : 0) != 0 ? empD.prl_sub_department.name : " ";
                    string subSubDept = (empD.sub_sub_department_id != null ? empD.sub_sub_department_id : 0) != 0 ? empD.prl_sub_sub_department.name : " ";
                    reportData.department = dept + " " + subDept + " " + subSubDept;
                    reportData.emp_category = empD.employee_category;

                    reportData.job_level = empD.job_level_id != 0 ? empD.prl_job_level.title : " ";
                    //reportData.job_location = empD.cost_centre_id != 0 ? empD.prl_cost_centre.cost_centre_name : " ";
                    reportData.joining_date = Emp.joining_date.ToString("dd-MMM-yyyy");

                    reportData.basic = Convert.ToDecimal(salaryPD.this_month_basic);
                    reportData.gender = Emp.gender;
                    reportData.fiscal_year = fiscalYr.fiscal_year;
                    reportData.assesment_year = fiscalYr.assesment_year;
                    reportData.tin = Emp.tin;

                    reportData.bank = Emp.bank_id != null ? Emp.prl_bank.bank_name : " ";
                    reportData.accNo = Emp.account_no;
                    reportData.routing_no = Emp.routing_no;


                    reportData.month_name = DateUtility.MonthName(it.Month);

                    reportData.Year = it.Year;
                    reportData.month_name = DateUtility.MonthName(it.Month);


                    //reportData.taxProcessId = empTax.id;

                    if (projected_e_date != "" && projected_s_date != "")
                    {
                        DateTime p_frmDate = Convert.ToDateTime(projected_s_date);
                        DateTime p_toDate = Convert.ToDateTime(projected_e_date);
                        reportData.projectedDateRange = DateUtility.MonthName(p_frmDate.Month).Substring(0, 3) + "'" + p_frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(p_toDate.Month).Substring(0, 3) + "'" + p_toDate.Year.ToString().Substring(2);
                    }
                    else
                    {
                        reportData.projectedDateRange = "";
                    }

                    reportData.tillDateRange = DateUtility.MonthName(frmDate.Month).Substring(0, 3) + "'" + frmDate.Year.ToString().Substring(2) + "-" + DateUtility.MonthName(toDate.Month).Substring(0, 3) + "'" + toDate.Year.ToString().Substring(2);
                    reportData.currentMonth = DateUtility.MonthName(currMonth);

                    decimal taxableIncome = 0;



                    DateTime fiscalYrStart = Convert.ToDateTime(s_date);
                    DateTime fiscalYrEnd = Convert.ToDateTime((fiscalYrStart.Year + 1) + "-" + 6 + "-" + 30);

                    reportData.fiscal_yr_start = fiscalYrStart.ToString("dd-MMM-yyyy");
                    reportData.fiscal_yr_end = fiscalYrEnd.ToString("dd-MMM-yyyy");

                    var bonusIds = dataContext.prl_allowance_name.AsEnumerable().Where(x => x.allowance_head_id == 18).ToList();

                    var empTaxD = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).ToList();

                    foreach (var a in empTaxD)
                    {
                        if (a.tax_item.Contains("Basic Salary"))
                        {
                            reportData.basic = a.total_taxable_income;

                        }
                        else if (a.tax_item.Contains("House Rent Allowance"))
                        {
                            reportData.hr_allowance = a.gross_annual_income;
                            reportData.exemption_hr = a.less_exempted;
                            reportData.taxable_hr = a.total_taxable_income;
                        }
                        else if (a.tax_item.Contains("Medical Allowance"))
                        {
                            reportData.medical_allowance = a.gross_annual_income;
                            reportData.exemption_medical = a.less_exempted;
                            reportData.taxable_medical = a.total_taxable_income;
                        }

                        else if (a.tax_item.Contains("Conveyance Allowance"))
                        {
                            reportData.conveyance_allowance = a.gross_annual_income;
                            reportData.exemption_conv = a.less_exempted;
                            reportData.taxable_conv = a.total_taxable_income;
                        }
                        else if (a.tax_item.Contains("Festival Bonus"))
                        {
                            reportData.bonus = a.total_taxable_income;
                        }
                        //else if (a.tax_item.Contains("Tax Refund"))
                        //{
                        //    reportData.bonus = a.total_taxable_income;
                        //}
                        else
                        {
                            reportData.other_allowance += a.total_taxable_income;
                        }
                    }

                    //Slab Wise Tax
                    var empTaxSlab = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).ToList();

                    int row = 0;

                    foreach (var eTs in empTaxSlab)
                    {
                        if (row == 0)
                        {
                            reportData.incomeTaxableAmount_0 = eTs.taxable_income;
                            reportData.individualTaxLiabilityAmount_0 = eTs.tax_liability;
                        }
                        else if (row == 1)
                        {
                            reportData.incomeTaxableAmount_10 = eTs.taxable_income;
                            reportData.individualTaxLiabilityAmount_10 = eTs.tax_liability;
                        }
                        else if (row == 2)
                        {
                            reportData.incomeTaxableAmount_15 = eTs.taxable_income;
                            reportData.individualTaxLiabilityAmount_15 = eTs.tax_liability;
                        }
                        else if (row == 3)
                        {
                            reportData.incomeTaxableAmount_20 = eTs.taxable_income;
                            reportData.individualTaxLiabilityAmount_20 = eTs.tax_liability;
                        }
                        else if (row == 4)
                        {
                            reportData.incomeTaxableAmount_25 = eTs.taxable_income;
                            reportData.individualTaxLiabilityAmount_25 = eTs.tax_liability;
                        }

                        else if (row == 5)
                        {
                            reportData.incomeTaxableAmount_30 = eTs.taxable_income;
                            reportData.individualTaxLiabilityAmount_30 = eTs.tax_liability;
                        }

                        row++;
                    }

                    //var _previousTax = dataContext.prl_employee_tax_process_detail.Where(e => e.fiscal_year_id == pFiscalYear && e.emp_id == item.emp_id).Sum(q => (decimal?)q.monthly_tax) ?? 0;
                    decimal totalAnnualTillDate = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.till_date_income) ?? 0;
                    decimal totalAnnualIncomeCurrentMonth = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.current_month_income) ?? 0;
                    decimal totalAnnualIncomeProjected = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.projected_income) ?? 0;
                    decimal totalLessExempted = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.less_exempted) ?? 0;
                    decimal totalTaxableIncome = dataContext.prl_employee_tax_process_detail.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.total_taxable_income) ?? 0;


                    reportData.totalAnnualIncomeTillDate = totalAnnualTillDate;
                    reportData.totalAnnualIncomeCurrentMonth = totalAnnualIncomeCurrentMonth;
                    reportData.totalAnnualIncomeProjected = totalAnnualIncomeProjected;
                    reportData.totalLessExempted = totalLessExempted;
                    reportData.totalTaxableIncome = totalTaxableIncome;


                    decimal totalIncomeTaxAmountTotal = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.taxable_income) ?? 0;
                    decimal TaxPayableAmount = dataContext.prl_employee_tax_slab.Where(x => x.emp_id == item.emp_id && x.tax_process_id == empTax.id).Sum(q => (decimal?)q.tax_liability) ?? 0;

                    reportData.incomeTaxableAmountTotal = totalIncomeTaxAmountTotal;
                    reportData.individualTaxLiabilityAmountTotal = TaxPayableAmount;

                    //........ Constant Head........... //

                    var incomeTaxParamDetails = dataContext.prl_income_tax_parameter_details.FirstOrDefault(x => x.assesment_year == reportData.assesment_year);
                    var MaxInvPercentage = incomeTaxParamDetails.max_investment_percentage.Value;



                    decimal netTaxPayable = (decimal?)empTax.yearly_tax ?? 0;
                    decimal totalTaxTillDate = dataContext.prl_employee_tax_process.Where(x => x.emp_id == item.emp_id && x.salary_month >= frmDate && x.salary_month <= toDate).Sum(q => (decimal?)q.monthly_tax) ?? 0;
                    decimal taxDeductedThisMonth = (decimal?)empTax.monthly_tax ?? 0;

                    decimal paid_total = dataContext.prl_employee_tax_process.Where(x => x.emp_id == item.emp_id && x.salary_month >= frmDate && x.salary_month <= toDate).Sum(q => (decimal?)q.paid_total) ?? 0;

                    decimal TaxToBeAdjusted = netTaxPayable - (totalTaxTillDate + taxDeductedThisMonth);


                    //************//
                    reportData.taxRefund = 0;
                    //************//

                    reportData.netTaxPayable = netTaxPayable;
                    reportData.totalTaxTillDate = totalTaxTillDate + taxDeductedThisMonth; ;
                    //reportData.taxDeductedThisMonth = taxDeductedThisMonth);
                    reportData.TaxToBeAdjusted = TaxToBeAdjusted - reportData.taxRefund; ;

                    EmpList.Add(reportData);
                }
            }
            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();

                string path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "Tax108Report.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    ViewBag.Years = DateUtility.GetYears();
                    ViewBag.Months = DateUtility.GetMonths();
                    return View("Tax108Report");
                }
                DateTime dt = new DateTime(it.Year, it.Month, 1);

                ReportDataSource rd = new ReportDataSource("DataSet1", EmpList);
                lr.DataSources.Add(rd);
                //lr.SetParameters(new ReportParameter("monthYr", dt.ToString("MM,yyy")));
                string reportType = "EXCELOPENXML";
                string mimeType;
                string encoding;
                string fileNameExtension = "xlsx";
                //string fileNameExtension = string.Format("{0}.{1}", "IncomeTaxReport", "xlsx");

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
        [HttpPost]
        public PartialViewResult SearchTaxParameter(IncomeTaxParameter _param)
        {
            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            var allowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);
            AllowanceUploadView up = new AllowanceUploadView();
            up.AllowanceNames = allowanceNames;

            return PartialView("_SearchTaxParameter", up);
        }

        [HttpGet]
        public ActionResult UploadTaxRefund()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult UploadTaxRefundForm()
        {
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxRefundUploadView tr = new TaxRefundUploadView();
            tr.FiscalYears = fiscalYears;
            return PartialView("_TaxRefundUploadForm", tr);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult UploadTaxRefund(TaxRefundUploadView trV, HttpPostedFileBase fileupload)
        {
            var lstDat = new List<IncomeTaxRefund>();
            if (ModelState.IsValid)
            {
                try
                {
                    fileupload.InputStream.Position = 0;
                    using (var package = new ExcelPackage(fileupload.InputStream))
                    {
                        var ws = package.Workbook.Worksheets.First();
                        var startRow = 2;
                        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            var d = new IncomeTaxRefund();

                            if (ws.Cells[rowNum, 1].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + "does not have an employee ID");
                            }
                            else
                            {
                                d.EmployeeID = ws.Cells[rowNum, 1].Value.ToString();
                            }

                            if (ws.Cells[rowNum, 2].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + "does not have amount");
                            }
                            else
                            {
                                decimal val = 0;
                                if (decimal.TryParse(ws.Cells[rowNum, 2].Value.ToString(), out val))
                                {
                                    d.refund_amount = val;
                                }
                                else
                                {
                                    d.ErrorMsg.Add("Row " + rowNum + " amount column should have decimal value");
                                }
                            }
                            lstDat.Add(d);
                        }
                    }
                    HttpContext.Cache.Insert("currentTaxRefundUploadInfo", trV, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    HttpContext.Cache.Insert("currentTaxRefundUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                }
                catch (Exception ex)
                {
                    var d = ex.Message;
                }
            }
            else
            {
                return View(trV);
            }
            return Json(new { isUploaded = true, message = "hello" }, "text/html");
        }

        public PartialViewResult LoadUploadedData(int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<IncomeTaxRefund> products = null;

            var lst = new List<IncomeTaxRefund>();
            lst = (List<IncomeTaxRefund>)HttpContext.Cache["currentTaxRefundUpload"];
            var pglst = lst.ToPagedList(pageIndex, pageSize);

            return PartialView("_TaxRefundUploadedData", pglst);
        }

        [PayrollAuthorize]
        public ActionResult SaveUploadedData()
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var lst = new List<IncomeTaxRefund>();
                lst = (List<IncomeTaxRefund>)HttpContext.Cache["currentTaxRefundUpload"];
                var dcv = (TaxRefundUploadView)HttpContext.Cache["currentTaxRefundUploadInfo"];
                var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month.ToString()), 1);
                var dnames = dataContext.prl_fiscal_year.ToList();

                foreach (var v in lst)
                {
                    var i = new prl_income_tax_refund();
                    i.fiscal_year_id = dnames.SingleOrDefault(x => x.fiscal_year.ToLower() == v.FiscalYearNameString.ToLower()).id;
                    i.emp_id = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower()).id;
                    i.refund_amount = v.refund_amount;
                    i.month_year = salmon;
                    i.created_by = User.Identity.Name;
                    i.created_date = DateTime.Now;
                    dataContext.prl_income_tax_refund.Add(i);
                }

                dataContext.SaveChanges();
                operationResult.IsSuccessful = true;
                operationResult.Message = "Tax refund uploaded successfully.";
                TempData.Add("msg", operationResult);
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
                TempData.Add("msg", operationResult);
            }
            return RedirectToAction("UploadTaxRefund");
        }

        [PayrollAuthorize]
        public ActionResult EditUploadedTaxRefund()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult GetTaxRefundDataSelection()
        {
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxRefundUploadView tr = new TaxRefundUploadView();
            tr.FiscalYears = fiscalYears;

            return PartialView("_GetTaxRefundDataSelection", tr);
        }

        [HttpPost]
        public PartialViewResult GgetTaxRefundDataSelection(TaxRefundUploadView trV)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                var dateToSearch = new DateTime(trV.Year, Convert.ToInt32(trV.Month), 1);
                ViewBag.did = trV.FiscalYear;
                ViewBag.dt = dateToSearch;
                var lst = dataContext.prl_income_tax_refund.Include("prl_fiscal_year").AsEnumerable().Where(x => x.month_year.Value.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.fiscal_year_id == trV.FiscalYear);
                var kk = Mapper.Map<List<IncomeTaxRefund>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxRefund", kk);
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
                    var original = dataContext.prl_income_tax_refund.SingleOrDefault(x => x.id == primKey);
                    original.refund_amount = amnt;
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
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.did = did;
                ViewBag.dt = dt;
                var lst = dataContext.prl_income_tax_refund.Include("prl_fiscal_year").AsEnumerable().Where(x => x.month_year.Value.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.fiscal_year_id == did);
                var kk = Mapper.Map<List<IncomeTaxRefund>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxRefund", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Tax Adjustment Upload

        [HttpGet]
        public ActionResult UploadTaxAdjustment()
        {

            return View();
        }

        [HttpGet]
        public PartialViewResult UploadTaxAdjustmentForm()
        {
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxAdjustmentUploadView tr = new TaxAdjustmentUploadView();
            tr.FiscalYears = fiscalYears;
            return PartialView("_TaxAdjustmentUploadForm", tr);
        }

        [HttpPost]
        public ActionResult UploadTaxAdjustment(TaxAdjustmentUploadView trV, HttpPostedFileBase fileupload)
        {

            var lstDat = new List<TaxAdjustmentUpload>();
            if (ModelState.IsValid)
            {
                try
                {
                    fileupload.InputStream.Position = 0;
                    using (var package = new ExcelPackage(fileupload.InputStream))
                    {
                        var ws = package.Workbook.Worksheets.First();
                        var startRow = 2;
                        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            var d = new TaxAdjustmentUpload();

                            if (ws.Cells[rowNum, 1].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + "does not have an employee ID");
                            }
                            else
                            {
                                d.EmployeeID = ws.Cells[rowNum, 1].Value.ToString();
                            }

                            if (ws.Cells[rowNum, 2].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + "does not have amount");
                            }
                            else
                            {
                                decimal val = 0;
                                if (decimal.TryParse(ws.Cells[rowNum, 2].Value.ToString(), out val))
                                {
                                    d.adjustment_amount = val;
                                }
                                else
                                {
                                    d.ErrorMsg.Add("Row " + rowNum + " amount column should have decimal value");
                                }
                            }

                            lstDat.Add(d);
                        }
                    }
                    HttpContext.Cache.Insert("currentTaxAdjustmentUploadInfo", trV, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    HttpContext.Cache.Insert("currentTaxAdjustmentUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                }
                catch (Exception ex)
                {
                    var d = ex.Message;
                }
            }
            else
            {
                return View(trV);
            }
            return Json(new { isUploaded = true, message = "hello" }, "text/html");
        }

        public PartialViewResult LoadUploadedAdjustmentData(int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<TaxAdjustmentUpload> products = null;

            var lst = new List<TaxAdjustmentUpload>();
            lst = (List<TaxAdjustmentUpload>)HttpContext.Cache["currentTaxAdjustmentUpload"];
            var pglst = lst.ToPagedList(pageIndex, pageSize);

            return PartialView("_TaxAdjustmentUploadedData", pglst);
        }

        public ActionResult SaveUploadedAdjustmentData()
        {

            OperationResult operationResult = new OperationResult();
            try
            {
                var lst = new List<TaxAdjustmentUpload>();
                lst = (List<TaxAdjustmentUpload>)HttpContext.Cache["currentTaxAdjustmentUpload"];
                var dcv = (TaxAdjustmentUploadView)HttpContext.Cache["currentTaxAdjustmentUploadInfo"];
                var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month.ToString()), 1);
                var dnames = dataContext.prl_fiscal_year.ToList();

                foreach (var v in lst)
                {
                    var i = new prl_income_tax_adjustment();
                    i.fiscal_year = dnames.SingleOrDefault(x => x.fiscal_year.ToLower() == v.FiscalYearNameString.ToLower()).id;
                    i.emp_id = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower()).id;
                    i.adjustment_amount = v.adjustment_amount;
                    i.month_year = salmon;
                    i.created_by = User.Identity.Name;
                    i.created_date = DateTime.Now;
                    dataContext.prl_income_tax_adjustment.Add(i);
                }

                dataContext.SaveChanges();

                operationResult.IsSuccessful = true;
                operationResult.Message = "Tax Adjustment uploaded successfully.";
                TempData.Add("msg", operationResult);
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
                TempData.Add("msg", operationResult);
            }
            return RedirectToAction("UploadTaxAdjustment");
        }

        public ActionResult EditUploadedTaxAdjustment()
        {

            return View();
        }

        [HttpGet]
        public PartialViewResult GetTaxAdjustmentDataSelection()
        {
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxAdjustmentUploadView ta = new TaxAdjustmentUploadView();
            ta.FiscalYears = fiscalYears;

            return PartialView("_GetTaxAdjustmentDataSelection", ta);
        }

        [HttpPost]
        public PartialViewResult GgetTaxAdjustmentDataSelection(TaxAdjustmentUploadView taV)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                var dateToSearch = new DateTime(taV.Year, Convert.ToInt32(taV.Month), 1);
                ViewBag.did = taV.FiscalYear;
                ViewBag.dt = dateToSearch;
                var lst = dataContext.prl_income_tax_adjustment.Include("prl_fiscal_year").AsEnumerable().Where(x => x.month_year.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.fiscal_year == taV.FiscalYear);
                var kk = Mapper.Map<List<TaxAdjustmentUpload>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxAdjustment", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateTaxAdjustmentRecord(HttpRequestMessage request, string name, string pk, string value)
        {

            try
            {
                int primKey = 0;
                decimal amnt = 0;
                if (Int32.TryParse(pk, out primKey) && decimal.TryParse(value, out amnt))
                {
                    var original = dataContext.prl_income_tax_adjustment.SingleOrDefault(x => x.id == primKey);
                    original.adjustment_amount = amnt;
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

        public PartialViewResult EditDataPagingTaxAdjustment(int did, DateTime dt, int? page)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.did = did;
                ViewBag.dt = dt;
                var lst = dataContext.prl_income_tax_adjustment.Include("prl_fiscal_year").AsEnumerable().Where(x => x.month_year.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.fiscal_year == did);
                var kk = Mapper.Map<List<TaxAdjustmentUpload>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxAdjustment", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult MonthlyTaxStatement()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            ViewBag.Departments = Mapper.Map<List<Department>>(dataContext.prl_department.ToList());

            return View(new ReportMonthlyTaxStatement
            {
                //RType. = true
            });
        }


        [PayrollAuthorize]
        [HttpPost]
        public ActionResult MonthlyTaxStatement(ReportMonthlyTaxStatement SS, FormCollection collection, string sButton)
        {
            var empNumber = new List<string>();

            var EmpList = new List<ReportMonthlyTaxStatement>();
            string year = SS.Year.ToString();

            EmpList = (from chln in dataContext.prl_income_tax_challan
                       join emp in dataContext.prl_employee on chln.emp_id equals emp.id
                       join espd in dataContext.vw_empsalaryprocessdetails on
                        new { Key1 = chln.emp_id, Key2 = chln.challan_date.Month, Key3 = chln.challan_date.Year }
                       equals new { Key1 = espd.id, Key2 = espd.salary_month.Month, Key3 = espd.salary_month.Year } into spd
                       from esd in spd.DefaultIfEmpty()

                       where
                       esd.salary_month.Year == SS.Year && esd.salary_month.Month == SS.month_no

                       select
                       new ReportMonthlyTaxStatement
                       {
                           Year = esd.salary_month.Year,
                           month_no = esd.salary_month.Year,
                           month_name = esd.month_name,
                           empId = esd.id,
                           empNo = esd.emp_no,
                           empName = esd.empName,
                           designation = esd.designation,
                           tin = emp.tin,
                           basic_salary_including_arrear = esd.this_month_basic,
                           total_allowance_with_benefit = esd.total_allowance > 0 ? (esd.total_allowance - esd.this_month_basic) : 0,
                           value_of_benefit_not_paid_in_cash = 0,
                           totalA = esd.total_allowance,
                           //amount_of_tax = esd.monthly_tax,
                           amount_of_tax = chln.amount != null ? chln.amount : 0,
                           challan_no = chln.challan_no != null ? chln.challan_no : " ",
                           challan_date = chln.challan_date,
                           bank_name = chln.challan_bank != null ? chln.challan_bank : " ",
                           challan_total_amount = chln.challan_total_amount,
                           tax_paid_till_last_month = (esd.tax_Paid != null ? esd.tax_Paid : 0) - (chln.amount != null ? chln.amount : 0),
                           remarks = chln.remarks,

                       }).ToList();

            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();
                string path = "";

                path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "MonthlyTaxStatementReport.rdlc");


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

        // Tax Adjustment Upload

        #region Tax Challan

        //[HttpGet]
        //public PartialViewResult UploadTaxChallanForm()
        //{
        //    var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
        //    var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
        //    TaxChallanUploadView tr = new TaxChallanUploadView();
        //    tr.FiscalYears = fiscalYears;
        //    return PartialView("_TaxChallanUploadForm", tr);
        //}

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult UploadTaxChallan()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxChallanUploadView tr = new TaxChallanUploadView();
            tr.FiscalYears = fiscalYears;
            return View(tr);
        }

        [HttpPost]
        public ActionResult UploadTaxChallan(HttpPostedFileBase uploadFile, TaxChallanUploadView tcV)
        {

            var dateTime = new DateTime(Convert.ToInt32(tcV.Year), Convert.ToInt32(tcV.Month), 1);

            // Allow any decimal, negetive, parentheses while parse an amount.
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands |
                                 NumberStyles.AllowParentheses | NumberStyles.Number;

            string filefullpath = string.Empty;
            var res = new OperationResult();
            try
            {
                if (uploadFile != null)
                {
                    var file = uploadFile;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);

                        //do stuff with the bytes
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Request.PhysicalApplicationPath, fileName);

                        //string filePath = Path.Combine(Request.PhysicalApplicationPath, "Files\\", fileName);

                        System.IO.File.WriteAllBytes(filePath, fileBytes);

                        //File Uploaded
                        XSSFWorkbook xssfWorkbook;

                        filefullpath = filePath;

                        //StreamReader streamReader = new StreamReader(model.ImportFile.InputStream);

                        using (FileStream fileStream = new FileStream(filefullpath, FileMode.Open, FileAccess.Read))
                        {
                            xssfWorkbook = new XSSFWorkbook(fileStream);
                        }

                        var challanXlsViewModelList = new List<ChallanXlsViewModel>();

                        //the columns
                        var properties = new string[] {
                             "emp_id",
                             "challan_no",
                             "amount",
                             "challan_date",
                             "challan_bank",
                             "challan_total_amount",
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
                                    string Emp_Id = GetRowCellValue(sheet, row, properties, "emp_id", "StringCellValue");
                                    string Challan_No = GetRowCellValue(sheet, row, properties, "challan_no", "StringCellValue");
                                    string Amount = GetRowCellValue(sheet, row, properties, "amount", "NumericCellValue");
                                    string Challan_Date = GetRowCellValue(sheet, row, properties, "challan_date", "DateCellValue");
                                    string Challan_Bank = GetRowCellValue(sheet, row, properties, "challan_bank", "StringCellValue");
                                    string Challan_Total = GetRowCellValue(sheet, row, properties, "challan_total_amount", "NumericCellValue");
                                    string Remarks = GetRowCellValue(sheet, row, properties, "remarks", "StringCellValue");

                                    var challanXlsViewModel = new ChallanXlsViewModel
                                    {
                                        Emp_Id = Emp_Id.ToString(),
                                        Challan_No = Challan_No.ToString(),
                                        Amount = decimal.Parse(string.IsNullOrEmpty(Amount) ? "0" : Amount, style),
                                        Challan_Date = Convert.ToDateTime(Challan_Date).ToString("yyyy-MM-dd HH:mm:ss"),
                                        Challan_Bank = Challan_Bank.ToString(),
                                        Challan_Total = decimal.Parse(string.IsNullOrEmpty(Challan_Total) ? "0" : Challan_Total, style),
                                        Remarks = Remarks.ToString()
                                    };

                                    challanXlsViewModelList.Add(challanXlsViewModel);
                                }
                                //else
                                //{
                                //    res.IsSuccessful = false;
                                //    res.Message = "The amount cell of " + row + "th row is empty. Please check the uploading file.";
                                //    TempData.Add("msg", res);
                                //}
                            }
                        }

                        ////////// duplicate check
                        var lstEmpNo = challanXlsViewModelList.AsEnumerable().Select(x => x.Emp_Id).ToList();
                        var lstSysEmpId = dataContext.prl_employee.AsEnumerable().Where(x => lstEmpNo.Contains(x.emp_no)).ToList();
                        var existingUploadedData = dataContext.prl_income_tax_challan.AsEnumerable()
                                .Where(x => x.fiscal_year_id == tcV.FiscalYear && x.challan_date.ToString("yyyy-MM") == dateTime.ToString("yyyy-MM"))
                                .ToList();
                        /////////////////////////////

                        #region Insert To Database

                        int returnSaveChanges = 0;
                        var dnames = dataContext.prl_allowance_name.ToList();
                        foreach (var v in challanXlsViewModelList)
                        {
                            var i = new prl_income_tax_challan();
                            
                            var singleOrDefault = lstSysEmpId.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.Emp_Id.ToLower());
                            if (singleOrDefault == null)
                            {
                                res.HasPartialError = true;
                                res.Messages.Add(" Could not find employee number " + v.Emp_Id);
                                continue;
                            }


                            if (dateTime.ToString("yyyy-MM") == Convert.ToDateTime(v.Challan_Date).ToString("yyyy-MM"))
                            {
                                res.HasPartialError = true;
                                res.Messages.Add("As per selection you have uploaded wrong Challan Date for " + v.Emp_Id);
                                continue;
                            }

                            var duplicateData = existingUploadedData.AsEnumerable().FirstOrDefault(x => x.emp_id == singleOrDefault.id && x.fiscal_year_id == tcV.FiscalYear && x.challan_date.ToString("yyyy-MM") == dateTime.ToString("yyyy-MM"));
                            if (duplicateData != null)
                            {
                                res.HasPartialError = true;
                                res.Messages.Add(" Employee " + v.Emp_Id + " challan info of this month already exist in the system.");
                                continue;
                            }

                            i.emp_id = singleOrDefault.id;
                            i.fiscal_year_id = tcV.FiscalYear;
                            i.challan_no = v.Challan_No;
                            i.amount = v.Amount;
                            i.challan_date = Convert.ToDateTime(v.Challan_Date);
                            i.challan_bank = v.Challan_Bank;
                            i.challan_total_amount = v.Challan_Total;
                            i.remarks = v.Remarks;
                            i.created_by = User.Identity.Name;
                            i.created_date = DateTime.Now;
                            dataContext.prl_income_tax_challan.Add(i);
                        }

                        returnSaveChanges = dataContext.SaveChanges();
                        #endregion

                        var monthYr = dateTime.ToString("MMM-yyyy");

                        if (returnSaveChanges > 0)
                        {
                            res.IsSuccessful = true;
                            res.Messages.Add( monthYr + " challan data has been uploaded successfully.");
                            TempData.Add("msg", res);
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Messages.Add("This " + monthYr + " challan data upload problem found. Please try correctly.");
                            TempData.Add("msg", res);
                        }
                        return RedirectToAction("UploadTaxChallan", "IncomeTax");
                    }
                    else
                    {
                        res.IsSuccessful = false;
                        res.Messages.Add("Upload can not be empty.");
                        TempData.Add("msg", res);

                        return RedirectToAction("UploadTaxChallan", "IncomeTax");
                    }
                }
                else
                {
                    //Upload file Null Message
                    res.IsSuccessful = false;
                    res.Messages.Add("Upload can not be empty.");
                    TempData.Add("msg", res);

                    return RedirectToAction("UploadTaxChallan", "IncomeTax");
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Messages.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                TempData.Add("msg", res);

                return RedirectToAction("UploadAllowance", "Allowance");
            }
            finally
            {
                if (System.IO.File.Exists(filefullpath))
                {
                    System.IO.File.Delete(filefullpath);
                }
            }
        }

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

        //[HttpGet]
        //public ActionResult UploadTaxChallan()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult UploadTaxChallan(TaxChallanUploadView trV, HttpPostedFileBase fileupload)
        //{
        //    OperationResult operationResult = new OperationResult();
        //    TempData.Remove("msg");

        //    var lstDat = new List<TaxChallanUpload>();
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            fileupload.InputStream.Position = 0;
        //            using (var package = new ExcelPackage(fileupload.InputStream))
        //            {
        //                var ws = package.Workbook.Worksheets.First();
        //                var startRow = 2;

        //                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        //                {
        //                    var d = new TaxChallanUpload();

        //                    if (ws.Cells[rowNum, 1].Value == null)
        //                    {
        //                        d.ErrorMsg.Add("Row " + rowNum + "does not have an employee ID");
        //                    }
        //                    else
        //                    {
        //                        string emp_no = ws.Cells[rowNum, 1].Value.ToString();
        //                        var empData = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == emp_no.ToLower());
        //                        if (empData == null)
        //                        {
        //                            d.ErrorMsg.Add(" Could not find employee number " + emp_no);
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            d.EmployeeID = emp_no;
        //                        }
        //                    }


        //                    if (ws.Cells[rowNum, 2].Value == null)
        //                    {
        //                        d.ErrorMsg.Add("Row " + rowNum + "does not have an Challan no.");
        //                    }
        //                    else
        //                    {
        //                        d.challan_no = ws.Cells[rowNum, 2].Value.ToString();
        //                    }

        //                    if (ws.Cells[rowNum, 3].Value == null)
        //                    {
        //                        d.ErrorMsg.Add("Row " + rowNum + "does not have amount");
        //                    }
        //                    else
        //                    {
        //                        decimal val = 0;
        //                        if (decimal.TryParse(ws.Cells[rowNum, 3].Value.ToString(), out val))
        //                        {
        //                            d.amount = val;
        //                        }
        //                        else
        //                        {
        //                            d.ErrorMsg.Add("Row " + rowNum + " amount column should have decimal value");
        //                        }
        //                     }

        //                    if (ws.Cells[rowNum, 4].Value != null && ws.Cells[rowNum, 4].Value.ToString() != "")
        //                    {
        //                        d.challan_bank = ws.Cells[rowNum, 4].Value.ToString();
        //                    }

        //                    if (ws.Cells[rowNum, 5].Value != null && ws.Cells[rowNum, 5].Value.ToString() != "")
        //                    {
        //                        decimal val = 0;
        //                        if (decimal.TryParse(ws.Cells[rowNum, 5].Value.ToString(), out val))
        //                        {
        //                            d.challan_total_amount = val;
        //                        }
        //                        else
        //                        {
        //                            d.ErrorMsg.Add("Row " + rowNum + " Grand Total of Challan amount column should have decimal value");
        //                        }
        //                    }

        //                    if (ws.Cells[rowNum, 6].Value != null && ws.Cells[rowNum, 6].Value != "")
        //                    {
        //                        d.remarks = ws.Cells[rowNum, 6].Value.ToString();
        //                    }

        //                    lstDat.Add(d);
        //                }
        //            }
        //            HttpContext.Cache.Insert("currentTaxChallanUploadInfo", trV, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
        //            HttpContext.Cache.Insert("currentTaxChallanUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
        //        }
        //        catch (Exception ex)
        //        {
        //            var d = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        return View(trV);
        //    }
        //    return Json(new { isUploaded = true, message = "hello" }, "text/html");
        //}

        public PartialViewResult LoadUploadedChallanData(int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<TaxChallanUpload> pglst = null;

            var lst = new List<TaxChallanUpload>();
            lst = (List<TaxChallanUpload>)HttpContext.Cache["currentTaxChallanUpload"];

            if (lst != null)
            {
                pglst = lst.ToPagedList(pageIndex, pageSize);
            }

            return PartialView("_TaxChallanUploadedData", pglst);
        }

        public ActionResult SaveUploadedChallanData()
        {
            OperationResult operationResult = new OperationResult();
            TempData.Remove("msg");
            try
            {
                var lst = new List<TaxChallanUpload>();
                lst = (List<TaxChallanUpload>)HttpContext.Cache["currentTaxChallanUpload"];
                var dcv = (TaxChallanUploadView)HttpContext.Cache["currentTaxChallanUploadInfo"];
                if (lst.Count > 0 && lst != null)
                {
                    var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month.ToString()), 1);
                    //var dnames = dataContext.prl_fiscal_year.ToList();

                        foreach (var v in lst)
                        {
                            int empId = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower()).id;
                            var existingData = dataContext.prl_income_tax_challan.AsEnumerable()
                                    .Where(x => x.fiscal_year_id == dcv.FiscalYear && x.challan_date == salmon && x.emp_id == empId)
                                    .ToList();

                            if (existingData.Count > 0 && existingData != null)
                            {
                                operationResult.IsSuccessful = false;
                                operationResult.Messages.Add(v.EmployeeID + "'s challan for this month already exist in the system.");
                                continue;
                            }
                            else
                            {
                                var i = new prl_income_tax_challan();
                                i.fiscal_year_id = dcv.FiscalYear;
                                i.emp_id = empId;
                                i.challan_no = v.challan_no;
                                i.amount = v.amount;
                                i.challan_date = salmon;
                                i.challan_bank = v.challan_bank;
                                i.challan_total_amount = v.challan_total_amount;
                                i.remarks = v.remarks;
                                i.created_by = User.Identity.Name;
                                i.created_date = DateTime.Now;

                                dataContext.prl_income_tax_challan.Add(i);

                            }
                        }

                        int returnSaveChanges = 0;
                        returnSaveChanges = dataContext.SaveChanges();
                        
                        if (returnSaveChanges > 0)
                        {
                            operationResult.IsSuccessful = true;
                            operationResult.Messages.Add("Tax Challan uploaded successfully.");
                            TempData.Add("msg", operationResult);
                        }
                        else
                        {
                            operationResult.IsSuccessful = false;
                            operationResult.Messages.Add("Problem found in that data. Please try correctly.");
                            TempData.Add("msg", operationResult);
                        }
                }
                else
                {
                    operationResult.IsSuccessful = false;
                     operationResult.Messages.Add("No data found. Please try again.");
                    TempData.Add("msg", operationResult);
                }
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Messages.Add(ex.Message);
                TempData.Add("msg", operationResult);
            }

            return RedirectToAction("UploadTaxChallan");
        }

        public ActionResult EditUploadedTaxChallan()
        {
            //try
            //{
            //    var _submenuList = new SubMenuGenerator(dataContext);
            //    ViewData["SubMenu"] = _submenuList.GenerateSubMenuByMenuName("IncomeTax");
            //}
            //catch
            //{
            //}
            return View();
        }

        [HttpGet]
        public PartialViewResult GetTaxChallanDataSelection()
        {
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxChallanUploadView ta = new TaxChallanUploadView();
            ta.FiscalYears = fiscalYears;

            return PartialView("_GetTaxChallanDataSelection", ta);
        }

        [HttpPost]
        public PartialViewResult GgetTaxChallanDataSelection(TaxChallanUploadView tcV)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                var dateToSearch = new DateTime(tcV.Year, Convert.ToInt32(tcV.Month), 1);
                ViewBag.did = tcV.FiscalYear;
                ViewBag.dt = dateToSearch;
                var lst = dataContext.prl_income_tax_challan.Include("prl_fiscal_year").AsEnumerable().Where(x => x.challan_date.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.fiscal_year_id == tcV.FiscalYear);
                var kk = Mapper.Map<List<TaxChallanUpload>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxChallan", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateTaxChallanRecord(HttpRequestMessage request, string name, string pk, string value)
        {
            try
            {
                int primKey = 0;
                decimal amnt = 0;
                if (Int32.TryParse(pk, out primKey) && decimal.TryParse(value, out amnt))
                {
                    var original = dataContext.prl_income_tax_challan.SingleOrDefault(x => x.id == primKey);
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

        public PartialViewResult EditDataPagingChallan(int did, DateTime dt, int? page)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.did = did;
                ViewBag.dt = dt;
                var lst = dataContext.prl_income_tax_challan.Include("prl_fiscal_year").AsEnumerable().Where(x => x.challan_date.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.fiscal_year_id == did);
                var kk = Mapper.Map<List<TaxChallanUpload>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxChallan", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Tax Challan



        [PayrollAuthorize]
        [HttpGet]
        public ActionResult YearlyInvestmentSheet()
        {
            int fsId = FindFiscalYear(DateTime.Now);

            var fisYr = dataContext.prl_fiscal_year.SingleOrDefault(f => f.id == fsId);
            ViewBag.fsYear = fisYr.fiscal_year;
            ViewBag.fsYearId = fisYr.id;

            var lstFiscalYear = dataContext.prl_fiscal_year.ToList();
            ViewBag.FiscalYear = lstFiscalYear;

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult YearlyInvestmentSheet(EmployeeYearlyInvestment vm, FormCollection collection, string sButton)
        {

            int fsId = FindFiscalYear(DateTime.Now);

            var fisYr = dataContext.prl_fiscal_year.SingleOrDefault(f => f.id == fsId);
            var lstFiscalYear = dataContext.prl_fiscal_year.ToList();

            var EmpList = new List<EmployeeYearlyInvestment>();

            EmpList = (from yi in dataContext.prl_employee_yearly_investment

                       join emp in dataContext.prl_employee on yi.emp_id equals emp.id into empGroup
                       from eG in empGroup.DefaultIfEmpty()

                       join empD in dataContext.prl_employee_details on eG.id equals empD.emp_id into empDGroup
                       from eDG in empDGroup.DefaultIfEmpty()

                       join fy in dataContext.prl_fiscal_year on yi.fiscal_year_id equals fy.id into fyGroup
                       from fG in fyGroup.DefaultIfEmpty()

                       where yi.fiscal_year_id == vm.fiscal_year_id

                       select new EmployeeYearlyInvestment
                       {
                           empId = yi.emp_id,
                           empNo = eG.emp_no,
                           empName = eG.name,
                           invested_amount = yi.invested_amount
                       }).ToList();



            if (EmpList.Count > 0)
            {
                LocalReport lr = new LocalReport();
                string path = "";

                path = Path.Combine(Server.MapPath("~/Views/Report/RDLC"), "EmployeeYearlyInvestmentReport.rdlc");


                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    ViewBag.fsYear = fisYr.fiscal_year;
                    ViewBag.fsYearId = fisYr.id;

                    ViewBag.FiscalYear = lstFiscalYear;

                    return View();
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

            ViewBag.fsYear = fisYr.fiscal_year;
            ViewBag.fsYearId = fisYr.id;
            ViewBag.FiscalYear = lstFiscalYear;

            return View();
        }

        public FileResult GetChallanUploadSample()
        {
            var fileName = "ChallanUploadFormat.xlsx";
            //string FileURL = "D:/1. Lukman_Official/Sample of Employees Data Import.xlsx";
            //string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/Sample of Salery Review Data Import.xlsx";
            string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/ChallanUploadFormat.xlsx";
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);

            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(FileBytes, "application/xlsx", fileName); GetSaleryReviewUplodSample
        }
    }
}