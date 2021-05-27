using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_job_level
    {
        public prl_job_level()
        {
            this.prl_employee_details = new List<prl_employee_details>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
    }
}
