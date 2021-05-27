using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_gf_setting
    {
        public prl_gf_setting()
        {
            //this.prl_employee_tax_process = new List<prl_employee_tax_process>();
            
        }

        public int id { get; set; }

        public decimal service_length_from { get; set; }
        public decimal service_length_to { get; set; }
        public decimal number_of_basic { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        //public virtual ICollection<prl_employee_tax_process> prl_employee_tax_process { get; set; }

    }
}
