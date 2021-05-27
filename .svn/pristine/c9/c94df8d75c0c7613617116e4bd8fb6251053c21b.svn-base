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

    public class AllowanceController : Controller
    {
        private readonly payroll_systemContext dataContext;

        //
        // GET: /Allowance/

        public AllowanceController(payroll_systemContext cont)
        {
            this.dataContext = cont;

        }

        public ActionResult AllowanceMain(string menuName)
        {
           
           
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            var lstAllHead = dataContext.prl_allowance_head.ToList();
            return View(Mapper.Map<List<AllowanceHead>>(lstAllHead));
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            var allHead = new AllowanceHead();
            return View(allHead);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(AllowanceHead item)
        {
            var res = new OperationResult();
            try
            {
                var allHead = Mapper.Map<prl_allowance_head>(item);
                dataContext.prl_allowance_head.Add(allHead);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = allHead.name + " created. ";
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
            var allHead = dataContext.prl_allowance_head.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<AllowanceHead>(allHead));
        }

        //
        // POST: /Allowance/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, AllowanceHead item)
        {
            var res = new OperationResult();
            try
            {
                var allHead = dataContext.prl_allowance_head.SingleOrDefault(x => x.id == item.id);
                allHead.name = item.name;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = item.name + " edited. ";
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
                var allHead = dataContext.prl_allowance_head.SingleOrDefault(x => x.id == id);
                if (allHead == null)
                {
                    return HttpNotFound();
                }
                name = allHead.name;
                dataContext.prl_allowance_head.Remove(allHead);
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
        public ActionResult ConfigureAllowance(int allwId = 0)
        {

            ViewBag.SelectedIndex = allwId;
            var prlGrds = dataContext.prl_grade.ToList();
            var grades = Mapper.Map<List<Grade>>(prlGrds);
            AllowanceConfiguration allwConfig;
            if (allwId == 0)
                allwConfig = new AllowanceConfiguration();
            else
            {
                var dbVal = dataContext.prl_allowance_configuration.SingleOrDefault(x => x.allowance_name_id == allwId);
                if (dbVal == null)
                {
                    dbVal = new prl_allowance_configuration();
                }
                allwConfig = Mapper.Map<AllowanceConfiguration>(dbVal);
                allwConfig.allowance_name_id = allwId;
                if (dbVal.allowance_name_id > 0)
                {
                    var ids = dbVal.prl_allowance_name.prl_grade.Select(x => x.id);
                    foreach (var g in grades)
                    {
                        if (ids.Contains(g.id))
                            g.IsSelected = true;
                    }
                }
            }

            allwConfig.Grades = grades;

            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            ViewBag.AllowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);
            return View(allwConfig);
        }

        [HttpPost]
        public ActionResult ConfigureAllowance(AllowanceConfiguration ac)
        {
            
            bool errorFound = false;
            var operationResult = new OperationResult();

            try
            {
                if (ModelState.IsValid)
                {
                    //check to see if grade is selected
                    var lstOfGrades = new List<prl_grade>();

                    if (!ac.Grades.Any(x => x.IsSelected == true))
                    {
                        errorFound = true;
                        ModelState.AddModelError("Grades", "No grade(s) selected.");
                    }
                    if (ac.flat_amount == null && ac.percent_amount == null)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Enter flat or percentage amount.");
                    }
                    if (ac.flat_amount <= 0 || ac.percent_amount <= 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Flat or percentage amount should be greater than zero.");
                    }
                    //if (ac.exempted_amount == null && ac.exempted_percentage == null)
                    //{
                    //    errorFound = true;
                    //    ModelState.AddModelError("", "Enter exempted flat or percentage amount.");
                    //}
                    if (!errorFound)
                    {
                        ac.Grades = ac.Grades.Where(x => x.IsSelected == true);
                        var ids = ac.Grades.Select(x => x.id).ToList();
                        lstOfGrades = dataContext.prl_grade.AsEnumerable().Where(x => ids.Contains(x.id)).ToList();
                    }
                    if (ac.deactivation_date != null)
                    {
                        var k = ((DateTime)ac.deactivation_date).Subtract((DateTime)ac.activation_date);
                        if (k.Days <= 0)
                        {
                            errorFound = true;
                            ModelState.AddModelError("deactivation_date", "Deactivation date should be greater than activation date");
                        }
                    }
                    if (!errorFound)
                    {
                        var prlConf = Mapper.Map<prl_allowance_configuration>(ac);
                        prlConf.prl_allowance_name = dataContext.prl_allowance_name.SingleOrDefault(x => x.id == ac.allowance_name_id);
                        if (prlConf.prl_allowance_name != null)
                        {
                            prlConf.prl_allowance_name.prl_grade.Clear();
                            prlConf.prl_allowance_name.prl_grade = lstOfGrades;
                        }
                        else
                        {
                            prlConf.prl_allowance_name.prl_grade = new Collection<prl_grade>();
                            prlConf.prl_allowance_name.prl_grade = lstOfGrades;
                        }

                        AllowanceService alls = new AllowanceService(dataContext);
                        operationResult.IsSuccessful = alls.CreateConfiguration(prlConf);
                        operationResult.Message = "Allowance saved successfully.";
                    }

                    if (!errorFound && operationResult.IsSuccessful)
                    {
                        operationResult.IsSuccessful = true;
                        operationResult.Message = "Configuration saved.";
                        TempData.Add("msg", operationResult);
                        return RedirectToAction("ConfigureAllowance");
                    }
                }
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", operationResult);
            }

            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            ViewBag.AllowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);
            return View(ac);
        }

        [PayrollAuthorize]
        public ActionResult IndividualAllowance(int eid = 0)
        {

            return View();
        }

        public JsonResult GetEmployeeSeach(string query)
        {
            var lst = dataContext.prl_employee.AsEnumerable().Where(x => x.name.Contains(query) || x.emp_no.Contains(query)).Select(x => new SearchEmployeeData() { id = x.id, name = x.name }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeAllowances(int empid)
        {
            ViewBag.EmpId = empid;

            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var lst = dataContext.prl_employee_individual_allowance.Where(x => x.emp_id == empid).ToList();
            return View("IndvAllowance", Mapper.Map<List<EmployeeIndividualAllowance>>(lst));
        }

        public ActionResult EmployeeAllowanceDetails(int edi, int empid)
        {
            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            ViewBag.AllowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);

            if (edi == 0)
            {
                var a = new EmployeeIndividualAllowance() { emp_id = empid };
                return View("EmpAllowance", Mapper.Map<EmployeeIndividualAllowance>(a));
            }

            var obj = dataContext.prl_employee_individual_allowance.SingleOrDefault(x => x.id == edi);

            return View("EmpAllowance", Mapper.Map<EmployeeIndividualAllowance>(obj));
        }

        [HttpPost]
        public ActionResult ChangeEmployeeAllowance(EmployeeIndividualAllowance eidObj)
        {
            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == eidObj.emp_id);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            ViewBag.AllowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);
            if (ModelState.IsValid)
            {
                try
                {
                    var newOb = Mapper.Map<prl_employee_individual_allowance>(eidObj);

                    if (eidObj.id == 0)
                    {
                        dataContext.prl_employee_individual_allowance.Add(newOb);
                    }
                    else
                    {
                        var extOb = dataContext.prl_employee_individual_allowance.SingleOrDefault(x => x.id == eidObj.id);
                        var entry = dataContext.Entry(extOb);
                        entry.Property(x => x.id).IsModified = false;
                        entry.CurrentValues.SetValues(newOb);
                        entry.State = System.Data.EntityState.Modified;
                    }
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                return View("EmpAllowance");
            }


            return RedirectToAction("GetEmployeeAllowances", new { empid = eidObj.emp_id });
        }

        [PayrollAuthorize]
        public ActionResult DeleteEmployeeAllowance(int id, int empid)
        {
            OperationResult operationResult = new OperationResult();
            if (ModelState.IsValid)
            {
                try
                {
                    var extOb = dataContext.prl_employee_individual_allowance.SingleOrDefault(x => x.id == id);
                    dataContext.prl_employee_individual_allowance.Remove(extOb);
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
            return RedirectToAction("GetEmployeeAllowances", new { empid = empid });
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult UploadAllowance()
        {
           
            return View();
        }

        [HttpGet]
        public PartialViewResult UploadForm()
        {
            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            var allowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);
            AllowanceUploadView up = new AllowanceUploadView();
            up.AllowanceNames = allowanceNames;
            return PartialView("_AllowanceUploadForm", up);
        }

        [HttpPost]
        public ActionResult UploadAllowance(AllowanceUploadView auV, HttpPostedFileBase fileupload)
        {
            var lstDat = new List<AllowanceUploadData>();
            if (ModelState.IsValid)
            {
                try
                {
                    fileupload.InputStream.Position = 0;
                    using (var package = new ExcelPackage(fileupload.InputStream))
                    {
                        var ws = package.Workbook.Worksheets.First();
                        var startRow = 2;

                        var firstColumPos = ws.Cells.FirstOrDefault(x => x.Value.ToString().Trim() == "ID Number");
                        startRow = firstColumPos.Start.Row + 1;

                        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            var d = new AllowanceUploadData();

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
                                d.AllowanceNameString = ws.Cells[rowNum, 3].Value.ToString();
                            }

                            lstDat.Add(d);
                        }
                    }
                    HttpContext.Cache.Insert("currentAllowanceUploadInfo", auV, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    HttpContext.Cache.Insert("currentAllowanceUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                }
                catch (Exception ex)
                {
                    var d = ex.Message;
                }
            }
            else
            {
                return View(auV);
            }
            return Json(new { isUploaded = true, message = "hello" }, "text/html");
        }

        public PartialViewResult LoadUploadedData(int? page)
        {
            int pageSize = 30;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<AllowanceUploadData> products = null;

            var lst = new List<AllowanceUploadData>();
            lst = (List<AllowanceUploadData>)HttpContext.Cache["currentAllowanceUpload"];
            var pglst = lst.ToPagedList(pageIndex, pageSize);

            return PartialView("_AllowanceUploadedData", pglst);
        }

        public ActionResult SaveUploadedData()
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var lst = new List<AllowanceUploadData>();
                lst = (List<AllowanceUploadData>)HttpContext.Cache["currentAllowanceUpload"];
                var dcv = (AllowanceUploadView)HttpContext.Cache["currentAllowanceUploadInfo"];
                var salmon = new DateTime(dcv.Year, Convert.ToInt32(dcv.Month.ToString()), 1);
                var dnames = dataContext.prl_allowance_name.ToList();
                var notFoundMsg = "";

                ////////// duplicate check
                var lstEmpNo = lst.AsEnumerable().Select(x => x.EmployeeID).ToList();
                var lstSysEmpId = dataContext.prl_employee.AsEnumerable().Where(x => lstEmpNo.Contains(x.emp_no)).ToList();
                var existingUploadedData = dataContext.prl_upload_allowance.Include("prl_allowance_name").AsEnumerable()
                        .Where(x => x.salary_month_year.Value.ToString("yyyy-MM") == salmon.ToString("yyyy-MM"))
                        .ToList();
                /////////////////////////////
                
                foreach (var v in lst)
                {
                    var i = new prl_upload_allowance();
                    var prlAllowanceName = dnames.SingleOrDefault(x => x.allowance_name.ToLower() == v.AllowanceNameString.ToLower());
                    if (prlAllowanceName == null)
                    {
                        operationResult.HasPartialError = true;
                        operationResult.Messages.Add(" Could not find allowance name " + v.AllowanceNameString);
                        continue;
                    }
                    i.allowance_name_id = prlAllowanceName.id;
                    var singleOrDefault = lstSysEmpId.AsEnumerable().SingleOrDefault(x => x.emp_no.ToLower() == v.EmployeeID.ToLower());
                    if (singleOrDefault == null)
                    {
                        operationResult.HasPartialError = true;
                        operationResult.Messages.Add(" Could not find employee number " + v.EmployeeID);
                        continue;
                    }

                    var duplicateData = existingUploadedData.AsEnumerable().FirstOrDefault(x => x.emp_id == singleOrDefault.id && x.allowance_name_id == prlAllowanceName.id);
                    if (duplicateData != null)
                    {
                        operationResult.HasPartialError = true;
                        operationResult.Messages.Add(" Employee " + v.EmployeeID + " allowance already exist in the system. ");
                        continue;
                    }

                    i.emp_id = singleOrDefault.id;
                    i.amount = v.amount;
                    i.salary_month_year = salmon;
                    i.created_by = "khurshid";
                    i.created_date = DateTime.Now;
                    dataContext.prl_upload_allowance.Add(i);
                }

                dataContext.SaveChanges();

                operationResult.IsSuccessful = true;
                operationResult.Message = "Allowance uploaded successfully. "+ notFoundMsg;
                TempData.Add("msg", operationResult);
            }
            catch (Exception ex)
            {
                operationResult.IsSuccessful = false;
                operationResult.Message = ex.Message;
                TempData.Add("msg", operationResult);
            }
            return RedirectToAction("UploadAllowance");
        }

        [PayrollAuthorize]
        public ActionResult EditUploadedAllowance()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult GetAllowanceDataSelection()
        {
            var prlAllowNames = dataContext.prl_allowance_name.ToList();
            var allowanceNames = Mapper.Map<List<AllowanceName>>(prlAllowNames);
            AllowanceUploadView up = new AllowanceUploadView();
            up.AllowanceNames = allowanceNames;

            return PartialView("_GetAllowanceDataSelection", up);
        }

        [HttpPost]
        public PartialViewResult GgetAllowanceDataSelection(AllowanceUploadView auV)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                var dateToSearch = new DateTime(auV.Year, Convert.ToInt32(auV.Month), 1);
                ViewBag.did = auV.AllowanceName;
                ViewBag.dt = dateToSearch;
                var lst = dataContext.prl_upload_allowance.Include("prl_allowance_name").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM")) && x.allowance_name_id == auV.AllowanceName);
                var kk = Mapper.Map<List<AllowanceUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedAllowances", kk);
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
                    var original = dataContext.prl_upload_allowance.SingleOrDefault(x => x.id == primKey);
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

        public PartialViewResult EditDataPaging(int did, DateTime dt, int? page)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.did = did;
                ViewBag.dt = dt;
                var lst = dataContext.prl_upload_allowance.Include("prl_allowance_name").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.allowance_name_id == did);
                var kk = Mapper.Map<List<AllowanceUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedAllowances", kk);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [PayrollAuthorize]
        public ActionResult ChildrenAllowance()
        {
            var list = dataContext.prl_employee_children_allowance.ToList();
            var vwList = Mapper.Map<List<ChildrenAllowance>>(list).ToPagedList(1, 15);
            return View(vwList);
        }

        public ActionResult Paging(int? page)
        {

            int pageSize = 15;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = dataContext.prl_employee_children_allowance.ToList();
            var vwList = Mapper.Map<List<ChildrenAllowance>>(list);
            var pglst = vwList.ToPagedList(pageIndex, pageSize);

            return View("ChildrenAllowance", pglst);
        }

        [PayrollAuthorize]
        public ActionResult SubmitChildrenAllowance(int? id)
        {
            if (id > 0)
            {
                var _Emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == id);
                return View(Mapper.Map<Employee>(_Emp));
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SubmitChildrenAllowance(int? empid, FormCollection collection, ChildrenAllowance childAllw, string sButton)
        {
            bool errorFound = false;
            var res = new OperationResult();
            int _Result = 0;
            try
            {
                if (sButton == "Search")
                {
                    if (empid == null && collection["Emp_No"] == null)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "Please select an employee or put employee no.");
                    }
                    else
                    {
                        if (empid != null)
                        {
                            var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid && x.is_active == 1);
                            var empD = Mapper.Map<Employee>(_empD);
                            ViewBag.Employee = empD;
                            childAllw.emp_id = empD.id;
                            return View(childAllw);
                        }
                        else
                        {
                            var _empD = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"] && x.is_active == 1);
                            if (_empD == null)
                            {
                                ModelState.AddModelError("", "Threre is no information for the given employee no.");
                            }
                            else
                            {
                                var empD = Mapper.Map<Employee>(_empD);
                                ViewBag.Employee = empD;
                                childAllw.emp_id = empD.id;
                                return View(childAllw);
                            }
                        }
                    }
                }
                else if (sButton == "Save")
                {
                    if (childAllw.emp_id == 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (!errorFound)
                    {

                        AllowanceService aService = new AllowanceService(dataContext);
                        var c_all = Mapper.Map<prl_employee_children_allowance>(childAllw);
                        _Result = aService.CreateChildrenAllowance(c_all);

                        if (_Result > 0)
                        {
                            res.IsSuccessful = true;
                            res.Message = "Children allowance is submitted successfully.";
                            TempData.Add("msg", res);
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Children allowance is not submitted successfully.";
                            TempData.Add("msg", res);
                        }
                    }
                }
                return RedirectToAction("ChildrenAllowance");
            }
            catch
            {
                return View();
            }
        }

        [PayrollAuthorize]
        public ActionResult DeleteChildrenAllowance(int id)
        {
            string name = "";
            int result = 0;
            var res = new OperationResult();
            try
            {
                AllowanceService _as = new AllowanceService(dataContext);
                result = _as.DeleteEmployeeChildAllowance(id);

                if (result > 0)
                {
                    res.IsSuccessful = true;
                    res.Message = "Children Allowance deleted successfully";
                    TempData.Add("msg", res);
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message = "Cannot delete";
                    TempData.Add("msg", res);
                }
                return RedirectToAction("ChildrenAllowance");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = name + " could not delete.";
                TempData.Add("msg", res);
                return RedirectToAction("ChildrenAllowance");
            }
        }
    }
}
