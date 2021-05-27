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
using PayrollWeb.Service;
using System.Collections.ObjectModel;
using PayrollWeb.Utility;
using OfficeOpenXml;
using System.Web.Caching;
using PagedList;
using System.Net.Http;
using System.Net;
using System.Web.Security;

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

                _paramDet = dataContext.prl_income_tax_parameter_details.FirstOrDefault(x => x.fiscal_year_id == _fs && x.gender == _gender);
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
            bool errorFound = false;
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
                    param.min_tax_amount = _taxParamDet.min_tax_amount;
                    param.max_house_rent_percentage = _taxParamDet.max_house_rent_percentage;
                    param.house_rent_not_exceding = _taxParamDet.house_rent_not_exceding;
                    param.max_conveyance_allowance_monthly = _taxParamDet.max_conveyance_allowance_monthly;
                    param.free_car = _taxParamDet.free_car;
                    param.medical_exemtion_percentage = _taxParamDet.medical_exemtion_percentage;
                    param.lfa_exemtion_percentage = _taxParamDet.lfa_exemtion_percentage;
                    
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
                    param.min_tax_amount = _taxParamDet.min_tax_amount;
                    param.max_house_rent_percentage = _taxParamDet.max_house_rent_percentage;
                    param.house_rent_not_exceding = _taxParamDet.house_rent_not_exceding;
                    param.max_conveyance_allowance_monthly = _taxParamDet.max_conveyance_allowance_monthly;
                    param.free_car = _taxParamDet.free_car;
                    param.medical_exemtion_percentage = _taxParamDet.medical_exemtion_percentage;
                    param.lfa_exemtion_percentage = _taxParamDet.lfa_exemtion_percentage;
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
        
        // Tax Adjustment Upload

        #region Tax Challan


        [HttpGet]
        public ActionResult UploadTaxChallan()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult UploadTaxChallanForm()
        {
            var prlFiscalYear = dataContext.prl_fiscal_year.ToList();
            var fiscalYears = Mapper.Map<List<FiscalYr>>(prlFiscalYear);
            TaxChallanUploadView tr = new TaxChallanUploadView();
            tr.FiscalYears = fiscalYears;
            return PartialView("_TaxChallanUploadForm", tr);
        }
        
        [HttpPost]
        public ActionResult UploadTaxAdjustment(TaxChallanUploadView trV, HttpPostedFileBase fileupload)
        {
            var lstDat = new List<TaxChallanUpload>();
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
                            var d = new TaxChallanUpload();

                            if (ws.Cells[rowNum, 1].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + "does not have an Challan no.");
                            }
                            else
                            {
                                d.challan_no = ws.Cells[rowNum, 1].Value.ToString();
                            }

                            //if (ws.Cells[rowNum, 1].Value == null)
                            //{
                            //    d.ErrorMsg.Add("Row " + rowNum + "does not have an employee ID");
                            //}
                            //else
                            //{
                            //    d.EmployeeID = ws.Cells[rowNum, 1].Value.ToString();
                            //}

                            if (ws.Cells[rowNum, 2].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + "does not have amount");
                            }
                            else
                            {
                                decimal val = 0;
                                if (decimal.TryParse(ws.Cells[rowNum, 2].Value.ToString(), out val))
                                {
                                    d.amount = val;
                                }
                                else
                                {
                                    d.ErrorMsg.Add("Row " + rowNum + " amount column should have decimal value");
                                }
                            }

                            lstDat.Add(d);
                        }
                    }
                    HttpContext.Cache.Insert("currentTaxChallanUploadInfo", trV, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    HttpContext.Cache.Insert("currentTaxChallanUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
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

        public PartialViewResult LoadUploadedChallanData(int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<TaxChallanUpload> products = null;

            var lst = new List<TaxChallanUpload>();
            lst = (List<TaxChallanUpload>)HttpContext.Cache["currentTaxChallanUpload"];
            var pglst = lst.ToPagedList(pageIndex, pageSize);

            return PartialView("_TaxChallanUploadedData", pglst);
        }
        
        public ActionResult SaveUploadedChallanData()
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var lst = new List<TaxChallanUpload>();
                lst = (List<TaxChallanUpload>)HttpContext.Cache["currentTaxChallanUpload"];
                var dcv = (TaxChallanUploadView)HttpContext.Cache["currentTaxChallanUploadInfo"];
                var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month.ToString()), 1);
                var dnames = dataContext.prl_fiscal_year.ToList();

                foreach (var v in lst)
                {
                    var i = new prl_income_tax_challan();
                    i.fiscal_year_id = dnames.SingleOrDefault(x => x.fiscal_year.ToLower() == v.FiscalYearNameString.ToLower()).id;
                    i.emp_id = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower()).id;
                    i.challan_no = v.challan_no;
                    i.amount = v.amount;
                    i.challan_date = salmon;
                    i.created_by = User.Identity.Name;
                    i.created_date = DateTime.Now;
                    dataContext.prl_income_tax_challan.Add(i);
                }

                dataContext.SaveChanges();

                operationResult.IsSuccessful = true;
                operationResult.Message = "Tax Challan uploaded successfully.";
                TempData.Add("msg", operationResult);
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
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
                var lst = dataContext.prl_income_tax_challan.Include("prl_fiscal_year").AsEnumerable().Where(x => x.challan_date.Value.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.fiscal_year_id == tcV.FiscalYear);
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
                var lst = dataContext.prl_income_tax_challan.Include("prl_fiscal_year").AsEnumerable().Where(x => x.challan_date.Value.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.fiscal_year_id == did);
                var kk = Mapper.Map<List<TaxChallanUpload>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedTaxChallan", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Tax Challan
    }
}
