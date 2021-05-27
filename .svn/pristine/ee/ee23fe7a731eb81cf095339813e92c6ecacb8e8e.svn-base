using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class IncomeTaxParameterDetail
    {
        public IncomeTaxParameterDetail()
        {
            prl_fiscal_year = new FiscalYr();
        }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }
        [HiddenInput(DisplayValue = true)]
        public Nullable<int> income_tax_parameter_id { get; set; }
        
        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Fiscal Year")]
        public int fiscal_year_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Assesment Year")]
        public string assesment_year { get; set; }

        [DisplayName("Type")]
        public string gender { get; set; }

        [DisplayName("Maximum Tax Age")]
        public Nullable<int> max_tax_age { get; set; }

        [DisplayName("Maximum Amount That can be Invested")]
        public Nullable<decimal> max_investment_amount { get; set; }

        [DisplayName("(%) of Taxable Income that can be shown as Investment")]
        public Nullable<decimal> max_investment_percentage { get; set; }

        [DisplayName("(%) of Investment that is exemted from Tax")]
        public Nullable<decimal> max_inv_exempted_percentage { get; set; }

        [DisplayName("Minimum Amount of Tax")]
        public Nullable<decimal> min_tax_amount { get; set; }

        [DisplayName("(%) of Basic allowed to House Rent")]
        public Nullable<decimal> max_house_rent_percentage { get; set; }

        [DisplayName("House Rent not Exceding")]
        public Nullable<decimal> house_rent_not_exceding { get; set; }

        [DisplayName("Maximum Conveyance Allowance")]
        public Nullable<decimal> max_conveyance_allowance_monthly { get; set; }

        [DisplayName("(%) of Basic for Company Car")]
        public Nullable<decimal> free_car { get; set; }

        [DisplayName("(%) of Exemption for LFA")]
        public Nullable<decimal> lfa_exemtion_percentage { get; set; }

        [DisplayName("(%) of Exemption for Medical")]
        public Nullable<decimal> medical_exemtion_percentage { get; set; }

        public FiscalYr prl_fiscal_year { get; set; }
    }
}