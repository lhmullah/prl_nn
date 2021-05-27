using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class SubSubDepartment
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [Display(Name = "Department Name")]
        public int department_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [Display(Name = "Sub Dept. Name")]
        public int sub_department_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [Display(Name = "Sub Sub Dept. Name")]
        public string name { get; set; }

        public virtual SubDepartment prl_sub_department { get; set; }
    }
}