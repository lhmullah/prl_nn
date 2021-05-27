using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportLoanSummary
    {
        public int salary_process_id { get; set; }
        public int emp_Id { get; set; }
        public string empNo { get; set; }
        public string empName { get; set; }
        public string designation { get; set; }
        public string grade { get; set; }
        public string category { get; set; }
        public DateTime joining_date { get; set; }

        public string loan_type_name { get; set; }
        public System.DateTime loan_start_date { get; set; }
        public System.DateTime loan_end_date { get; set; }
        public decimal principal_amount { get; set; }
        public string salary_month { get; set; }
        public int salary_year { get; set; }
        public decimal this_month_installment { get; set; }
        public decimal loan_realized { get; set; }
        public decimal loan_balance { get; set; }
    }
}