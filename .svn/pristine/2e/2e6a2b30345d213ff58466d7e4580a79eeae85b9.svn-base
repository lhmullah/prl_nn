using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class TaxChallanUploadView
    {
        [Required(ErrorMessage = "Year can not be null.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month can not be null.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Name can not be null.")]
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