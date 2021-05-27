using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportBankAdvice
    {
        public int emp_id { get; set; }
        public string empNo { get; set; }
        public string empName { get; set; }
        public int bankId { get; set; }
        public string bank { get; set; }
        public string accNo { get; set; }
        public string routing_no { get; set; }

        public string accType { get; set; }
        public int bankBranchId { get; set; }
        public Nullable<decimal> netPay { get; set; }
        public string reason { get; set; }
        public string debitAccNo { get; set; }
        public string email { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string MonthName { get; set; }
        public int monthYear { get; set; }

        public string SelectedEmployees { get; set; }

        public string batch { get; set; }

        public string remarks { get; set; }

        [DisplayName("Select Any Date of a Month ")]
        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", ErrorMessage = "Enter a valid date.")]
        [Required(ErrorMessage = "Must select a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> SelectDate { get; set; }
    }
}