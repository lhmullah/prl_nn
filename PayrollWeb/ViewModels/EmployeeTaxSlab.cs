using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class EmployeeTaxSlab
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public Nullable<int> tax_process_id { get; set; }
        public Nullable<int> fiscal_year_id { get; set; }
        public Nullable<System.DateTime> salary_date { get; set; }
        public Nullable<int> salary_month { get; set; }
        public Nullable<int> salary_year { get; set; }
        public Nullable<decimal> current_rate { get; set; }
        public string parameter { get; set; }
        public Nullable<decimal> taxable_income { get; set; }
        public Nullable<decimal> tax_liability { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    }
}