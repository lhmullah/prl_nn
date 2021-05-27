using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportSalaryChange
    {
        public int eId { get; set; }
        public string empId { get; set; }
        public string empName { get; set; }
        public DateTime entrydate { get; set; }
        public DateTime effectiveFrom { get; set; }
        public string grade { get; set; }
        public decimal newBasic { get; set; }
        public decimal oldbasic { get; set; }
        public string reason { get; set; }
    }
}