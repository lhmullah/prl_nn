using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_allowance_name
    {
        public prl_allowance_name()
        {
            this.prl_allowance_configuration = new List<prl_allowance_configuration>();
            this.prl_employee_individual_allowance = new List<prl_employee_individual_allowance>();
            this.prl_salary_allowances = new List<prl_salary_allowances>();
            this.prl_upload_allowance = new List<prl_upload_allowance>();
            this.prl_grade = new List<prl_grade>();
        }

        public int id { get; set; }
        public int allowance_head_id { get; set; }
        public string allowance_name { get; set; }
        public string description { get; set; }
        public string gl_code { get; set; }
        public virtual ICollection<prl_allowance_configuration> prl_allowance_configuration { get; set; }
        public virtual prl_allowance_head prl_allowance_head { get; set; }
        public virtual ICollection<prl_employee_individual_allowance> prl_employee_individual_allowance { get; set; }
        public virtual ICollection<prl_salary_allowances> prl_salary_allowances { get; set; }
        public virtual ICollection<prl_upload_allowance> prl_upload_allowance { get; set; }
        public virtual ICollection<prl_grade> prl_grade { get; set; }
    }
}
