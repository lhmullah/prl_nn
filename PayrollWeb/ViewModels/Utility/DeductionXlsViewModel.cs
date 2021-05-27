using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels.Utility
{
    public class DeductionXlsViewModel
    {
        public string Id { get; set; }
        public decimal DeductionAmount { get; set; }
        public string DeductionName { get; set; }
        public string remarks { get; set; }
    }
}