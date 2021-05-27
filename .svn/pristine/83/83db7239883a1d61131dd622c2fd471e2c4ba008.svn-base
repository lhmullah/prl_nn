using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_bonus_process
    {
        public prl_bonus_process()
        {
            this.prl_bonus_process_detail = new List<prl_bonus_process_detail>();
        }

        public int id { get; set; }
        public int bonus_name_id { get; set; }
        public int fiscal_year_id { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string batch_no { get; set; }
        public Nullable<System.DateTime> process_date { get; set; }
        public Nullable<System.DateTime> festival_date { get; set; }
        public string is_festival { get; set; }
        public int religion_id { get; set; }
        public Nullable<int> company_id { get; set; }
        public Nullable<int> grade_id { get; set; }
        public Nullable<int> division_id { get; set; }
        public Nullable<int> department_id { get; set; }
        public string gender { get; set; }
        public string is_pay_with_salary { get; set; }
        public string is_available_in_payslip { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual prl_bonus_name prl_bonus_name { get; set; }
        public virtual ICollection<prl_bonus_process_detail> prl_bonus_process_detail { get; set; }
    }
}
