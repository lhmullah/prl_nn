using System;

namespace com.linde.Model
{
    public partial class prl_upload_pf_dividend_yearly
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public System.DateTime dividend_month_year { get; set; }

        public decimal own_contributed_amount { get; set; }
        public decimal company_contributed_amount { get; set; }
        public decimal total_contributed_amount { get; set; }

        public decimal own_dividend_amount { get; set; }
        public decimal company_dividend_amount { get; set; }
        public decimal total_dividend_amount { get; set; }

        public decimal principal_amount { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        public virtual prl_employee prl_employee { get; set; }
    }
}
