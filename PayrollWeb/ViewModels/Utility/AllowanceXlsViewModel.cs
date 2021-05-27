using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels.Utility
{
    public class AllowanceXlsViewModel
    {
        public string Id { get; set; }
        public decimal NoOfAllowance { get; set; }
        public string remarks { get; set; }
        public string AllowanceName { get; set; }
    }
}