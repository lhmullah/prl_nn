using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportMonthlyAllowance
    {
        public int empId { get; set; }
        public string empNo { get; set; }
        public string empName { get; set; }
        public string designation { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime monthYear { get; set; }
        public int processId { get; set; }

        public decimal office_staff_ot { get; set; }
        public decimal factory_staff_ot { get; set; }
        public decimal shift_allowance { get; set; }
        public decimal officiating { get; set; }
        public decimal mid_month_advance { get; set; }
        public decimal lda { get; set; }
        public decimal ramadan_allowance { get; set; }

        public string month_name { get; set; }
        public decimal totalAllowance { get; set; }

    }

}