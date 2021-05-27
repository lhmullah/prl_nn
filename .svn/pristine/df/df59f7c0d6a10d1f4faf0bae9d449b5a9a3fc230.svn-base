using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Designation
    {
        [HiddenInput(DisplayValue=true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Name can not be empty.")]
        [DisplayName("Designation Name")]
        public string name { get; set; }
    }
}