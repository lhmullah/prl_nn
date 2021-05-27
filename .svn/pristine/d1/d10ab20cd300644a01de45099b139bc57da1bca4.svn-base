using AutoMapper;
using com.gpit.DataContext;
using com.gpit.Model;
using OfficeOpenXml;
using PagedList;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Service;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;



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

        [PayrollAuthorize]
        public ActionResult Edit(int id)
        {
            var dedHead = dataContext.prl_deduction_head.SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<DeductionHead>(dedHead));
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
                    name.gl_code = dn.gl_code;
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
                if (dbVal.prl_deduction_name != null)
                {
                    var ids = dbVal.prl_deduction_name.prl_grade.Select(x => x.id);
                    foreach (var g in grades)
                    {
                        if (ids.Contains(g.id))
                            g.IsSelected = true;
                    }
                }
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
                    //check to see if grade is selected
                    var lstOfGrades = new List<prl_grade>();

                    if (!dc.Grades.Any(x => x.IsSelected == true))
                    {
                        errorFound = true;
                        ModelState.AddModelError("Grades", "No grade(s) selected.");
                    }
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
                    if (!errorFound)
                    {
                        dc.Grades = dc.Grades.Where(x=> x.IsSelected == true).ToList();
                        var ids = dc.Grades.Select(x => x.id ).ToList();
                        lstOfGrades = dataContext.prl_grade.AsEnumerable().Where(x => ids.Contains(x.id)).ToList();

                    }
                    if (dc.deactivation_date != null)
                    {
                        var k = ((DateTime) dc.deactivation_date).Subtract((DateTime) dc.activation_date);
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
                        if (prlConf.prl_deduction_name != null)
                        {
                            prlConf.prl_deduction_name.prl_grade.Clear();
                            prlConf.prl_deduction_name.prl_grade = lstOfGrades;
                        }
                        else
                        {
                            prlConf.prl_deduction_name.prl_grade = new Collection<prl_grade>();
                            prlConf.prl_deduction_name.prl_grade = lstOfGrades;
                        }

                        DeductionService ds = new DeductionService(dataContext);
                        operationResult.IsSuccessful = ds.CreateConfiguration(prlConf);
                        operationResult.Message = "Deduction saved successfully.";
                    }

                    if (!errorFound && operationResult.IsSuccessful)
                    {
                        operationResult.IsSuccessful = true;
                        operationResult.Message = "Configuration saved.";
                        TempData.Add("msg", operationResult);
                        return RedirectToAction("ConfigureDeduction", new {dnid = 0});
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

        public JsonResult GetEmployeeSeach(string query)
        {
            var lst =
                dataContext.prl_employee.AsEnumerable()
                    .Where(x => x.name.ToLower().Contains(query.ToLower()) || x.emp_no.Contains(query))
                    .Select(x => new SearchEmployeeData() {id = x.id, name = x.name})
                    .ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeDeductions(int empid)
        {
            ViewBag.EmpId = empid;

            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var lst = dataContext.prl_employee_individual_deduction.Where(x => x.emp_id == empid).ToList();
            return View("IndvDeduction", Mapper.Map<List<EmployeeIndividualDeduction>>(lst));
        }

        public ActionResult EmployeeDeductionDetails(int edi, int empid)
        {
            var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
            ViewBag.Employee = Mapper.Map<Employee>(emp);

            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            ViewBag.DeductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);

            if (edi == 0)
            {
                var a = new EmployeeIndividualDeduction() {emp_id = empid};
                return View("EmpDedcution", Mapper.Map<EmployeeIndividualDeduction>(a));
            }

            var obj = dataContext.prl_employee_individual_deduction.SingleOrDefault(x => x.id == edi);

            return View("EmpDedcution", Mapper.Map<EmployeeIndividualDeduction>(obj));
        }

        [HttpPost]
        public ActionResult ChangeEmployeeDeduction(EmployeeIndividualDeduction eidObj)
        {
            OperationResult operationResult = new OperationResult();



            var emp =
                dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == eidObj.emp_id);
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


            return RedirectToAction("GetEmployeeDeductions", new {empid = eidObj.emp_id});
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
            return RedirectToAction("GetEmployeeDeductions", new {empid = empid});
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult UploadDeduction()
        {

            return View();
        }

        [HttpGet]
        public PartialViewResult UploadForm()
        {
            var prlDeducNames = dataContext.prl_deduction_name.ToList();
            var deductionNames = Mapper.Map<List<DeductionName>>(prlDeducNames);
            DeductionUploadView up = new DeductionUploadView();
            up.DeductionNames = deductionNames;
            return PartialView("_DeductionUploadForm",up);
        }

        [HttpPost]
        public ActionResult UploadDeduction(DeductionUploadView dcv,HttpPostedFileBase fileupload)
        {
            var lstDat = new List<DeductionUploadData>();
            if (ModelState.IsValid)
            {
                try
                {
                    fileupload.InputStream.Position = 0;
                    using (var package = new ExcelPackage(fileupload.InputStream))
                    {
                        var ws = package.Workbook.Worksheets.First();
                        var startRow =  2;

                        var firstColumPos = ws.Cells.FirstOrDefault(x => x.Value.ToString().Trim() == "ID Number");
                        startRow = firstColumPos.Start.Row + 1;

                        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            var d = new DeductionUploadData();

                            if (ws.Cells[rowNum, 1].Value == null)
                            {
                                d.ErrorMsg.Add("Row "+rowNum + "does not have an employee ID");
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
                                d.ErrorMsg.Add("Row " + rowNum + " does not have deduction name");
                            }
                            else
                            {
                                d.DeductionNameString = ws.Cells[rowNum, 3].Value.ToString();
                            }
                           
                            lstDat.Add(d);
                        }
                    }
                    HttpContext.Cache.Insert("currentDeductionUploadInfo", dcv, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    HttpContext.Cache.Insert("currentDeductionUpload", lstDat, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                }
                catch (Exception ex)
                {
                    var d = ex.Message;
                }
            }
            else
            {
                return View(dcv);
            }
            return Json(new { isUploaded = true, message = "hello" }, "text/html");
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

            return PartialView("_DeductionUploadedData",pglst);
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
            
            return PartialView("_GetDeductionDataSelection",up);
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
                var lst = dataContext.prl_upload_deduction.Include("prl_deduction_name").Include("prl_employee").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dateToSearch.ToString("yyyy-MM"))&& x.deduction_name_id==dcv.DeductionName);
                var kk = Mapper.Map<List<DeductionUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return PartialView("_UploadedDeductions", kk);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        [HttpPost]
        public JsonResult UpdateRecord(HttpRequestMessage request,string name, string pk, string value)
        {
            try
            {
                int primKey = 0;
                decimal amnt = 0;
                if (Int32.TryParse(pk, out primKey) && decimal.TryParse(value,out amnt))
                {
                    var original = dataContext.prl_upload_deduction.SingleOrDefault(x => x.id == primKey);
                    original.amount = amnt;
                    original.updated_by = "russell";
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

        public PartialViewResult EditDataPaging(int did,DateTime dt, int? page)
        {
            int pageSize = 30;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.did = did;
            ViewBag.dt = dt;
            var lst = dataContext.prl_upload_deduction.Include("prl_deduction_name").Include("prl_employee").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.deduction_name_id==did);
            var kk = Mapper.Map<List<DeductionUploadData>>(lst).ToPagedList(pageIndex, pageSize);

            return PartialView("_UploadedDeductions", kk);
        }
      
        
    }
}
