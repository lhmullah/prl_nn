using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_over_time_configuration
    {
        public prl_over_time_configuration()
        {
            this.prl_over_time_amount = new List<prl_over_time_amount>();
        }

        public int id { get; set; }
        public int over_time_id { get; set; }
        public string name { get; set; }
        public string formula { get; set; }
        public virtual prl_over_time prl_over_time { get; set; }
        public virtual ICollection<prl_over_time_amount> prl_over_time_amount { get; set; }
    }
}
