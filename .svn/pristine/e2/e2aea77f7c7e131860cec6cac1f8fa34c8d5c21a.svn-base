using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class AllowanceName
    {

        public virtual AllowanceHead prl_allowance_head { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [Display(Name = "Allowance Head")]
        public int allowance_head_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [Display(Name = "Allowance Name")]
        public string allowance_name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }


        [Display(Name = "GL Code")]
        public string gl_code { get; set; }

        public bool IsSelected { get; set; }

    }
}