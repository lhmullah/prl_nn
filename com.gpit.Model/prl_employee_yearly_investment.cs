using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_employee_yearly_investment
    {
        public prl_employee_yearly_investment()
        {
            
        }

        public int id { get; set; }
        public int emp_id { get; set; }
        public int fiscal_year_id { get; set; }
        public Nullable<decimal> invested_amount { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }

        public virtual prl_employee prl_employee { get; set; }
        public virtual prl_fiscal_year prl_fiscal_year { get; set; }
    }
}
