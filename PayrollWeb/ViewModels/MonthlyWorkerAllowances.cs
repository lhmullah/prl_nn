using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class MonthlyWorkerAllowances
    {
        public MonthlyWorkerAllowances()
        {
           prl_allowance_name = new AllowanceName();
        }

        public AllowanceName prl_allowance_name { get; set; }

        [Required(ErrorMessage = "Must select a year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Must select a month")]
        public int Month { get; set; }

        [DisplayName("Process Date")]
        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Enter a valid date.")]
        [Required(ErrorMessage = "Must select a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ProcessDate { get; set; }
    }
}