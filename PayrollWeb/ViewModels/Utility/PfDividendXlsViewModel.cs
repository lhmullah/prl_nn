using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels.Utility
{
    public class PfDividendXlsViewModel
    {
        public string Id { get; set; }
        public decimal own_contributed_amount { get; set; }
        public decimal company_contributed_amount { get; set; }
        public decimal own_dividend_amount { get; set; }
        public decimal company_dividend_amount { get; set; }
    }
}