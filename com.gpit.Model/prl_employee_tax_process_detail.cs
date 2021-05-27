using System;

namespace com.linde.Model
{
    public partial class prl_employee_tax_process_detail
    {
        public int id { get; set; }
        public Nullable<int> tax_process_id { get; set; }
        public Nullable<int> salary_process_id { get; set; }
        public Nullable<int> emp_id { get; set; }
        public Nullable<int> allowance_head_id { get; set; }
        public string allowance_head_name { get; set; }
        public Nullable<int> bonus_id { get; set; }
        public string bonus_name { get; set; }
        public string tax_item { get; set; }

        public decimal till_date_income { get; set; }
        public decimal current_month_income { get; set; }
        public decimal projected_income { get; set; }

        public decimal gross_annual_income { get; set; }
        public decimal less_exempted { get; set; }
        public decimal total_taxable_income { get; set; }
        public virtual prl_employee_tax_process prl_employee_tax_process { get; set; }
    }
}
