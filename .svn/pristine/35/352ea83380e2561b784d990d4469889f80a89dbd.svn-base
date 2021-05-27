using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_company
    {
        public prl_company()
        {
            this.prl_employee = new List<prl_employee>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public string primary_phone { get; set; }
        public string secondary_phone { get; set; }
        public string email { get; set; }
        public string web { get; set; }
        public string logo { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual ICollection<prl_employee> prl_employee { get; set; }
    }
}
