using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_location
    {
        public prl_location()
        {
            this.prl_employee_details = new List<prl_employee_details>();
        }

        public int id { get; set; }
        public string location_name { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
    }
}
