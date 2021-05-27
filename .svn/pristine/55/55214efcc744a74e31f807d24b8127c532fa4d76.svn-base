using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class BonusProcessDetail
    {
        public int id { get; set; }
        public int bonus_process_id { get; set; }
        public int emp_id { get; set; }
        public string batch_no { get; set; }
        public decimal amount { get; set; }
        public Nullable<decimal> bonus_tax_amount { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public virtual BonusProcess prl_bonus_process { get; set; }
        public virtual Employee prl_employee { get; set; }
    }
}