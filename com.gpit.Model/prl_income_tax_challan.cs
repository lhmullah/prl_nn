using System;

namespace com.linde.Model
{
    public partial class prl_income_tax_challan
    {
        public int id { get; set; }
        //public Nullable<int> company_id { get; set; }
        public int emp_id { get; set; }
        public Nullable<int> fiscal_year_id { get; set; }
        public string challan_no { get; set; }
        public Nullable<decimal> amount { get; set; }
        public System.DateTime challan_date { get; set; }

        public string challan_bank { get; set; }
        public Nullable<decimal> challan_total_amount { get; set; }
        public string remarks { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}
