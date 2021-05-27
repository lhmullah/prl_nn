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
    public class LeaveWithoutPayController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public LeaveWithoutPayController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            var lstLeave = dataContext.prl_leave_without_pay_settings.ToList().OrderBy(p=>p.Lwp_type);
            return View(Mapper.Map<List<LeaveWithoutPaySetting>>(lstLeave));
        }

        [PayrollAuthorize]
        public ActionResult Create()
        {
            var lst = dataContext.prl_allowance_name.ToList();
            var lst2 = Mapper.Map<List<AllowanceName>>(lst);
            ViewBag.AllowanceNames = lst2;
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeaveWithoutPaySetting item)
        {
            var res = new OperationResult();
            try
            {
                if (item.Lwp_type == "SELECT")
                {
                    res.IsSuccessful = false;
                    res.Message = "Please Select a Leave Type.";
                    TempData.Add("msg", res);
                    return RedirectToAction("Create");
                }
                var lwp = Mapper.Map<prl_leave_without_pay_settings>(item);
                dataContext.prl_leave_without_pay_settings.Add(lwp);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Leave type created.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch
            {
                res.IsSuccessful = false;
                res.Message = "";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int id)
        {
            var prlLeave = dataContext.prl_leave_without_pay_settings.SingleOrDefault(x => x.id == id);
            var lw = Mapper.Map<LeaveWithoutPaySetting>(prlLeave);

            var lst = dataContext.prl_allowance_name.ToList();
            var lst2 = Mapper.Map<List<AllowanceName>>(lst);
            ViewBag.AllowanceNames = lst2;
            return View(lw);
        }

        [HttpPost]
        public ActionResult Edit(LeaveWithoutPaySetting item)
        {
            var res = new OperationResult();
            try
            {
                var lwp = dataContext.prl_leave_without_pay_settings.SingleOrDefault(x => x.id == item.id);
                lwp.percentage_of_basic = item.percentage_of_basic;
                lwp.allowance_id = item.allowance_id;
                lwp.percentage_of_allowance = item.percentage_of_allowance;
                lwp.Lwp_type = item.Lwp_type;
                dataContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            var lst = dataContext.prl_allowance_head.ToList();
            ViewBag.AllowanceNames = Mapper.Map<List<AllowanceHead>>(lst);
            return View();
        }

        public ActionResult Delete(int id)
        {
            var res = new OperationResult();
            try
            {
                var allName = dataContext.prl_leave_without_pay_settings.SingleOrDefault(x => x.id == id);
                if (allName == null)
                {
                    return HttpNotFound();
                }
                dataContext.prl_leave_without_pay_settings.Remove(allName);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Information deleted successfully.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = "Data cannot deleted.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
        }

        [PayrollAuthorize]
        public ActionResult EmployeeLWP()
        {

            var list = dataContext.prl_employee_leave_without_pay.ToList();
            var vwList = Mapper.Map<List<EmployeeLeaveWithoutPay>>(list).ToPagedList(1, 15);
            return View(vwList);
        }

        public ActionResult Paging(int? page)
        {

            int pageSize = 15;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = dataContext.prl_employee_leave_without_pay.ToList();
            var vwList = Mapper.Map<List<EmployeeLeaveWithoutPay>>(list);
            var pglst = vwList.ToPagedList(pageIndex, pageSize);

            return View("EmployeeLWP", pglst);
        }


        [PayrollAuthorize]
        public ActionResult AssignEmployeeLWP(int? id)
        {

            if (id > 0)
            {
                var _Emp = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == id);
                return View(Mapper.Map<Employee>(_Emp));
            }
            else
            {
                var lvTyp = dataContext.prl_leave_without_pay_settings.Distinct(); ;
                ViewBag.Settings = lvTyp;
                return View();
            }
        }

        [HttpPost]
        public ActionResult AssignEmployeeLWP(int? empid, FormCollection collection, EmployeeLeaveWithoutPay e_lwp, string sButton)
        {
            var lvTyp = dataContext.prl_leave_without_pay_settings.Distinct();
            ViewBag.Settings = lvTyp;

            bool errorFound = false;
            var res = new OperationResult();

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
                            e_lwp.emp_id = empD.id;
                            return View(e_lwp);
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
                                e_lwp.emp_id = empD.id;
                                return View(e_lwp);
                            }
                        }
                    }
                }
                else if (sButton == "Save")
                {
                    if (e_lwp.emp_id == 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (!errorFound)
                    {
                        var lwp = new prl_employee_leave_without_pay();
                        lwp.emp_id = e_lwp.emp_id;
                        lwp.strat_date = e_lwp.strat_date;
                        lwp.end_date = e_lwp.end_date;
                        lwp.setting_id = e_lwp.setting_id;//////

                        dataContext.prl_employee_leave_without_pay.Add(lwp);
                        dataContext.SaveChanges();

                        res.IsSuccessful = true;
                        res.Message = " Employee confirmed successfully.";
                        TempData.Add("msg", res);

                        return RedirectToAction("EmployeeLWP");
                    }
                }
                
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return View();
        }

        public ActionResult ModifyEmployeeLWP(int id)
        {
            var e_lwp = dataContext.prl_employee_leave_without_pay.FirstOrDefault(q => q.emp_id == id);
            var lwp = Mapper.Map<EmployeeLeaveWithoutPay>(e_lwp);
            var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == id);
            var empD = Mapper.Map<Employee>(_empD);
            ViewBag.Employee = empD;
            return View(lwp);
        }

        [HttpPost]
        public ActionResult ModifyEmployeeLWP(int id, EmployeeLeaveWithoutPay collection)
        {
            bool errorFound = false;
            var res = new OperationResult();
            try
            {
                var eLWP = dataContext.prl_employee_leave_without_pay.FirstOrDefault(q => q.id == collection.id);
                eLWP.no_of_days = collection.no_of_days;
                eLWP.strat_date = collection.strat_date;
                eLWP.end_date = collection.end_date;
                eLWP.setting_id = collection.setting_id;
                eLWP.updated_by = "";
                eLWP.updated_date = DateTime.Now;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Successfully edited.";
                TempData.Add("msg", res);
                return RedirectToAction("EmployeeLWP");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", res);
            }
            return View();
        }

        public ActionResult EditDataPaging(int did, DateTime dt, int? page)
        {
            try
            {
                int pageSize = 30;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.did = did;
                ViewBag.dt = dt;
                //var lst = dataContext.prl_employee_free_car.Include("prl_employee").AsEnumerable().Where(x=>x.
                var lst = dataContext.prl_upload_allowance.Include("prl_allowance_name").AsEnumerable().Where(x => x.salary_month_year.Value.ToString("yyyy-MM").Contains(dt.ToString("yyyy-MM")) && x.allowance_name_id == did);
                var kk = Mapper.Map<List<AllowanceUploadData>>(lst).ToPagedList(pageIndex, pageSize);

                return View("EmployeeLWP", kk);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [PayrollAuthorize]
        public ActionResult LWPDelete(int id)
        {
            string name = "";
            var res = new OperationResult();
            try
            {
                var e_lwp = dataContext.prl_employee_leave_without_pay.SingleOrDefault(x => x.id == id);
                if (e_lwp == null)
                {
                    return HttpNotFound();
                }
                dataContext.prl_employee_leave_without_pay.Remove(e_lwp);
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Employee's leave without pay deleted";
                TempData.Add("msg", res);
                return RedirectToAction("EmployeeLWP");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = "Could not delete.";
                TempData.Add("msg", res);
                return RedirectToAction("EmployeeLWP");
            }
        }
    }
}
