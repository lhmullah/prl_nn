using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_salary_process
    {
        public prl_salary_process()
        {
            this.prl_employee_tax_process = new List<prl_employee_tax_process>();
            this.prl_salary_allowances = new List<prl_salary_allowances>();
            this.prl_salary_deductions = new List<prl_salary_deductions>();
            this.prl_salary_process_detail = new List<prl_salary_process_detail>();
        }

        public int id { get; set; }
        public string batch_no { get; set; }
        public Nullable<System.DateTime> salary_month { get; set; }
        public Nullable<System.DateTime> process_date { get; set; }
        public Nullable<System.DateTime> payment_date { get; set; }
        public Nullable<int> company_id { get; set; }
        public Nullable<int> division_id { get; set; }
        public Nullable<int> department_id { get; set; }
        public Nullable<int> grade_id { get; set; }
        public string gender { get; set; }
        public string is_disbursed { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual ICollection<prl_employee_tax_process> prl_employee_tax_process { get; set; }
        public virtual ICollection<prl_salary_allowances> prl_salary_allowances { get; set; }
        public virtual ICollection<prl_salary_deductions> prl_salary_deductions { get; set; }
        public virtual ICollection<prl_salary_process_detail> prl_salary_process_detail { get; set; }
    }
}
