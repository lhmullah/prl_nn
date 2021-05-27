using com.linde.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class AllowanceUploadData
    {
        public AllowanceUploadData()
        {
            ErrorMsg = new List<string>();
        }

        public List<string> ErrorMsg { get; set; }
        public string EmployeeID { get; set; }
        public string AllowanceNameString { get; set; }
        
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Allowance Name can not be null or empty")]
        [DisplayName("Allowance Name")]
        public int allowance_name_id { get; set; }

        [Required(ErrorMessage = "Employee ID can not be null or empty")]
        [DisplayName("Employee ID")]
        public int emp_id { get; set; }

        [Required(ErrorMessage = "ID. can not be empty.")]
        [DisplayName("Employee ID.")]
        public string emp_no { get; set; }

        [Required(ErrorMessage = "Name can not be empty.")]
        [DisplayName("Employee Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Salary month year can not be null or empty")]
        [DisplayName("Salary Month")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> salary_month_year { get; set; }

        [Required(ErrorMessage = "Amount can not empty")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> amount { get; set; }


        public Nullable<decimal> percentage { get; set; }
        public Nullable<System.DateTime> effective_from { get; set; }
        public Nullable<System.DateTime> effective_to { get; set; }
        public string remarks { get; set; }

        public virtual AllowanceName prl_allowance_name { get; set; }
        public virtual prl_employee prl_employee { get; set; }
    }
}