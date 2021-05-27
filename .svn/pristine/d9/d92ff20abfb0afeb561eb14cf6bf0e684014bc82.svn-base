using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.gpit.Model;
using System.ComponentModel.DataAnnotations;
using PayrollWeb.Models;

namespace PayrollWeb.ViewModels
{
    public class EmployeeView
    {
        public ActionMode Mode { get; set; }
        public int id { get; set; }
        public string emp_no { get; set; }
        [DisplayName("Employee name")]
        public string name { get; set; }
        [DisplayName("Present Address")]
        public string present_address { get; set; }
        [DisplayName("Permanent Address")]
        public string permanent_address { get; set; }
        [DisplayName("Phone Number")]
        public string phone { get; set; }
        [DisplayName("Email")]
        public string email { get; set; }
        [DisplayName("Religion")]
        public ICollection<Religion> Religion { get; set; }

        public string gender { get; set; }
        [DisplayName("Religion")]
        public IEnumerable<SelectListItem> Genders
        {
            get
            {
                return EnumToSelectItemList.GetEnumSelectList<Genders>();
            }
        }
       
        public string marital_status { get; set; }
        public Nullable<int> company_id { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public Nullable<int> bank_id { get; set; }
        public Nullable<int> bank_branch_id { get; set; }
        public string account_no { get; set; }
        public System.DateTime dob { get; set; }
        public System.DateTime joining_date { get; set; }
        public string tin { get; set; }
        public Nullable<System.DateTime> confirmation_date { get; set; }
        public string is_confirmed { get; set; }
        public string is_pf_member { get; set; }
        public string is_gf_member { get; set; }
        public Nullable<System.DateTime> termination_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}