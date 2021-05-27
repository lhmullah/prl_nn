using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class BonusUploadView
    {
        [Required(ErrorMessage = "Fiscal year can not be null.")]
        public int FiscalYear { get; set; }

        [Required(ErrorMessage = "Month can not be null.")]
        public string MonthYear { get; set; }

        [Required(ErrorMessage = "Year can not be null.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Name can not be null.")]
        public int BonusName { get; set; }

        public List<BonusName> BonusNames { get; set; }

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