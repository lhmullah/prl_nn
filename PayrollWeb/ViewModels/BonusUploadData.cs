using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class BonusUploadData
    {
        public BonusUploadData()
        {
            ErrorMsg = new List<string>();
        }

        public string EmployeeID { get; set; }
        public string BonusNameString { get; set; }
        public List<string> ErrorMsg { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Bonus Name can not be null or empty")]
        [DisplayName("Bonus Name")]
        public int bonus_name_id { get; set; }

        [Required(ErrorMessage = "Employee ID can not be null or empty")]
        [DisplayName("Employee ID")]
        public int emp_id { get; set; }

        [DisplayName("Fiscal Year")]
        public Nullable<int> fiscal_year { get; set; }

        [Required(ErrorMessage = "Salary month year can not be null or empty")]
        [DisplayName("Employee ID")]
        [DataType(DataType.Date)]
        public System.DateTime month_year { get; set; }

        public string is_percentage { get; set; }

        [Required(ErrorMessage = "Amount can not empty")]
        [DataType(DataType.Currency)]
        public decimal amount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> effective_from { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> effective_to { get; set; }
                
        public virtual BonusName prl_bonus_name { get; set; }
    }
}