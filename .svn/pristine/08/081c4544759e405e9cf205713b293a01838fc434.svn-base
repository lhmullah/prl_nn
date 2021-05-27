using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class OTConfiguration
    {
        public OTConfiguration()
        {
            //this.prl_over_time_amount = new List<prl_over_time_amount>();
        }
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [DisplayName("Overtime Id")]
        [Required(ErrorMessage = "Over Time can not null or empty")]
        public int over_time_id { get; set; }

        [DisplayName("Overtime name")]
        [Required(ErrorMessage = "Over Time can not null or empty")]
        public string name { get; set; }


        [DisplayName("Formula")]
        [Required(ErrorMessage = "Formula can not null or empty")]
        public string formula { get; set; }
        public virtual Overtime prl_over_time { get; set; }
       // public virtual ICollection<prl_over_time_amount> prl_over_time_amount { get; set; }
    }
}