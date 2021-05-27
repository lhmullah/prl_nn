using System;


namespace com.linde.Model
{
    public partial class prl_upload_staff_allowance
    {
        public int id { get; set; }
        public int allowance_name_id { get; set; }
        public int emp_id { get; set; }
        public Nullable<System.DateTime> salary_month_year { get; set; }
        public Nullable<decimal> no_of_entry { get; set; }
        public Nullable<decimal> amount_or_percentage { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        public virtual prl_allowance_name prl_allowance_name { get; set; }
        public virtual prl_employee prl_employee { get; set; }
    }
}
