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
    public class AllowanceConfiguration
    {
        public AllowanceConfiguration()
        {
           prl_allowance_name = new AllowanceName();
           Grades = new List<Grade>();
        }

        public AllowanceName prl_allowance_name { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Allownce Name")]
        public int allowance_name_id { get; set; }

        [DisplayName("Active")]
        public bool is_active { get; set; }

        [DisplayName("Monthly")]
        public bool is_monthly { get; set; }

        [DisplayName("Taxable")]
        public bool is_taxable { get; set; }

        [DisplayName("Is Individual")]
        public bool is_individual { get; set; }

        [DisplayName("Gender")]
        public string gender { get; set; }

        [DisplayName("Confirmation Required")]
        public bool is_confirmation_required { get; set; }

        [DisplayName("Depends on Working Hour")]
        public bool depends_on_working_hour { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Activation Date")]
        public Nullable<System.DateTime> activation_date { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("De-Activation Date")]
        public Nullable<System.DateTime> deactivation_date { get; set; }

        [DisplayName("Project Rest Of The Year")]
        public bool project_rest_year { get; set; }

        [DisplayName("Once Off Deduction")]
        public Nullable<decimal> once_off_deduction { get; set; }

        [DisplayName("Fixed Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> flat_amount { get; set; }

        [DisplayName("Percentage of Basic Salary")]
        public Nullable<decimal> percent_amount { get; set; }

        [DisplayName("Max. AMount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> max_amount { get; set; }

        [NumericLessThan("max_amount", AllowEquality = true)]
        [DisplayName("Min. Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> min_amount { get; set; }

        [DisplayName("Exempted Amount")]
        public Nullable<decimal> exempted_amount { get; set; }

        [DisplayName("(%) of Exempted Amount")]
        public Nullable<decimal> exempted_percentage { get; set; }

        public IEnumerable<Grade> Grades { get; set; }
    }
}