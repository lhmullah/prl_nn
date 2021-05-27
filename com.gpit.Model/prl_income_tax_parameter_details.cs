using System;

namespace com.linde.Model
{
    public partial class prl_income_tax_parameter_details
    {
        public int id { get; set; }
        public Nullable<int> income_tax_parameter_id { get; set; }
        public Nullable<int> fiscal_year_id { get; set; }
        public string assesment_year { get; set; }
        public string gender { get; set; }
        public Nullable<int> max_tax_age { get; set; }
        public Nullable<decimal> max_investment_amount { get; set; }
        public Nullable<decimal> max_investment_percentage { get; set; }
        public Nullable<decimal> max_inv_exempted_percentage { get; set; }
        public Nullable<decimal> max_amount_for_max_exemption_percent { get; set; }
        public Nullable<decimal> min_inv_exempted_percentage { get; set; }
        public Nullable<decimal> min_tax_amount { get; set; }
        public Nullable<decimal> max_house_rent_percentage { get; set; }
        public Nullable<decimal> house_rent_not_exceding { get; set; }
        public Nullable<decimal> max_conveyance_allowance { get; set; }
        public Nullable<decimal> free_car { get; set; }
        public Nullable<decimal> lfa_exemption_percentage { get; set; }
        public Nullable<decimal> medical_exemption_percentage { get; set; }
        public Nullable<decimal> medical_not_exceding { get; set; }
        public Nullable<decimal> max_wppf_exemption_amount { get; set; }
        public virtual prl_fiscal_year prl_fiscal_year { get; set; }
    }
}
