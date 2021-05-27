using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_loan_entry
    { 
        public prl_loan_entry()
        {
            this.prl_loan_payment_summary = new List<prl_loan_payment_summary>();
        }

        public int id { get; set; }
        public int emp_id { get; set; }
        public int deduction_name_id { get; set; }
        public System.DateTime loan_start_date { get; set; }
        public System.DateTime loan_end_date { get; set; }
        public decimal principal_amount { get; set; }
        public decimal monthly_installment { get; set; }

        public virtual prl_employee prl_employee { get; set; }
        public virtual prl_deduction_name prl_deduction_name { get; set; }
        public virtual ICollection<prl_loan_payment_summary> prl_loan_payment_summary { get; set; }
    }
}
