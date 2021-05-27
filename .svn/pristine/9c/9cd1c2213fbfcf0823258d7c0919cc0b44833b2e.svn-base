using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class BankBranch
    {
        public virtual Bank prl_bank { get; set; }

        [HiddenInput(DisplayValue=true)]
        public int id { get; set; }

        [Required(ErrorMessage="Please select a bank.")]
        [DisplayName("Bank")]
        public int bank_id { get; set; }



        [Required(ErrorMessage = "Branch name can not be empty.")]
        [DisplayName("Branch Name")]
        public string branch_name { get; set; }

        [DisplayName("Branch Code")]
        public string branch_code { get; set; }
    }
}