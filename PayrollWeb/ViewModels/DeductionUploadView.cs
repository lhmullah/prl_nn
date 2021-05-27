using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PayrollWeb.Utility;
using System.ComponentModel;

namespace PayrollWeb.ViewModels
{
    public class DeductionUploadView
    {
        public int empid { get; set; }

        [Required(ErrorMessage = "Year can not be null.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month can not be null.")]
        public int Month { get; set; }

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

        [Required(ErrorMessage = "Amount can not be null.")]
        [Range(0.5, int.MaxValue, ErrorMessage = "Value should be greater than or equal {1}")]
        [DisplayName("Amount/Value")]
        public decimal amount { get; set; }

        public string remarks { get; set; }

    }
}