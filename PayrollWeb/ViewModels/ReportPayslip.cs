using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportPayslip
    {
        public int eId { get; set; }

        [DisplayName("Employee ID.")]
        public string empNo { get; set; }
        [DisplayName("Employee Name")]
        public string empName { get; set; }
        [DisplayName("Designation")]
        public string designation { get; set; }
        [DisplayName("job Level")]
        public string job_level { get; set; }
        [DisplayName("Emp. Category")]
        public string category { get; set; }
        [DisplayName("Cost Centre")]
        public string cost_centre { get; set; }

        [DisplayName("Department")]
        public string department { get; set; }

        [DisplayName("Joining Date")]
        public DateTime joining_date { get; set; }

        [DisplayName("Basic Salary")]
        public decimal basicSalary { get; set; }

        public Nullable<decimal> this_month_basic { get; set; }
        public Nullable<decimal> pf { get; set; }

        [DisplayName("Total Allowance")]
        public decimal totalEarnings { get; set; }

        [DisplayName("Total Deduction")]
        public decimal totalDeduction { get; set; }

        [DisplayName("Net Pay")]
        public Nullable<decimal> netPay { get; set; }

        public int salary_process_id { get; set; }

        public Nullable <decimal> tax { get; set; }

        public int no_of_days_in_month { get; set; }
        public int no_of_working_days { get; set; }


        [DisplayName("Payment Mode")]
        public string paymentMode { get; set; }

        [DisplayName("Bank")]
        public string bank { get; set; }

        [DisplayName("Account No")]
        public string accNo { get; set; }

        public string routing_no { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public DateTime salary_date { get; set; }
        public int processId { get; set; }
    }

    public class AllowanceDeduction
    {
        public string head { get; set; }
        public decimal value { get; set; }
    }
}