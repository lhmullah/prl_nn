using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_sub_department
    {
        public prl_sub_department()
        {
            this.prl_sub_sub_department = new List<prl_sub_sub_department>();
            this.prl_employee_details = new List<prl_employee_details>();
        }

        public int id { get; set; }
        public int department_id { get; set; }
        public string name { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public virtual prl_department prl_department { get; set; }
        public virtual ICollection<prl_sub_sub_department> prl_sub_sub_department { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
    }
}
