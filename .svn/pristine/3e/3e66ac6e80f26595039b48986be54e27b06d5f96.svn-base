using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class DeductionName
    {

        public virtual DeductionHead prl_deduction_head { get; set; }

        public  ICollection<Grade> prl_grade { get; set; }

        public DeductionName()
        {
            prl_grade = new List<Grade>();
        }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Deduction head")]
        public int deduction_head_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("GL code")]
        public string gl_code { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Deduction name")]
        public string deduction_name { get; set; }
    }
}