using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PayrollWeb.Utility;
using System.ComponentModel;

namespace PayrollWeb.ViewModels
{
    public class AllowanceUploadView
    {
        public int empid { get; set; }

        [Required(ErrorMessage = "Year can not be null.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month can not be null.")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Name can not be null.")]
        public int AllowanceName { get; set; }

        public List<AllowanceName> AllowanceNames { get; set; }

        public Dictionary<string, int> GetMonths()
        {
            return DateUtility.GetMonths();
        }

        public List<int> GetYears()
        {
            return DateUtility.GetYears();
        }

        public string remarks { get; set; }

        [Required(ErrorMessage = "Amount can not be null.")]
        [Range(1, int.MaxValue, ErrorMessage = "Salary should be greater than {1}")]
        [DisplayName("Amount/Value")]
        public decimal amount { get; set; }
    }
}