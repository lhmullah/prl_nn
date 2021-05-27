using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_employee_tax_process
    {
        public prl_employee_tax_process()
        {
            this.prl_employee_tax_process_detail = new List<prl_employee_tax_process_detail>();
            this.prl_employee_tax_slab = new List<prl_employee_tax_slab>();
        }

        public int id { get; set; }
        public int emp_id { get; set; }
        public int salary_process_id { get; set; }
        public int fiscal_year_id { get; set; }
        public System.DateTime salary_month { get; set; }
        public Nullable<decimal> yearly_tax { get; set; }
        public decimal monthly_tax { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public virtual prl_employee prl_employee { get; set; }
        public virtual prl_fiscal_year prl_fiscal_year { get; set; }
        public virtual prl_salary_process prl_salary_process { get; set; }
        public virtual ICollection<prl_employee_tax_process_detail> prl_employee_tax_process_detail { get; set; }
        public virtual ICollection<prl_employee_tax_slab> prl_employee_tax_slab { get; set; }
    }
}
