using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class EmployeeTaxProcess
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public int salary_process_id { get; set; }
        public int fiscal_year_id { get; set; }
        public System.DateTime salary_month { get; set; }
        public Nullable<decimal> yearly_tax { get; set; }
        public decimal monthly_tax { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Employee prl_employee { get; set; }
        public FiscalYr prl_fiscal_year { get; set; }
        //public virtual prl_salary_process prl_salary_process { get; set; }
    }
}