using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class SalaryProcessModel
    {
        [Required(ErrorMessage = "Must select a year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Must select a month")]
        public int Month { get; set; }

        [DisplayName("Process Date")]
        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Enter a valid date.")]
        [Required(ErrorMessage = "Must select a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SalaryProcessDate { get; set; }

        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Enter a valid date.")]
        [Required(ErrorMessage = "Must select a date")]
        [DisplayName("Salary Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SalaryPaymentDate { get; set; }

        public int Department { get; set; }

        public int Division { get; set; }

        public int Grade { get; set; }

        public string emp_category { get; set; }

        public string SelectedEmployeesOnly { get; set; }
        public string SalaryProcesseFor { get; set; }
    }
}