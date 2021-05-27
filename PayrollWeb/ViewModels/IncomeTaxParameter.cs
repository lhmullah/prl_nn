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
    public class IncomeTaxParameter
    {
        public FiscalYr prl_fiscal_year { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Fiscal Year")]
        public int fiscal_year_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Assesment Year")]
        public string assessment_year { get; set; }

        [DisplayName("Type")]
        public string gender { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Minimum Amount")]
        public decimal slab_mininum_amount { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Maximum Amount")]
        public decimal slab_maximum_amount { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Percentage")]
        public decimal slab_percentage { get; set; }
    }
}