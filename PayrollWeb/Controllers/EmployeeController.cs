using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using com.linde.DataContext;
using com.linde.Model;
using PagedList;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;
using PayrollWeb.CustomSecurity;
using PayrollWeb.ViewModels.Utility;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Data.OleDb;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;

namespace PayrollWeb.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly payroll_systemContext dataContext;
        List<string> errorIds = new List<string>();

        public EmployeeController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        public ActionResult Index(int? empid, FormCollection collection, string sButton)
        {
            //if (Session["_EmpD"] != null)
            //    Session["_EmpD"] = null;
            //if (Session["NewEmp"] != null)
            //    Session["NewEmp"] = null;
            //if (Session["NewEmpForEdit"] != null)
            //    Session["NewEmpForEdit"] = null;
            //if (Session["NewEmpDetailForEdit"] != null)
            //    Session["NewEmpDetailForEdit"] = null;

            if (Request.Cookies["_EmpD"] != null)
            {
                Response.Cookies["_EmpD"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies["NewEmp"] != null)
            {
                Response.Cookies["NewEmp"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies["NewEmpForEdit"] != null)
            {
                Response.Cookies["NewEmpForEdit"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies["NewEmpDetailForEdit"] != null)
            {
                Response.Cookies["NewEmpDetailForEdit"].Expires = DateTime.Now.AddDays(-1);
            }


            var lists = new List<Employee>().ToPagedList(1, 1);

            if (sButton == null)
            {
                var lstEmp = dataContext.prl_employee.Include("prl_employee_details").OrderBy(x => x.id);
                lists = Mapper.Map<List<Employee>>(lstEmp).ToPagedList(1, 25);
            }
            else
            {
                if (empid == null && string.IsNullOrEmpty(collection["Emp_No"]))
                {
                    //errorFound = true;
                    ModelState.AddModelError("", "Please select an employee or put employee ID");
                }
                else
                {
                    if (empid != null)
                    {
                        var _emp = dataContext.prl_employee.Include("prl_employee_details").Where(x => x.id == empid);
                        lists = Mapper.Map<List<Employee>>(_emp).ToPagedList(1, 1);
                    }
                    else
                    {
                        var _emp = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().Where(x => x.emp_no == collection["Emp_No"]);
                        if (_emp.Count() > 0)
                        {
                            lists = Mapper.Map<List<Employee>>(_emp).ToPagedList(1, 1);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Threre is no information for the given employee ID");
                        }
                    }
                }
            }
            return View(lists);
        }

        public ActionResult Paging(int? page)
        {
            int pageSize = 25;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var _employees = dataContext.prl_employee.Include("prl_employee_details").OrderBy(x => x.emp_no);
            var empList = Mapper.Map<List<Employee>>(_employees);

            var pglst = empList.ToPagedList(pageIndex, pageSize);

            return View("Index", pglst);
        }

        [PayrollAuthorize]
        public ActionResult Details(int id)
        {
            var lstEmpD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == id);
            return View(Mapper.Map<Employee>(lstEmpD));
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {

            var _Emp = new Employee();
            //if (Session["NewEmp"] != null)
            //{
            //    _Emp = (Employee)Session["NewEmp"];
            //}

            if (Request.Cookies["NewEmp"] != null)
            {
                //_Emp = (Employee)Session["NewEmp"];

                string cookievalue = Request.Cookies["NewEmp"].Value.ToString();
                _Emp = JsonConvert.DeserializeObject<Employee>(cookievalue);
            }
            var lstReligion = dataContext.prl_religion.ToList();
            ViewBag.Religions = lstReligion;

            //var lstCompany = dataContext.prl_company.ToList();
            //ViewBag.Companies = lstCompany;

            _Emp.joining_date = DateTime.Now;
            return View(_Emp);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Create(Employee emp)
        {
            var res = new OperationResult();
            if (ModelState.IsValid)
            {
                var empInfo = dataContext.prl_employee.SingleOrDefault(x => x.emp_no == emp.emp_no);

                if (empInfo == null)
                {
                    //Session["NewEmp"] = emp;
                    string jsonString = JsonConvert.SerializeObject(emp);
                    Response.Cookies["NewEmp"].Value = jsonString;

                    return RedirectToAction("CreateEmpDetails");
                }
                else
                {
                    res.IsSuccessful = false;
                    res.Message = "This Employee ID is already existed.";
                    TempData.Add("msg", res);

                    // return RedirectToAction("Create");
                }
            }

            var lstReligion = dataContext.prl_religion.ToList();
            ViewBag.Religions = lstReligion;

            //var lstCompany = dataContext.prl_company.ToList();
            //ViewBag.Companies = lstCompany;

            return View();
        }

        [PayrollAuthorize]
        public ActionResult CreateEmpDetails()
        {
            //var _Emp = (Employee)Session["NewEmp"];
            string cookievalue = Request.Cookies["NewEmp"].Value.ToString();
            var _Emp = JsonConvert.DeserializeObject<Employee>(cookievalue);

            var EmpD = new EmployeeDetails();
            EmpD.name = _Emp.name;
            //if (Session["_EmpD"] == null)
            //{

            //}
            if (Request.Cookies["_EmpD"] == null)
            {

            }
            else
            {
                //var _empD = (EmployeeDetails)Session["_EmpD"];
                string cookievalueDetails = Request.Cookies["_EmpD"].Value.ToString();
                var _empD = JsonConvert.DeserializeObject<EmployeeDetails>(cookievalueDetails);

                EmpD.emp_status = _empD.emp_status;
                EmpD.employee_category = _empD.employee_category;
                EmpD.job_level_id = _empD.job_level_id != null ? _empD.job_level_id : 0;

                //EmpD.department_id = _empD.department_id;
                //EmpD.sub_department_id = _empD.sub_department_id != null ? _empD.sub_department_id : 0;
                //EmpD.sub_sub_department_id = _empD.sub_sub_department_id != null ? _empD.sub_sub_department_id : 0;

                EmpD.designation_id = _empD.designation_id;
                EmpD.cost_centre_id = _empD.cost_centre_id;

                EmpD.basic_salary = _empD.basic_salary;
               // EmpD.marital_status = _empD.marital_status;
                //EmpD.blood_group = _empD.blood_group;
                EmpD.parmanent_address = _empD.parmanent_address;
                EmpD.present_address = _empD.present_address;
                EmpD.created_by = User.Identity.Name;
                EmpD.created_date = DateTime.Now;
            }

            var lstCostCentre = dataContext.prl_cost_centre.ToList();
            ViewBag.CostCentre = lstCostCentre;

            var lstDesig = dataContext.prl_designation.ToList();
            ViewBag.Designations = lstDesig;

            var lstDept = dataContext.prl_department.ToList();
            ViewBag.Departments = lstDept;

            ViewBag.SubDepartments = "";
            ViewBag.SubSubDepartments = "";

            var lstJobLevel = dataContext.prl_job_level.ToList();
            ViewBag.JobLevel = lstJobLevel;

            return View(EmpD);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult CreateEmpDetails(EmployeeDetails empD, string submitButton)
        {
            var res = new OperationResult();
            //var _InsertingEmp = (Employee)Session["NewEmp"];

            string cookievalue = Request.Cookies["NewEmp"].Value.ToString();
            var _InsertingEmp = JsonConvert.DeserializeObject<Employee>(cookievalue);

            if (submitButton == "Previous")
            {
                //Session["_EmpD"] = empD;

                string jsonString = JsonConvert.SerializeObject(empD);
                Response.Cookies["_EmpD"].Value = jsonString;

                return RedirectToAction("Create");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        empD.created_by = User.Identity.Name;
                        empD.created_date = DateTime.Now;


                        _InsertingEmp.prl_employee_details.Add(empD);

                        var nwEmp = Mapper.Map<prl_employee>(_InsertingEmp);
                        nwEmp.is_active = 1;
                        nwEmp.created_by = User.Identity.Name;
                        nwEmp.created_date = DateTime.Now;
                        dataContext.prl_employee.Add(nwEmp);
                        dataContext.SaveChanges();

                        //var _salaryReview = new SalaryReview();
                        //_salaryReview.emp_id = nwEmp.id;
                        //_salaryReview.current_basic = empD.basic_salary;
                        //_salaryReview.new_basic = empD.basic_salary;

                        //_salaryReview.increment_reason = "Joining";
                        //_salaryReview.description = "Joined";
                        //_salaryReview.effective_from = _InsertingEmp.joining_date;
                        //_salaryReview.is_arrear_calculated = "NO";
                        //_salaryReview.created_by = User.Identity.Name;
                        //_salaryReview.created_date = DateTime.Now;

                        //var _review = Mapper.Map<prl_salary_review>(_salaryReview);
                        //dataContext.prl_salary_review.Add(_review);
                        //dataContext.SaveChanges();

                        Users CreateUser = new Users()
                        {
                            Emp_Id = nwEmp.id,
                            User_Name = nwEmp.emp_no,
                            Email = nwEmp.email != "" && nwEmp.email !=null ? nwEmp.email:"N/A",
                            Role_Name = "User",
                            Password = nwEmp.emp_no + "@" + nwEmp.id.ToString() + DateTime.Now.Millisecond.ToString(), // first Time Password as Like same as emp_no
                            PasswordQuestion = "What is your company name?",
                            PasswordAnswer = "Novo Nordisk",
                            created_by = User.Identity.Name,
                            created_date = DateTime.Today
                        };

                        var _prl_users = Mapper.Map<prl_users>(CreateUser);
                        dataContext.prl_users.Add(_prl_users);
                        dataContext.SaveChanges();

                        //Sending Email to Employee

                        var emp_prl_user = dataContext.prl_users.SingleOrDefault(x => x.emp_id == nwEmp.id);

                        if (emp_prl_user != null)
                        {
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp_prl_user.emp_id);

                            if (emp.email != "N/A" && emp.email != null && emp.email != "" && emp.is_active == 1)
                            {
                                if (Utility.CommonFunctions.IsValidEmail(emp.email) == true)
                                {
                                    EmailNotification_NewEmployee(emp, emp.email, emp_prl_user.password, "PayrollAccount");
                                }
                            }

                        }

                        res.IsSuccessful = true;
                        res.Message = "Employee information of " + nwEmp.name + " saved successfully.";
                        TempData.Add("msg", res);

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                    }
                }

                var lstCostCentre = dataContext.prl_cost_centre.ToList();
                ViewBag.CostCentre = lstCostCentre;

                var lstDesig = dataContext.prl_designation.ToList();
                ViewBag.Designations = lstDesig;

                var lstDept = dataContext.prl_department.ToList();
                ViewBag.Departments = lstDept;

                var lstSubDept = dataContext.prl_sub_department.ToList();
                ViewBag.SubDepartments = lstSubDept;

                var lstSubSubDept = dataContext.prl_sub_sub_department.ToList();
                ViewBag.SubSubDepartments = lstSubSubDept;

                var lstJobLevel = dataContext.prl_job_level.ToList();
                ViewBag.JobLevel = lstJobLevel;


                return View();
            }
        }

        [PayrollAuthorize]
        public ActionResult Edit(int? id)
        {
            var _empInfoById = new Employee();
            //if (Session["NewEmpForEdit"] != null)
            //{
            //    _empInfoById = (Employee)Session["NewEmpForEdit"];
            //}

            if (Request.Cookies["NewEmpForEdit"] != null)
            {
                string cookievalue = Request.Cookies["NewEmpForEdit"].Value.ToString();
                _empInfoById = JsonConvert.DeserializeObject<Employee>(cookievalue);
            }
            else
            {
                var empD = dataContext.prl_employee.SingleOrDefault(x => x.id == id).prl_employee_details.OrderByDescending(x => x.id).First();
                var _empDetailsfoForEdit = Mapper.Map<EmployeeDetails>(empD);

                _empInfoById = dataContext.prl_employee.Where(x => x.id == id).Select(x => new Employee
                {
                    emp_no = x.emp_no,
                    name = x.name,
                    joining_date = x.joining_date,
                    official_contact_no = x.official_contact_no,
                    personal_contact_no = x.personal_contact_no,
                    email = x.email,
                    personal_email = x.personal_email,
                    religion_id = x.religion_id,
                    gender = x.gender,
                    dob = x.dob,
                    tin = x.tin,

                }).SingleOrDefault();

                _empInfoById.prl_employee_details.Add(_empDetailsfoForEdit);
            }

            var lstReligion = dataContext.prl_religion.ToList();
            ViewBag.Religions = lstReligion;

            return View(_empInfoById);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Edit(Employee UpdatingEmp)
        {
            var res = new OperationResult();
            if (ModelState.IsValid)
            {
                try
                {
                    //Session["NewEmpForEdit"] = UpdatingEmp;
                    string jsonString = JsonConvert.SerializeObject(UpdatingEmp);
                    Response.Cookies["NewEmpForEdit"].Value = jsonString;

                    return RedirectToAction("EditEmpDetails");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }

            var lstReligion = dataContext.prl_religion.ToList();
            ViewBag.Religions = lstReligion;

            return View();
        }

        [PayrollAuthorize]
        public ActionResult EditEmpDetails()
        {

            var _empDetailsForEdit = new EmployeeDetails();
            //var _empInfoForEdit = (Employee)Session["NewEmpForEdit"];

            string cookievalue = Request.Cookies["NewEmpForEdit"].Value.ToString();
            var _empInfoForEdit = JsonConvert.DeserializeObject<Employee>(cookievalue);

            //if (Session["NewEmpDetailForEdit"] != null)
            //{
            //    _empDetailsForEdit = (EmployeeDetails)Session["NewEmpDetailForEdit"];
            //}

            if (Request.Cookies["NewEmpDetailForEdit"] != null)
            {
                string cookievalueDetails = Request.Cookies["NewEmpDetailForEdit"].Value.ToString();
                _empDetailsForEdit = JsonConvert.DeserializeObject<EmployeeDetails>(cookievalueDetails);
            }
            else
            {
                if (_empInfoForEdit != null)
                {
                    _empDetailsForEdit = Mapper.Map<EmployeeDetails>(_empInfoForEdit.prl_employee_details[0]);
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }
            ViewData["EmpNum"] = _empInfoForEdit.emp_no;

            var lstCostCentre = dataContext.prl_cost_centre.ToList();
            ViewBag.CostCentre = lstCostCentre;

            var lstDesig = dataContext.prl_designation.ToList();
            ViewBag.Designations = lstDesig;

            //var lstDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == _empInfoForEdit.id).prl_department.id.ToString();
            //ViewBag.Departments = lstDept;

            //var subDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == _empInfoForEdit.id).sub_department_id;

            //if (subDept != 0 && subDept != null)
            //{
            //    var lstSubDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == _empInfoForEdit.id).prl_sub_department.id.ToString();
            //    ViewBag.SubDepartments = lstSubDept;
            //}

            //var subSubDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == _empInfoForEdit.id).sub_sub_department_id;

            //if (subSubDept != 0 && subSubDept != null)
            //{
            //    var lstSubSubDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == _empInfoForEdit.id).prl_sub_sub_department.id.ToString();
            //    ViewBag.SubSubDepartments = lstSubSubDept;
            //}

            //var lstJobLevel = dataContext.prl_job_level.ToList();
            //ViewBag.JobLevel = lstJobLevel;


            return View(_empDetailsForEdit);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EditEmpDetails(int emp_id, EmployeeDetails empD, string submitButton)
        {
            var res = new OperationResult();
            if (submitButton == "Previous")
            {
                //Session["NewEmpDetailForEdit"] = empD;
                string jsonString = JsonConvert.SerializeObject(empD);
                Response.Cookies["NewEmpDetailForEdit"].Value = jsonString;
                return RedirectToAction("Edit");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var _empD = dataContext.prl_employee_details.SingleOrDefault(x => x.emp_id == emp_id);
                        _empD.designation_id = empD.designation_id;
                        _empD.job_level_id = empD.job_level_id != null ? empD.job_level_id : 0;
                        _empD.department_id = empD.department_id;
                        _empD.sub_department_id = empD.sub_department_id != null ? empD.sub_department_id : 0;
                        _empD.sub_sub_department_id = empD.sub_sub_department_id != null ? empD.sub_sub_department_id : 0;
                        _empD.division_id = empD.division_id != null ? empD.division_id : 0;
                        //_empD.emp_status = empD.emp_status;
                        //_empD.employee_category = empD.employee_category;
                        _empD.cost_centre_id = empD.cost_centre_id != null ? empD.cost_centre_id : 0;
                        //_empD.grade_id = empD.grade_id != null ? empD.grade_id : 0;
                        _empD.basic_salary = empD.basic_salary;
                        //_empD.blood_group = empD.blood_group;
                        _empD.parmanent_address = empD.parmanent_address;
                        _empD.present_address = empD.present_address;
                        //_empD.marital_status = empD.marital_status;
                        _empD.updated_by = User.Identity.Name;
                        _empD.updated_date = DateTime.Now;

                        //var emp = (Employee)Session["NewEmpForEdit"];

                        string cookievalue = Request.Cookies["NewEmpForEdit"].Value.ToString();
                        var emp = JsonConvert.DeserializeObject<Employee>(cookievalue);

                        var _emp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp.id);
                        _emp.name = emp.name;
                        _emp.joining_date = emp.joining_date;
                        _emp.dob = emp.dob;
                        _emp.official_contact_no = emp.official_contact_no;
                        _emp.personal_contact_no = emp.personal_contact_no;
                        _emp.email = emp.email;
                        _emp.personal_email = emp.personal_email;
                        _emp.gender = emp.gender;
                        _emp.religion_id = emp.religion_id;

                        _emp.tin = emp.tin;

                        _emp.prl_employee_details.Add(_empD);
                        _emp.updated_by = User.Identity.Name;
                        _emp.updated_date = DateTime.Now;

                        dataContext.SaveChanges();

                        var User_self = dataContext.prl_users.SingleOrDefault(x => x.emp_id == emp.id);

                        if (User_self != null)
                        {
                            if (emp.email != "N/A" && emp.email != null && emp.email != "")
                            {
                                if (Utility.CommonFunctions.IsValidEmail(emp.email) == true)
                                {
                                    User_self.email = emp.email;
                                    dataContext.SaveChanges();
                                }
                            }
                        }

                        //if (Session["NewEmpForEdit"] != null)
                        //    Session.Remove("NewEmpForEdit");
                        //if (Session["NewEmpDetailForEdit"] != null)
                        //    Session.Remove("NewEmpDetailForEdit");

                        if (Request.Cookies["NewEmpForEdit"] != null)
                        {
                            Response.Cookies["NewEmpForEdit"].Expires = DateTime.Now.AddDays(-1);
                        }

                        if (Request.Cookies["NewEmpDetailForEdit"] != null)
                        {
                            Response.Cookies["NewEmpDetailForEdit"].Expires = DateTime.Now.AddDays(-1);
                        }

                        res.IsSuccessful = true;
                        res.Message = "The Information of " + emp.name + " has been successfully updated.";
                        TempData.Add("msg", res);

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                    }
                }

                var lstCostCentre = dataContext.prl_cost_centre.ToList();
                ViewBag.CostCentre = lstCostCentre;

                var lstDesig = dataContext.prl_designation.ToList();
                ViewBag.Designations = lstDesig;

                var lstDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == emp_id).prl_department.id.ToString();
                ViewBag.Departments = lstDept;

                var subDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == emp_id).sub_department_id;

                if (subDept != 0 && subDept != null)
                {
                    var lstSubDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == emp_id).prl_sub_department.id.ToString();
                    ViewBag.SubDepartments = lstSubDept;
                }

                var subSubDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == emp_id).sub_sub_department_id;

                if (subDept != 0 && subDept != null)
                {
                    var lstSubSubDept = dataContext.prl_employee_details.SingleOrDefault(f => f.emp_id == emp_id).prl_sub_sub_department.id.ToString();
                    ViewBag.SubSubDepartments = lstSubSubDept;
                }

                var lstJobLevel = dataContext.prl_job_level.ToList();
                ViewBag.JobLevel = lstJobLevel;

                return View();
            }
        }

        // Department Dropdown Load
        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Department()
        {
            var deptList = dataContext.prl_department.ToList();
            List<DropDown> ddlDept = new List<DropDown>();
            foreach (var item in deptList)
            {
                DropDown dropDown = new DropDown();
                dropDown.Value = item.id.ToString();
                dropDown.Text = item.name.ToString();
                ddlDept.Add(dropDown);
            }
            return Json(ddlDept);
        }

        // SubDepartment Dropdown Load

        [HttpPost]
        public ActionResult SubDepartment(int? deptId)
        {
            var subList = dataContext.prl_sub_department.Where(sub => sub.department_id == deptId).ToList();

            List<DropDown> ddlSub = new List<DropDown>();
            foreach (var item in subList)
            {
                DropDown dropDown = new DropDown();
                dropDown.Value = item.id.ToString();
                dropDown.Text = item.name.ToString();
                ddlSub.Add(dropDown);
            }
            return Json(ddlSub);
        }

        // SubSubDepartment Dropdown load
        [HttpPost]
        public ActionResult SubSubDepartment(int? subDeptId)
        {
            var subSubList = dataContext.prl_sub_sub_department.Where(sub => sub.sub_department_id == subDeptId).ToList();

            List<DropDown> ddlSubSub = new List<DropDown>();
            foreach (var item in subSubList)
            {
                DropDown dropDown = new DropDown();
                dropDown.Value = item.id.ToString();
                dropDown.Text = item.name.ToString();
                ddlSubSub.Add(dropDown);
            }
            return Json(ddlSubSub);
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Employee/Delete/5

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
        public ActionResult SearchEmployee(string SearchFor)
        {
            var empS = new EmployeeSearch();
            empS.SearchFor = SearchFor;
            return View(empS);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult SearchEmployee(int? empid, FormCollection collection)
        {
            bool errorFound = false;
            var res = new OperationResult();

            int _empId = 0;

            if (empid == null && string.IsNullOrEmpty(collection["Emp_No"]))
            {
                errorFound = true;
                ModelState.AddModelError("", "Please select an employee or put employee no.");
            }
            else
            {
                if (empid != null)
                {
                    _empId = Convert.ToInt32(empid);
                }
                else
                {
                    string empNo = collection["Emp_No"];
                    var _emp = dataContext.prl_employee.SingleOrDefault(x => x.emp_no == empNo);
                    if (_emp == null)
                    {
                        ModelState.AddModelError("", "Threre is no information for the given employee no.");
                    }
                    else
                    {
                        _empId = _emp.id;
                    }
                }
            }
            if (_empId > 0)
            {
                return RedirectToAction("AddEmpDetails", new { empid = _empId });
            }


            return View();
        }

        public JsonResult GetEmployeeSearch(string query)
        {
            var lst =
                dataContext.prl_employee.AsEnumerable()
                .Where(x => x.name.ToLower().Contains(query) || x.emp_no.Contains(query))
                .Select(x => new SearchEmployeeData() { id = x.id, name = x.name + " (" + x.emp_no + ")" })
                .ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeSearchForSalaryReview(string query)
        {
            var lst =
                dataContext.prl_salary_review
                .Join(dataContext.prl_employee,
                sr => sr.emp_id,
                e => e.id,
                (sr, e) => new { e.id, e.name, e.emp_no })
                .Where(x => x.name.ToLower().Contains(query) || x.emp_no.Contains(query))
                .Select(x => new SearchEmployeeData() { id = x.id, name = x.name + " (" + x.emp_no + ")" })
                .ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult AddEmpDetails(string msg, int? Id)
        {

            var empD = new EmployeeDetails();

            if (Id != null)
            {
                var emp = dataContext.prl_employee.SingleOrDefault(x => x.id == Id);
                empD.name = emp.name + " (" + emp.emp_no + ")";
                empD.emp_id = emp.id;
                var emP = dataContext.prl_employee_details.Where(p => p.emp_id == Id).OrderByDescending(x => x.id).First();

                empD.id = emP.id;
                empD.designation_id = emP.designation_id;
                empD.division_id = emP.division_id;

                empD.department_id = emP.department_id;
                empD.sub_department_id = emP.sub_department_id;
                empD.sub_sub_department_id = emP.sub_sub_department_id;

                empD.marital_status = emP.marital_status;
                empD.employee_category = emP.employee_category;
                empD.grade_id = (emP.grade_id != null) ? (int?)emP.grade_id : null;
                empD.marital_status = emP.marital_status;
                empD.basic_salary = emP.basic_salary;
            }
            else
            {
                ModelState.AddModelError("", msg);
            }

            var lstCostCentre = dataContext.prl_cost_centre.ToList();
            ViewBag.CostCentre = lstCostCentre;

            var lstDesig = dataContext.prl_designation.ToList();
            ViewBag.Designations = lstDesig;

            var lstDept = dataContext.prl_department.ToList();
            ViewBag.Departments = lstDept;

            var lstSubDept = dataContext.prl_sub_department.ToList();
            ViewBag.SubDepartments = lstSubDept;

            var lstSubSubDept = dataContext.prl_sub_sub_department.ToList();
            ViewBag.SubSubDepartments = lstSubSubDept;

            var lstJobLevel = dataContext.prl_job_level.ToList();
            ViewBag.JobLevel = lstJobLevel;

            return View(empD);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult AddEmpDetails(int? empid, FormCollection collection, EmployeeDetails empD, string sButton)
        {
            var res = new OperationResult();
            try
            {
                if (sButton == "Search")
                {
                    if (empid == null && string.IsNullOrEmpty(collection["Emp_No"]))
                    {
                        return RedirectToAction("AddEmpDetails", new { msg = "Please select an employee or put employee Id" });
                    }
                    else
                    {
                        if (empid != null)
                        {
                            return RedirectToAction("AddEmpDetails", new { Id = empid });
                        }
                        else
                        {
                            string empNo = collection["Emp_No"];
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.emp_no == empNo);

                            if (emp == null)
                            {
                                return RedirectToAction("AddEmpDetails", new { msg = "Threre is no information for the selected employee" });
                            }
                            else
                            {
                                return RedirectToAction("AddEmpDetails", new { Id = emp.id });
                            }
                        }
                    }
                }
                else
                {
                    var empDfromDb = dataContext.prl_employee_details.Where(p => p.emp_id == empD.emp_id).OrderByDescending(x => x.id).First();
                    if (empDfromDb.designation_id == empD.designation_id && empDfromDb.department_id == empD.department_id
                        && empDfromDb.division_id == empD.division_id && empDfromDb.emp_status == empD.emp_status
                        && empDfromDb.employee_category == empD.employee_category

                        && empDfromDb.grade_id == empD.grade_id && empDfromDb.basic_salary == empD.basic_salary
                        )
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var nwEmp = Mapper.Map<prl_employee_details>(empD);
                        nwEmp.updated_by = User.Identity.Name;
                        nwEmp.updated_date = DateTime.Now;

                        dataContext.prl_employee_details.Add(nwEmp);
                        dataContext.SaveChanges();
                        res.IsSuccessful = true;
                        res.Message = "Employee information is successfully updated.";
                        TempData.Add("msg", res);

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var lstCostCentre = dataContext.prl_cost_centre.ToList();
            ViewBag.CostCentre = lstCostCentre;

            var lstDesig = dataContext.prl_designation.ToList();
            ViewBag.Designations = lstDesig;

            var lstDept = dataContext.prl_department.ToList();
            ViewBag.Departments = lstDept;

            var lstSubDept = dataContext.prl_sub_department.ToList();
            ViewBag.SubDepartments = lstSubDept;

            var lstSubSubDept = dataContext.prl_sub_sub_department.ToList();
            ViewBag.SubSubDepartments = lstSubSubDept;

            var lstJobLevel = dataContext.prl_job_level.ToList();
            ViewBag.JobLevel = lstJobLevel;


            return View();
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult EmpConfirmation(int? id)
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

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EmpConfirmation(int? empid, FormCollection collection, Employee emp, string sButton)
        {

            bool errorFound = false;
            var res = new OperationResult();

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
                        if (empid != null)
                        {
                            var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                            var empD = Mapper.Map<Employee>(_empD);
                            ViewBag.Employee = empD;
                            return View(empD);
                        }
                        else
                        {
                            var _empD = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"]);
                            if (_empD == null)
                            {
                                ModelState.AddModelError("", "Threre is no information for the given employee no.");
                            }
                            else
                            {
                                var empD = Mapper.Map<Employee>(_empD);
                                ViewBag.Employee = empD;
                                return View(empD);
                            }
                        }
                    }
                }
                else if (sButton == "Save")
                {

                    if (emp.id == 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (!errorFound)
                    {
                        var _confirmingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp.id);
                        _confirmingEmp.confirmation_date = emp.confirmation_date;
                        _confirmingEmp.is_confirmed = Convert.ToSByte(true);
                        _confirmingEmp.updated_by = User.Identity.Name;
                        _confirmingEmp.updated_date = DateTime.Today;


                        var empD = dataContext.prl_employee_details.SingleOrDefault(x=>x.id==emp.id);
                        empD.emp_status = "Permanent";

                        dataContext.SaveChanges();

                        

                        res.IsSuccessful = true;
                        res.Message = " Employee confirmed successfully.";
                        TempData.Add("msg", res);

                        return RedirectToAction("EmpConfirmation", new { id = _confirmingEmp.id });
                    }
                }
                else if (sButton == "Undo")
                {
                    if (emp.id > 0)
                    {
                        var salaryProcess = dataContext.prl_salary_process_detail.Where(p => p.emp_id == emp.id);
                        if (salaryProcess.Count() > 0)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Confirmation of this employee can't be undo.");
                        }
                    }
                    else
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (!errorFound)
                    {
                        var _confirmingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp.id);
                        _confirmingEmp.confirmation_date = null;
                        _confirmingEmp.is_confirmed = Convert.ToSByte(false);
                        _confirmingEmp.updated_by = User.Identity.Name;
                        _confirmingEmp.updated_date = DateTime.Today;

                        var empD = dataContext.prl_employee_details.SingleOrDefault(x => x.id == emp.id);
                        empD.emp_status = "On Probation";

                        dataContext.SaveChanges();

                        res.IsSuccessful = true;
                        res.Message = " Confirmation cancelled.";
                        TempData.Add("msg", res);

                        return RedirectToAction("EmpConfirmation", new { id = _confirmingEmp.id });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return View();
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult EmpBankInfo(string empIdbankId)
        {

            var _Emp = new Employee();

            if (empIdbankId != null)
            {
                string[] ids = empIdbankId.Split(',');

                int empId = Convert.ToInt32(ids[0]);
                int bankId = ids[1] == "" ? 0 : Convert.ToInt32(ids[1]);

                var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empId);
                _Emp = Mapper.Map<Employee>(emp);
                _Emp.bank_id = bankId;

                var _bankInfo = dataContext.prl_bank.ToList();
                ViewBag.Banks = _bankInfo;

                var _banches = dataContext.prl_bank_branch.Where(x => x.bank_id == bankId).ToList();
                ViewBag.Branches = _banches;

                return View(_Emp);
            }
            else
            {
                var _bankInfo = dataContext.prl_bank.Where(x => x.id == 0).ToList();
                ViewBag.Banks = _bankInfo;

                var _banches = dataContext.prl_bank_branch.Where(x => x.bank_id == 0).ToList();
                ViewBag.Branches = _banches;

                return View();
            }
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EmpBankInfo(int? empid, FormCollection collection, Employee emp, string sButton)
        {

            var _empD = new Employee();

            bool errorFound = false;
            var res = new OperationResult();

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
                        if (empid != null)
                        {
                            var empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                            _empD = Mapper.Map<Employee>(empD);
                        }
                        else
                        {
                            var empD = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"]);
                            if (empD == null)
                            {
                                ModelState.AddModelError("", "Threre is no information for the given employee no.");
                            }
                            else
                            {
                                _empD = Mapper.Map<Employee>(empD);
                            }
                        }
                    }
                }
                else if (sButton == "Save")
                {
                    if (emp.id == 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    else
                    {
                        if (emp.bank_id == null)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Please select bank.");
                        }
                        if (emp.bank_branch_id == null)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Please select bank branch.");
                        }
                        if (emp.account_type == null)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Please select account type.");
                        }
                        if (string.IsNullOrEmpty(emp.account_no))
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Please provide account no.");
                        }
                        if (string.IsNullOrEmpty(emp.routing_no))
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Please select routing_no.");
                        }

                        var _Emp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp.id);
                        if (!errorFound)
                        {
                            _Emp.bank_id = emp.bank_id;
                            _Emp.bank_branch_id = emp.bank_branch_id;
                            _Emp.account_type = emp.account_type;
                            _Emp.account_no = emp.account_no;
                            _Emp.routing_no = emp.routing_no;
                            _Emp.updated_by = User.Identity.Name;
                            _Emp.updated_date = DateTime.Today;

                            dataContext.SaveChanges();

                            res.IsSuccessful = true;
                            res.Messages.Add("Bank information updated successfully.");
                            //res.Message = "Bank information updated successfully.";
                            TempData.Add("msg", res);
                        }
                        _empD = Mapper.Map<Employee>(_Emp);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            int bankId = _empD.bank_id == null ? 1 : Convert.ToInt16(_empD.bank_id);

            var _bInfo = dataContext.prl_bank.ToList();
            ViewBag.Banks = _bInfo;

            var _banch = dataContext.prl_bank_branch.Where(x => x.bank_id == bankId).ToList();
            ViewBag.Branches = _banch;

            if (_empD.id > 0)
            {
                ViewBag.Employee = _empD;
                return View(_empD);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddBankInfoFile(HttpPostedFileBase postedFile)
        {
            var res = new OperationResult();

            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        res.IsSuccessful = false;
                        res.Message = "Please select the excel file with .xls or .xlsx extension";
                        TempData.Add("msg", res);

                        return RedirectToAction("EmpBankInfo", "Employee");
                    }

                    string folderPath = Server.MapPath("~/Files/");
                    //Check Directory exists else create one
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    //Save file to folder
                    var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Get file extension

                    string excelConString = "";

                    //Get connection string using extension 
                    switch (fileExtension)
                    {
                        //If uploaded file is Excel 1997-2007.
                        case ".xls":
                            excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                        //If uploaded file is Excel 2007 and above
                        case ".xlsx":
                            excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                    }

                    //Read data from first sheet of excel into datatable
                    DataTable dt = new DataTable();
                    excelConString = string.Format(excelConString, filePath);

                    using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                    {
                        using (OleDbCommand excelDbCommand = new OleDbCommand())
                        {
                            using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                            {
                                excelDbCommand.Connection = excelOledbConnection;

                                excelOledbConnection.Open();
                                //Get schema from excel sheet
                                DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                if (excelSchema != null)
                                {
                                    //Get sheet name
                                    string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    excelOledbConnection.Close();

                                    //Read Data from First Sheet.
                                    excelOledbConnection.Open();
                                    excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                    excelDataAdapter.SelectCommand = excelDbCommand;
                                    //Fill datatable from adapter
                                    excelDataAdapter.Fill(dt);
                                }

                                excelOledbConnection.Close();
                            }
                        }
                    }

                    //Insert records to Employee table.
                    using (var dataContext = new payroll_systemContext())
                    {
                        //Loop through datatable and add employee data to employee table. 
                        foreach (DataRow row in dt.Rows)
                        {

                            if (row.ItemArray != null)
                            {
                                if (!row.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString()))))
                                {
                                    string emp_no = (row[0].ToString());
                                    var emp = dataContext.prl_employee.Where(x => x.emp_no == emp_no).FirstOrDefault();
                                    if (emp != null)
                                    {
                                        emp.account_no=(row[1]).ToString().Trim();
                                        emp.routing_no = (row[2]).ToString().Trim();
                                        emp.bank_id = 1;
                                        emp.bank_branch_id = 1;
                                        emp.account_type = "Payroll";
                                        var tin = (row[3]);
                                        if ((row[3]).ToString().Trim() == null || string.IsNullOrWhiteSpace((row[3]).ToString()))
                                        {
                                            emp.tin = "0";
                                        }

                                        else { emp.tin = (row[3]).ToString().Trim(); } 
                                    }
                                    
                                    else
                                    {
                                        errorIds.Add(emp_no);
                                    }
                                }

                            }

                            dataContext.SaveChanges();
                        }

                    }

                    res.IsSuccessful = true;
                    res.Messages.Add("Uploaded successfully.");

                    string errIdMsg = "";

                    if (errorIds.Count > 0)
                    {
                        errIdMsg = string.Join(", ", errorIds);
                        res.Messages.Add("Upload Not successful for these Ids " + errIdMsg);
                    }

                    TempData.Add("msg", res);

                }

                catch (Exception ex)
                {
                    res.IsSuccessful = false;
                    res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData.Add("msg", res);

                    return RedirectToAction("EmpBankInfo", "Employee");
                }
            }
            else
            {
                //Upload file Null Message
                res.IsSuccessful = false;
                res.Message = "Please select a file to Upload";
                TempData.Add("msg", res);

                return RedirectToAction("EmpBankInfo", "Employee");
            }

            return RedirectToAction("EmpBankInfo", "Employee");
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult AddSalaryReview()
        {

            var lstReview = dataContext.prl_salary_review.Where(x => x.id == 0).ToList();
            ViewBag.SalReview = Mapper.Map<List<SalaryReview>>(lstReview);

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult AddSalaryReview(int? empid, FormCollection collection, string sButton)
        {
            var _empD = new Employee();
            var _salaryReview = new SalaryReview();

            bool errorFound = false;
            var res = new OperationResult();

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
                        if (empid != null)
                        {
                            var empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                            _empD = Mapper.Map<Employee>(empD);
                            _salaryReview.emp_id = _empD.id;
                            _salaryReview.current_basic = _empD.prl_employee_details.OrderByDescending(x => x.id).First().basic_salary;
                        }
                        else
                        {
                            var empD = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"]);
                            if (empD == null)
                            {
                                ModelState.AddModelError("", "Threre is no information for the given employee no.");
                            }
                            else
                            {
                                _empD = Mapper.Map<Employee>(empD);
                                _salaryReview.emp_id = _empD.id;
                                _salaryReview.current_basic = _empD.prl_employee_details.OrderByDescending(x => x.id).First().basic_salary;
                            }
                        }
                    }
                }
                else if (sButton == "Save")
                {
                    if (string.IsNullOrEmpty(collection["emp_id"]))
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }

                    if (!errorFound)
                    {
                        var emp_id = Convert.ToInt32(collection["emp_id"]);
                        var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == emp_id);
                        _empD = Mapper.Map<Employee>(emp);

                        var salary_review = dataContext.prl_salary_review.Where(p => p.emp_id == emp_id).OrderByDescending(x => x.id).FirstOrDefault();

                        string e_date = collection["effective_from"].ToString();

                        if (salary_review != null && salary_review.effective_from.Value.Month == Convert.ToDateTime(e_date).Month && salary_review.effective_from.Value.Year == Convert.ToDateTime(e_date).Year)
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "The Salary of " + emp.emp_no + " already has been reviewed for this month.");
                        }
                        else
                        {
                            _salaryReview.emp_id = Convert.ToInt32(collection["emp_id"]);

                            _salaryReview.current_basic = Convert.ToDecimal(collection["current_basic"]);
                            _salaryReview.new_basic = Convert.ToDecimal(collection["new_basic"]);

                            _salaryReview.effective_from = Convert.ToDateTime(e_date);
                            _salaryReview.increment_reason = collection["increment_reason"];

                            _salaryReview.description = collection["description"];
                            _salaryReview.is_arrear_calculated = "NO";
                            _salaryReview.created_by = User.Identity.Name;
                            _salaryReview.created_date = DateTime.Now;

                            var _review = Mapper.Map<prl_salary_review>(_salaryReview);

                            dataContext.prl_salary_review.Add(_review);

                            var empD = dataContext.prl_employee_details.Where(p => p.emp_id == _review.emp_id).OrderByDescending(x => x.id).First();
                            empD.basic_salary = _salaryReview.new_basic;
                            empD.updated_by = User.Identity.Name;
                            empD.updated_date = DateTime.Now;

                            if (_salaryReview.increment_reason == "Confirmation")
                            {
                                var _confirmingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == _review.emp_id);
                                _confirmingEmp.confirmation_date = _salaryReview.effective_from;
                                _confirmingEmp.is_confirmed = Convert.ToSByte(true);
                                _confirmingEmp.updated_by = User.Identity.Name;
                                _confirmingEmp.updated_date = DateTime.Today;

                                // Probation to Permanent
                                empD.emp_status = "Permanent";
                            }

                            dataContext.SaveChanges();
                            res.IsSuccessful = true;
                            res.Messages.Add("Reviewed information saved successfully.");
                            //res.Message = "Reviewed information saved successfully.";
                            TempData.Add("msg", res);
                        }
                    }
                    if (res.IsSuccessful == true)
                    {
                        _salaryReview.current_basic = Convert.ToDecimal(collection["new_basic"]);
                        _salaryReview.new_basic = 0;
                        _salaryReview.effective_from = _salaryReview.effective_from;
                    }
                    else
                    {
                        var emp_id = Convert.ToInt32(collection["emp_id"]);
                        var salary_review = dataContext.prl_salary_review.Where(p => p.emp_id == emp_id).OrderByDescending(x => x.id).FirstOrDefault();
                        if (salary_review != null)
                        {
                            _salaryReview.current_basic = salary_review.new_basic;
                            _salaryReview.new_basic = 0;
                            _salaryReview.effective_from = _salaryReview.effective_from;
                        }
                        else
                        {
                            _salaryReview.current_basic = _empD.prl_employee_details.OrderByDescending(x => x.id).First().basic_salary;
                            _salaryReview.new_basic = 0;
                            _salaryReview.effective_from = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            if (_empD.id > 0)
            {
                var lstReviewByEmp = dataContext.prl_salary_review.Where(x => x.emp_id == _empD.id).OrderByDescending(p => p.id);
                ViewBag.SalReview = Mapper.Map<List<SalaryReview>>(lstReviewByEmp);
                ViewBag.Employee = _empD;

                return View(_salaryReview);
            }
            var lstReview = dataContext.prl_salary_review.Where(x => x.id == 0).OrderByDescending(p => p.id);
            ViewBag.SalReview = Mapper.Map<List<SalaryReview>>(lstReview);
            return View();
        }

        [HttpPost]
        public ActionResult AddSalaryReviewFile(HttpPostedFileBase postedFile)
        {
            var res = new OperationResult();

            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        res.IsSuccessful = false;
                        res.Message = "Please select the excel file with .xls or .xlsx extension";
                        TempData.Add("msg", res);

                        return RedirectToAction("AddSalaryReview", "Employee");
                    }

                    string folderPath = Server.MapPath("~/Files/");
                    //Check Directory exists else create one
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    //Save file to folder
                    var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Get file extension

                    string excelConString = "";

                    //Get connection string using extension 
                    switch (fileExtension)
                    {
                        //If uploaded file is Excel 1997-2007.
                        case ".xls":
                            excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                        //If uploaded file is Excel 2007 and above
                        case ".xlsx":
                            excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                    }

                    //Read data from first sheet of excel into datatable
                    DataTable dt = new DataTable();
                    excelConString = string.Format(excelConString, filePath);

                    using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                    {
                        using (OleDbCommand excelDbCommand = new OleDbCommand())
                        {
                            using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                            {
                                excelDbCommand.Connection = excelOledbConnection;

                                excelOledbConnection.Open();
                                //Get schema from excel sheet
                                DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                if (excelSchema != null)
                                {
                                    //Get sheet name
                                    string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    excelOledbConnection.Close();

                                    //Read Data from First Sheet.
                                    excelOledbConnection.Open();
                                    excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                    excelDataAdapter.SelectCommand = excelDbCommand;
                                    //Fill datatable from adapter
                                    excelDataAdapter.Fill(dt);
                                }

                                excelOledbConnection.Close();
                            }
                        }
                    }

                    //Insert records to Employee table.
                    using (var dataContext = new payroll_systemContext())
                    {
                        //Loop through datatable and add employee data to employee table. 
                        foreach (DataRow row in dt.Rows)
                        {

                            if (row.ItemArray != null)
                            {
                                if (!row.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString()))))
                                {
                                    var empSR = GetEmployeeFromExcelRow(row);

                                    if (empSR.emp_id > 0)
                                    {
                                        var empD = dataContext.prl_employee_details.Where(x => x.emp_id == empSR.emp_id).FirstOrDefault();

                                        empSR.current_basic = empD.basic_salary;
                                        empSR.is_arrear_calculated = "No";
                                        empSR.created_by = User.Identity.Name;
                                        empSR.created_date = DateTime.Now;

                                        dataContext.prl_salary_review.Add(empSR);

                                        var emp = dataContext.prl_employee.Where(x => x.id == empSR.emp_id).FirstOrDefault();

                                        //Update previous_basic in Employee_Details
                                        empD.basic_salary = empSR.new_basic;


                                        if (empSR.increment_reason == "Confirmation")
                                        {
                                            emp.confirmation_date = empSR.effective_from;
                                            emp.is_confirmed = 1;

                                            empD.emp_status = "Permanent";
                                        }
                                    }

                                }
                            }

                            dataContext.SaveChanges();
                        }

                    }

                    res.IsSuccessful = true;
                    res.Messages.Add("Reviewed information saved successfully.");

                    string errIdMsg = "";

                    if (errorIds.Count > 0)
                    {
                        errIdMsg = string.Join(", ", errorIds);
                        res.Messages.Add("Review Not successful for these Ids " + errIdMsg);
                    }

                    TempData.Add("msg", res);

                }

                catch (Exception ex)
                {
                    res.IsSuccessful = false;
                    res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData.Add("msg", res);

                    return RedirectToAction("AddSalaryReview", "Employee");
                }
            }
            else
            {
                //Upload file Null Message
                res.IsSuccessful = false;
                res.Message = "Please select a file to Upload";
                TempData.Add("msg", res);

                return RedirectToAction("AddSalaryReview", "Employee");
            }

            return RedirectToAction("AddSalaryReview", "Employee");
        }

        private static DataTable GetSchemaFromExcel(OleDbConnection excelOledbConnection)
        {
            return excelOledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        }

        private prl_salary_review GetEmployeeFromExcelRow(DataRow row)
        {
            var salaryReview = new prl_salary_review();
            string emp_no = (row[0].ToString()); //Employee Table
            var es_date = DateTime.Parse(row[3].ToString().Trim());

            var emp = dataContext.prl_employee.Where(x => x.emp_no == emp_no).FirstOrDefault();

            if (emp != null)
            {
                var empId = emp.id;
                var empSR = dataContext.prl_salary_review
                    .Where(x => x.emp_id == empId && x.effective_from.Value.Year == es_date.Year && x.effective_from.Value.Month == es_date.Month).FirstOrDefault();

                //Check Last Salary Review
                if (empSR != null)
                {
                    var lastReason = empSR.increment_reason;
                    empSR.new_basic = Convert.ToDecimal(row[1]);
                    empSR.increment_reason = row[2].ToString().Trim();
                    empSR.effective_from = DateTime.Parse(row[3].ToString().Trim());
                    empSR.description = row[4].ToString().Trim();
                    dataContext.SaveChanges();
                    var newReason = empSR.increment_reason;


                    var empUpdate = dataContext.prl_employee.Where(x => x.id == empSR.emp_id).FirstOrDefault();
                    var empD = dataContext.prl_employee_details.Where(x => x.emp_id == empSR.emp_id).FirstOrDefault();
                    empD.basic_salary = empSR.new_basic;

                    if (lastReason == "Confirmation")
                    {
                        empUpdate.is_confirmed = 0;
                        empUpdate.confirmation_date = null;
                        empD.emp_status = "On Probation";
                        dataContext.SaveChanges();
                    }

                    if (newReason == "Confirmation")
                    {
                        empUpdate.confirmation_date = empSR.effective_from;
                        empUpdate.is_confirmed = 1;
                        empD.emp_status = "Permanent";
                        dataContext.SaveChanges();
                    }

                    dataContext.SaveChanges();
                    return new prl_salary_review() { emp_id = 0 };

                }

                else
                {
                    salaryReview.emp_id = empId;
                    salaryReview.new_basic = Convert.ToDecimal(row[1]);
                    salaryReview.increment_reason = row[2].ToString().Trim();
                    salaryReview.effective_from = DateTime.Parse(row[3].ToString().Trim());
                    salaryReview.description = row[4].ToString().Trim();

                    return salaryReview;
                }
            }

            else
            {
                errorIds.Add(emp_no);
                return new prl_salary_review() { emp_id = 0 };
            }
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult EditSalaryReview(string empIdbankId)
        {
            var lstReview = dataContext.prl_salary_review.Where(x => x.id == 0).ToList();
            ViewBag.SalReview = Mapper.Map<List<SalaryReview>>(lstReview);

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EditSalaryReview(int? empid, FormCollection collection, string sButton)
        {

            var _empD = new Employee();
            var _salaryReview = new SalaryReview();

            bool errorFound = false;
            var res = new OperationResult();

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
                        if (empid != null)
                        {
                            var empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == empid);
                            _empD = Mapper.Map<Employee>(empD);

                            var ReviewList = dataContext.prl_salary_review.Where(x => x.emp_id == _empD.id).ToList();

                            if (ReviewList.Any())
                            {
                                var Review = ReviewList.OrderByDescending(p => p.id).First();

                                _salaryReview.id = Review.id;
                                _salaryReview.emp_id = Review.emp_id;
                                _salaryReview.current_basic = Review.current_basic;
                                _salaryReview.new_basic = Review.new_basic;
                                _salaryReview.effective_from = Review.effective_from;
                                _salaryReview.increment_reason = Review.increment_reason;
                                _salaryReview.description = Review.description;
                            }
                            else
                            {

                                _salaryReview.emp_id = _empD.id;
                                _salaryReview.current_basic = _empD.prl_employee_details.OrderByDescending(x => x.id).First().basic_salary;

                            }

                        }
                        else
                        {
                            var empD = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().SingleOrDefault(x => x.emp_no == collection["Emp_No"]);
                            if (empD == null)
                            {
                                ModelState.AddModelError("", "Threre is no information for the given employee no.");
                            }
                            else
                            {
                                _empD = Mapper.Map<Employee>(empD);

                                var ReviewList = dataContext.prl_salary_review.Where(x => x.emp_id == _empD.id).ToList();

                                if (ReviewList.Any())
                                {
                                    var Review = dataContext.prl_salary_review.Where(x => x.emp_id == _empD.id).OrderByDescending(p => p.id).First();

                                    _salaryReview.id = Review.id;
                                    _salaryReview.emp_id = Review.emp_id;
                                    _salaryReview.current_basic = Review.current_basic;
                                    _salaryReview.new_basic = Review.new_basic;
                                    _salaryReview.effective_from = Review.effective_from;
                                    _salaryReview.increment_reason = Review.increment_reason;
                                    _salaryReview.description = Review.description;
                                }
                                else
                                {
                                    _salaryReview.emp_id = _empD.id;
                                    _salaryReview.current_basic = _empD.prl_employee_details.OrderByDescending(x => x.id).First().basic_salary;

                                }

                            }
                        }

                    }
                }
                else if (sButton == "Update")
                {
                    if (string.IsNullOrEmpty(collection["emp_id"]))
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }

                    if (!errorFound)
                    {
                        _salaryReview.id = Convert.ToInt16(collection["id"]);
                        _salaryReview.emp_id = Convert.ToInt16(collection["emp_id"]);

                        var emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == _salaryReview.emp_id);
                        _empD = Mapper.Map<Employee>(emp);

                        _salaryReview.current_basic = Convert.ToDecimal(collection["current_basic"]);
                        _salaryReview.new_basic = Convert.ToDecimal(collection["new_basic"]);
                        _salaryReview.effective_from = Convert.ToDateTime(collection["effective_from"]);
                        _salaryReview.increment_reason = collection["increment_reason"];
                        _salaryReview.description = collection["description"];

                        var _review = dataContext.prl_salary_review.SingleOrDefault(x => x.id == _salaryReview.id);
                        _review.new_basic = _salaryReview.new_basic;
                        _review.effective_from = _salaryReview.effective_from;
                        _review.increment_reason = _salaryReview.increment_reason;
                        _review.description = _salaryReview.description;
                        _salaryReview.is_arrear_calculated = "NO";

                        var empD = dataContext.prl_employee_details.Where(p => p.emp_id == _empD.id).OrderByDescending(x => x.id).First();
                        empD.basic_salary = _review.new_basic;
                        empD.updated_by = User.Identity.Name;
                        empD.updated_date = DateTime.Now;

                        if (_salaryReview.increment_reason == "Confirmation")
                        {
                            var _confirmingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == _empD.id);
                            _confirmingEmp.confirmation_date = _salaryReview.effective_from;
                            _confirmingEmp.is_confirmed = Convert.ToSByte(true);
                            _confirmingEmp.updated_by = User.Identity.Name;
                            _confirmingEmp.updated_date = DateTime.Today;
                        }

                        dataContext.SaveChanges();
                        res.IsSuccessful = true;
                        res.Message = "Reviewed information updated successfully.";
                        TempData.Add("msg", res);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            if (_empD.id > 0)
            {
                var lstReviewByEmp = dataContext.prl_salary_review.Where(x => x.emp_id == _empD.id).OrderByDescending(p => p.id);
                ViewBag.SalReview = Mapper.Map<List<SalaryReview>>(lstReviewByEmp);
                ViewBag.Employee = _empD;

                return View(_salaryReview);
            }
            var lstReview = dataContext.prl_salary_review.Where(x => x.id == 0).OrderByDescending(p => p.id);
            ViewBag.SalReview = Mapper.Map<List<SalaryReview>>(lstReview);
            return View();
        }


        public ActionResult EmployeeDiscontinue()
        {

            return View();
        }

        [HttpPost]
        public ActionResult EmployeeDiscontinue(int? empid, FormCollection collection, EmployeeDiscontinue empDisCon, string sButton)
        {
            bool errorFound = false;
            var res = new OperationResult();
            var _emp = new Employee();
            //byte disconFlag = 1;
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
                        if (empid != null)
                        {
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.id == empid);
                            _emp = Mapper.Map<Employee>(emp);
                        }
                        else
                        {
                            string enpNo = collection["Emp_No"];
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.emp_no == enpNo);
                            _emp = Mapper.Map<Employee>(emp);
                        }
                        empDisCon.emp_id = _emp.id;
                        empDisCon.empInfo = _emp.name + "(" + _emp.emp_no + ")";

                        var lst = dataContext.prl_employee_discontinue.AsEnumerable().Where(p => p.emp_id == empDisCon.emp_id);

                        if (lst.Count() > 0)
                        {
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.id == empid);

                            var conDisCon = lst.OrderByDescending(x => x.id).First();

                            if (conDisCon.is_active == "N" && emp.is_active == 0)
                            {
                                empDisCon.status = "Discontinued";
                                empDisCon.discontinue_date = conDisCon.discontinue_date;
                                empDisCon.discontination_type = conDisCon.discontination_type;
                                empDisCon.discontinueAfterCurrentMonth = conDisCon.with_salary == "Y" ? true : false;
                                empDisCon.remarks = conDisCon.remarks;
                                //empDisCon.continue_pf = conDisCon.continue_pf == "Y" ? true : false;
                                //empDisCon.continue_gf = conDisCon.continue_gf == "Y" ? true : false;
                                //empDisCon.consider_for_next_tax_year = conDisCon.consider_for_next_tax_year == "Y" ? true : false;
                                
                            }
                            else
                            {
                                DateTime emptyDate = new DateTime();
                                if ((conDisCon.continution_date != emptyDate || conDisCon.continution_date != null) && conDisCon.discontinue_date <= conDisCon.continution_date)
                                {
                                    empDisCon.status = "Recontinued";
                                    empDisCon.discontinue_date = conDisCon.discontinue_date;
                                    empDisCon.discontination_type = conDisCon.discontination_type;
                                    empDisCon.discontinueAfterCurrentMonth = conDisCon.with_salary == "Y" ? true : false;
                                    empDisCon.remarks = conDisCon.remarks;
                                }
                                else
	                            {
                                    empDisCon.status = "Discontinuation on processing";
                                    empDisCon.discontinue_date = conDisCon.discontinue_date;
                                    empDisCon.discontination_type = conDisCon.discontination_type;
                                    empDisCon.discontinueAfterCurrentMonth = conDisCon.with_salary == "Y" ? true : false;
                                    empDisCon.remarks = conDisCon.remarks;
	                            }
                            }
                        }
                        else
                        {
                            empDisCon.status = "Continuing";
                            empDisCon.discontinue_date = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy-MM-dd"));
                        }
                    }
                }
                else
                {
                    if (empDisCon.emp_id == 0)
                    {
                        //disconFlag = 0;
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (empDisCon.discontinue_date == null)
                    {
                        //disconFlag = 0;
                        errorFound = true;
                        ModelState.AddModelError("", "Please select discontinuation date.");
                    }

                    var lst = dataContext.prl_employee_discontinue.AsEnumerable().Where(p => p.emp_id == empDisCon.emp_id);
                    if (lst.Count() > 0)
                    {
                        var stat = lst.OrderByDescending(x => x.id).First().is_active;
                        if (stat == "N")
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "This employee has been already discontinued.");
                        }
                        else if (stat == "Y")
                        {
                            errorFound = true;
                            ModelState.AddModelError("", "Sorry, already discontinuation done for the salary.");
                        }
                    }

                    if (!errorFound)
                    {
                        var empDisC = new prl_employee_discontinue();

                        empDisC.emp_id = empDisCon.emp_id;
                        empDisC.discontinue_date = empDisCon.discontinue_date;
                        empDisC.discontination_type = empDisCon.discontination_type;

                        //Edited by Rakib
                        if (empDisCon.discontination_type != "Suspension")
                        {

                            if (empDisCon.discontinueAfterCurrentMonth == true)
                            {
                                empDisC.with_salary = "Y";
                                empDisC.is_active = "Y";
                            }
                            else
                            {
                                empDisC.with_salary = "N";
                                empDisC.is_active = "N";

                                //Commented for Permanent Inactive after salary disburse

                                var _updatingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == empDisCon.emp_id);
                                _updatingEmp.is_active = 0;
                                dataContext.SaveChanges();
                            }
                        }

                        else
                        {
                            empDisC.is_active = "N";
                            empDisC.with_salary = "N";

                            //Commented for Permanent Inactive after salary disburse

                            var _updatingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == empDisCon.emp_id);
                            _updatingEmp.is_active = 0;
                            dataContext.SaveChanges();

                            //if (empDisCon.discontinueAfterCurrentMonth == true)
                            //empDisC.with_salary = "N";
                            //else
                            //    empDisC.with_salary = "Y";

                        }


                        empDisC.created_by = User.Identity.Name;
                        empDisC.created_date = DateTime.Now;

                        dataContext.prl_employee_discontinue.Add(empDisC);
                        dataContext.SaveChanges();

                        

                        res.IsSuccessful = true;
                        res.Messages.Add(empDisCon.empInfo + " is being successfully discontinuing.");
                        TempData.Add("msg", res);

                        empDisCon.status = "Discontinuation on processing";
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return View(empDisCon);
        }


        [HttpPost]
        public ActionResult AddEmpDiscontinueFile(HttpPostedFileBase postedFile)
        {
            var res = new OperationResult();

            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        res.IsSuccessful = false;
                        res.Message = "Please select the excel file with .xls or .xlsx extension";
                        TempData.Add("msg", res);

                        return RedirectToAction("EmployeeDiscontinue", "Employee");
                    }

                    string folderPath = Server.MapPath("~/Files/");
                    //Check Directory exists else create one
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    //Save file to folder
                    var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Get file extension

                    string excelConString = "";

                    //Get connection string using extension 
                    switch (fileExtension)
                    {
                        //If uploaded file is Excel 1997-2007.
                        case ".xls":
                            excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                        //If uploaded file is Excel 2007 and above
                        case ".xlsx":
                            excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                    }

                    //Read data from first sheet of excel into datatable
                    DataTable dt = new DataTable();
                    excelConString = string.Format(excelConString, filePath);

                    using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                    {
                        using (OleDbCommand excelDbCommand = new OleDbCommand())
                        {
                            using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                            {
                                excelDbCommand.Connection = excelOledbConnection;

                                excelOledbConnection.Open();
                                //Get schema from excel sheet
                                DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                //Get sheet name
                                string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                excelOledbConnection.Close();

                                //Read Data from First Sheet.
                                excelOledbConnection.Open();
                                excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                excelDataAdapter.SelectCommand = excelDbCommand;
                                //Fill datatable from adapter
                                excelDataAdapter.Fill(dt);
                                excelOledbConnection.Close();
                            }
                        }
                    }

                    //Insert records to Employee table.
                    using (var dataContext = new payroll_systemContext())
                    {
                        //Loop through datatable and add employee data to employee table. 
                        foreach (DataRow row in dt.Rows)
                        {

                            if (row.ItemArray != null)
                            {
                                if (!row.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString()))))
                                {
                                    var empD = GetDiscontinueEmpFromExcelRow(row);

                                    if (empD.emp_id > 0)
                                    {
                                        dataContext.prl_employee_discontinue.Add(empD);
                                    }

                                }

                            }
                        }

                        dataContext.SaveChanges();
                    }

                    res.IsSuccessful = true;
                    res.Messages.Add("Uploaded information saved successfully.");

                    string errIdMsg = "";

                    if (errorIds.Count > 0)
                    {
                        errIdMsg = string.Join(",", errorIds);
                        res.Messages.Add("Upload Not successful for these Ids " + errIdMsg);

                    }

                    TempData.Add("msg", res);

                }

                catch (Exception ex)
                {
                    res.IsSuccessful = false;
                    res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData.Add("msg", res);

                    return RedirectToAction("EmployeeDiscontinue", "Employee");
                }
            }
            else
            {
                //Upload file Null Message
                res.IsSuccessful = false;
                res.Message = "Please select a file to Upload";
                TempData.Add("msg", res);

                return RedirectToAction("EmployeeDiscontinue", "Employee");
            }

            return RedirectToAction("EmployeeDiscontinue", "Employee");
        }

        private prl_employee_discontinue GetDiscontinueEmpFromExcelRow(DataRow row)
        {
            var emp_Discontinue = new prl_employee_discontinue();

            string emp_no = (row[0].ToString());
            DateTime dconDate = DateTime.Parse(row[1].ToString().Trim());
            string dconType = (row[2].ToString().Trim());
            string yesOrNo = (row[3].ToString().Trim());
            string dconRemark = (row[4].ToString().Trim());

            var emp = dataContext.prl_employee.Where(x => x.emp_no == emp_no).FirstOrDefault();

            if (emp != null)
            {
                var empId = emp.id;
                var empD = dataContext.prl_employee_discontinue
                    .Where(x => x.emp_id == empId).FirstOrDefault();

                if (empD == null)
                {
                    emp_Discontinue.emp_id = empId;
                    emp_Discontinue.discontinue_date = dconDate;
                    emp_Discontinue.discontination_type = dconType;
                    emp_Discontinue.remarks = dconRemark;

                    if (dconType != "Suspension")
                    {
                        emp_Discontinue.with_salary = yesOrNo == "Yes" ? "Y" : "N";
                        emp_Discontinue.without_salary = yesOrNo == "Yes" ? "N" : "Y";
                        emp_Discontinue.is_active = "Y";
                    }
                    else
                    {
                        emp_Discontinue.with_salary = yesOrNo == "Yes" ? "N" : "Y";
                        emp_Discontinue.without_salary = yesOrNo == "Yes" ? "Y" : "N";
                        emp_Discontinue.is_active = "N";

                        emp.is_active = 0;
                        dataContext.SaveChanges();
                    }

                    return emp_Discontinue;
                }

                else
                {
                    return new prl_employee_discontinue() { emp_id = 0 };
                }
            }

            else
            {
                errorIds.Add(emp_no);
                return new prl_employee_discontinue() { emp_id = 0 };
            }
        }


        [NonAction]
        public void EmailNotification_NewEmployee(prl_employee emp, string emailID, string empPassword, string emailFor)
        {
            try
            {
                //var verifyUrl = "/User/" + emailFor + "/" + activationCode;
                //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var link = "www.recombd.com/self-services";
                string linkName = "Click Here to Login";
                var fromEmail = new MailAddress("info@recombd.com", "ReCom Support");
                var fromEmailPassword = "Recom@2021#"; // Replace with actual password

                var toEmail = new MailAddress(emailID);

                string subject = "";
                string body = "";
                if (emailFor == "PayrollAccount")
                {
                    // For Testing Purpose

                    //subject = "Test Email " + monthYear;
                    //body = "<br/><br/><b>Dear " + employeeName + ",</b>" +
                    //    "<br/> Sorry have disturbed you. This is a Test mail :) :)" +
                    //    "<br/><br/>Best Wishes with Regards" +
                    //    "<br/><b>Novo Nordisk Pharma (Pvt.) Ltd</b>" +
                    //    "<br/>Dhaka, Bangladesh" +

                    //    "<br/><br/><b>Note: This is a system generated email, do not reply directly to this email id.</b>";

                    subject = "Your Payroll Self-Service account";

                    body = "<br/><br/><b>Dear " + emp.name + ",</b>" +
                        "<br/>This is to inform you that your <b>Payroll self-service</b> account has been successfully created. " +
                        "<br/>You are requested to login your payroll account and change your password." +
                        "<br/>You can also edit your others blank information. In every month after salary process," +
                        "you can check your salary payslip through this account." +
                        "<br/><b>Your Username :</b>  " + emp.emp_no + "" +
                        "<br/><b>Default Password :</b>  " + empPassword + "" +
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
            catch (Exception)
            {

                //throw;
            }
        }

        public ActionResult UndoEmployeeDiscontinue()
        {
            return View();
        }


        [HttpPost]
        public ActionResult UndoEmployeeDiscontinue(int? empid, FormCollection collection, EmployeeDiscontinue empDisCon, string sButton)
        {
            bool errorFound = false;
            var res = new OperationResult();
            var _emp = new Employee();
            byte UndodisconFlag = 1;
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
                        if (empid != null)
                        {
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.id == empid);
                            _emp = Mapper.Map<Employee>(emp);
                        }
                        else
                        {
                            string enpNo = collection["Emp_No"];
                            var emp = dataContext.prl_employee.SingleOrDefault(x => x.emp_no == enpNo);
                            _emp = Mapper.Map<Employee>(emp);
                        }
                        empDisCon.emp_id = _emp.id;
                        empDisCon.empInfo = _emp.name + "(" + _emp.emp_no + ")";

                        var lst = dataContext.prl_employee_discontinue.AsEnumerable().Where(p => p.emp_id == empDisCon.emp_id).ToList();
                        if (lst.Count() > 0)
                        {
                            var conDisCon = lst.OrderByDescending(x => x.id).First();
                            if (conDisCon.is_active == "Y")
                            {
                                empDisCon.status = "Continuing";
                            }
                            else
                            {
                                empDisCon.status = "Discontinuing";
                            }
                        }
                        else
                        {
                            empDisCon.status = "Continuing";
                        }
                    }
                }

                else
                {
                    if (empDisCon.emp_id == 0)
                    {
                        UndodisconFlag = 0;
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (empDisCon.continution_date == null)
                    {
                        UndodisconFlag = 0;
                        errorFound = true;
                        ModelState.AddModelError("", "Please select continuation date.");
                    }

                    var discontinue_date = dataContext.prl_employee_discontinue.AsEnumerable().Where(p => p.emp_id == empDisCon.emp_id).FirstOrDefault().discontinue_date;

                    if (empDisCon.continution_date < discontinue_date)
                    {
                        UndodisconFlag = 0;
                        errorFound = true;
                        ModelState.AddModelError("", "Continuation date must be greater then Discontinuation date");
                    }

                    if (!errorFound)
                    {
                        var lst = dataContext.prl_employee_discontinue.AsEnumerable().Where(p => p.emp_id == empDisCon.emp_id).ToList();
                        if (lst.Count() > 0)
                        {
                            var ActiveInactive = lst.OrderByDescending(x => x.id).First().is_active;
                            if (ActiveInactive == "Y")
                            {
                                UndodisconFlag = 0;
                                errorFound = true;
                                ModelState.AddModelError("", "This employee is already continuing.");
                            }
                            
                        }
                        else
                        {
                            UndodisconFlag = 0;
                            errorFound = true;
                            ModelState.AddModelError("", "This employee is already continuing.");
                        }

                        if (UndodisconFlag == 1)
                        {
                            var UndoDisC = dataContext.prl_employee_discontinue.Where(x => x.emp_id == empDisCon.emp_id).OrderByDescending(x => x.id).FirstOrDefault();

                            UndoDisC.is_active = "Y";
                            UndoDisC.continution_date = empDisCon.continution_date;
                            UndoDisC.updated_by = User.Identity.Name;
                            UndoDisC.updated_date = DateTime.Now;
                            UndoDisC.with_salary = "Y";
                            UndoDisC.without_salary = "N";

                            dataContext.SaveChanges();

                            var _updatingEmp = dataContext.prl_employee.SingleOrDefault(x => x.id == empDisCon.emp_id);
                            _updatingEmp.is_active = 1;
                            dataContext.SaveChanges();

                            res.IsSuccessful = true;
                            res.Message = "Employee continued successfully.";
                            TempData.Add("msg", res);

                            empDisCon.status = "Continuing";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return View(empDisCon);
        }

        public ActionResult EmployeeFreeCar()
        {
            return View();
        }

        #region Import

        [PayrollAuthorize]
        public ActionResult EmpImport(int? empid, FormCollection collection, string sButton)
        {
            //if (Session["_EmpD"] != null)
            //    Session["_EmpD"] = null;
            //if (Session["NewEmp"] != null)
            //    Session["NewEmp"] = null;
            //if (Session["NewEmpForEdit"] != null)
            //    Session["NewEmpForEdit"] = null;
            //if (Session["NewEmpDetailForEdit"] != null)
            //    Session["NewEmpDetailForEdit"] = null;
            if (Request.Cookies["_EmpD"] != null)
            {
                Response.Cookies["_EmpD"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies["NewEmp"] != null)
            {
                Response.Cookies["NewEmp"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies["NewEmpForEdit"] != null)
            {
                Response.Cookies["NewEmpForEdit"].Expires = DateTime.Now.AddDays(-1);
            }

            if (Request.Cookies["NewEmpDetailForEdit"] != null)
            {
                Response.Cookies["NewEmpDetailForEdit"].Expires = DateTime.Now.AddDays(-1);
            }

            var lists = new List<Employee>().ToPagedList(1, 1);

            if (sButton == null)
            {
                var lstEmp = dataContext.prl_employee.Include("prl_employee_details").OrderBy(x => x.emp_no);
                lists = Mapper.Map<List<Employee>>(lstEmp).ToPagedList(1, 25);
            }
            else
            {
                if (empid == null && string.IsNullOrEmpty(collection["Emp_No"]))
                {
                    //errorFound = true;
                    ModelState.AddModelError("", "Please select an employee or put employee ID");
                }
                else
                {
                    if (empid != null)
                    {
                        var _emp = dataContext.prl_employee.Include("prl_employee_details").Where(x => x.id == empid);
                        lists = Mapper.Map<List<Employee>>(_emp).ToPagedList(1, 1);
                    }
                    else
                    {
                        var _emp = dataContext.prl_employee.Include("prl_employee_details").AsEnumerable().Where(x => x.emp_no == collection["Emp_No"]);
                        if (_emp.Count() > 0)
                        {
                            lists = Mapper.Map<List<Employee>>(_emp).ToPagedList(1, 1);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Threre is no information for the given employee ID");
                        }
                    }
                }
            }

            return View(lists);
        }

        [HttpPost]
        public ActionResult EmpImport(HttpPostedFileBase EmpImportFile)
        {
            var res = new OperationResult();
            int flagCount = 0;

            if (EmpImportFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(EmpImportFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        res.IsSuccessful = false;
                        res.Messages.Add("Please select the excel file with .xls or .xlsx extension");
                        TempData.Add("msg", res);

                        return RedirectToAction("EmpImport", "Employee");
                    }

                    string folderPath = Server.MapPath("~/Files/");
                    //Check Directory exists else create one
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    //Save file to folder
                    var filePath = folderPath + Path.GetFileName(EmpImportFile.FileName);
                    EmpImportFile.SaveAs(filePath);

                    //Get file extension

                    string excelConString = "";

                    //Get connection string using extension 
                    switch (fileExtension)
                    {
                        //If uploaded file is Excel 1997-2007.
                        case ".xls":
                            excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                        //If uploaded file is Excel 2007 and above
                        case ".xlsx":
                            excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                            break;
                    }

                    //Read data from first sheet of excel into datatable
                    DataTable dt = new DataTable();
                    excelConString = string.Format(excelConString, filePath);

                    using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                    {
                        using (OleDbCommand excelDbCommand = new OleDbCommand())
                        {
                            using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                            {
                                excelDbCommand.Connection = excelOledbConnection;

                                excelOledbConnection.Open();
                                //Get schema from excel sheet
                                DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                //Get sheet name
                                string sheetName = excelSchema.Rows[1]["TABLE_NAME"].ToString();
                                excelOledbConnection.Close();

                                //Read Data from First Sheet.
                                excelOledbConnection.Open();
                                excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                excelDataAdapter.SelectCommand = excelDbCommand;
                                //Fill datatable from adapter
                                excelDataAdapter.Fill(dt);
                                excelOledbConnection.Close();
                            }
                        }
                    }

                    //Insert records to  tables.
                    using (var dataContext = new payroll_systemContext())
                    {
                        //Loop through datatable and add employee data to employee table.  
                            int empTotal = 0;
                            foreach (DataRow row in dt.Rows)
                            {

                                if (row.ItemArray != null)
                                {
                                    if (!row.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString()))))
                                    {
                                        prl_employee employee = new prl_employee();
                                        prl_employee_details employeeDetails = new prl_employee_details();
                                        
                                        string emp_no = (row[0].ToString()); //Employee No
                                        var name = row[1].ToString().Trim();
                                        var email = row[2].ToString().Trim();
                                        var gender = row[3].ToString().Trim();
                                        var joining_date = DateTime.Parse(row[4].ToString().Trim());
                                        //var joining_date = DateTime.ParseExact(date, "dd/MM/yyyy", 
                                        //    CultureInfo.InvariantCulture);
                                        var basic_salary = Convert.ToDecimal(row[9]);
                                        
                                        //Department Info
                                        var dptName = row[5].ToString().Trim();
                                        var dptId = 0;
                                        var dpt = dataContext.prl_department
                                            .Where(x => x.name == dptName).FirstOrDefault();
                                        dptId = dpt != null ? dpt.id : 0;

                                        //SubDepartment Info
                                        var sdptName = row[6].ToString().Trim();
                                        var sdptId = 0;
                                        var sdpt = dataContext.prl_sub_department
                                            .Where(x => x.name == sdptName).FirstOrDefault();
                                        sdptId = sdpt != null ? sdpt.id : 0;

                                        //SubSubDepartment Info
                                        var ssdptName = row[7].ToString().Trim();
                                        var ssdptId = 0;
                                        var ssdpt = dataContext.prl_sub_sub_department
                                            .Where(x => x.name == ssdptName).FirstOrDefault();
                                        ssdptId = ssdpt != null ? ssdpt.id : 0;

                                        //Designation Info
                                        var designationName = row[8].ToString().Trim();
                                        var designationId = 0;
                                        var designation = dataContext.prl_designation
                                            .Where(x => x.name == designationName).FirstOrDefault();
                                        designationId = designation != null ? designation.id : 0;


                                        
                                        var emp = dataContext.prl_employee.Where(x => x.emp_no == emp_no).FirstOrDefault();
                                        var empD = dataContext.prl_employee_details
                                            .Where(x =>x.prl_employee.emp_no==emp_no).FirstOrDefault();

                                        if (emp != null && empD != null)
                                        {
                                            //Emp Exist in DB
                                            errorIds.Add(emp_no);

                                        }

                                        else if (emp != null && empD == null)
                                        {
                                            //EmployeeDetails Info
                                            employeeDetails.emp_id = emp.id;
                                            employeeDetails.department_id = dptId;
                                            employeeDetails.sub_department_id = sdptId;
                                            employeeDetails.sub_sub_department_id = ssdptId;
                                            employeeDetails.designation_id = designationId;
                                            employeeDetails.basic_salary = basic_salary;
                                            employeeDetails.created_by = User.Identity.Name;
                                            employeeDetails.created_date = DateTime.Today;

                                            dataContext.prl_employee_details.Add(employeeDetails);
                                            dataContext.SaveChanges();

                                            //User Create Info
                                            Users CreateUser = new Users()
                                            {
                                                Emp_Id = emp.id,
                                                User_Name = emp.emp_no,
                                                Email = emp.email,
                                                Role_Name = "User",
                                                Password = emp.emp_no + "@" + emp.id.ToString() + DateTime.Now.Millisecond.ToString(), // first Time Password 
                                                PasswordQuestion = "What is your company name?",
                                                PasswordAnswer = "NovoNordisk",
                                                created_by = User.Identity.Name,
                                                created_date = DateTime.Today
                                            };

                                            var _prl_users = Mapper.Map<prl_users>(CreateUser);
                                            dataContext.prl_users.Add(_prl_users);
                                            dataContext.SaveChanges();

                                            //Sending Email to Employee

                                            var emp_prl_user = dataContext.prl_users.SingleOrDefault(x => x.emp_id == emp.id);

                                            if (emp_prl_user != null)
                                            {
                                                var _emp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp_prl_user.emp_id);

                                                if (empTotal <= 499) // gmail email sent limit 500 per day.
                                                {
                                                    if (_emp.email != "N/A" && _emp.email != null && _emp.email != "" && _emp.is_active == 1)
                                                    {
                                                        if (Utility.CommonFunctions.IsValidEmail(_emp.email) == true)
                                                        {
                                                            EmailNotification_NewEmployee(_emp, _emp.email, emp_prl_user.password, "PayrollAccount");
                                                            empTotal++;
                                                        }

                                                    }
                                                }
                                            }

                                        }

                                        else
                                        {
                                            //Employee Info
                                            employee.emp_no = emp_no;
                                            employee.name = name;
                                            employee.email = email;
                                            employee.gender = gender;
                                            employee.joining_date = joining_date;
                                            employee.is_active = 1;
                                            employee.religion_id = 1;
                                            employee.created_by = User.Identity.Name;
                                            employee.created_date = DateTime.Today;

                                            dataContext.prl_employee.Add(employee);
                                            dataContext.SaveChanges();


                                            //EmployeeDetails Info
                                            var newEmp = dataContext.prl_employee
                                                .Where(x => x.emp_no == emp_no).FirstOrDefault();

                                            employeeDetails.emp_id = newEmp.id;
                                            employeeDetails.department_id = dptId;
                                            employeeDetails.sub_department_id = sdptId;
                                            employeeDetails.sub_sub_department_id = ssdptId;
                                            employeeDetails.designation_id = designationId;
                                            employeeDetails.basic_salary = basic_salary;
                                            employeeDetails.created_by = User.Identity.Name;
                                            employeeDetails.created_date = DateTime.Today;

                                            dataContext.prl_employee_details.Add(employeeDetails);
                                            dataContext.SaveChanges();

                                            //User Create Info
                                            Users CreateUser = new Users()
                                            {
                                                Emp_Id = newEmp.id,
                                                User_Name = newEmp.emp_no,
                                                Email = newEmp.email,
                                                Role_Name = "User",
                                                Password = newEmp.emp_no + "@" + newEmp.id.ToString() + DateTime.Now.Millisecond.ToString(), // first Time Password 
                                                PasswordQuestion = "What is your company name?",
                                                PasswordAnswer = "NovoNordisk",
                                                created_by = User.Identity.Name,
                                                created_date = DateTime.Today
                                            };

                                            var _prl_users = Mapper.Map<prl_users>(CreateUser);
                                            dataContext.prl_users.Add(_prl_users);
                                            dataContext.SaveChanges();

                                            //Sending Email to Employee

                                            var emp_prl_user = dataContext.prl_users.SingleOrDefault(x => x.emp_id == newEmp.id);

                                            if (emp_prl_user != null)
                                            {
                                                var _emp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp_prl_user.emp_id);

                                                if (empTotal <= 499) // gmail email sent limit 500 per day.
                                                {
                                                    if (_emp.email != "N/A" && _emp.email != null && _emp.email != "" && _emp.is_active == 1)
                                                    {
                                                        if (Utility.CommonFunctions.IsValidEmail(_emp.email) == true)
                                                        {
                                                            EmailNotification_NewEmployee(_emp, _emp.email, emp_prl_user.password, "PayrollAccount");
                                                            empTotal++;
                                                        }

                                                    }
                                                }
                                            }

                                            flagCount++;
                                        }

                                    }

                                }

                                dataContext.SaveChanges();
                                
                               
                            }
                    }

                    res.IsSuccessful = true;
                    res.Messages.Add(flagCount+ " Employee(s) Info Uploaded successfully.");
                    
                    string errIdMsg = "";

                    if (errorIds.Count > 0)
                    {
                        errIdMsg = string.Join(", ", errorIds);
                        res.Messages.Add("Already Existed these IDs in Database " + errIdMsg);
                    }
                    
                    TempData.Add("msg", res);
                }
                
                catch (Exception ex)
                {
                    res.IsSuccessful = false;
                    res.Messages.Add(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                    //res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData.Add("msg", res);

                    return RedirectToAction("EmpImport", "Employee");
                }
            }
            else
            {
                //Upload file Null Message
                res.IsSuccessful = false;
                res.Messages.Add("Please select a file to Upload");
                TempData.Add("msg", res);

                return RedirectToAction("EmpImport", "Employee");
            }

            return RedirectToAction("EmpImport", "Employee");
        }


        [HttpPost]
        public ActionResult EmpImport1(HttpPostedFileBase EmpImportFile)
        {
            string filefullpath = string.Empty;
            var res = new OperationResult();
            try
            {
                if (EmpImportFile != null)
                {
                    var file = EmpImportFile;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);
                        //do stuff with the bytes
                        string fileName = file.FileName;
                        string filePath = Path.Combine(Request.PhysicalApplicationPath, "Files\\", fileName);

                        System.IO.File.WriteAllBytes(filePath, fileBytes);

                        //File Uploaded
                        XSSFWorkbook xssfWorkbook;

                        filefullpath = filePath;

                        //StreamReader streamReader = new StreamReader(model.ImportFile.InputStream);

                        using (FileStream fileStream = new FileStream(filefullpath, FileMode.Open, FileAccess.Read))
                        {
                            xssfWorkbook = new XSSFWorkbook(fileStream);
                        }
                        var employeeList = new List<Employee>();
                        var employeeDetailsList = new List<EmployeeDetails>();
                        var usersList = new List<Users>();

                        var employeeXlsViewModelList = new List<EmployeeXlsViewModel>();

                        //the columns
                        var properties = new string[] {
                            //emp_info
                            "EMPLOYEE_NUMBER",
                            "EMPLOYEE_NAME",
                            "MOBILE_NUMBER",
                            "PERSONAL_MOBILE_NUMBER",
                            "EMAIL_ADDRESS",
                            "PERSONAL_EMAIL_ADDRESS",
                            "DATE_OF_BIRTH",
                            "GENDER",
                            "DATE_OF_JOINING",
                            "CONFIRMATION_DATE",
                            "IS_CONFIRMED",
                            "RELIGION",

                            "SALARY_ACCOUNT_NUMBER",
                            "ROUTING_NO",
                            "TIN",
                            "IS_ACTIVE",

                            //Emp_Details
                            "EMPLOYEE_STATUS",
                            "EMPLOYEE_CATEGORY",
                            "JOB_LEVEL",
                            "DEPARTMENT",
                            "SUB_DEPARTMENT",
                            "SUB_SUB_DEPARTMENT",
                            "DESIGNATION",
                            "COST_CENTRE",
                            "BASIC_SALARY",
                            "MARITAL_STATUS",
                            "BLOOD_GROUP",
                            "PRESENT_ADDRESS",
                            "PERMANENT_ADDRESS"
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
                                    string EMPLOYEE_NUMBER = GetRowCellValue(sheet, row, properties, "EMPLOYEE_NUMBER", "StringCellValue");
                                    string EMPLOYEE_NAME = GetRowCellValue(sheet, row, properties, "EMPLOYEE_NAME", "StringCellValue");
                                    string MOBILE_NUMBER = GetRowCellValue(sheet, row, properties, "MOBILE_NUMBER", "StringCellValue");
                                    string PERSONAL_MOBILE_NUMBER = GetRowCellValue(sheet, row, properties, "PERSONAL_MOBILE_NUMBER", "StringCellValue");
                                    
                                    string EMAIL_ADDRESS = GetRowCellValue(sheet, row, properties, "EMAIL_ADDRESS", "StringCellValue");
                                    if (string.IsNullOrEmpty(EMAIL_ADDRESS))
                                    {
                                        if (Utility.CommonFunctions.IsValidEmail(EMAIL_ADDRESS) == false)
                                        {
                                            EMAIL_ADDRESS = "N/A";
                                        }
                                    }
                                    

                                    string PERSONAL_EMAIL_ADDRESS = GetRowCellValue(sheet, row, properties, "PERSONAL_EMAIL_ADDRESS", "StringCellValue");

                                    if (string.IsNullOrEmpty(PERSONAL_EMAIL_ADDRESS))
                                    {
                                        if (Utility.CommonFunctions.IsValidEmail(PERSONAL_EMAIL_ADDRESS) == false)
                                        {
                                            PERSONAL_EMAIL_ADDRESS = "N/A";
                                        }
                                    }

                                    string DATE_OF_BIRTH = GetRowCellValue(sheet, row, properties, "DATE_OF_BIRTH", "DateCellValue");
                                    string GENDER = GetRowCellValue(sheet, row, properties, "GENDER", "StringCellValue");
                                    string DATE_OF_JOINING = GetRowCellValue(sheet, row, properties, "DATE_OF_JOINING", "DateCellValue");
                                    string CONFIRMATION_DATE = GetRowCellValue(sheet, row, properties, "CONFIRMATION_DATE", "DateCellValue");
                                    string IS_CONFIRMED = GetRowCellValue(sheet, row, properties, "IS_CONFIRMED", "StringCellValue");
                                    string RELIGION = GetRowCellValue(sheet, row, properties, "RELIGION", "StringCellValue");

                                    string SALARY_ACCOUNT_NUMBER = GetRowCellValue(sheet, row, properties, "SALARY_ACCOUNT_NUMBER", "StringCellValue");
                                    string ROUTING_NO = GetRowCellValue(sheet, row, properties, "ROUTING_NO", "StringCellValue");
                                    string TIN = GetRowCellValue(sheet, row, properties, "TIN", "StringCellValue");
                                    string IS_ACTIVE = GetRowCellValue(sheet, row, properties, "IS_ACTIVE", "StringCellValue");

                                    string EMPLOYEE_STATUS = GetRowCellValue(sheet, row, properties, "EMPLOYEE_STATUS", "StringCellValue");
                                    string EMPLOYEE_CATEGORY = GetRowCellValue(sheet, row, properties, "EMPLOYEE_CATEGORY", "StringCellValue");
                                    string JOB_LEVEL = GetRowCellValue(sheet, row, properties, "JOB_LEVEL", "StringCellValue");
                                    string DEPARTMENT = GetRowCellValue(sheet, row, properties, "DEPARTMENT", "StringCellValue");
                                    string SUB_DEPARTMENT = GetRowCellValue(sheet, row, properties, "SUB_DEPARTMENT", "StringCellValue");
                                    string SUB_SUB_DEPARTMENT = GetRowCellValue(sheet, row, properties, "SUB_SUB_DEPARTMENT", "StringCellValue");
                                    string DESIGNATION = GetRowCellValue(sheet, row, properties, "DESIGNATION", "StringCellValue");
                                    string COST_CENTRE = GetRowCellValue(sheet, row, properties, "COST_CENTRE", "StringCellValue");
                                    string BASIC_SALARY = GetRowCellValue(sheet, row, properties, "BASIC_SALARY", "StringCellValue");
                                    string MARITAL_STATUS = GetRowCellValue(sheet, row, properties, "MARITAL_STATUS", "StringCellValue");
                                    string BLOOD_GROUP = GetRowCellValue(sheet, row, properties, "BLOOD_GROUP", "StringCellValue");
                                    string PRESENT_ADDRESS = GetRowCellValue(sheet, row, properties, "PRESENT_ADDRESS", "StringCellValue");
                                    string PERMANENT_ADDRESS = GetRowCellValue(sheet, row, properties, "PERMANENT_ADDRESS", "StringCellValue");

                                    var employeeXlsViewModel = new EmployeeXlsViewModel
                                    {
                                        EMPLOYEE_NUMBER = EMPLOYEE_NUMBER.ToString(),
                                        EMPLOYEE_NAME = EMPLOYEE_NAME,
                                        MOBILE_NUMBER = MOBILE_NUMBER.ToString(),
                                        PERSONAL_MOBILE_NUMBER = PERSONAL_MOBILE_NUMBER.ToString(),
                                        EMAIL_ADDRESS = EMAIL_ADDRESS,
                                        PERSONAL_EMAIL_ADDRESS = PERSONAL_EMAIL_ADDRESS,
                                        DATE_OF_BIRTH = DATE_OF_BIRTH != "" ? Convert.ToDateTime(DATE_OF_BIRTH).ToString("yyyy-MM-dd HH:mm:ss") : "0001-01-01 00:00:00",
                                        GENDER = GENDER == "Male" ? "Male" : "Female",
                                        DATE_OF_JOINING = Convert.ToDateTime(DATE_OF_JOINING).ToString("yyyy-MM-dd HH:mm:ss"),
                                        CONFIRMATION_DATE = CONFIRMATION_DATE != "" ? Convert.ToDateTime(CONFIRMATION_DATE).ToString("yyyy-MM-dd HH:mm:ss") : "0001-01-01 00:00:00",
                                        IS_CONFIRMED = IS_CONFIRMED != "" || IS_CONFIRMED == "Yes" || IS_CONFIRMED == "True" ? "True" : "False",
                                        RELIGION = RELIGION,
                                        SALARY_BANK_CODE = "",
                                        SALARY_BRANCH_CODE = "",
                                        ACCOUNT_TYPE = "",
                                        SALARY_ACCOUNT_NUMBER = SALARY_ACCOUNT_NUMBER.ToString(),
                                        ROUTING_NO = ROUTING_NO.ToString(),
                                        TIN = TIN.ToString(),

                                        IS_ACTIVE = IS_ACTIVE != "" || IS_ACTIVE == "Yes" || IS_ACTIVE == "True" ? "True" : "False",
                                        EMPLOYEE_STATUS = EMPLOYEE_STATUS,
                                        EMPLOYEE_CATEGORY = EMPLOYEE_CATEGORY,
                                        JOB_LEVEL = JOB_LEVEL,
                                        DEPARTMENT = DEPARTMENT,
                                        SUB_DEPARTMENT = SUB_DEPARTMENT,
                                        SUB_SUB_DEPARTMENT = SUB_SUB_DEPARTMENT,
                                        DESIGNATION = DESIGNATION,
                                        COST_CENTRE = COST_CENTRE,
                                        BASIC_SALARY = BASIC_SALARY,
                                        MARITAL_STATUS = MARITAL_STATUS,
                                        BLOOD_GROUP = BLOOD_GROUP,
                                        PRESENT_ADDRESS = PRESENT_ADDRESS,
                                        PARMANENT_ADDRESS = PERMANENT_ADDRESS
                                    };

                                    employeeXlsViewModelList.Add(employeeXlsViewModel);
                                }
                            }
                        }

                        #region Insert To Database

                        int returnSaveChanges = 0;
                        int empTotal = 0;

                        foreach (var employeeXlsViewModel in employeeXlsViewModelList)
                        {
                            #region Employee, EmployeeDetails

                            var empInfo = dataContext.prl_employee.FirstOrDefault(item => item.emp_no == employeeXlsViewModel.EMPLOYEE_NUMBER);

                            // Check is Staff Id Existed or Not

                            if (empInfo != null)
                            {
                                res.IsSuccessful = false;
                                res.Message = "The Staff Id of " + empInfo.emp_no + " already existed in the database.";
                                TempData.Add("msg", res);

                                return RedirectToAction("EmpImport", "Employee");
                            }
                            else
                            {
                                var religion = dataContext.prl_religion.FirstOrDefault(item => item.name.ToLower() == employeeXlsViewModel.RELIGION.ToLower());
                                //var bank = dataContext.prl_bank.ToList().FirstOrDefault(item => item.bank_code.ToLower() == employeeXlsViewModel.SALARY_BANK_CODE.ToLower());
                                //var branch = dataContext.prl_bank_branch.ToList().FirstOrDefault(item => item.branch_code.ToLower() == employeeXlsViewModel.SALARY_BRANCH_CODE.ToLower());

                                Employee employee = new Employee()
                                {
                                    emp_no = employeeXlsViewModel.EMPLOYEE_NUMBER.ToString(),
                                    name = employeeXlsViewModel.EMPLOYEE_NAME,
                                    official_contact_no = employeeXlsViewModel.MOBILE_NUMBER,
                                    personal_contact_no = employeeXlsViewModel.PERSONAL_MOBILE_NUMBER,
                                    email = employeeXlsViewModel.EMAIL_ADDRESS,
                                    personal_email = employeeXlsViewModel.PERSONAL_EMAIL_ADDRESS,
                                    dob = Convert.ToDateTime(employeeXlsViewModel.DATE_OF_BIRTH),
                                    gender = employeeXlsViewModel.GENDER,
                                    joining_date = Convert.ToDateTime(employeeXlsViewModel.DATE_OF_JOINING),

                                    confirmation_date = Convert.ToDateTime(employeeXlsViewModel.CONFIRMATION_DATE),
                                    //DateTime.ParseExact(employeeXlsViewModel.CONFIRMATION_DATE, "yyyy-MM-dd HH:mm:ss", null),

                                    is_confirmed = bool.Parse(employeeXlsViewModel.IS_CONFIRMED),
                                    religion_id = religion != null ? religion.id : 0,
                                    //bank_id = bank != null ? bank.id : 0,
                                    //bank_branch_id = bank != null ? branch.id : 0,
                                    bank_id = employeeXlsViewModel.SALARY_BANK_CODE != "" ? 1 : 1,
                                    bank_branch_id = employeeXlsViewModel.SALARY_BRANCH_CODE != "" ? 1 : 1,
                                    //account_type = employeeXlsViewModel.ACCOUNT_TYPE,
                                    account_type = "Salary",
                                    account_no = employeeXlsViewModel.SALARY_ACCOUNT_NUMBER,
                                    routing_no = employeeXlsViewModel.ROUTING_NO,
                                    tin = employeeXlsViewModel.TIN,

                                    is_active = bool.Parse(employeeXlsViewModel.IS_ACTIVE),

                                    created_by = User.Identity.Name,
                                    created_date = DateTime.Today
                                };

                                var _prl_employee = Mapper.Map<prl_employee>(employee);
                                dataContext.prl_employee.Add(_prl_employee);
                                returnSaveChanges = dataContext.SaveChanges();

                                employeeList.Add(employee);

                                var grade = dataContext.prl_grade.FirstOrDefault(item => item.grade.ToLower() == employeeXlsViewModel.GRADE.ToLower());
                                var jobLevel = dataContext.prl_job_level.FirstOrDefault(item => item.title.ToLower() == employeeXlsViewModel.JOB_LEVEL.ToLower());
                                var department = dataContext.prl_department.FirstOrDefault(item => item.name.ToLower() == employeeXlsViewModel.DEPARTMENT.ToLower());

                                prl_sub_department sub_department = new prl_sub_department();
                                prl_sub_sub_department sub_sub_department = new prl_sub_sub_department();


                                if (employeeXlsViewModel.SUB_DEPARTMENT == "")
                                {
                                    sub_department = dataContext.prl_sub_department.FirstOrDefault(item => item.id == 0);
                                }
                                else
                                {
                                    sub_department = dataContext.prl_sub_department.FirstOrDefault(item => item.name.ToLower() == employeeXlsViewModel.SUB_DEPARTMENT.ToLower());
                                }


                                if (employeeXlsViewModel.SUB_SUB_DEPARTMENT == "")
                                {
                                    sub_sub_department = dataContext.prl_sub_sub_department.FirstOrDefault(item => item.id == 0);
                                }
                                else
                                {
                                    sub_sub_department = dataContext.prl_sub_sub_department.FirstOrDefault(item => item.name.ToLower() == employeeXlsViewModel.SUB_SUB_DEPARTMENT.ToLower());
                                }

                                var designation = dataContext.prl_designation.FirstOrDefault(item => item.name.ToLower() == employeeXlsViewModel.DESIGNATION.ToLower());
                                var cost_centre = dataContext.prl_cost_centre.FirstOrDefault(item => item.cost_centre_name.ToLower() == employeeXlsViewModel.COST_CENTRE.ToLower());


                                EmployeeDetails employeeDetails = new EmployeeDetails()
                                {
                                    emp_id = _prl_employee.id,
                                    emp_status = employeeXlsViewModel.EMPLOYEE_STATUS.ToString(),
                                    employee_category = employeeXlsViewModel.EMPLOYEE_CATEGORY.ToString(),
                                    job_level_id = jobLevel != null ? jobLevel.id : 0,
                                    grade_id = grade != null ? grade.id : 0,
                                    department_id = department != null ? department.id : 0,
                                    sub_department_id = sub_department != null ? sub_department.id : 0,
                                    sub_sub_department_id = sub_sub_department != null ? sub_sub_department.id : 0,

                                    designation_id = designation != null ? designation.id : 0,
                                    cost_centre_id = cost_centre != null ? cost_centre.id : 0,
                                    basic_salary = Convert.ToDecimal(employeeXlsViewModel.BASIC_SALARY) <= 0 ? 1 : Convert.ToDecimal(employeeXlsViewModel.BASIC_SALARY),
                                    marital_status = employeeXlsViewModel.MARITAL_STATUS.ToString(),
                                    blood_group = employeeXlsViewModel.BLOOD_GROUP.ToString(),
                                    parmanent_address = employeeXlsViewModel.PARMANENT_ADDRESS.ToString(),
                                    present_address = employeeXlsViewModel.PRESENT_ADDRESS.ToString(),
                                    created_by = User.Identity.Name,
                                    created_date = DateTime.Today
                                };

                                var _prl_employee_details = Mapper.Map<prl_employee_details>(employeeDetails);

                                dataContext.prl_employee_details.Add(_prl_employee_details);

                                returnSaveChanges = dataContext.SaveChanges();

                                employeeDetailsList.Add(employeeDetails);

                                Users CreateUser = new Users()
                                {
                                    Emp_Id = _prl_employee.id,
                                    User_Name = _prl_employee.emp_no,
                                    Email = _prl_employee.email,
                                    Role_Name = "User",
                                    Password = _prl_employee.emp_no, // first Time Password as Like same as emp_no
                                    PasswordQuestion = "What is your company name?",
                                    PasswordAnswer = "NovoNordisk",
                                    created_by = User.Identity.Name,
                                    created_date = DateTime.Today
                                };

                                var _prl_users = Mapper.Map<prl_users>(CreateUser);
                                dataContext.prl_users.Add(_prl_users);
                                returnSaveChanges = dataContext.SaveChanges();

                                usersList.Add(CreateUser);

                                //Sending Email to Employee

                                var emp_prl_user = dataContext.prl_users.SingleOrDefault(x => x.emp_id == _prl_employee.id);

                                if (emp_prl_user != null)
                                {
                                    var emp = dataContext.prl_employee.SingleOrDefault(x => x.id == emp_prl_user.emp_id);

                                    if (empTotal <= 499) // gmail email sent limit 500 per day.
                                    {
                                        if (emp.email != "N/A" && emp.email != null && emp.email != "" && emp.is_active == 1)
                                        {
                                            if (Utility.CommonFunctions.IsValidEmail(emp.email) == true)
                                            {
                                                EmailNotification_NewEmployee(emp, emp.email, emp_prl_user.password, "PayrollAccount");
                                                empTotal++;
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        if (returnSaveChanges > 0)
                        {
                            res.IsSuccessful = true;
                            res.Message = "Employee data uploaded successfully.";
                            TempData.Add("msg", res);
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Employee data do not uploaded successfully.";
                            TempData.Add("msg", res);
                        }

                        return RedirectToAction("EmpImport", "Employee");
                    }
                    else
                    {
                        res.IsSuccessful = false;
                        res.Message = "Upload can not be empty.";
                        TempData.Add("msg", res);

                        return RedirectToAction("EmpImport", "Employee");
                    }

                }
                else
                {
                    //Upload file Null Message
                    res.IsSuccessful = false;
                    res.Message = "Upload can not be empty.";
                    TempData.Add("msg", res);

                    return RedirectToAction("EmpImport", "Employee");
                }

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", res);

                return RedirectToAction("EmpImport", "Employee");
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

        #endregion

        public FileResult GetEmpDataUplodSample()
        {
            var fileName = "Sample of Employees Data Import.xlsx";
            //string FileURL = "D:/1. Lukman_Official/Sample of Employees Data Import.xlsx";
            string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/Sample of Employees Data Import.xlsx";
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);

            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(FileBytes, "application/xlsx", fileName); 
        }

        public FileResult GetSaleryReviewUplodSample()
        {
            var fileName = "SaleryReviewFormat.xlsx";
            //string FileURL = "D:/1. Lukman_Official/Sample of Employees Data Import.xlsx";
            //string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/Sample of Salery Review Data Import.xlsx";
            string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/SaleryReviewFormat.xlsx";
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);

            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(FileBytes, "application/xlsx", fileName); GetSaleryReviewUplodSample
        }

        public FileResult GetBankInfoUplodSample()
        {
            var fileName = "BankInfoFormat.xlsx";
            //string FileURL = "D:/1. Lukman_Official/Sample of Employees Data Import.xlsx";
            //string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/Sample of Salery Review Data Import.xlsx";
            string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/BankInfoFormat.xlsx";
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);

            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(FileBytes, "application/xlsx", fileName); GetSaleryReviewUplodSample
        }

        public FileResult GetEmployeeDiscontinueUplodSample()
        {
            var fileName = "EmployeeDiscontinueFormat.xlsx";
            //string FileURL = "D:/1. Lukman_Official/Sample of Employees Data Import.xlsx";
            //string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/Sample of Salery Review Data Import.xlsx";
            string FileURL = "C:/inetpub/wwwroot/prl_novo_nordisk/EmployeeDiscontinueFormat.xlsx";
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);

            return File(FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(FileBytes, "application/xlsx", fileName); GetSaleryReviewUplodSample
        }


    }
}
