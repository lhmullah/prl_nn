using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class DeductionUploadView
    {
        [Required(ErrorMessage = "Year can not be null.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month can not be null.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Name can not be null.")]
        public int DeductionName { get; set; }

        public List<DeductionName> DeductionNames { get; set; }

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