using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Division
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [DisplayName("Division")]
        [Required(ErrorMessage = "Name can not be empty.")]
        public string name { get; set; }

    }
}