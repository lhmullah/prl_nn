using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PayrollWeb.Utility;
using System.ComponentModel;

namespace PayrollWeb.ViewModels
{
    public class TaxChallanUploadView
    {
        [Required(ErrorMessage = "Year can not be null.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month can not be null.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Fiscal Year can not be null or empty")]
        [DisplayName("Fiscal Year")]
        public int FiscalYear { get; set; }

        public List<FiscalYr> FiscalYears { get; set; }

        public Dictionary<string, int> GetMonths()
        {
            return DateUtility.GetMonths();
        }

        public List<int> GetYears()
        {
            return DateUtility.GetYears();
        }
    }
}