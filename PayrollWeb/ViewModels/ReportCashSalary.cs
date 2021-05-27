using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportCashSalary
    {
        public string empNo { get; set; }
        public string empName { get; set; }

        public int department_id { get; set; }
        public int sub_department_id { get; set; }
        public int sub_sub_department_id { get; set; }

        public string department { get; set; }
        public string sub_department { get; set; }
        public string sub_sub_department { get; set; }

        public Nullable<decimal> salary_amount { get; set; }
        public string email { get; set; }
        public string designation { get; set; }


        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string MonthName { get; set; }
        public int monthYear { get; set; }
        public DateTime payment_date { get; set; }

        public string heading { get; set; }
        public string remarks { get; set; }

        [DisplayName("Select Any Date of a Month ")]
        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Enter a valid date.")]
        [Required(ErrorMessage = "Must select a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> SelectDate { get; set; }
    }
}