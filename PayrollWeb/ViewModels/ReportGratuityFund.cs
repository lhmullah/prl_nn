using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportGratuityFund
    {
        public int eId { get; set; }

        [DisplayName("Employee ID.")]
        public string empNo { get; set; }
        [DisplayName("Employee Name")]
        public string empName { get; set; }
        [DisplayName("Designation")]
        public string designation { get; set; }
        [DisplayName("Grade")]
        public string grade { get; set; }
        [DisplayName("Emp. Category")]
        public string category { get; set; }

        [DisplayName("Joining Date")]
        public DateTime joining_date { get; set; }

        [DisplayName("Division")]
        public string division { get; set; }
        [DisplayName("Department")]
        public string department { get; set; }
        [DisplayName("Basic Salary")]
        public decimal basicSalary { get; set; }

        [DisplayName("Payment Mode")]
        public string paymentMode { get; set; }

        [DisplayName("Bank")]
        public string bank { get; set; }

        [DisplayName("Account No")]
        public string accNo { get; set; }

        public string routing_no { get; set; }

        [DisplayName("Net Pay")]
        public Nullable<decimal> netPay { get; set; }

        public int serviceLength { get; set; }
        public int age { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
    }
}