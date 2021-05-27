using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using com.gpit.DataContext;
using com.gpit.Model;
using PagedList;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Controllers
{
    public class FreeCarController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public FreeCarController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        [PayrollAuthorize]
        public ActionResult Index()
        {
            
            var list = dataContext.prl_employee_free_car.ToList().OrderByDescending(p=>p.start_date);
            var vwList = Mapper.Map<List<EmployeeFreeCar>>(list).ToPagedList(1, 15); 
            return View(vwList);
        }

        public ActionResult Paging(int? page)
        {

            int pageSize = 15;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = dataContext.prl_employee_free_car.ToList().OrderByDescending(p => p.start_date);
            var vwList = Mapper.Map<List<EmployeeFreeCar>>(list);

            var pglst = vwList.ToPagedList(pageIndex, pageSize);

            return View("Index", pglst);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        [PayrollAuthorize]
        public ActionResult Create(int? id)
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

        //
        // POST: /FreeCar/Create

        [HttpPost]
        public ActionResult Create(int? empid, FormCollection collection,Employee emp, EmployeeFreeCar f_car, string sButton)
        {
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
                            f_car.emp_id = empD.id;
                            return View(f_car);
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
                                f_car.emp_id = empD.id;
                                return View(f_car);
                            }
                        }
                    }
                }
                else if (sButton == "Save")
                {
                    if (f_car.emp_id == 0)
                    {
                        errorFound = true;
                        ModelState.AddModelError("", "No employee selected.");
                    }
                    if (!errorFound)
                    {
                        var freeC = new prl_employee_free_car();
                        freeC.emp_id = f_car.emp_id;
                        freeC.start_date = f_car.start_date;
                        freeC.is_active = "Y";
                        freeC.is_projected = f_car.is_projected;
                        freeC.created_by = User.Identity.Name;
                        freeC.created_date = DateTime.Now;
                        dataContext.prl_employee_free_car.Add(freeC);
                        dataContext.SaveChanges();

                        res.IsSuccessful = true;
                        res.Message = " Employee confirmed successfully.";
                        TempData.Add("msg", res);
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /FreeCar/Edit/5

        public ActionResult Edit(int id)
        {
            var f_car = dataContext.prl_employee_free_car.FirstOrDefault(q => q.emp_id == id);
            var carF = Mapper.Map<EmployeeFreeCar>(f_car);
            var _empD = dataContext.prl_employee.Include("prl_employee_details").SingleOrDefault(x => x.id == id);
            var empD = Mapper.Map<Employee>(_empD);
            ViewBag.Employee = empD;
            return View(carF);
        }

        //
        // POST: /FreeCar/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, EmployeeFreeCar collection)
        {
            bool errorFound = false;
            var res = new OperationResult();
            try
            {
                var fCar = dataContext.prl_employee_free_car.FirstOrDefault(q => q.id == collection.id);
                fCar.is_active = collection.is_active;
                fCar.is_projected = collection.is_projected;
                fCar.start_date = collection.start_date;
                fCar.end_date = collection.end_date;
                fCar.updated_by = User.Identity.Name;
                fCar.updated_date = DateTime.Now;
                dataContext.SaveChanges();

                res.IsSuccessful = true;
                res.Message = "Successfully edited.";
                TempData.Add("msg", res);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData.Add("msg", res);
            }
            return View();
        }

        //
        // GET: /FreeCar/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /FreeCar/Delete/5

        public PartialViewResult EditDataPaging(int did, DateTime dt, int? page)
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

                return PartialView("_UploadedAllowances", kk);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
