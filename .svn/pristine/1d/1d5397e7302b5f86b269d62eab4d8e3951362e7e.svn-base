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
    public class BonusProcess
    {
        public BonusProcess()
        {
            //prl_bonus_process_detail = new BonusProcessDetail();
            prl_bonus_name = new BonusName();
            prl_department = new Department();
            prl_division = new Division();
            prl_religion = new Religion();
            prl_grade = new Grade();
            prl_company = new Company();
            prl_fiscal_year = new FiscalYr();
        }

       // public virtual BonusProcessDetail prl_bonus_process_detail { get; set; }
        public virtual BonusName prl_bonus_name { get; set; }
        public virtual Department prl_department { get; set; }
        public virtual Division prl_division { get; set; }
        public virtual Religion prl_religion { get; set; }
        public virtual Grade prl_grade { get; set; }
        public virtual Company prl_company { get; set; }
        public virtual FiscalYr prl_fiscal_year { get; set; }


        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Bonus Name")]
        public int bonus_name_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Fiscal Year")]
        public int fiscal_year_id { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Month")]
        public string month { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Year")]
        public string year { get; set; }

        [DisplayName("Batch No.")]
        public string batch_no { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Process Date")]
        public Nullable<System.DateTime> process_date { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Festival Date")]
        public Nullable<System.DateTime> festival_date { get; set; }

        [Required(ErrorMessage = "Can not be null or empty.")]
        [DisplayName("Relegion")]
        public int religion_id { get; set; }

        [DisplayName("Company")]
        public int company_id { get; set; }

        [DisplayName("Grade")]
        public int grade_id { get; set; }

        [DisplayName("Division")]
        public int division_id { get; set; }

        [DisplayName("Department")]
        public int department_id { get; set; }

        [DisplayName("Gender")]
        public string gender { get; set; }

        [DisplayName("Pay with Next Salary")]
        public string is_pay_with_salary { get; set; }

        public string is_available_in_payslip { get; set; }
        
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}