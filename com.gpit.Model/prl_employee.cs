using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_employee
    {
        public prl_employee()
        {
            this.prl_arrear_configuration = new List<prl_arrear_configuration>();
            this.prl_bonus_hold = new List<prl_bonus_hold>();
            this.prl_bonus_process_detail = new List<prl_bonus_process_detail>();
            this.prl_employee_discontinue = new List<prl_employee_discontinue>();
            this.prl_employee_children_allowance = new List<prl_employee_children_allowance>();
            this.prl_employee_details = new List<prl_employee_details>();
            this.prl_employee_free_car = new List<prl_employee_free_car>();
            this.prl_employee_tax_process = new List<prl_employee_tax_process>();
            this.prl_employee_yearly_investment = new List<prl_employee_yearly_investment>();
            this.prl_employee_individual_allowance = new List<prl_employee_individual_allowance>();
            this.prl_employee_individual_deduction = new List<prl_employee_individual_deduction>();
            this.prl_employee_leave_without_pay = new List<prl_employee_leave_without_pay>();
            this.prl_employee_settlement = new List<prl_employee_settlement>();
            this.prl_income_tax_adjustment = new List<prl_income_tax_adjustment>();
            this.prl_income_tax_refund = new List<prl_income_tax_refund>();
            this.prl_over_time_amount = new List<prl_over_time_amount>();
            this.prl_salary_allowances = new List<prl_salary_allowances>();
            this.prl_salary_deductions = new List<prl_salary_deductions>();
            this.prl_salary_hold = new List<prl_salary_hold>();
            this.prl_salary_process_detail = new List<prl_salary_process_detail>();
            this.prl_salary_review = new List<prl_salary_review>();
            this.prl_upload_allowance = new List<prl_upload_allowance>();
            this.prl_upload_bonus = new List<prl_upload_bonus>();
            this.prl_upload_deduction = new List<prl_upload_deduction>();
            this.prl_upload_staff_allowance = new List<prl_upload_staff_allowance>();
            this.prl_workers_allowances = new List<prl_workers_allowances>();
            this.prl_loan_entry = new List<prl_loan_entry>();
            this.prl_upload_pf_dividend_yearly = new List<prl_upload_pf_dividend_yearly>();
            this.prl_certificate_upload = new List<prl_certificate_upload>();
        }

        public int id { get; set; }
        public string emp_no { get; set; }
        public string name { get; set; }
        public string official_contact_no { get; set; }
        public string personal_contact_no { get; set; }
        public string email { get; set; }
        public string personal_email { get; set; }
        public int religion_id { get; set; }
        public string gender { get; set; }
        public Nullable<int> bank_id { get; set; }
        public Nullable<int> bank_branch_id { get; set; }
        public string account_type { get; set; }
        public string account_no { get; set; }
        public string routing_no { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public System.DateTime joining_date { get; set; }
        public string tin { get; set; }
        public Nullable<System.DateTime> confirmation_date { get; set; }
        public Nullable<sbyte> is_confirmed { get; set; }
        
        public Nullable<sbyte> is_active { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual ICollection<prl_arrear_configuration> prl_arrear_configuration { get; set; }
        public virtual prl_bank prl_bank { get; set; }
        public virtual prl_bank_branch prl_bank_branch { get; set; }
        public virtual ICollection<prl_bonus_hold> prl_bonus_hold { get; set; }
        public virtual ICollection<prl_bonus_process_detail> prl_bonus_process_detail { get; set; }
        public virtual ICollection<prl_employee_discontinue> prl_employee_discontinue { get; set; }
        public virtual prl_religion prl_religion { get; set; }
        public virtual ICollection<prl_employee_children_allowance> prl_employee_children_allowance { get; set; }
        public virtual ICollection<prl_employee_details> prl_employee_details { get; set; }
        public virtual ICollection<prl_employee_free_car> prl_employee_free_car { get; set; }
        public virtual ICollection<prl_employee_tax_process> prl_employee_tax_process { get; set; }
        public virtual ICollection<prl_employee_yearly_investment> prl_employee_yearly_investment { get; set; }
        public virtual ICollection<prl_employee_individual_allowance> prl_employee_individual_allowance { get; set; }
        public virtual ICollection<prl_employee_individual_deduction> prl_employee_individual_deduction { get; set; }
        public virtual ICollection<prl_employee_leave_without_pay> prl_employee_leave_without_pay { get; set; }
        public virtual ICollection<prl_employee_settlement> prl_employee_settlement { get; set; }
        public virtual ICollection<prl_income_tax_adjustment> prl_income_tax_adjustment { get; set; }
        public virtual ICollection<prl_income_tax_refund> prl_income_tax_refund { get; set; }
        public virtual ICollection<prl_over_time_amount> prl_over_time_amount { get; set; }
        public virtual ICollection<prl_salary_allowances> prl_salary_allowances { get; set; }
        public virtual ICollection<prl_salary_deductions> prl_salary_deductions { get; set; }
        public virtual ICollection<prl_salary_hold> prl_salary_hold { get; set; }
        public virtual ICollection<prl_salary_process_detail> prl_salary_process_detail { get; set; }
        public virtual ICollection<prl_salary_review> prl_salary_review { get; set; }
        public virtual ICollection<prl_upload_allowance> prl_upload_allowance { get; set; }
        public virtual ICollection<prl_upload_bonus> prl_upload_bonus { get; set; }
        public virtual ICollection<prl_upload_deduction> prl_upload_deduction { get; set; }
        public virtual ICollection<prl_upload_staff_allowance> prl_upload_staff_allowance { get; set; }
        public virtual ICollection<prl_workers_allowances> prl_workers_allowances { get; set; }
        public virtual ICollection<prl_loan_entry> prl_loan_entry { get; set; }
        public virtual ICollection<prl_upload_pf_dividend_yearly> prl_upload_pf_dividend_yearly { get; set; }

        public virtual ICollection<prl_certificate_upload> prl_certificate_upload { get; set; }
    }
}
