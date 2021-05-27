using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_grade
    {
        public prl_grade()
        {
            this.prl_employee_details = new List<prl_employee_details>();
            this.prl_allowance_name = new List<prl_allowance_name>();
            this.prl_bonus_name = new List<prl_bonus_name>();
            this.prl_deduction_name = new List<prl_deduction_name>();
        }

        public int id { get; set; }
        public string grade { get; set; }
        public Nullable<decimal> upper_basic { get; set; }
        public Nullable<decimal> lower_basic { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
        public virtual ICollection<prl_allowance_name> prl_allowance_name { get; set; }
        public virtual ICollection<prl_bonus_name> prl_bonus_name { get; set; }
        public virtual ICollection<prl_deduction_name> prl_deduction_name { get; set; }
    }
}
