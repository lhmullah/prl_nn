using System.Data.Entity;
using com.linde.DataContext.Mapping;
using com.linde.Model;

namespace com.linde.DataContext
{
    public partial class payroll_systemContext : DbContext
    {
        static payroll_systemContext()
        {
            Database.SetInitializer<payroll_systemContext>(null);
        }

        public payroll_systemContext()
            : base("Name=payroll_systemContext")
        {
            
        }

        public DbSet<prl_allowance_configuration> prl_allowance_configuration { get; set; }
        public DbSet<prl_allowance_head> prl_allowance_head { get; set; }
        public DbSet<prl_allowance_name> prl_allowance_name { get; set; }
        public DbSet<prl_arrear_configuration> prl_arrear_configuration { get; set; }
        public DbSet<prl_bank> prl_bank { get; set; }
        public DbSet<prl_bank_branch> prl_bank_branch { get; set; }
        public DbSet<prl_bonus_configuration> prl_bonus_configuration { get; set; }
        public DbSet<prl_bonus_hold> prl_bonus_hold { get; set; }
        public DbSet<prl_bonus_name> prl_bonus_name { get; set; }
        public DbSet<prl_bonus_process> prl_bonus_process { get; set; }
        public DbSet<prl_bonus_process_detail> prl_bonus_process_detail { get; set; }
        //public DbSet<prl_company> prl_company { get; set; }
        public DbSet<prl_company_bank> prl_company_bank { get; set; }
        public DbSet<prl_deduction_configuration> prl_deduction_configuration { get; set; }
        public DbSet<prl_deduction_head> prl_deduction_head { get; set; }
        public DbSet<prl_deduction_name> prl_deduction_name { get; set; }

        public DbSet<prl_department> prl_department { get; set; }
        public DbSet<prl_sub_department> prl_sub_department { get; set; }
        public DbSet<prl_sub_sub_department> prl_sub_sub_department { get; set; }

        public DbSet<prl_designation> prl_designation { get; set; }
        public DbSet<prl_division> prl_division { get; set; }
        public DbSet<prl_employee> prl_employee { get; set; }
        public DbSet<prl_employee_children_allowance> prl_employee_children_allowance { get; set; }
        public DbSet<prl_employee_details> prl_employee_details { get; set; }
        public DbSet<prl_employee_discontinue> prl_employee_discontinue { get; set; }
        public DbSet<prl_employee_free_car> prl_employee_free_car { get; set; }
        public DbSet<prl_employee_individual_allowance> prl_employee_individual_allowance { get; set; }
        public DbSet<prl_employee_individual_deduction> prl_employee_individual_deduction { get; set; }
        public DbSet<prl_employee_leave_without_pay> prl_employee_leave_without_pay { get; set; }
        public DbSet<prl_employee_settlement> prl_employee_settlement { get; set; }
        public DbSet<prl_employee_settlement_allowance> prl_employee_settlement_allowance { get; set; }
        public DbSet<prl_employee_settlement_deduction> prl_employee_settlement_deduction { get; set; }
        public DbSet<prl_employee_settlement_detail> prl_employee_settlement_detail { get; set; }
        public DbSet<prl_employee_settlement_over_time> prl_employee_settlement_over_time { get; set; }
        public DbSet<prl_employee_tax_process> prl_employee_tax_process { get; set; }
        public DbSet<prl_employee_yearly_investment> prl_employee_yearly_investment { get; set; }
        public DbSet<prl_employee_tax_process_detail> prl_employee_tax_process_detail { get; set; }
        public DbSet<prl_employee_tax_slab> prl_employee_tax_slab { get; set; }
        public DbSet<prl_fiscal_year> prl_fiscal_year { get; set; }
        public DbSet<prl_grade> prl_grade { get; set; }
        public DbSet<prl_income_tax_adjustment> prl_income_tax_adjustment { get; set; }
        public DbSet<prl_income_tax_challan> prl_income_tax_challan { get; set; }
        public DbSet<prl_income_tax_parameter> prl_income_tax_parameter { get; set; }
        public DbSet<prl_income_tax_parameter_details> prl_income_tax_parameter_details { get; set; }
        public DbSet<prl_income_tax_refund> prl_income_tax_refund { get; set; }
        public DbSet<prl_leave_without_pay_settings> prl_leave_without_pay_settings { get; set; }
        public DbSet<prl_menu> prl_menu { get; set; }
        public DbSet<prl_month_end> prl_month_end { get; set; }
        public DbSet<prl_over_time> prl_over_time { get; set; }
        public DbSet<prl_over_time_amount> prl_over_time_amount { get; set; }
        public DbSet<prl_over_time_configuration> prl_over_time_configuration { get; set; }
        public DbSet<prl_religion> prl_religion { get; set; }
        public DbSet<prl_role_privilege> prl_role_privilege { get; set; }
        public DbSet<prl_salary_allowances> prl_salary_allowances { get; set; }
        public DbSet<prl_salary_deductions> prl_salary_deductions { get; set; }
        public DbSet<prl_salary_hold> prl_salary_hold { get; set; }
        public DbSet<prl_salary_process> prl_salary_process { get; set; }
        public DbSet<prl_salary_process_detail> prl_salary_process_detail { get; set; }
        public DbSet<prl_salary_review> prl_salary_review { get; set; }
        public DbSet<prl_sub_menu> prl_sub_menu { get; set; }
        public DbSet<prl_upload_allowance> prl_upload_allowance { get; set; }
        public DbSet<prl_upload_bonus> prl_upload_bonus { get; set; }
        public DbSet<prl_upload_deduction> prl_upload_deduction { get; set; }
        public DbSet<prl_upload_time_card_entry> prl_upload_time_card_entry { get; set; }
        public DbSet<prl_upload_staff_allowance> prl_upload_staff_allowance { get; set; }
        public DbSet<prl_allowance_staff> prl_allowance_staff { get; set; }
        public DbSet<prl_workers_allowances> prl_workers_allowances { get; set; }

        public DbSet<vw_empwisemonthlyallowances> vw_empwisemonthlyallowances { get; set; }
        public DbSet<vw_empsalaryprocessdetails> vw_empsalaryprocessdetails { get; set; }

        public DbSet<prl_gf_setting> prl_gf_setting { get; set; }
        public DbSet<prl_loan_entry> prl_loan_entry { get; set; }
        public DbSet<prl_loan_payment_summary> prl_loan_payment_summary { get; set; }
        public DbSet<prl_upload_pf_dividend_yearly> prl_upload_pf_dividend_yearly { get; set; }
        public DbSet<prl_job_level> prl_job_level { get; set; }
        public DbSet<prl_cost_centre> prl_cost_centre { get; set; }
        public DbSet<prl_users> prl_users { get; set; }
        public DbSet<prl_certificate_upload> prl_certificate_upload { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new prl_allowance_configurationMap());
            modelBuilder.Configurations.Add(new prl_allowance_headMap());
            modelBuilder.Configurations.Add(new prl_allowance_nameMap());
            modelBuilder.Configurations.Add(new prl_arrear_configurationMap());
            modelBuilder.Configurations.Add(new prl_bankMap());
            modelBuilder.Configurations.Add(new prl_bank_branchMap());
            modelBuilder.Configurations.Add(new prl_bonus_configurationMap());
            modelBuilder.Configurations.Add(new prl_bonus_holdMap());
            modelBuilder.Configurations.Add(new prl_bonus_nameMap());
            modelBuilder.Configurations.Add(new prl_bonus_processMap());
            modelBuilder.Configurations.Add(new prl_bonus_process_detailMap());
            //modelBuilder.Configurations.Add(new prl_companyMap());
            modelBuilder.Configurations.Add(new prl_deduction_configurationMap());
            modelBuilder.Configurations.Add(new prl_deduction_headMap());
            modelBuilder.Configurations.Add(new prl_deduction_nameMap());

            modelBuilder.Configurations.Add(new prl_departmentMap());
            modelBuilder.Configurations.Add(new prl_sub_departmentMap());
            modelBuilder.Configurations.Add(new prl_sub_sub_departmentMap());
           
            modelBuilder.Configurations.Add(new prl_designationMap());
            modelBuilder.Configurations.Add(new prl_divisionMap());
            modelBuilder.Configurations.Add(new prl_employeeMap());
            modelBuilder.Configurations.Add(new prl_employee_children_allowanceMap());
            modelBuilder.Configurations.Add(new prl_employee_detailsMap());
            modelBuilder.Configurations.Add(new prl_employee_discontinueMap());
            modelBuilder.Configurations.Add(new prl_employee_free_carMap());
            modelBuilder.Configurations.Add(new prl_employee_individual_allowanceMap());
            modelBuilder.Configurations.Add(new prl_employee_individual_deductionMap());
            modelBuilder.Configurations.Add(new prl_employee_leave_without_payMap());
            modelBuilder.Configurations.Add(new prl_employee_settlementMap());
            modelBuilder.Configurations.Add(new prl_employee_settlement_allowanceMap());
            modelBuilder.Configurations.Add(new prl_employee_settlement_deductionMap());
            modelBuilder.Configurations.Add(new prl_employee_settlement_detailMap());
            modelBuilder.Configurations.Add(new prl_employee_settlement_over_timeMap());
            modelBuilder.Configurations.Add(new prl_employee_tax_processMap());
            modelBuilder.Configurations.Add(new prl_employee_tax_process_detailMap());
            modelBuilder.Configurations.Add(new prl_employee_tax_slabMap());
            modelBuilder.Configurations.Add(new prl_employee_yearly_investmentMap());
            modelBuilder.Configurations.Add(new prl_fiscal_yearMap());
            modelBuilder.Configurations.Add(new prl_gradeMap());
            modelBuilder.Configurations.Add(new prl_income_tax_adjustmentMap());
            modelBuilder.Configurations.Add(new prl_income_tax_challanMap());
            modelBuilder.Configurations.Add(new prl_income_tax_parameterMap());
            modelBuilder.Configurations.Add(new prl_income_tax_parameter_detailsMap());
            modelBuilder.Configurations.Add(new prl_income_tax_refundMap());
            modelBuilder.Configurations.Add(new prl_leave_without_pay_settingsMap());
            modelBuilder.Configurations.Add(new prl_menuMap());
            modelBuilder.Configurations.Add(new prl_month_endMap());
            modelBuilder.Configurations.Add(new prl_over_timeMap());
            modelBuilder.Configurations.Add(new prl_over_time_amountMap());
            modelBuilder.Configurations.Add(new prl_over_time_configurationMap());
            modelBuilder.Configurations.Add(new prl_religionMap());
            modelBuilder.Configurations.Add(new prl_role_privilegeMap());
            modelBuilder.Configurations.Add(new prl_salary_allowancesMap());
            modelBuilder.Configurations.Add(new prl_salary_deductionsMap());
            modelBuilder.Configurations.Add(new prl_salary_holdMap());
            modelBuilder.Configurations.Add(new prl_salary_processMap());
            modelBuilder.Configurations.Add(new prl_salary_process_detailMap());
            modelBuilder.Configurations.Add(new prl_salary_reviewMap());
            modelBuilder.Configurations.Add(new prl_sub_menuMap());
            modelBuilder.Configurations.Add(new prl_upload_allowanceMap());
            modelBuilder.Configurations.Add(new prl_upload_bonusMap());
            modelBuilder.Configurations.Add(new prl_upload_deductionMap());
            modelBuilder.Configurations.Add(new prl_upload_time_card_entryMap());
            modelBuilder.Configurations.Add(new prl_upload_staff_allowanceMap());
            modelBuilder.Configurations.Add(new prl_allowance_staffMap());
            modelBuilder.Configurations.Add(new prl_workers_allowancesMap());

            modelBuilder.Configurations.Add(new prl_gf_settingMap());
            modelBuilder.Configurations.Add(new prl_loan_entryMap());
            modelBuilder.Configurations.Add(new prl_loan_payment_summaryMap());
            modelBuilder.Configurations.Add(new prl_upload_pf_dividend_yearlyMap());

            modelBuilder.Configurations.Add(new prl_job_levelMap());
            modelBuilder.Configurations.Add(new prl_cost_centreMap());

            modelBuilder.Configurations.Add(new prl_usersMap());
            modelBuilder.Configurations.Add(new prl_certificate_uploadMap());
        }
    }
}
