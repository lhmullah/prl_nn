using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class EmployeeLeaveWithoutPay
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [DisplayName("Leave Type")]
        public int setting_id { get; set; }

        [DisplayName("Employee Name")]
        public int emp_id { get; set; }

        [DisplayName("No. of Days")]
        public Nullable<int> no_of_days { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime strat_date { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> end_date { get; set; }

        public Employee prl_employee { get; set; }
        public LeaveWithoutPaySetting prl_leave_without_pay_settings { get; set; }
    }
}