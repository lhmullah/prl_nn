using System;

namespace com.linde.Model
{
    public partial class prl_loan_payment_summary
    { 
        public int id { get; set; }
        public int loan_entry_id { get; set; }
        public int salary_process_id { get; set; }
        public DateTime salary_month_year { get; set; }
        public decimal this_month_paid { get; set; }
        public decimal loan_realized { get; set; }
        public decimal loan_balance { get; set; }

        public virtual prl_salary_process prl_salary_process { get; set; }
        public virtual prl_loan_entry prl_loan_entry { get; set; }
    }
}
