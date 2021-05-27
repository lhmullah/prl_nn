using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class EmployeeYearlyInvestment
    {
        public int id { get; set; }
        public int empId { get; set; }
        public string empNo { get; set; }
        public string empName { get; set; }

        public int fiscal_year_id { get; set; }

        public Nullable<decimal> invested_amount { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }

        public Employee prl_employee { get; set; }
        public FiscalYr prl_fiscal_year { get; set; }
    }
}