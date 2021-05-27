using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class TaxChallanUpload
    {
        public TaxChallanUpload()
        {
            ErrorMsg = new List<string>();
        }

        public string EmployeeID { get; set; }
        public string FiscalYearNameString { get; set; }
        public List<string> ErrorMsg { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [HiddenInput(DisplayValue = true)]
        public Nullable<int> company_id { get; set; }

        [Required(ErrorMessage = "Employee ID can not be null or empty")]
        [DisplayName("Employee ID")]
        public Nullable<int> emp_id { get; set; }

        [Required(ErrorMessage = "Fiscal Year can not be null or empty")]
        [DisplayName("Fiscal Year")]
        public Nullable<int> fiscal_year_id { get; set; }

        [Required(ErrorMessage = "Fiscal Year can not be null or empty")]
        [DisplayName("Challan No.")]
        public string challan_no { get; set; }

        [Required(ErrorMessage = "Fiscal Year can not be null or empty")]
        [DisplayName("Amount")]
        public Nullable<decimal> amount { get; set; }

        [Required(ErrorMessage = "Challan month year can not be null or empty")]
        [DisplayName("Challan Date")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> challan_date { get; set; }

        public virtual FiscalYr prl_fiscal_year { get; set; }
    }
}