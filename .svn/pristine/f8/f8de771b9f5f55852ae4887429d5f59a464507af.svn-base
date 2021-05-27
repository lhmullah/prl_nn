using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class Overtime
    {
        public Overtime()
        {
            this.prl_over_time_configuration = new List<OTConfiguration>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        public virtual ICollection<OTConfiguration> prl_over_time_configuration { get; set; }
    }
}