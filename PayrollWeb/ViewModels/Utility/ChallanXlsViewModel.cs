using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels.Utility
{
    public class ChallanXlsViewModel
    {
        public string Emp_Id { get; set; }
        public string Challan_No { get; set; }
        public decimal Amount { get; set; }
        public string Challan_Date { get; set; }
        public string Challan_Bank { get; set; }
        public decimal Challan_Total { get; set; }
        public string Remarks { get; set; }
    }
}