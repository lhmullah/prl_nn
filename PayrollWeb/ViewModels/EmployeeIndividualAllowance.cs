using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class EmployeeIndividualAllowance
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int emp_id { get; set; }

        [DisplayName("Allowance Name")]
        [Required(ErrorMessage = "Allowance Name cannot be empty.")]
        public int allowance_name_id { get; set; }

        [DisplayName("Flat Amount")]
        public Nullable<decimal> flat_amount { get; set; }

        [DisplayName("Percentage Amount")]
        public Nullable<decimal> percentage { get; set; }

        [DisplayName("Effective From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> effective_from { get; set; }

        [DisplayName("Effective To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> effective_to { get; set; }
        
        public virtual AllowanceName prl_allowance_name { get; set; }
        public virtual Employee prl_employee { get; set; }
    }
}