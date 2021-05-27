using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_over_time
    {
        public prl_over_time()
        {
            this.prl_over_time_configuration = new List<prl_over_time_configuration>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        public Nullable<int> max_value { get; set; }
        public virtual ICollection<prl_over_time_configuration> prl_over_time_configuration { get; set; }
    }
}
