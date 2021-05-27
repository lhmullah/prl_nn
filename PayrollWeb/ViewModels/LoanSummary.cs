using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class LoanSummary 
    {
        [HiddenInput(DisplayValue = true)]
        
        public int id { get; set; }

        //public LoanSummary()
        //{
        //    prl_employee = new List<Employee>();
        //    //prl_employee_details = new List<EmployeeDetails>();
        //}

        public int salary_process_id { get; set; }

        public int emp_Id { get; set; }



        [Required(ErrorMessage = "ID can not be empty.")]
        [DisplayName("Employee ID.")]
        public string emp_no { get; set; }

        public string name { get; set; }

        [Required(ErrorMessage = "Loan Type can not be empty.")]
        [DisplayName("Loan Type")]
        public int deduction_name_id { get; set; }

        [Required(ErrorMessage = "Deduciton Name can not be empty.")]
        [DisplayName("Deduction Name")]
        public string deduction_name { get; set; }

        [Required(ErrorMessage = "Start Date can not be empty.")]
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime loan_start_date { get; set; }

        [Required(ErrorMessage = "End Date can not be empty.")]
        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime loan_end_date { get; set; }

        [Required(ErrorMessage = "Principal Amount can not be empty.")]
        [DisplayName("Principal Amount")]
        public decimal principal_amount { get; set; }

        [DisplayName("Monthly Installment")]
        public decimal monthly_installment { get; set; }

        [DisplayName("Loan Realized")]
        public decimal loan_realized { get; set; }

        [DisplayName("Loan Balance")]
        public decimal loan_balance { get; set; }

        public virtual DeductionName prl_deduction_name { get; set; }
        public virtual Employee prl_employee { get; set; }

        //public List<Employee> prl_employee { get; set; }
        //public List<EmployeeDetails> prl_employee_details { get; set; }
    }
}
