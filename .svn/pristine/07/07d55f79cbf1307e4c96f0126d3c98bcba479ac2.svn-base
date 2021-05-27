using System;

namespace com.gpit.Model
{
    public partial class prl_over_time_amount
    {
        public int id { get; set; }
        public int over_time_config_id { get; set; }
        public int emp_id { get; set; }
        public System.DateTime month_year { get; set; }
        public Nullable<int> time_card_upload_id { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<decimal> actual_total { get; set; }
        public Nullable<decimal> pay_total { get; set; }
        public Nullable<decimal> arrear_amount { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual prl_employee prl_employee { get; set; }
        public virtual prl_over_time_configuration prl_over_time_configuration { get; set; }
        public virtual prl_upload_time_card_entry prl_upload_time_card_entry { get; set; }
    }
}
