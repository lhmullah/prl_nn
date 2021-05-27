using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_bank
    {
        public prl_bank()
        {
            this.prl_bank_branch = new List<prl_bank_branch>();
            this.prl_employee = new List<prl_employee>();
        }

        public int id { get; set; }
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public virtual ICollection<prl_bank_branch> prl_bank_branch { get; set; }
        public virtual ICollection<prl_employee> prl_employee { get; set; }
    }
}
