using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportMonthlyTaxStatement
    {
        public int Year { get; set; }
        public int month_no { get; set; }
        public string month_name { get; set; }

        //For RDLC
        public int empId { get; set; }
        public string empNo { get; set; }
        public string empName { get; set; }
        public string designation { get; set; }
        public string tin { get; set; }

        public Nullable<decimal> basic_salary_including_arrear { get; set; }
        public Nullable<decimal> total_allowance_with_benefit { get; set; }
        public Nullable<decimal> value_of_benefit_not_paid_in_cash { get; set; }
        public Nullable<decimal> totalA { get; set; }

        public Nullable<decimal> amount_of_tax { get; set; }

        public string challan_no { get; set; }
        public DateTime challan_date { get; set; }
        public string bank_name { get; set; }

        public Nullable<decimal> challan_total_amount { get; set; }
        public Nullable<decimal> tax_paid_till_last_month { get; set; }

        public string remarks { get; set; }

    }
}
