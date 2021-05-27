using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_religion
    {
        public prl_religion()
        {
            this.prl_employee = new List<prl_employee>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public Nullable<int> no_of_bonus { get; set; }
        public virtual ICollection<prl_employee> prl_employee { get; set; }
    }
}
