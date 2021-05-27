using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_cost_centre
    {
        public prl_cost_centre()
        {
            this.prl_employee_details = new List<prl_employee_details>();
        }

        public int id { get; set; }
        public string cost_centre_name { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
    }
}
