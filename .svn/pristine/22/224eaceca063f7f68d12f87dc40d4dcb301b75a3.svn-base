using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_fiscal_year
    {
        public prl_fiscal_year()
        {
            this.prl_employee_tax_process = new List<prl_employee_tax_process>();
            this.prl_income_tax_adjustment = new List<prl_income_tax_adjustment>();
            this.prl_income_tax_parameter = new List<prl_income_tax_parameter>();
            this.prl_income_tax_parameter_details = new List<prl_income_tax_parameter_details>();
        }

        public int id { get; set; }
        public string fiscal_year { get; set; }
        public string assesment_year { get; set; }
        public virtual ICollection<prl_employee_tax_process> prl_employee_tax_process { get; set; }
        public virtual ICollection<prl_income_tax_adjustment> prl_income_tax_adjustment { get; set; }
        public virtual ICollection<prl_income_tax_parameter> prl_income_tax_parameter { get; set; }
        public virtual ICollection<prl_income_tax_parameter_details> prl_income_tax_parameter_details { get; set; }
    }
}
