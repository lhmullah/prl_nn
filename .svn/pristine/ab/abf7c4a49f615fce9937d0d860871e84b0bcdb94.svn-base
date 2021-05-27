using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class BonusHold
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int emp_id { get; set; }

        [DisplayName("Bonus Name")]
        [Required(ErrorMessage = "Bonus Name cannot be empty.")]
        public int bonus_name_id { get; set; }

        [DisplayName("Month")]
        public string month { get; set; }

        [DisplayName("Year")]
        public string year { get; set; }

        [DisplayName("Is Hold")]
        public string is_holded { get; set; }

        [DisplayName("Hold Reason")]
        public string hold_reason { get; set; }

        [DisplayName("Hold Date")]
        public Nullable<System.DateTime> hold_from { get; set; }

        [DisplayName("Hold Date To")]
        public Nullable<System.DateTime> hold_to { get; set; }

        [DisplayName("UnHold Date")]
        public Nullable<System.DateTime> unhold_date { get; set; }

        [DisplayName("UnHold Reason")]
        public string unhold_reason { get; set; }

        public virtual BonusName prl_bonus_name { get; set; }
        public virtual Employee prl_employee { get; set; }

        public Dictionary<string, string> BonusMonth { get; set; }

        public Dictionary<string, string> BonusYear { get; set; }
    }
}