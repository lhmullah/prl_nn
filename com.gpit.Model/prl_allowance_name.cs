using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_allowance_name
    {
        public prl_allowance_name()
        {
            this.prl_allowance_configuration = new List<prl_allowance_configuration>();
            this.prl_employee_individual_allowance = new List<prl_employee_individual_allowance>();
            this.prl_salary_allowances = new List<prl_salary_allowances>();
            this.prl_upload_allowance = new List<prl_upload_allowance>();
            this.prl_upload_staff_allowance = new List<prl_upload_staff_allowance>();
            this.prl_allowance_staff = new List<prl_allowance_staff>();
            this.prl_grade = new List<prl_grade>();
            this.prl_workers_allowances = new List<prl_workers_allowances>();
        }

        public int id { get; set; }
        public int allowance_head_id { get; set; }
        public string allowance_name { get; set; }
        public string description { get; set; }
        public virtual ICollection<prl_allowance_configuration> prl_allowance_configuration { get; set; }
        public virtual prl_allowance_head prl_allowance_head { get; set; }
        public virtual ICollection<prl_employee_individual_allowance> prl_employee_individual_allowance { get; set; }
        public virtual ICollection<prl_salary_allowances> prl_salary_allowances { get; set; }
        public virtual ICollection<prl_upload_allowance> prl_upload_allowance { get; set; }
        public virtual ICollection<prl_upload_staff_allowance> prl_upload_staff_allowance { get; set; }
        public virtual ICollection<prl_allowance_staff> prl_allowance_staff { get; set; }
        public virtual ICollection<prl_grade> prl_grade { get; set; }
        public virtual ICollection<prl_workers_allowances> prl_workers_allowances { get; set; }
    }
}
