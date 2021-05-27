using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_designation
    {
        public prl_designation()
        {
            this.prl_employee_details = new List<prl_employee_details>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
    }
}
