using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_bonus_name
    {
        public prl_bonus_name()
        {
            this.prl_bonus_configuration = new List<prl_bonus_configuration>();
            this.prl_bonus_hold = new List<prl_bonus_hold>();
            this.prl_bonus_process = new List<prl_bonus_process>();
            this.prl_upload_bonus = new List<prl_upload_bonus>();
            this.prl_grade = new List<prl_grade>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public virtual ICollection<prl_bonus_configuration> prl_bonus_configuration { get; set; }
        public virtual ICollection<prl_bonus_hold> prl_bonus_hold { get; set; }
        public virtual ICollection<prl_bonus_process> prl_bonus_process { get; set; }
        public virtual ICollection<prl_upload_bonus> prl_upload_bonus { get; set; }
        public virtual ICollection<prl_grade> prl_grade { get; set; }
    }
}
