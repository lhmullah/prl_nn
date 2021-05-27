using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class EmployeeDiscontinue
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public string is_active { get; set; }
        public string status { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime discontinue_date { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> continution_date { get; set; }
        public string discontination_type { get; set; }
        public string with_salary { get; set; }
        public string without_salary { get; set; }
        [DisplayName("Remarks")]
        public string remarks { get; set; }

        public string empInfo { get; set; }
        public bool discontinueAfterCurrentMonth { get; set; }
    }
}