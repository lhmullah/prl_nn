using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class AllowanceHead
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Allowance Head Name cannot be empty.")]
        [Display(Name = "Allowance Head Name")]
        public string name { get; set; }
    }
}