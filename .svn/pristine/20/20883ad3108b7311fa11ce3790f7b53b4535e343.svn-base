using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_deduction_name
    {
        public prl_deduction_name()
        {
            this.prl_deduction_configuration = new List<prl_deduction_configuration>();
            this.prl_employee_individual_deduction = new List<prl_employee_individual_deduction>();
            this.prl_salary_deductions = new List<prl_salary_deductions>();
            this.prl_upload_deduction = new List<prl_upload_deduction>();
            this.prl_grade = new List<prl_grade>();
        }

        public int id { get; set; }
        public int deduction_head_id { get; set; }
        public string gl_code { get; set; }
        public string deduction_name { get; set; }
        public virtual ICollection<prl_deduction_configuration> prl_deduction_configuration { get; set; }
        public virtual prl_deduction_head prl_deduction_head { get; set; }
        public virtual ICollection<prl_employee_individual_deduction> prl_employee_individual_deduction { get; set; }
        public virtual ICollection<prl_salary_deductions> prl_salary_deductions { get; set; }
        public virtual ICollection<prl_upload_deduction> prl_upload_deduction { get; set; }
        public virtual ICollection<prl_grade> prl_grade { get; set; }
    }
}
