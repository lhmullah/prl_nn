using System;

namespace com.linde.Model
{
    public partial class prl_employee_tax_slab
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public Nullable<int> tax_process_id { get; set; }
        //public Nullable<int> salary_process_id { get; set; }
        public Nullable<int> fiscal_year_id { get; set; }
        public Nullable<System.DateTime> salary_date { get; set; }
        public Nullable<int> salary_month { get; set; }
        public Nullable<int> salary_year { get; set; }
        public decimal current_rate { get; set; }
        public string parameter { get; set; }
        public decimal taxable_income { get; set; }
        public decimal tax_liability { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public virtual prl_employee_tax_process prl_employee_tax_process { get; set; }
    }
}
