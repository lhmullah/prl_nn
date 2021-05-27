using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Grade
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Name can not be empty.")]
        [DisplayName("Grade Name")]
        public string grade { get; set; }

        public Nullable<decimal> upper_basic { get; set; }
        public Nullable<decimal> lower_basic { get; set; }

        public bool IsSelected { get; set; }
    }
}