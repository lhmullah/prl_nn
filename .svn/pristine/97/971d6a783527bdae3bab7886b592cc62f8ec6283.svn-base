using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using AutoMapper;
using AutoMapper.Internal;
using com.gpit.DataContext;
using com.gpit.Model;
using OfficeOpenXml;
using PayrollWeb.Service;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Controllers
{
    public class OvertimeController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public OvertimeController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult Index()
        {
            var lst = dataContext.prl_over_time_configuration.ToList();
            return View(Mapper.Map<List<OTConfiguration>>(lst));
        }

        
        // POST: /Overtime/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        //
        // POST: /Overtime/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Overtime/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
               var org = dataContext.prl_over_time_configuration.SingleOrDefault(x => x.id == id);
                dataContext.prl_over_time_configuration.Remove(org);
                dataContext.SaveChanges();
                return Json(new {status = "success", msg = "Successfully deleted"},JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message },JsonRequestBehavior.AllowGet); 
                
            }

           
        }

        //
        // POST: /Overtime/Delete/5

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

        [HttpGet]
        public ActionResult CreateOTConfig()
        {
            var newOt = new OTConfiguration();
            var lstOv = dataContext.prl_over_time.ToList();
            ViewBag.Overtimes = Mapper.Map<List<Overtime>>(lstOv);
            return PartialView("_OvertimeConfig", newOt);
        }

        [HttpPost]
        public ActionResult CreateOTConfig(OTConfiguration otc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newPrlOverConf = Mapper.Map<prl_over_time_configuration>(otc);
                    dataContext.prl_over_time_configuration.Add(newPrlOverConf);
                    dataContext.SaveChanges();
                    return Json(new { status = "success", msg = "Could not save." });
                }
                catch (Exception ex)
                {
                   return Json(new {status = "error", msg = ex.Message});
                }
            }
            else
            {
                return Json(new {status = "error", msg = "Could not save."});
            }
        }
        [HttpGet]
        public ActionResult UploadTimeCard()
        {
            ViewBag.Years = DateUtility.GetYears();
            ViewBag.Months = DateUtility.GetMonths();
            return View();
        }
        [HttpPost]
        public ActionResult UploadTimeCardEntryFile(HttpPostedFileBase fileupload, int Year, int Month)
        {
            var errorList = new List<string>();

           if (Year > 0 && Month > 0 && fileupload != null)
           {
                var lstDat = new List<TimeCard>();
            try
            {
                fileupload.InputStream.Position = 0;
                using (var package = new ExcelPackage(fileupload.InputStream))
                {
                    var ws = package.Workbook.Worksheets.First();
                    var startRow = 2;
                    var firstColumPos = ws.Cells.FirstOrDefault(x => x.Value.ToString().Trim() == "Employee ID");
                    startRow = firstColumPos.Start.Row + 1;

                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var d = new TimeCard();
                        d.month = new DateTime(Year, Month, 1).ToString("MMM");
                        d.year = Year;
                        d.IsInvalid = false;
                        d.UplodedForMonth = new DateTime(Year, Month, 1);

                        // 1.Emp No
                        if (ws.Cells[rowNum, 1].Value == null)
                        {
                            errorList.Add("Row " + rowNum + "does not have an employee ID");
                            d.IsInvalid = true;
                        }
                        else
                            d.emp_no = ws.Cells[rowNum, 1].Value.ToString();

                        // 2.Overtime month 
                        DateTime dt = new DateTime();
                        if (ws.Cells[rowNum, 2].Value == null ||!DateTime.TryParse(ws.Cells[rowNum, 2].Value.ToNullSafeString(), out dt)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have submission date");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.submission_date = dt;
                        }

                        // 3.Double Rate
                        decimal rt = 0;
                        if (ws.Cells[rowNum, 3].Value == null ||
                            !decimal.TryParse(ws.Cells[rowNum, 3].Value.ToNullSafeString(), out rt)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have double rate");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.double_rate_hour = rt;
                        }

                        // 4. Tri Rate
                        decimal trt = 0;
                        if (ws.Cells[rowNum, 4].Value == null && !decimal.TryParse(ws.Cells[rowNum, 4].Value.ToNullSafeString(), out rt)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have triple");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.triple_rate_hour = trt;
                        }

                        // 5. Tri Rate
                        decimal tetrart = 0;
                        if (ws.Cells[rowNum, 5].Value == null ||
                            !decimal.TryParse(ws.Cells[rowNum, 5].Value.ToNullSafeString(), out tetrart)) // tetra Rate
                        {
                            errorList.Add("Row " + rowNum + " does not have four times rate");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.four_times_rate_hour = tetrart;
                        }

                        //6.  Total Overtime
                        decimal total = 0;
                        if (ws.Cells[rowNum, 6].Value == null ||
                            !decimal.TryParse(ws.Cells[rowNum, 6].Value.ToString(), out total)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have total over time");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.total_ot = total;
                        }


                        //7. Evening shift
                        if (ws.Cells[rowNum, 7].Value == null ||!decimal.TryParse(ws.Cells[rowNum, 7].Value.ToNullSafeString(), out total))//
                        {
                            errorList.Add("Row " + rowNum + " does not have eveving shift value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.evening_shift = total;
                        }

                        //8. Night Shift
                        if (ws.Cells[rowNum, 8].Value == null ||
                            !decimal.TryParse(ws.Cells[rowNum, 8].Value.ToNullSafeString(), out total)) // Night shift
                        {
                            errorList.Add("Row " + rowNum + " does not have night shift value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.night_shift = total;
                        }

                        //9.Weekend Shift
                        if (ws.Cells[rowNum, 9].Value == null || !decimal.TryParse(ws.Cells[rowNum, 9].Value.ToNullSafeString(), out total))
                            // Weekend shift
                        {
                            errorList.Add("Row " + rowNum + " does not have weekend shift value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.weekend = total;
                        }

                        //10. Total (sum of shift)
                        if (ws.Cells[rowNum, 10].Value == null || !decimal.TryParse(ws.Cells[rowNum, 10].Value.ToNullSafeString(), out total))
                           
                        {
                            errorList.Add("Row " + rowNum + " does not have total of all shift value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.total_shift = total;
                        }

                        // 11. Oncall
                        if (ws.Cells[rowNum, 11].Value == null ||
                            !decimal.TryParse(ws.Cells[rowNum, 11].Value.ToNullSafeString(), out total)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have on call value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.on_call_days = total;
                        }

                        //12. one way
                        int t = 0;
                        if (ws.Cells[rowNum, 12].Value == null ||
                            !int.TryParse(ws.Cells[rowNum, 12].Value.ToNullSafeString(), out t)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have one way value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.one_way = t;
                        }

                        //13. two way
                        if (ws.Cells[rowNum, 13].Value == null ||
                            !int.TryParse(ws.Cells[rowNum, 13].Value.ToNullSafeString(), out t)) 
                        {
                            errorList.Add("Row " + rowNum + " does not have two way value");
                            d.IsInvalid = true;
                        }
                        else
                        {
                            d.two_way = t;
                        }

                        d.created_by = "russell";
                        d.created_date = DateTime.Now;

                        lstDat.Add(d);
                    }
                }
                TimeCardRawView tcr = new TimeCardRawView();
                tcr.Cards = lstDat;
                tcr.ErroList = errorList;

                //check for duplicate and non-existent employees 

                var importedEmpNo = lstDat.Where(x => x.IsInvalid == false).Select(x => x.emp_no).ToList();
                if (importedEmpNo != null)
                {
                    var existingEmpNo =  dataContext.prl_employee.AsEnumerable().Where(x => importedEmpNo.Contains(x.emp_no)).Select(x=>x.emp_no).ToList();
                    if (existingEmpNo != null)
                    {
                        var nonExistingEmps = importedEmpNo.Except(existingEmpNo);
                        foreach(var nonE in nonExistingEmps)
                        {
                            tcr.ErroList.Add("Employee Number "+ nonE +" does not exists.");
                            tcr.Cards.RemoveAll(x => x.emp_no == nonE);
                        }
                        foreach (var c in tcr.Cards)
                        {
                            if (dataContext.prl_upload_time_card_entry.AsEnumerable().Any(x =>x.emp_no == c.emp_no && x.submission_date.Value.ToString("yyyy-MM") == c.submission_date.Value.ToString("yyyy-MM")))
                            {
                                tcr.ErroList.Add("Employee Number " + c.emp_no + " has a duplicate time card entry.");
                                c.IsInvalid = true;
                            }
                        }
                    }
                }

                HttpContext.Cache.Insert(string.Format("currentTcUploadInfo_{0}",Year+"_"+Month), tcr, null, DateTime.Now.AddMinutes(5),Cache.NoSlidingExpiration);
                return Json(new {isUploaded = true, message = "hello"}, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new {isUploaded = false, message = ex.Message}, "text/html");
            }
           
           }
           else
           {
              return Json(new { isUploaded = false, message = "Please select year and month first then the file" }, "text/html");
           }
        }
        [HttpGet]
        public PartialViewResult GetUnsavedUploadedDataResult(int year,int month)
        {
            var key = string.Format("currentTcUploadInfo_{0}", year + "_" + month);
            var k = (TimeCardRawView)HttpContext.Cache[key];
            ViewBag.Errors = k.ErroList;
            return PartialView("_UploadResult");
        }
        [HttpGet]
        public ActionResult SaveTimeCardEntry(int year, int month)
        {
            try
            {
                var forMonth = new DateTime(year, month, 1);
                var key = string.Format("currentTcUploadInfo_{0}", year + "_" + month);
                var k = (TimeCardRawView)HttpContext.Cache[key];

                var allowanceService = new AllowanceService(dataContext);
                var cards = k.Cards.Where(x => x.IsInvalid == false);
                var lstPrlUploadTimeCardEntry = Mapper.Map<List<prl_upload_time_card_entry>>(cards);
                lstPrlUploadTimeCardEntry.Each(x=> { x.created_by = User.Identity.Name; x.created_date = DateTime.Now;});
                lstPrlUploadTimeCardEntry.ForEach(x=> { x.for_month = forMonth; });
                bool res = allowanceService.SaveTimeCardEntry(lstPrlUploadTimeCardEntry);
                var operationResult = new OperationResult();
                operationResult.IsSuccessful = true;
                operationResult.Message = "Time card entry processed and saved successfully";
                TempData["msg"] = operationResult;
                return Json(new { status = "success", msg = "Time card entry processed and saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch ( Exception ex)
            {
                var operationResult = new OperationResult();
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
                TempData["msg"] = operationResult;
                return Json(new { status = "success", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateFormula(HttpRequestMessage request, string name, string pk, string value)
        {
            try
            {
                int primKey = 0;
                
                if (Int32.TryParse(pk, out primKey) && !string.IsNullOrWhiteSpace(value.Trim()))
                {
                    var original = dataContext.prl_over_time_configuration.SingleOrDefault(x => x.id == primKey);
                    original.formula = value.Trim();
                    dataContext.SaveChanges();
                    request.CreateResponse(HttpStatusCode.OK);
                    return Json(new { status = "success", msg = "Successfully updated" }, "json", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    request.CreateResponse(HttpStatusCode.OK);
                    return Json(new { status = "error", msg = "Formula must be non-null string" }, "json", JsonRequestBehavior.AllowGet);
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
