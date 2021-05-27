using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class CostCentre
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [DisplayName("Cost Centre Name")]
        [Required(ErrorMessage = "Name can not be empty.")]
        public string cost_centre_name { get; set; }
    }
}