using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Employee
    {

        public Employee()
        {
            prl_employee_details = new List<EmployeeDetails>();
        }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "ID can not be empty.")]
        [DisplayName("Employee ID.")]
        public string emp_no { get; set; }

        [Required(ErrorMessage = "Name can not be empty.")]
        [DisplayName("Employee Name")]
        public string name { get; set; }

        [DisplayName("Official Contact No")]
        public string official_contact_no { get; set; }

        [DisplayName("Personal Contact No")]
        public string personal_contact_no { get; set; }

        [DisplayName("Email")]
        [RegularExpression(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", ErrorMessage = "Enter a valid email address.")]
        public string email { get; set; }

        [DisplayName("Personal Email")]
        [RegularExpression(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", ErrorMessage = "Enter a valid email address.")]
        public string personal_email { get; set; }

        //[Required(ErrorMessage = "Religion can not be empty.")]
        [DisplayName("Religion")]
        public int religion_id { get; set; }

        [DisplayName("Gender")]
        public string gender { get; set; }

        //[Required(ErrorMessage = "Date of birth can not be empty.")]
        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dob { get; set; }
        
        [Required(ErrorMessage = "Join date can not be empty.")]
        [DisplayName("Join Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime joining_date { get; set; }
        
        [DisplayName("TIN")]
        public string tin { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> confirmation_date { get; set; }

        [DisplayName("Is Confirmed")]
        public bool is_confirmed { get; set; }

        [UIHint("YesNo")]
        [DisplayName("Is Active")]
        public bool is_active { get; set; }

        public Nullable<int> bank_id { get; set; }
        public Nullable<int> bank_branch_id { get; set; }
        public string account_type { get; set; }
        public string account_no { get; set; }
        public string routing_no { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        public List<EmployeeDetails> prl_employee_details { get; set; }
        public virtual Bank prl_bank { get; set; }
        public virtual BankBranch prl_bank_branch { get; set; }
        //public virtual Company prl_company { get; set; }
        public virtual Religion prl_religion { get; set; }
    }
}