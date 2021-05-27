using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class AllowanceStaff
    {
        public AllowanceStaff()
        {
           prl_allowance_name = new AllowanceName();
           Grades = new Grade();
        }

        public AllowanceName prl_allowance_name { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Allownce Name")]
        public int allowance_name_id { get; set; }

        [DisplayName("Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> amount { get; set; }

        [DisplayName("Percentage of Basic")]
        public Nullable<decimal> percent_amount { get; set; }

        public Grade Grades { get; set; }
    }
}