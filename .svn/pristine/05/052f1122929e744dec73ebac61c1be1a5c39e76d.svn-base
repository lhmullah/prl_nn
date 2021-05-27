using System;
using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_employee_settlement
    {
        public prl_employee_settlement()
        {
            this.prl_employee_settlement_allowance = new List<prl_employee_settlement_allowance>();
            this.prl_employee_settlement_deduction = new List<prl_employee_settlement_deduction>();
            this.prl_employee_settlement_detail = new List<prl_employee_settlement_detail>();
        }

        public int id { get; set; }
        public int emp_id { get; set; }
        public decimal pf_own_amount { get; set; }
        public decimal pf_company_amount { get; set; }
        public decimal gf_amount { get; set; }
        public decimal fractional_salary_earning { get; set; }
        public decimal bonus_earning_amount { get; set; }
        public decimal ot_amount { get; set; }
        public decimal other_allowances { get; set; }
        public decimal total_employee_earnings { get; set; }
        public decimal fractional_salary_deduction { get; set; }
        public decimal bonus_deduction { get; set; }
        public decimal other_deductions { get; set; }
        public decimal total_company_earnings { get; set; }
        public Nullable<System.DateTime> settlement_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual prl_employee prl_employee { get; set; }
        public virtual ICollection<prl_employee_settlement_allowance> prl_employee_settlement_allowance { get; set; }
        public virtual ICollection<prl_employee_settlement_deduction> prl_employee_settlement_deduction { get; set; }
        public virtual ICollection<prl_employee_settlement_detail> prl_employee_settlement_detail { get; set; }
    }
}
