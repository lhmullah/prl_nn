using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using com.gpit.DataContext;
using AutoMapper;
using com.gpit.Model;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using PagedList;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Service;
using PayrollWeb.ViewModels;
using PayrollWeb.Utility;
using System.Web.Security;

namespace PayrollWeb.Controllers
{
    public class BonusController : Controller
    {
        private readonly payroll_systemContext dataContext;
        //
        // GET: /Bonus/

        public BonusController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        
        public ActionResult BonusMain(string menuName)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            var _bonus = dataContext.prl_bonus_name.ToList();
            return View(Mapper.Map<List<BonusName>>(_bonus));
        }

        //
        // GET: /Bonus/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {

            var _bonus = new BonusName();
            return View(_bonus);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(BonusName item)
        {
            var res = new OperationResult();
            try
            {
                var _bonus = Mapper.Map<prl_bonus_name>(item);
                dataContext.prl_bonus_name.Add(_bonus);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = _bonus.name + " created. ";
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
            

            var _bonus = dataContext.prl_bonus_name.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<BonusName>(_bonus));
        }

        //
        // POST: /Bonus/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, BonusName item)
        {
            var res = new OperationResult();
            try
            {
                var _bonus = dataContext.prl_bonus_name.SingleOrDefault(x => x.id == item.id);
                _bonus.name = item.name;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = _bonus.name + " edited. ";
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
                var _bonus = dataContext.prl_bonus_name.SingleOrDefault(x => x.id == id);
                if (_bonus == null)
                {
                    return HttpNotFound();
                }
                name = _bonus.name;
                dataContext.prl_bonus_name.Remove(_bonus);
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

        //
        // POST: /Bonus/Delete/5

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
        public ActionResult ConfigureBonus(int bonusId = 0)
        {
            

            ViewBag.SelectedIndex = bonusId;
            var prlGrds = dataContext.prl_grade.ToList();
            var grades = Mapper.Map<List<Grade>>(prlGrds);
            BonusConfiguration bonusConfig;
            if (bonusId == 0)
                bonusConfig = new BonusConfiguration();
            else
            {
                var dbVal = dataContext.prl_bonus_configuration.SingleOrDefault(x => x.bonus_name_id == bonusId);
                if (dbVal == null)
                {
                    dbVal = new prl_bonus_configuration();
                }
                bonusConfig = Mapper.Map<BonusConfiguration>(dbVal);
                bonusConfig.bonus_name_id = bonusId;
                if (dbVal.bonus_name_id > 0)
                {
                    var ids = dbVal.prl_bonus_name.prl_grade.Select(x => x.id);
                    foreach (var g in grades)
                    {
                        if (ids.Contains(g.id))
                            g.IsSelected = true;
                    }
                }
            }

            bonusConfig.Grades = grades;

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);
            return View(bonusConfig);
        }

        [HttpPost]
        public ActionResult ConfigureBonus(BonusConfiguration bc)
        {
            bool errorFound = false;
            var operationResult = new OperationResult();

            try
            {
                if (ModelState.IsValid)
                {
                    //check to see if grade is selected
                    var lstOfGrades = new List<prl_grade>();

                    if (bc.is_festival == "SELECT")
                    {
                        errorFound = true;
                        ModelState.AddModelError("is_festival", "Please Select.");
                    }
                    if (!bc.Grades.Any(x => x.IsSelected == true))
                    {
                        errorFound = true;
                        ModelState.AddModelError("Grades", "No grade(s) selected.");
                    }

                    if (bc.flat_amount == null && bc.percentage_of_basic == null)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Enter flat or percentage amount.");
                    }
                    if (bc.flat_amount <= 0 || bc.percentage_of_basic <= 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Flat or percentage amount should be greater than zero.");
                    }
                    if (!errorFound)
                    {
                        bc.Grades = bc.Grades.Where(x => x.IsSelected == true);
                        var ids = bc.Grades.Select(x => x.id).ToList();
                        lstOfGrades = dataContext.prl_grade.AsEnumerable().Where(x => ids.Contains(x.id)).ToList();
                    }
                    if (!errorFound)
                    {
                        var prlConf = Mapper.Map<prl_bonus_configuration>(bc);
                        prlConf.prl_bonus_name = dataContext.prl_bonus_name.SingleOrDefault(x => x.id == bc.bonus_name_id);
                        if (prlConf.prl_bonus_name != null)
                        {
                            prlConf.prl_bonus_name.prl_grade.Clear();
                            prlConf.prl_bonus_name.prl_grade = lstOfGrades;
                        }
                        else
                        {
                            prlConf.prl_bonus_name.prl_grade = new Collection<prl_grade>();
                            prlConf.prl_bonus_name.prl_grade = lstOfGrades;
                        }

                        BonusService bns = new BonusService(dataContext);
                        operationResult.IsSuccessful = bns.CreateConfiguration(prlConf);
                        operationResult.Message = "Bonus Configuration saved successfully.";
                    }

                    if (!errorFound && operationResult.IsSuccessful)
                    {
                        operationResult.IsSuccessful = true;
                        operationResult.Message = "Configuration saved.";
                        TempData.Add("msg", operationResult);
                        return RedirectToAction("ConfigureBonus");
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", operationResult);
            }

            

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);
            return View(bc);
        }

        public JsonResult GetEmployeeSeach(string query)
        {
            var lst = dataContext.prl_employee.AsEnumerable()
                .Where(x => x.name.ToLower().Contains(query.ToLower()) || x.emp_no.Contains(query))
                .Select(x => new SearchEmployeeData() { id = x.id, name = x.name }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [PayrollAuthorize]
        public ActionResult HoldBonus(int bonusId = 0)
        {

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);

            return View();
        }

        [HttpPost]
        public ActionResult HoldBonus(BonusHold _hldBonus)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult BonusProcess()
        {
            

            BonusProcess _processBonus = new BonusProcess();

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);

            var prlDivisionNames = dataContext.prl_division.ToList();
            ViewBag.DivisionNames = Mapper.Map<List<Division>>(prlDivisionNames);

            var prlDeptNames = dataContext.prl_department.ToList();
            ViewBag.DeptNames = Mapper.Map<List<Department>>(prlDeptNames);

            var prlGradeNames = dataContext.prl_grade.ToList();
            ViewBag.Grades = Mapper.Map<List<Grade>>(prlGradeNames);

            var prlReligionNames = dataContext.prl_religion.ToList();
            ViewBag.Religions = Mapper.Map<List<Religion>>(prlReligionNames);

            return View(_processBonus);
        }

        [HttpPost]
        public ActionResult BonusProcess(BonusProcess _processBonus)
        {
            var res = new OperationResult();
            int _result = 0;
            try
            {
                BonusService _service = new BonusService(dataContext);
                _result = _service.ProcessBonus(_processBonus, "", 0);
                if (_result > 0)
                {
                    res.IsSuccessful = true;
                    res.Message = "Bonus has been processed successfully.";
                    TempData.Add("msg", res);
                    return RedirectToAction("BonusProcess");
                }
                else
                {
                    if (_result == -101)
                    {
                        res.IsSuccessful = false;
                        res.Message = "Already bonus is processed for these above criteria.";
                    }
                    else
                    {
                        res.IsSuccessful = false;
                        res.Message = "Bonus has not been processed.";
                    }
                    TempData.Add("msg", res);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);
            var prlDivisionNames = dataContext.prl_division.ToList();
            ViewBag.DivisionNames = Mapper.Map<List<Division>>(prlDivisionNames);
            var prlDeptNames = dataContext.prl_department.ToList();
            ViewBag.DeptNames = Mapper.Map<List<Department>>(prlDeptNames);
            var prlGradeNames = dataContext.prl_grade.ToList();
            ViewBag.Grades = Mapper.Map<List<Grade>>(prlGradeNames);
            var prlReligionNames = dataContext.prl_religion.ToList();
            ViewBag.Religions = Mapper.Map<List<Religion>>(prlReligionNames);

            

            return View();
        }

        [PayrollAuthorize]
        public ActionResult UndoBonusProcess()
        {
            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);
            
            var prlReligionNames = dataContext.prl_religion.ToList();
            ViewBag.Religions = Mapper.Map<List<Religion>>(prlReligionNames);
            
            return View();
        }

        [HttpPost]
        public ActionResult UndoBonusProcess(BonusProcess _processBonus)
        {
            var res = new OperationResult();
            bool _result = false;
            try
            {
                BonusService _service = new BonusService();
                _result = _service.RollbackProcess(_processBonus, "", 0);
                if (_result == true)
                {
                    res.IsSuccessful = true;
                    res.Message = "Bonus has been rollbacked.";
                    TempData.Add("msg", res);
                    return RedirectToAction("UndoBonusProcess");
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message = "Bonus has not been rollbacked.";
                    TempData.Add("msg", res);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);
            var prlReligionNames = dataContext.prl_religion.ToList();
            ViewBag.Religions = Mapper.Map<List<Religion>>(prlReligionNames);


            return View();
        }

        public ActionResult DisburseBonus(string _param = "")
        {
            int bonusId = 0;
            DateTime festival_date = new DateTime();


            BonusProcess _bonusProcess;
            if (_param == "")
            {
                _bonusProcess = new BonusProcess();
            }
            else
            {
                string[] _param1 = _param.Split('T');
                bonusId = int.Parse(_param1[0].ToString());
                string _dt = _param1[1].ToString();
                festival_date = Convert.ToDateTime(_dt);

                var dbVal = dataContext.prl_bonus_process.SingleOrDefault(x => x.bonus_name_id == bonusId && x.festival_date.Value.Month == festival_date.Month
                                                                          && x.festival_date.Value.Year == festival_date.Year);
                if (dbVal == null)
                {
                    dbVal = new prl_bonus_process();
                }
                _bonusProcess = Mapper.Map<BonusProcess>(dbVal);
                _bonusProcess.bonus_name_id = bonusId;
                _bonusProcess.batch_no = dbVal.batch_no;
            }
            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);
            
            return View(_bonusProcess);
        }

        [HttpPost]
        public ActionResult DisburseBonus(BonusProcess _bnsProcess)
        {
            var res = new OperationResult();
            int _result = 0;
            try
            {
                BonusService _service = new BonusService(dataContext);
                _result = _service.DisburseProcess(_bnsProcess, "", 0);
                if (_result > 0)
                {
                    res.IsSuccessful = true;
                    res.Message = "Bonus Disbursement successfully.";
                    TempData.Add("msg", res);
                    return RedirectToAction("DisburseBonus");
                }
                else if (_result == -909)
                {
                    res.IsSuccessful = false;
                    res.Message = "Bonus already disbursed.";
                    TempData.Add("msg", res);
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message = "Disbursement of Bonus failed.";
                    TempData.Add("msg", res);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var prlBonusNames = dataContext.prl_bonus_name.ToList();
            ViewBag.BonusNames = Mapper.Map<List<BonusName>>(prlBonusNames);

            var prlReligionNames = dataContext.prl_religion.ToList();
            ViewBag.Religions = Mapper.Map<List<Religion>>(prlReligionNames);


            return View();
        }

        [PayrollAuthorize]
        [HttpGet]
        public PartialViewResult getBonusBatchNumber()
        {
            var prlBonusName = dataContext.prl_bonus_name.ToList();
            var bonusNames = Mapper.Map<List<BonusName>>(prlBonusName);
            BonusUploadView up = new BonusUploadView();
            up.BonusNames = bonusNames;
            return PartialView("_BonusUploadForm", up);
        }

        [HttpGet]
        public ActionResult UploadBonus()
        {

            return View();
        }

        [HttpGet]
        public PartialViewResult UploadBonusForm()
        {
            var prlBonusName = dataContext.prl_bonus_name.ToList();
            var bonusNames = Mapper.Map<List<BonusName>>(prlBonusName);
            BonusUploadView up = new BonusUploadView();
            up.BonusNames = bonusNames;
            return PartialView("_BonusUploadForm", up);
        }

        [HttpPost]
        public ActionResult UploadBonuse(BonusUploadView buV, HttpPostedFileBase fileupload)
        {
            var lstDat = new List<BonusUploadData>();
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
                            var d = new BonusUploadData();

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
                                    d.amount = val;
                                }
                                else
                                {
                                    d.ErrorMsg.Add("Row " + rowNum + " amount column should have decimal value");
                                }
                            }
                            if (ws.Cells[rowNum, 3].Value == null)
                            {
                                d.ErrorMsg.Add("Row " + rowNum + " does not have allowance name");
                            }
                            else
                            {
                                d.BonusNameString = ws.Cells[rowNum, 3].Value.ToString();
                            }

                            lstDat.Add(d);
                        }
                    }
                    HttpContext.Cache.Insert("currentBonusUploadInfo", buV, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    HttpContext.Cache.Insert("currentBonusUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                }
                catch (Exception ex)
                {
                    var d = ex.Message;
                }
            }
            else
            {
               
                return View(buV);
            }
            return Json(new { isUploaded = true, message = "hello" }, "text/html");
        }

        public PartialViewResult LoadUploadedBonusData(int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<BonusUploadData> products = null;

            var lst = new List<BonusUploadData>();
            lst = (List<BonusUploadData>)HttpContext.Cache["currentBonusUpload"];
            var pglst = lst.ToPagedList(pageIndex, pageSize);

            return PartialView("_BonusUploadedData", pglst);
        }

        public ActionResult SaveUploadedBonusData()
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var lst = new List<BonusUploadData>();
                lst = (List<BonusUploadData>)HttpContext.Cache["currentBonusUpload"];
                var dcv = (BonusUploadView)HttpContext.Cache["currentBonusUploadInfo"];
                // Need to Change
                var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.MonthYear.ToString()), 1);
                var dnames = dataContext.prl_bonus_name.ToList();

                foreach (var v in lst)
                {
                    var i = new prl_upload_bonus();
                    i.bonus_name_id = dnames.SingleOrDefault(x => x.name.ToLower() == v.BonusNameString.ToLower()).id;
                    i.emp_id = dataContext.prl_employee.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower()).id;
                    i.amount = v.amount;
                    i.month_year = salmon;
                    i.created_by = "khurshid";
                    i.created_date = DateTime.Now;
                    dataContext.prl_upload_bonus.Add(i);
                }

                dataContext.SaveChanges();

                operationResult.IsSuccessful = true;
                operationResult.Message = "Bonus uploaded successfully.";
                TempData.Add("msg", operationResult);
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
                TempData.Add("msg", operationResult);
            }
            return RedirectToAction("UploadBonus");
        }

        [PayrollAuthorize]
        public ActionResult EditUploadedBonuses()
        {

            return View();
        }

        [HttpGet]
        public PartialViewResult GetBonusDataSelection()
        {
            var prlBnsNames = dataContext.prl_bonus_name.ToList();
            var bonusNames = Mapper.Map<List<BonusName>>(prlBnsNames);
            BonusUploadView up = new BonusUploadView();
            up.BonusNames = bonusNames;

            return PartialView("_GetBonusDataSelection", up);
        }

        [HttpPost]
        public PartialViewResult GetBonusDataSelection(BonusUploadView buV)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                var dateToSearch = new DateTime(buV.Year, Convert.ToInt32(buV.MonthYear), 1);
                ViewBag.did = buV.BonusName;
                ViewBag.dt = dateToSearch;
                var lst = dataContext.prl_upload_bonus.Include("prl_bonus_name").AsEnumerable().Where(x => x.month_year.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.bonus_name_id == buV.BonusName);
                var kk = Mapper.Map<List<BonusUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedBonuses", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateBonusRecord(HttpRequestMessage request, string name, string pk, string value)
        {
            try
            {
                int primKey = 0;
                decimal amnt = 0;
                if (Int32.TryParse(pk, out primKey) && decimal.TryParse(value, out amnt))
                {
                    var original = dataContext.prl_upload_bonus.SingleOrDefault(x => x.id == primKey);
                    original.amount = amnt;
                    original.updated_by = "khurshid";
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

        public PartialViewResult EditBonusDataPaging(int did, DateTime dt, int? page)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.did = did;
                ViewBag.dt = dt;
                var lst = dataContext.prl_upload_bonus.Include("prl_bonus_name").AsEnumerable().Where(x => x.month_year.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.bonus_name_id == did);
                var kk = Mapper.Map<List<BonusUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedBonuses", kk);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
