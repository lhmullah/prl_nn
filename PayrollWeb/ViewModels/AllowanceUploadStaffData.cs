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
    public class AllowanceUploadStaffData
    {
        public AllowanceUploadStaffData()
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

        [Required(ErrorMessage = "Salary month year can not be null or empty")]
        [DisplayName("Employee ID")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> salary_month_year { get; set; }

        [Required(ErrorMessage = "No of allowance can not be empty.")]
        [DisplayName("No of count")]
        public Nullable<decimal> no_of_entry { get; set; }

        public Nullable<decimal> amount_or_percentage { get; set; }

        public virtual prl_allowance_name prl_allowance_name { get; set; }
        public virtual prl_employee prl_employee { get; set; }

    }
}