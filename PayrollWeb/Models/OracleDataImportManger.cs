using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using com.gpit.DataContext;
using MySql.Data.MySqlClient;
//using Oracle.DataAccess.Client;
using com.gpit.Model;

namespace PayrollWeb.Models
{
    public class OracleDataImportManger
    {
        protected DataImportProcessedEventArgs args;
        public OracleDataImportManger()
        {
            args = new DataImportProcessedEventArgs();
            args.TotalData = 0;
            args.NumberOfDataProcessed = 0;
        }

        public event EventHandler<DataImportProcessedEventArgs> DataImportProcessed;
        public  DataImportModel GetEmployees()
        {
            try
            {
                var result = new DataImportModel();
                var lstReligions = new List<prl_religion>();
                var lstCompanies = new List<prl_company>();
                var lstBankBranches = new List<prl_bank_branch>();
                var lstBanks = new List<prl_bank>();
                var dictEmployees = new Dictionary<string, prl_employee>();
                var dictEmployeeDetails = new Dictionary<string,prl_employee_details>();
                var dictInnerEmpd = new  Dictionary<string,prl_employee_details>();
                var lstDivisions = new List<prl_division>();
                var lstDepartments = new List<prl_department>();
                var lstGrades = new List<prl_grade>();
                var lstLocations = new List<prl_location>();
                var lstDesignation = new List<prl_designation>();

                string connectionString = ConfigurationManager.ConnectionStrings["oracleDataImport"].ConnectionString;

                using (var contxt = new payroll_systemContext())
                {
                    lstReligions = contxt.prl_religion.ToList();
                    lstCompanies = contxt.prl_company.ToList();
                    contxt.prl_bank_branch.ToList();
                    lstBanks = contxt.prl_bank.Include("prl_bank_branch").ToList();
                    lstDivisions = contxt.prl_division.ToList();
                    lstDepartments = contxt.prl_department.ToList();
                    lstGrades = contxt.prl_grade.ToList();
                    lstLocations = contxt.prl_location.ToList();
                    lstDesignation = contxt.prl_designation.ToList();

                    using (var con = new OracleConnection(connectionString))
                    {
                        con.Open();
                        const string cmdText = @"select distinct employee_number,employee_name, 
                                    mobile_number, 	email_address, 	religion,	gender,	marital_status, 
                                    company_code, 	fathers_name, 	salary_bank_code,salary_bank,salary_bank_code ,
                                    salary_branch,salary_branch_code, salary_account_number, date_of_birth, date_of_joining, 
                                    tin, confirmation_date, termination_date, basicsal_grade_effective_date,basic_salary,
                                    division,department,grade,designation,category,location
                                    from payroll_test_khurshid";

                        using (var command = new OracleCommand(cmdText, con))
                        {
                            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                            while (reader.Read())
                            {
                                var emp = new prl_employee();
                                emp.emp_no = reader["employee_number"].ToString();
                                emp.name = reader["employee_name"].ToString();

                                emp.phone = reader["mobile_number"].ToString();
                                emp.email = reader["email_address"].ToString();
                                if (reader["religion"] != null)
                                {
                                    string rel = reader["religion"].ToString().ToLower();
                                    var relObj =lstReligions.AsEnumerable().FirstOrDefault(x => x.name.ToLower().Contains(rel));
                                    if (relObj != null)
                                    {
                                        emp.religion_id = relObj.id;
                                    }
                                    else
                                    {
                                        var newReligion = new prl_religion();
                                        newReligion.name = rel;
                                        newReligion.no_of_bonus = 1;
                                        contxt.prl_religion.Add(newReligion);
                                        contxt.SaveChanges();
                                        emp.religion_id = newReligion.id;
                                        lstReligions = contxt.prl_religion.ToList();
                                    }
                                }
                                if (reader["gender"] != null)
                                {
                                    var g = reader["gender"].ToString().ToLower();
                                    emp.gender = g == "m" ? "Male" : "Female";
                                }
                                emp.marital_status = reader["marital_status"].ToString();
                                if (reader["company_code"] != null)
                                {
                                    string companyText = reader["company_code"].ToString().ToLower();
                                    var company = lstCompanies.AsEnumerable().FirstOrDefault(x => x.name.ToLower().Contains(companyText));
                                    emp.company_id = company.id;
                                }
                                emp.father_name = reader["fathers_name"] == null ? "" : reader["fathers_name"].ToString();

                                if (reader["salary_bank_code"] != null)
                                {
                                    var bt = reader["salary_bank_code"].ToString().ToLower();
                                    var bank =lstBanks.AsEnumerable().SingleOrDefault(x => x.bank_code.ToLower() == bt);
                                    if (bank != null)
                                    {
                                        var branch = bank.prl_bank_branch.AsEnumerable().FirstOrDefault(x => x.branch_code.ToLower() == reader["salary_branch_code"].ToString().ToLower());
                                        if (branch != null)
                                        {
                                            emp.bank_id = branch.bank_id;
                                            emp.bank_branch_id = branch.id;
                                        }
                                        else
                                        {
                                            var newBranch = new prl_bank_branch();
                                            newBranch.bank_id = bank.id;
                                            newBranch.branch_code = reader["salary_branch_code"].ToString();
                                            newBranch.branch_name = reader["salary_branch"].ToString();
                                            contxt.prl_bank_branch.Add(newBranch);
                                            contxt.SaveChanges();
                                            emp.bank_id = branch.bank_id;
                                            emp.bank_branch_id = newBranch.id;
                                            lstBanks = contxt.prl_bank.Include("prl_bank_branch").ToList();
                                        }
                                    }
                                    else
                                    {
                                        var newBank = new prl_bank();
                                        newBank.bank_name = reader["salary_bank"].ToString();
                                        newBank.bank_code = reader["salary_bank_code"].ToString();
                                        var newBranch = new prl_bank_branch();
                                        newBranch.bank_id = newBank.id;
                                        newBranch.branch_code = reader["salary_branch_code"].ToString();
                                        newBranch.branch_name = reader["salary_branch"].ToString();
                                        newBank.prl_bank_branch.Add(newBranch);
                                        contxt.prl_bank.Add(newBank);
                                        contxt.SaveChanges();
                                        emp.bank_id = newBranch.bank_id;
                                        emp.bank_branch_id = newBranch.id;
                                        lstBanks = contxt.prl_bank.Include("prl_bank_branch").ToList();
                                    }
                                }
                                emp.account_no = reader["salary_account_number"].ToString();

                                if (reader["date_of_birth"] != null)
                                {
                                    emp.dob = Convert.ToDateTime(reader["date_of_birth"].ToString());
                                }
                                if (reader["date_of_joining"] != null)
                                {
                                    emp.joining_date = Convert.ToDateTime(reader["date_of_joining"].ToString());
                                }

                                emp.tin = reader["tin"] == null ? "" : reader["tin"].ToString();
                                if (reader["confirmation_date"] != null)
                                {
                                    var cdate = reader["confirmation_date"].ToString();
                                    if (!string.IsNullOrWhiteSpace(cdate))
                                        emp.confirmation_date =
                                            Convert.ToDateTime(reader["confirmation_date"].ToString());
                                }

                                if (reader["termination_date"] != null)
                                {
                                    var tdate = reader["termination_date"].ToString();
                                    if (!string.IsNullOrWhiteSpace(tdate))
                                        emp.termination_date = Convert.ToDateTime(reader["termination_date"].ToString());
                                }

                                //employee details part here
                                var empDetails = new prl_employee_details();
                                empDetails.emp_status = reader["category"].ToString();

                                if (!string.IsNullOrWhiteSpace(reader["division"].ToString()))
                                {
                                    var d = reader["division"].ToString().ToLower();
                                    var dv = lstDivisions.SingleOrDefault(x => x.name.ToLower() == d);
                                    if (dv != null)
                                    {
                                        empDetails.division_id = dv.id;
                                    }
                                    else
                                    {
                                        var newDivision = new prl_division();
                                        newDivision.name = reader["division"].ToString();
                                        contxt.prl_division.Add(newDivision);
                                        contxt.SaveChanges();
                                        empDetails.division_id = newDivision.id;
                                        lstDivisions = contxt.prl_division.ToList();
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(reader["department"].ToString()))
                                {
                                    var d = reader["department"].ToString().ToLower();
                                    var dpt = lstDepartments.SingleOrDefault(x => x.name.ToLower() == d);
                                    if (dpt != null)
                                    {
                                        empDetails.department_id = dpt.id;
                                    }
                                    else
                                    {
                                        var newDepartment = new prl_department();
                                        newDepartment.name = reader["department"].ToString();
                                        contxt.prl_department.Add(newDepartment);
                                        contxt.SaveChanges();
                                        empDetails.department_id = newDepartment.id;
                                        lstDepartments = contxt.prl_department.ToList();
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(reader["grade"].ToString()))
                                {
                                    var g = reader["grade"].ToString();
                                    var grd = lstGrades.SingleOrDefault(x => x.grade.ToLower() == g.ToLower());
                                    if (grd != null)
                                    {
                                        empDetails.grade_id = grd.id;
                                    }
                                    else
                                    {
                                        var newGrade = new prl_grade();
                                        newGrade.grade = g;
                                        contxt.prl_grade.Add(newGrade);
                                        contxt.SaveChanges();
                                        empDetails.grade_id = newGrade.id;
                                        lstGrades = contxt.prl_grade.ToList();
                                    }
                                }
                                empDetails.basic_salary = Convert.ToDecimal(reader["basic_salary"].ToString());
                                if (reader["location"] != null)
                                {
                                    var l = reader["location"].ToString();
                                    var loc = lstLocations.SingleOrDefault(x => x.location_name.ToLower() == l.ToLower());
                                    if (loc != null)
                                    {
                                        empDetails.posting_location_id = loc.id;
                                    }
                                    else
                                    {
                                        var newLocation = new prl_location();
                                        newLocation.location_name = l;
                                        contxt.prl_location.Add(newLocation);
                                        contxt.SaveChanges();
                                        empDetails.posting_location_id = newLocation.id;
                                        lstLocations = contxt.prl_location.ToList();
                                    }
                                }
                                if (reader["designation"] !=null)
                                {
                                    var dsgn = reader["designation"].ToString().Trim();
                                    var exDsgn = lstDesignation.SingleOrDefault(x => x.name.ToLower() == dsgn.ToLower());
                                    if (exDsgn != null)
                                    {
                                        empDetails.designation_id = exDsgn.id;
                                    }
                                    else
                                    {
                                        var nwDsgn = new prl_designation();
                                        nwDsgn.name = dsgn;
                                        contxt.prl_designation.Add(nwDsgn);
                                        contxt.SaveChanges();
                                        empDetails.designation_id = nwDsgn.id;
                                        lstDesignation = contxt.prl_designation.ToList();
                                    }

                                }
                               
                               // emp.prl_employee_details.Add(empDetails);
                                
                                dictEmployees.Add(emp.emp_no, emp);
                                dictEmployeeDetails.Add(emp.emp_no, empDetails);
                            }
                            result.ListEmployeesWithHash = dictEmployees;
                            result.ListEmployeeDetailsWithHash = dictEmployeeDetails;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public  DataImportResult SaveData(DataImportModel importedData)
        {
            MySqlCommand mySqlCommand = null;
            MySqlConnection mySqlConnection = null;
            var result = new DataImportResult();
            args.TotalData = importedData.ListEmployeesWithHash.Count;
            try
            {
                using (var cntx = new payroll_systemContext())
                {
                    var objectContext = ((IObjectContextAdapter) cntx).ObjectContext;
                    objectContext.Connection.Open();

                    mySqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["payroll_systemContext"].ToString());
                    mySqlCommand =new MySqlCommand();
                    mySqlCommand.Connection = mySqlConnection;
                    mySqlConnection.Open();
                    //using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                    //{
                    cntx.Configuration.AutoDetectChangesEnabled = false;

                    //1. read existing employees
                    var extEmp = cntx.prl_employee.ToList();

                    // 2. read existing emp details 
                    var extEmpDetails = cntx.prl_employee_details.AsEnumerable()
                            .GroupBy(x => x.emp_id, (key, xs) => xs.OrderByDescending(x => x.id).First())
                            .ToList();

                    foreach (var k in importedData.ListEmployeesWithHash)
                    {
                        // get imported employee object
                        var imprtEmp = k.Value;
                        //find if employee exists in db list
                        var extingEmp = extEmp.FirstOrDefault(x => x.emp_no == imprtEmp.emp_no);
                        

                        #region  existing employee update

                        //employee exists 
                        if (extingEmp != null)
                        {
                            if (!HasSameValues(imprtEmp, extingEmp))
                            {
                                UpdateEmployeeObject(imprtEmp,mySqlCommand,result);
                            }

                            var extingEmpDetl = extEmpDetails.SingleOrDefault(x => x.emp_id == extingEmp.id);
                            if (extingEmpDetl == null && importedData.ListEmployeeDetailsWithHash.ContainsKey(k.Key))
                            {
                                var temp = importedData.ListEmployeeDetailsWithHash[k.Key];
                                temp.emp_id = extingEmp.id;
                                SaveEmployeeDetailObject(importedData.ListEmployeeDetailsWithHash[k.Key], mySqlCommand,result);
                            }
                            else if (extingEmpDetl != null && importedData.ListEmployeeDetailsWithHash.ContainsKey(k.Key))
                            {
                                var temp = importedData.ListEmployeeDetailsWithHash[k.Key];
                                temp.emp_id = extingEmp.id;
                                if (!HasSameValues(extingEmpDetl, temp))
                                {
                                    SaveEmployeeDetailObject(temp,mySqlCommand,result);
                                }
                            }
                        } 
                        #endregion
                            //employee is new so insert everything
                        else
                        {
                            int empid = SaveEmployeeOject(imprtEmp,mySqlCommand,result);
                            Trace.WriteLine("--saved new emp -- " + imprtEmp.emp_no);
                            if (empid > 0)
                            {
                                var d = importedData.ListEmployeeDetailsWithHash[k.Key];
                                d.emp_id = empid;
                                int empdetid = SaveEmployeeDetailObject(d, mySqlCommand,result);
                                Trace.WriteLine("--saved new emp details -- " + d.emp_id);
                            }
                        }
                    }
                    // ts.Complete();
                    //}//transaction end

                } //end of using 
            }
            catch (DbEntityValidationException e)
            {
                string s = "";

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        s = s + ve.PropertyName + " " + ve.ErrorMessage;
                    }
                }

                var m = e.Message;
               
            }
            catch (Exception excep)
            {
                var m = excep.Message;
                
            }
            finally
            {
                if (mySqlConnection != null)
                {
                    mySqlConnection.Close();
                    mySqlConnection = null;
                }
            }
            args.IsProcessingComplete = true;
            OnDataProcessed(args);
            return result;
        }
        private bool HasSameValues(prl_employee a, prl_employee b)
        {
                if(a.emp_no == b.emp_no &&
                a.name  == b.name &&
                a.phone == b.phone &&
                a.email == b.email &&
                a.religion_id == b.religion_id &&
                a.gender == b.gender &&
                a.marital_status == b.marital_status &&
                a.company_id == b.company_id &&
                a.father_name == b.father_name &&
                a.bank_id == b.bank_id &&
                a.bank_branch_id == b.bank_branch_id &&
                a.account_no == b.account_no &&
                a.dob == b.dob &&
                a.joining_date ==b.joining_date &&
                a.tin == b.tin &&
                a.confirmation_date ==b.confirmation_date &&
                a.termination_date == b.termination_date) return true;
            return false;
        }
        private bool HasSameValues(prl_employee_details a, prl_employee_details b)
        {
            if (a.emp_status == b.emp_status &&
                a.division_id == b.division_id &&
                a.department_id == b.department_id &&
                a.grade_id == b.grade_id &&
                a.basic_salary == b.basic_salary &&
                a.posting_location_id == b.posting_location_id &&
                a.designation_id == b.designation_id) return true;

            return false;
        }
        private int SaveEmployeeOject(prl_employee newEmp,MySqlCommand command,DataImportResult operationResult)
        {
            try
            {
                const string commandText = @"INSERT INTO prl_employee (emp_no, NAME,present_address,permanent_address, phone, 
	                            email, 	religion_id, gender, marital_status, company_id, father_name, mother_name, 	bank_id, 
	                            bank_branch_id, account_no, payment_mode, dob, joining_date, tin, confirmation_date, is_confirmed, 
	                            is_pf_member, is_gf_member, termination_date, picture, is_active, created_by, created_date, 
	                            updated_by, updated_date)
	                            VALUES	(?emp_no, ?name, ?present_address, ?permanent_address, ?phone, ?email, 
	                            ?religion_id, 	?gender,?marital_status,?company_id,?father_name,?mother_name, 
	                            ?bank_id, ?bank_branch_id,?account_no,?payment_mode,?dob,?joining_date, 
	                            ?tin,?confirmation_date, ?is_confirmed, ?is_pf_member, ?is_gf_member,?termination_date, ?picture, 
                                ?is_active, ?created_by, ?created_date, ?updated_by,?updated_date);";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?emp_no", newEmp.emp_no);
                command.Parameters.AddWithValue("?name", newEmp.name);
                command.Parameters.AddWithValue("?present_address", newEmp.present_address);
                command.Parameters.AddWithValue("?permanent_address", newEmp.permanent_address);
                command.Parameters.AddWithValue("?phone", newEmp.phone);
                command.Parameters.AddWithValue("?email", newEmp.email);
                command.Parameters.AddWithValue("?religion_id", newEmp.religion_id);
                command.Parameters.AddWithValue("?gender", newEmp.gender);
                command.Parameters.AddWithValue("?marital_status", newEmp.marital_status);
                command.Parameters.AddWithValue("?company_id", newEmp.company_id);
                command.Parameters.AddWithValue("?father_name", newEmp.father_name);
                command.Parameters.AddWithValue("?mother_name", newEmp.mother_name);
                command.Parameters.AddWithValue("?bank_id", newEmp.bank_id);
                command.Parameters.AddWithValue("?bank_branch_id", newEmp.bank_branch_id);
                command.Parameters.AddWithValue("?account_no", newEmp.account_no);
                command.Parameters.AddWithValue("?payment_mode", newEmp.payment_mode);
                command.Parameters.AddWithValue("?dob", newEmp.dob);
                command.Parameters.AddWithValue("?joining_date", newEmp.joining_date);
                command.Parameters.AddWithValue("?tin", newEmp.tin);
                command.Parameters.AddWithValue("?confirmation_date", newEmp.confirmation_date);
                command.Parameters.AddWithValue("?is_confirmed", newEmp.is_confirmed);
                command.Parameters.AddWithValue("?is_pf_member", newEmp.is_pf_member);
                command.Parameters.AddWithValue("?is_gf_member", newEmp.is_gf_member);
                command.Parameters.AddWithValue("?termination_date", newEmp.termination_date);
                command.Parameters.AddWithValue("?picture", newEmp.picture);
                command.Parameters.AddWithValue("?is_active", 1);
                command.Parameters.AddWithValue("?created_by", newEmp.created_by);
                command.Parameters.AddWithValue("?created_date", DateTime.Now);
                command.Parameters.AddWithValue("?updated_by", newEmp.updated_by);
                command.Parameters.AddWithValue("?updated_date", newEmp.updated_date);

                command.ExecuteNonQuery();
                var k = command.LastInsertedId;
                if (k != null)
                {
                    ulong qkwl = (ulong)k;
                    int Id = (int)qkwl;
                    if (Id > 0)
                    {
                        args.NumberOfDataProcessed = args.NumberOfDataProcessed + 1;
                        OnDataProcessed(args);
                        return Id;
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("--Error in new emp save action -- "+ex.Message);
                operationResult.HasError = true;
                operationResult.Messages.Add("Could not save employee number "+newEmp.emp_no);
            }
            return 0;

        }
        private int SaveEmployeeDetailObject(prl_employee_details newEmpDet, MySqlCommand command, DataImportResult operationResult)
        {
            try
            {
                const string commandText = @"INSERT INTO prl_employee_details (emp_id, emp_status, grade_id, 	division_id, 
	                            department_id, 	basic_salary,designation_id,is_hold,posting_location_id, posting_date, contract_start_date, contract_end_date, 
	                            created_by, created_date, updated_by, updated_date)
	                            VALUES
	                            (?emp_id, ?emp_status,?grade_id,?division_id,?department_id, ?basic_salary, ?designation_id,?is_hold,?posting_location_id,
                                ?posting_date, ?contract_start_date, ?contract_end_date, ?created_by, ?created_date, ?updated_by,?updated_date);";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?emp_id", newEmpDet.emp_id);
                command.Parameters.AddWithValue("?emp_status", newEmpDet.emp_status);
                command.Parameters.AddWithValue("?grade_id", newEmpDet.grade_id);
                command.Parameters.AddWithValue("?division_id", newEmpDet.division_id);
                command.Parameters.AddWithValue("?department_id", newEmpDet.department_id);
                command.Parameters.AddWithValue("?basic_salary", newEmpDet.basic_salary);
                command.Parameters.AddWithValue("?designation_id", newEmpDet.designation_id);
                command.Parameters.AddWithValue("?is_hold", newEmpDet.is_hold);
                command.Parameters.AddWithValue("?posting_location_id", newEmpDet.posting_location_id);
                command.Parameters.AddWithValue("?posting_date", newEmpDet.posting_date);
                command.Parameters.AddWithValue("?contract_start_date", newEmpDet.contract_start_date);
                command.Parameters.AddWithValue("?contract_end_date", newEmpDet.contract_end_date);
                command.Parameters.AddWithValue("?created_by", newEmpDet.created_by);
                command.Parameters.AddWithValue("?created_date", DateTime.Now);
                command.Parameters.AddWithValue("?updated_by", newEmpDet.updated_by);
                command.Parameters.AddWithValue("?updated_date", newEmpDet.updated_date);

                command.ExecuteNonQuery();
                var k = command.LastInsertedId;
                if (k != null)
                {
                    ulong qkwl = (ulong)k;
                    int Id = (int)qkwl;
                    if (Id > 0)
                    {
                        args.NumberOfDataProcessed = args.NumberOfDataProcessed + 1;
                        OnDataProcessed(args); 
                        return Id;
                    }
                        
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("--Error in new employee details save action"+ ex.Message);
                operationResult.HasError = true;
                operationResult.Messages.Add("Could not save employee details with employee ID " + newEmpDet.emp_id);
            }
            return 0;
        }
        private void UpdateEmployeeObject(prl_employee emp, MySqlCommand command,DataImportResult operationResult)
        {
            try
            {
                const string commandText = @"UPDATE prl_employee 	SET
	                            emp_no = ?emp_no , NAME = ?name , present_address = ?present_address , 
	                            permanent_address = ?permanent_address , 	phone = ?phone , 	email = ?email , 	
                                religion_id = ?religion_id , gender = ?gender , marital_status = ?marital_status , 
	                            company_id = ?company_id , father_name = ?father_name , mother_name = ?mother_name , 
	                            bank_id = ?bank_id , bank_branch_id = ?bank_branch_id , account_no = ?account_no , 
                            	payment_mode = ?payment_mode , dob = ?dob , joining_date = ?joining_date , tin = ?tin , 
                                confirmation_date = ?confirmation_date , is_confirmed = ?is_confirmed , 
                                is_pf_member = ?is_pf_member , is_gf_member = ?is_gf_member , 
	                            termination_date = ?termination_date , picture = ?picture , is_active = ?is_active , 
                                created_by = ?created_by , created_date = ?created_date , updated_by = ?updated_by , 
	                            updated_date = ?updated_date WHERE emp_no = ?emp_no;";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?emp_no", emp.emp_no);
                command.Parameters.AddWithValue("?name", emp.name);
                command.Parameters.AddWithValue("?present_address", emp.present_address);
                command.Parameters.AddWithValue("?permanent_address", emp.permanent_address);
                command.Parameters.AddWithValue("?phone", emp.phone);
                command.Parameters.AddWithValue("?email", emp.email);
                command.Parameters.AddWithValue("?religion_id", emp.religion_id);
                command.Parameters.AddWithValue("?gender", emp.gender);
                command.Parameters.AddWithValue("?marital_status", emp.marital_status);
                command.Parameters.AddWithValue("?company_id", emp.company_id);
                command.Parameters.AddWithValue("?father_name", emp.father_name);
                command.Parameters.AddWithValue("?mother_name", emp.mother_name);
                command.Parameters.AddWithValue("?bank_id", emp.bank_id);
                command.Parameters.AddWithValue("?bank_branch_id", emp.bank_branch_id);
                command.Parameters.AddWithValue("?account_no", emp.account_no);
                command.Parameters.AddWithValue("?payment_mode", emp.payment_mode);
                command.Parameters.AddWithValue("?dob", emp.dob);
                command.Parameters.AddWithValue("?joining_date", emp.joining_date);
                command.Parameters.AddWithValue("?tin", emp.tin);
                command.Parameters.AddWithValue("?confirmation_date", emp.confirmation_date);
                command.Parameters.AddWithValue("?is_confirmed", emp.is_confirmed);
                command.Parameters.AddWithValue("?is_pf_member", emp.is_pf_member);
                command.Parameters.AddWithValue("?is_gf_member", emp.is_gf_member);
                command.Parameters.AddWithValue("?termination_date", emp.termination_date);
                command.Parameters.AddWithValue("?picture", emp.picture);
                command.Parameters.AddWithValue("?is_active", emp.is_active);
                command.Parameters.AddWithValue("?created_by", emp.created_by);
                command.Parameters.AddWithValue("?created_date", emp.created_date);
                command.Parameters.AddWithValue("?updated_by", emp.updated_by);
                command.Parameters.AddWithValue("?updated_date", DateTime.Now);
                command.ExecuteNonQuery();
                args.NumberOfDataProcessed = args.NumberOfDataProcessed + 1;
                OnDataProcessed(args); 

            }
            catch (Exception ex)
            {
                Trace.WriteLine("--Error in employee update action--"+ ex.Message);
                operationResult.HasError = true;
                operationResult.Messages.Add("Could not update employee number " + emp.emp_no);
                
            }
        }
        private void UpdateEmployeeDetailsObject(prl_employee_details empDetails,MySqlCommand command,DataImportResult operationResult)
        {
            try
            {
                const string commandText = @"UPDATE prl_employee_details SET
	                            emp_status = ?emp_status ,  grade_id = ?grade_id , division_id = ?division_id , 
	                            department_id = ?department_id , basic_salary = ?basic_salary , 
	                            designation_id = ?designation_id ,  is_hold = ?is_hold , 
	                            posting_location_id = ?posting_location_id ,  posting_date = ?posting_date , 
	                            contract_start_date = ?contract_start_date ,  contract_end_date = ?contract_end_date , 
	                            created_by = ?created_by , created_date = ?created_date ,  updated_by = ?updated_by , 
	                            updated_date = ?updated_date
	                            WHERE
	                            id = ?id ;";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?id", empDetails.id);
                command.Parameters.AddWithValue("?emp_status", empDetails.emp_status);
                command.Parameters.AddWithValue("?grade_id", empDetails.grade_id);
                command.Parameters.AddWithValue("?division_id", empDetails.division_id);
                command.Parameters.AddWithValue("?department_id", empDetails.department_id);
                command.Parameters.AddWithValue("?basic_salary", empDetails.basic_salary);
                command.Parameters.AddWithValue("?designation_id", empDetails.designation_id);
                command.Parameters.AddWithValue("?is_hold", empDetails.is_hold);
                command.Parameters.AddWithValue("?posting_location_id", empDetails.posting_location_id);
                command.Parameters.AddWithValue("?posting_date", empDetails.posting_date);
                command.Parameters.AddWithValue("?contract_start_date", empDetails.contract_start_date);
                command.Parameters.AddWithValue("?contract_end_date", empDetails.contract_end_date);
                command.Parameters.AddWithValue("?created_by", empDetails.created_by);
                command.Parameters.AddWithValue("?created_date", empDetails.created_date);
                command.Parameters.AddWithValue("?updated_by", empDetails.updated_by);
                command.Parameters.AddWithValue("?updated_date", DateTime.Now);
                command.ExecuteNonQuery();
                args.NumberOfDataProcessed = args.NumberOfDataProcessed + 1;
                OnDataProcessed(args); 
            }
            catch (Exception ex)
            {
                Trace.WriteLine("--Error in employee update action--"+ex.Message);
                operationResult.HasError = true;
                operationResult.Messages.Add("Could not update employee details with ID " + empDetails.emp_id);
            }
        }

        private int SaveEmployeeSalaryReviewOject(prl_employee_details newEmpDet, MySqlCommand command, DataImportResult operationResult)
        {
            try
            {
                const string commandText = @"INSERT INTO prl_salary_review
                                                    (emp_id,current_basic,new_basic,increment_reason,description,effective_from,
                                                     is_arrear_calculated,created_by,created_date)
                                                    VALUES (?emp_id,?current_basic,?new_basic,?increment_reason,?description,
                                                            ?effective_from,?is_arrear_calculated,?created_by,?created_date);";
                command.Parameters.Clear();
                command.CommandText = commandText;
                command.Parameters.AddWithValue("?emp_id", newEmpDet.id);
                command.Parameters.AddWithValue("?current_basic", newEmpDet.basic_salary);
                command.Parameters.AddWithValue("?new_basic", newEmpDet.basic_salary);
                command.Parameters.AddWithValue("?increment_reason", "Data Pull");
                command.Parameters.AddWithValue("?description", "Data Pull");
                command.Parameters.AddWithValue("?effective_from", "");
                command.Parameters.AddWithValue("?is_arrear_calculated", "No");
                command.Parameters.AddWithValue("?created_by", "");
                command.Parameters.AddWithValue("?created_date", DateTime.Now);

                command.ExecuteNonQuery();
                var k = command.LastInsertedId;
                if (k != null)
                {
                    ulong qkwl = (ulong)k;
                    int Id = (int)qkwl;
                    if (Id > 0)
                    {
                        args.NumberOfDataProcessed = args.NumberOfDataProcessed + 1;
                        OnDataProcessed(args);
                        return Id;
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("--Error in new emp save action -- " + ex.Message);
                operationResult.HasError = true;
                operationResult.Messages.Add("Could not save employee number " + newEmpDet.id);
            }
            return 0;

        }

        protected virtual void OnDataProcessed(DataImportProcessedEventArgs e)
        {
            EventHandler<DataImportProcessedEventArgs> handler = DataImportProcessed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
    public class DataImportResult
    {
        public DataImportResult()
        {
            Messages = new List<string>();
        }
        public bool HasError { get; set; }
        public List<string> Messages { get; set; }
    }

    public class DataImportProcessedEventArgs : EventArgs
    {
        public int TotalData { get; set; }
        public int NumberOfDataProcessed { get; set; }
        public bool IsProcessingComplete { get; set; }
    }
    
}