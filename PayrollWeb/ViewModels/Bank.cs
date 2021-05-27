using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Bank
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Bank Name cannot be empty.")]
        [Display(Name = "Bank Name")]
        public string bank_name { get; set; }
        [Display(Name = "Bank Code")]
        public string bank_code { get; set; }
    }
}