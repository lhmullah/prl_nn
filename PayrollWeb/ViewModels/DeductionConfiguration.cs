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
    public class DeductionConfiguration
    {

        public DeductionConfiguration()
        {
           prl_deduction_name= new DeductionName();
           Grades = new List<Grade>();
        }
       
        public DeductionName prl_deduction_name { get; set; }


        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Deduction Name")]
        public int deduction_name_id { get; set; }

        
        [DisplayName("Monthly")]
        public bool is_monthly { get; set; }


        [DisplayName("Active")]
        public bool is_active { get; set; }

        [DisplayName("Taxable")]
        public bool is_taxable { get; set; }

       
        [DisplayName("Is Individual")]
        public bool is_individual { get; set; }

        
        [DisplayName("Gender")]
        public string gender { get; set; }

       
        [DisplayName("Confirmation Required")]
        public bool is_confirmation_required { get; set; }

        
        [DisplayName("Depends On Working Hour")]
        public bool depends_on_working_hour { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Activation Date")]
        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Enter a valid date.")]
        public Nullable<System.DateTime> activation_date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Deactivation Date")]
        public Nullable<System.DateTime> deactivation_date { get; set; }

        [DisplayName("Project Rest of the Year")]
        public bool project_rest_year { get; set; }

        [DisplayName("Once Off Deduction")]
        public decimal? once_off_deduction { get; set; }


        [DisplayName("Fixed Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> flat_amount { get; set; }


        [DisplayName("Percentage of Basic Salary")]
        public Nullable<decimal> percent_amount { get; set; }


        [DisplayName("Max Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> max_amount { get; set; }

        [NumericLessThan("max_amount", AllowEquality = true)]
        [DisplayName("Min Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> min_amount { get; set; }

        public IEnumerable<Grade> Grades { get; set; }

    }
}