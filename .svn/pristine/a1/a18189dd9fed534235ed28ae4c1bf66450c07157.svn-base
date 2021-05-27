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
    public class BonusConfiguration
    {
        public BonusConfiguration()
        {
           prl_bonus_name = new BonusName();
           Grades = new List<Grade>();
        }

        public BonusName prl_bonus_name { get; set; }

        public IEnumerable<Grade> Grades { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Bonus Name")]
        public int bonus_name_id { get; set; }

        [DisplayName("Confirmation Required")]
        public string confirmed_emp { get; set; }

        [DisplayName("Is Festival")]
        public string is_festival { get; set; }

        [DisplayName("Gender")]
        public string gender_dependant { get; set; }

        [DisplayName("Taxable")]
        public string is_taxable { get; set; }

        [DisplayName("Fixed Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> flat_amount { get; set; }

        [DisplayName("(%) of Basic Salary")]
        public Nullable<decimal> percentage_of_basic { get; set; }

        [DisplayName("No. of Basic(s)")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> number_of_basic { get; set; }

        [DisplayName("Basic of Days")]
        public Nullable<int> basic_of_days { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
               ApplyFormatInEditMode = true)]
        [DisplayName("Effective From")]
        public Nullable<System.DateTime> effective_from { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
               ApplyFormatInEditMode = true)]
        [DisplayName("Effective To")]
        public Nullable<System.DateTime> effective_to { get; set; }
    }
}