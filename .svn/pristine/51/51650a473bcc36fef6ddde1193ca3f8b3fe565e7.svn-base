using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class LeaveWithoutPaySetting
    {
        public LeaveWithoutPaySetting()
        {
            this.prl_employee_leave_without_pay = new List<EmployeeLeaveWithoutPay>();
            Allowances = new List<AllowanceName>();
        }

        public int id { get; set; }

        [Required(ErrorMessage = "Leave Type cannot be empty.")]
        [DisplayName("Leave Type")]
        public string Lwp_type { get; set; }

        [DisplayName("Allowances Name")]
        public Nullable<int> allowance_id { get; set; }

        [DisplayName("Percentage of Basic")]
        public Nullable<decimal> percentage_of_basic { get; set; }

        [DisplayName("Percentage of Allowance")]
        public Nullable<decimal> percentage_of_allowance { get; set; }

        public virtual ICollection<EmployeeLeaveWithoutPay> prl_employee_leave_without_pay { get; set; }

        public IEnumerable<AllowanceName> Allowances { get; set; }

        
    }
}