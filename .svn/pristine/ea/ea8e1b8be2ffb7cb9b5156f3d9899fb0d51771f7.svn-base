using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class DeductionUploadData
    {
        public DeductionUploadData()
        {
            ErrorMsg = new List<string>();
            
        }
        public string EmployeeID { get; set; }
        public string DeductionNameString { get; set; }

        
        public List<string> ErrorMsg { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Employee ID can not be null or empty")]
        [DisplayName("Employee ID")]
        public int deduction_name_id { get; set; }

        [Required(ErrorMessage = "Employee ID can not be null or empty")]
        [DisplayName("Employee ID")]
        public int emp_id { get; set; }

        [Required(ErrorMessage = "Salary month year can not be null or empty")]
        [DisplayName("Employee ID")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> salary_month_year { get; set; }

        [Required(ErrorMessage = "Amount can not empty")]
        [DataType(DataType.Currency)]
        public decimal amount { get; set; }
       
        public Nullable<System.DateTime> effective_from { get; set; }
        public Nullable<System.DateTime> effective_to { get; set; }
        
        public virtual DeductionName prl_deduction_name { get; set; }
        public virtual Employee prl_employee { get; set; }
    }
}