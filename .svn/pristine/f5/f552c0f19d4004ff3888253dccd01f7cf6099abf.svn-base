using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class TaxAdjustmentUpload
    {
        public TaxAdjustmentUpload()
        {
            ErrorMsg = new List<string>();
        }

        public string EmployeeID { get; set; }
        public string FiscalYearNameString { get; set; }
        public List<string> ErrorMsg { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Employee ID can not be null or empty")]
        [DisplayName("Employee ID")]
        public int emp_id { get; set; }

        [Required(ErrorMessage = "Fiscal Year can not be null or empty")]
        [DisplayName("Fiscal Year")]
        public int fiscal_year { get; set; }

        [Required(ErrorMessage = "Refund month year can not be null or empty")]
        [DisplayName("Month Year")]
        [DataType(DataType.Date)]
        public System.DateTime month_year { get; set; }

        [Required(ErrorMessage = "Amount can not empty")]
        [DataType(DataType.Currency)]
        public decimal adjustment_amount { get; set; }

        public virtual FiscalYr prl_fiscal_year { get; set; }
    }
}