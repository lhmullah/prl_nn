using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_department
    {
        public prl_department()
        {
            this.prl_employee_details = new List<prl_employee_details>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
    }
}
