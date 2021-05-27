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
    public class ChildrenAllowance
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [HiddenInput(DisplayValue = true)]
        [DisplayName("Employee Name")]
        public int emp_id { get; set; }

        [DisplayName("No. of Children")]
        [Required(ErrorMessage = "No. of Children cannot be empty.")]
        public int no_of_children { get; set; }

        [DisplayName("Amount")]
        [Required(ErrorMessage = "Amount cannot be empty.")]
        public decimal amount { get; set; }

        [DisplayName("Effective From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> effective_from { get; set; }

        public Nullable<sbyte> is_active { get; set; }
        
        public Employee prl_employee { get; set; }
    }
}