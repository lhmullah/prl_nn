using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class FiscalYr
    {
        [HiddenInput(DisplayValue=true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Year can not be empty.")]
        [DisplayName("Fiscal Year")]
        public string fiscal_year { get; set; }

        [Required(ErrorMessage = "Assesment Year can not be empty.")]
        [DisplayName("Assesment Year")]
        public string assesment_year { get; set; }
    }
}