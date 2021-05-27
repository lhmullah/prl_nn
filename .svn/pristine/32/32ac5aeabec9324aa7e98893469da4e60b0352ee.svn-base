using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_bank_branch
    {
        public prl_bank_branch()
        {
            this.prl_employee = new List<prl_employee>();
        }

        public int id { get; set; }
        public int bank_id { get; set; }
        public string branch_name { get; set; }
        public string branch_code { get; set; }
        public virtual prl_bank prl_bank { get; set; }
        public virtual ICollection<prl_employee> prl_employee { get; set; }
    }
}
