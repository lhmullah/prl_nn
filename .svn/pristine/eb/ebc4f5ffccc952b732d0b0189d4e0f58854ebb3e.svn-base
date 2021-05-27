using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_leave_without_pay_settings
    {
        public prl_leave_without_pay_settings()
        {
            this.prl_employee_leave_without_pay = new List<prl_employee_leave_without_pay>();
        }

        public int id { get; set; }
        public string Lwp_type { get; set; }
        public Nullable<int> allowance_id { get; set; }
        public Nullable<decimal> percentage_of_basic { get; set; }
        public Nullable<decimal> percentage_of_allowance { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual ICollection<prl_employee_leave_without_pay> prl_employee_leave_without_pay { get; set; }
    }
}
