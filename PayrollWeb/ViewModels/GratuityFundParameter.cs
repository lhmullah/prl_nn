using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class GratuityFundParameter
    {
        [HiddenInput(DisplayValue=true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Service Length From can not be empty.")]
        [DisplayName("Service Length From")]
        public decimal service_length_from { get; set; }

        [Required(ErrorMessage = "Service Length To can not be empty.")]
        [DisplayName("Service Length To")]
        public decimal service_length_to { get; set; }

        [Required(ErrorMessage = "No of Basic can not be empty.")]
        [DisplayName("No of Basic")]
        public decimal number_of_basic { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}