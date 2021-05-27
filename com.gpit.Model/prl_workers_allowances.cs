using System;


namespace com.linde.Model
{
    public partial class prl_workers_allowances
    {
        public int id { get; set; }
        public int allowance_process_id { get; set; }
        public System.DateTime salary_month { get; set; }
        public System.DateTime process_date { get; set; }
        public decimal calculation_for_days { get; set; }
        public int emp_id { get; set; }
        public int allowance_name_id { get; set; }
        public string allowance_name { get; set; }
        public decimal amount { get; set; }
        public Nullable<decimal> arrear_amount { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        public virtual prl_allowance_name prl_allowance_name { get; set; }
        public virtual prl_employee prl_employee { get; set; }
    }
}
