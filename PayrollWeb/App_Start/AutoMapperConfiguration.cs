using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AutoMapper;
using com.linde.Model;
using Microsoft.Ajax.Utilities;
using PayrollWeb.ViewModels;


namespace PayrollWeb.App_Start
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            //Dept
            ConfigDepartment();
            ConfigDepartmentR();

            ConfigSubDepartment();
            ConfigSubDepartmentR();

            ConfigSubSubDepartment();
            ConfigSubSubDepartmentR();

            //Job Level
            ConfigJobLevel();
            ConfigJobLevelR();

            //Cost Centre
            ConfigCostCentre();
            ConfigCostCentreR();

            //Desig
            ConfigDBToDesignation();
            ConfigDesignatioToDb();

            //Bank
            ConfigDBToBank();
            ConfigBankToDB();

            //Bank
            ConfigDBToBankBranch();
            ConfigBankBranchToDB();

            //Allowance Head
            ConfigDBToAllowanceHead();
            ConfigAllowanceHeadToDB();

            //Allowance Name
            ConfigDBToAllowanceName();
            ConfigAllowanceNameToDB();

            //Deduction Head
            ConfigDBToDeductionHead();
            ConfigDeductionHeadToDB();

            //Grade
            ConfigDBToGrade();
            ConfigGradeToDB();

            //Fiscal Year
            ConfigDBToFiscalYr();
            ConfigFiscalYrToDB();

            //Bonus Name
            ConfigDBToBonusName();
            ConfigBonusNameToDB();
            ConfigureBonus();

            //Company
            ConfigDBToCompanyInfo();
            ConfigCompanyInfoToDB();

            //Employee
            ConfigDBToEmployeeInfo();
            ConfigEmployeeInfoToDB();

            //Employee Details
            ConfigDBToEmployeeDetailsInfo();
            ConfigEmployeeDetailsInfoToDB();

            ConfigureEmployeeDiscontinue();

            //Deduction Name
            ConfiDeductionName();

            ConfiDeductionConfiguration();
            ConfigAllowanceConfiguration();

            ConfigureDBSubMenu();
            ConfigureSubMenuDB();

            ConfigureMenu();

            ConfigureDivision();
            ConfigureReligion();


            ConfigIndividualDeduction();

            ConfigIndividualAllowance();

            ConfigureBonusHold();

            ConfigureBonusProcess();

            
            //Incoem Tax
            ConfigureIncomeTaxParameter();
            ConfigureIncomeTaxParameterDetail();
            ConfigureTaxProcess();
            ConfigureTaxProcessDetail();
            ConfigureTaxSlab();

            ConfigureYearlyInvestment();

            ConfigDeductionUploadData();
            ConfigAllowanceUploadData();
            ConfigAllowanceUploadStaffData();


            ConfigTimeCard();
            ConfigSalaryReview();


            ConfigureEmployeeFreeCar();

            ConfigureEmployeeChildrenAllowance();

            ConfigureLeaveWithoutPaySetting();

            ConfigureEmployeeLeaveWithoutPay();

            ConfigureRolePrivilege();

            ConfigureBonusProcessDetail();


            //Gratuity Fund
            ConfigDBToGFparameterSetting();
            ConfigGFparameterSettingToDB();

            //Loan Payment Summary
            ConfigDBToLoanPaymentSummary();
            ConfigLoanPaymentSummaryToDB();

            //Loan Entry
            ConfigDBToLoanEntry();
            ConfigLoanEntryToDB();

            //User
            ConfigDBToUser();
            ConfigUserToDB();

            //Certificates
            ConfigDBToCertificates();
            ConfigCertificatesToDB();


            //Mapper.AssertConfigurationIsValid();
        }

        private static void ConfigureEmployeeLeaveWithoutPay()
        {
            Mapper.CreateMap<EmployeeLeaveWithoutPay, prl_employee_leave_without_pay>();
            Mapper.CreateMap<prl_employee_leave_without_pay, EmployeeLeaveWithoutPay>();
        }

        private static void ConfigureLeaveWithoutPaySetting()
        {
            Mapper.CreateMap<LeaveWithoutPaySetting, prl_leave_without_pay_settings>();
            Mapper.CreateMap<prl_leave_without_pay_settings, LeaveWithoutPaySetting>();
        }

        private static void ConfigureEmployeeChildrenAllowance()
        {
            Mapper.CreateMap<prl_employee_children_allowance, ChildrenAllowance>();
            Mapper.CreateMap<ChildrenAllowance, prl_employee_children_allowance>()
                .ForMember(s => s.is_active, m => m.MapFrom(x => (x.is_active != null) ? Convert.ToSByte(x.is_active) : (sbyte?)null));
        }

        private static void ConfigureEmployeeFreeCar()
        {
            Mapper.CreateMap<EmployeeFreeCar, prl_employee_free_car>();
            Mapper.CreateMap<prl_employee_free_car, EmployeeFreeCar>();
        }

        private static void ConfigureTaxSlab()
        {
            Mapper.CreateMap<EmployeeTaxSlab, prl_employee_tax_slab>();
            Mapper.CreateMap<prl_employee_tax_slab, EmployeeTaxSlab>();
        }

        private static void ConfigureTaxProcessDetail()
        {
            Mapper.CreateMap<EmployeeTaxProcessDetail, prl_employee_tax_process_detail>();
            Mapper.CreateMap<prl_employee_tax_process_detail, EmployeeTaxProcessDetail>();
        }

        private static void ConfigureTaxProcess()
        {
            Mapper.CreateMap<EmployeeTaxProcess, prl_employee_tax_process>();
            Mapper.CreateMap<prl_employee_tax_process, EmployeeTaxProcess>();
        }

        private static void ConfigureYearlyInvestment()
        {
            Mapper.CreateMap<EmployeeYearlyInvestment, prl_employee_yearly_investment>();
            Mapper.CreateMap<prl_employee_yearly_investment, EmployeeYearlyInvestment>();
        }

        private static void ConfigTimeCard()
        {
            Mapper.CreateMap<TimeCard, prl_upload_time_card_entry>();
            Mapper.CreateMap<prl_upload_time_card_entry, TimeCard>();
        }
		
		private static void ConfigAllowanceUploadData()
        {
            Mapper.CreateMap<prl_upload_allowance, AllowanceUploadData>();
            Mapper.CreateMap<AllowanceUploadData, prl_upload_allowance>();
        }

        private static void ConfigAllowanceUploadStaffData()
        {
            Mapper.CreateMap<prl_upload_staff_allowance, AllowanceUploadStaffData>();
            Mapper.CreateMap<AllowanceUploadStaffData, prl_upload_staff_allowance>();
        }

        private static void ConfigDeductionUploadData()
        {
            Mapper.CreateMap<prl_upload_deduction, DeductionUploadData>();
            Mapper.CreateMap<DeductionUploadData, prl_upload_deduction>();
        }


        private static void ConfigureIncomeTaxParameter()
        {
            Mapper.CreateMap<prl_income_tax_parameter, IncomeTaxParameter>();
            Mapper.CreateMap<IncomeTaxParameter, prl_income_tax_parameter>();
        }

        private static void ConfigureIncomeTaxParameterDetail()
        {
            Mapper.CreateMap<prl_income_tax_parameter_details, IncomeTaxParameterDetail>();
            Mapper.CreateMap<IncomeTaxParameterDetail, prl_income_tax_parameter_details>();
        }

        private static void ConfigDBToCertificates()
        {
            Mapper.CreateMap<prl_certificate_upload, CertificateUploadVM>();
        }

        private static void ConfigCertificatesToDB()
        {
            Mapper.CreateMap<CertificateUploadVM, prl_certificate_upload>();
        }

        private static void ConfigureBonusProcess()
        {
            Mapper.CreateMap<prl_bonus_process, BonusProcess>();
            Mapper.CreateMap<BonusProcess, prl_bonus_process>();
        }

        private static void ConfigureBonusProcessDetail()
        {
            Mapper.CreateMap<prl_bonus_process_detail, BonusProcessDetail>();
            Mapper.CreateMap<BonusProcessDetail, prl_bonus_process_detail>();
        }

        private static void ConfigureBonusHold()
        {
            Mapper.CreateMap<prl_bonus_hold, BonusHold>();
            Mapper.CreateMap<BonusHold, prl_bonus_hold>();
        }

        private static void ConfigIndividualAllowance()
        {
            Mapper.CreateMap<prl_employee_individual_allowance, EmployeeIndividualAllowance>();
            Mapper.CreateMap<EmployeeIndividualAllowance, prl_employee_individual_allowance>();
        }

        private static void ConfigIndividualDeduction()
        {
            Mapper.CreateMap<prl_employee_individual_deduction, EmployeeIndividualDeduction>();
            Mapper.CreateMap<EmployeeIndividualDeduction, prl_employee_individual_deduction>();
        }
		
		private static void ConfigureBonus()
        {
            Mapper.CreateMap<prl_bonus_configuration, BonusConfiguration>();
            Mapper.CreateMap<BonusConfiguration, prl_bonus_configuration>();
        }

        private static void ConfigureDivision()
        {
            Mapper.CreateMap<prl_division, Division>();
            Mapper.CreateMap<Division, prl_division>();
        }

        private static void ConfigureMenu()
        { 
            Mapper.CreateMap<prl_menu, Menu>();
            Mapper.CreateMap<Menu, prl_menu>();
        }

        private static void ConfigureDBSubMenu()
        {
            Mapper.CreateMap<prl_sub_menu, SubMenu>();
        }

        private static void ConfigureSubMenuDB()
        {
            Mapper.CreateMap<SubMenu, prl_sub_menu>();
        }

        private static void ConfiDeductionConfiguration()
        {
            Mapper.CreateMap<prl_deduction_configuration, DeductionConfiguration>();
            Mapper.CreateMap<DeductionConfiguration, prl_deduction_configuration>()
                .ForMember(s => s.is_confirmation_required,m =>m.MapFrom(x =>(x.is_confirmation_required != null) ? Convert.ToSByte(x.is_confirmation_required) : (sbyte?) null))
                .ForMember(s => s.is_monthly, m => m.MapFrom(x => (x.is_monthly != null) ? Convert.ToSByte(x.is_monthly) :  Convert.ToSByte(false)))
                .ForMember(s => s.is_taxable, m => m.MapFrom(x => (x.is_taxable != null) ? Convert.ToSByte(x.is_taxable) : Convert.ToSByte(false)))
                .ForMember(s => s.is_individual, m => m.MapFrom(x => (x.is_individual != null) ? Convert.ToSByte(x.is_individual) : Convert.ToSByte(false)))
                .ForMember(s => s.depends_on_working_hour, m => m.MapFrom(x => (x.depends_on_working_hour != null) ? Convert.ToSByte(x.depends_on_working_hour) : Convert.ToSByte(false)))
                .ForMember(s => s.project_rest_year, m => m.MapFrom(x => (x.project_rest_year != null) ? Convert.ToSByte(x.project_rest_year) : Convert.ToSByte(false)))
                .ForMember(s => s.is_active, m => m.MapFrom(x => (x.is_active != null) ? Convert.ToSByte(x.is_active) : Convert.ToSByte(false)))
                ;
        }
		
		 private static void ConfigAllowanceConfiguration()
        {
            Mapper.CreateMap<prl_allowance_configuration, AllowanceConfiguration>();
            Mapper.CreateMap<AllowanceConfiguration, prl_allowance_configuration>()
                .ForMember(s => s.is_confirmation_required, m => m.MapFrom(x => (x.is_confirmation_required != null) ? Convert.ToSByte(x.is_confirmation_required) : (sbyte?)null))
                .ForMember(s => s.is_active, m => m.MapFrom(x => (x.is_active != null) ? Convert.ToSByte(x.is_active) : Convert.ToSByte(false)))
                .ForMember(s => s.is_monthly, m => m.MapFrom(x => (x.is_monthly != null) ? Convert.ToSByte(x.is_monthly) : Convert.ToSByte(false)))
                .ForMember(s => s.is_once_off_tax, m => m.MapFrom(x => (x.is_once_off_tax != null) ? Convert.ToSByte(x.is_once_off_tax) : Convert.ToSByte(false)))
                .ForMember(s => s.is_taxable, m => m.MapFrom(x => (x.is_taxable != null) ? Convert.ToSByte(x.is_taxable) : Convert.ToSByte(false)))
                .ForMember(s => s.is_individual, m => m.MapFrom(x => (x.is_individual != null) ? Convert.ToSByte(x.is_individual) : Convert.ToSByte(false)))
                .ForMember(s => s.depends_on_working_hour, m => m.MapFrom(x => (x.depends_on_working_hour != null) ? Convert.ToSByte(x.depends_on_working_hour) : Convert.ToSByte(false)))
                .ForMember(s => s.project_rest_year, m => m.MapFrom(x => (x.project_rest_year != null) ? Convert.ToSByte(x.project_rest_year) : Convert.ToSByte(false)))
                .ForMember(s => s.is_once_off_tax, m => m.MapFrom(x => (x.is_once_off_tax != null) ? Convert.ToSByte(x.is_once_off_tax) : Convert.ToSByte(false)))
                ;
        }

        private static void ConfiDeductionName()
        {
            Mapper.CreateMap<prl_deduction_name, DeductionName>()
                .ForMember(s => s.id,m =>m.MapFrom(x=>x.id));
            Mapper.CreateMap<DeductionName, prl_deduction_name>();
        }

        private static void ConfigDepartment()
        {
            Mapper.CreateMap<prl_department, Department>();
        }

        private static void ConfigDepartmentR()
        {
            Mapper.CreateMap<Department, prl_department>();
                //.ForMember(d => d., m => m.MapFrom(s => s.id))
                //;
        }

        private static void ConfigSubDepartment()
        {
            Mapper.CreateMap<prl_sub_department, SubDepartment>();
        }
        private static void ConfigSubDepartmentR()
        {
            Mapper.CreateMap<SubDepartment, prl_sub_department>();
                
        }

        private static void ConfigSubSubDepartment()
        {
            Mapper.CreateMap<prl_sub_sub_department, SubSubDepartment>();
        }

        private static void ConfigSubSubDepartmentR()
        {
            Mapper.CreateMap<SubSubDepartment, prl_sub_sub_department>();  
        }

        private static void ConfigJobLevel()
        {
            Mapper.CreateMap<prl_job_level, JobLevel>();
        }
        private static void ConfigJobLevelR()
        {
            Mapper.CreateMap<JobLevel, prl_job_level>();
        }

        private static void ConfigCostCentre()
        {
            Mapper.CreateMap<prl_cost_centre, CostCentre>();
        }
        private static void ConfigCostCentreR()
        {
            Mapper.CreateMap<CostCentre, prl_cost_centre>();
        }

        private static void ConfigDBToDesignation()
        {
            Mapper.CreateMap<prl_designation, Designation>();
        }
        private static void ConfigDesignatioToDb()
        {
            Mapper.CreateMap<Designation, prl_designation>();
        }

        private static void ConfigDBToBank()
        {
            Mapper.CreateMap<prl_bank, Bank>();
        }

        private static void ConfigBankToDB()
        {
            Mapper.CreateMap<Bank, prl_bank>();
        }

        private static void ConfigDBToBankBranch()
        {
            Mapper.CreateMap<prl_bank_branch, BankBranch>();
        }

        private static void ConfigBankBranchToDB()
        {
            Mapper.CreateMap<BankBranch, prl_bank_branch>();
        }

        private static void ConfigDBToAllowanceHead()
        {
            Mapper.CreateMap<prl_allowance_head, AllowanceHead>();
        }

        private static void ConfigAllowanceHeadToDB()
        {
            Mapper.CreateMap<AllowanceHead, prl_allowance_head>();
        }

        private static void ConfigDBToDeductionHead()
        {
            Mapper.CreateMap<prl_deduction_head, DeductionHead>();
        }

        private static void ConfigDeductionHeadToDB()
        {
            Mapper.CreateMap<DeductionHead, prl_deduction_head>();
        }

        private static void ConfigDBToGrade()
        {
            Mapper.CreateMap<prl_grade, Grade>();
        }

        private static void ConfigGradeToDB()
        {
            Mapper.CreateMap<Grade, prl_grade>();
        }

        private static void ConfigDBToFiscalYr()
        {
            Mapper.CreateMap<prl_fiscal_year, FiscalYr>();
        }

        private static void ConfigFiscalYrToDB()
        {
            Mapper.CreateMap<FiscalYr, prl_fiscal_year>();
        }

        private static void ConfigDBToGFparameterSetting()
        {
            Mapper.CreateMap<prl_gf_setting, GratuityFundParameter>();
        }

        private static void ConfigGFparameterSettingToDB()
        {
            Mapper.CreateMap<GratuityFundParameter, prl_gf_setting>();
        }

        private static void ConfigDBToLoanPaymentSummary()
        {
            Mapper.CreateMap<prl_loan_payment_summary, LoanSummary>();
        }

        private static void ConfigLoanPaymentSummaryToDB()
        {
            Mapper.CreateMap<LoanSummary, prl_loan_payment_summary>();
        }

        private static void ConfigDBToLoanEntry()
        {
            Mapper.CreateMap<prl_loan_entry, LoanEntry>();
        }

        private static void ConfigLoanEntryToDB()
        {
            Mapper.CreateMap<LoanEntry, prl_loan_entry>();
        }

        private static void ConfigDBToBonusName()
        {
            Mapper.CreateMap<prl_bonus_name, BonusName>();
        }

        private static void ConfigBonusNameToDB()
        {
            Mapper.CreateMap<BonusName, prl_bonus_name>();
        }

        private static void ConfigDBToCompanyInfo()
        {
            Mapper.CreateMap<prl_company, Company>();
        }

        private static void ConfigCompanyInfoToDB()
        {
            Mapper.CreateMap<Company, prl_company>();
        }

        private static void ConfigDBToEmployeeInfo()
        {
            Mapper.CreateMap<prl_employee, Employee>();
        }

        private static void ConfigEmployeeInfoToDB()
        {
            Mapper.CreateMap<Employee, prl_employee>()
                .ForMember(s => s.is_confirmed, m => m.MapFrom(x => (x.is_confirmed != null) ? Convert.ToSByte(x.is_confirmed) : Convert.ToSByte(false)))
                .ForMember(s => s.is_active, m => m.MapFrom(x => (x.is_active != null) ? Convert.ToSByte(x.is_active) : Convert.ToSByte(false)))
                ;
        }

        private static void ConfigDBToEmployeeDetailsInfo()
        {
            Mapper.CreateMap<prl_employee_details, EmployeeDetails>();
        }

        private static void ConfigEmployeeDetailsInfoToDB()
        {
            Mapper.CreateMap<EmployeeDetails, prl_employee_details>();
        }

        private static void ConfigureEmployeeDiscontinue()
        {
            Mapper.CreateMap<EmployeeDiscontinue, prl_employee_discontinue>();
            Mapper.CreateMap<prl_employee_discontinue, EmployeeDiscontinue>();
        }

        private static void ConfigDBToAllowanceName()
        {
            Mapper.CreateMap<prl_allowance_name, AllowanceName>();
        }

        private static void ConfigAllowanceNameToDB()
        {
            Mapper.CreateMap<AllowanceName, prl_allowance_name>();
        }
        private static void ConfigureReligion()
        {
            Mapper.CreateMap<prl_religion, Religion>();
            Mapper.CreateMap<Religion, prl_religion>();
        }

        private static void ConfigSalaryReview()
        {
            Mapper.CreateMap<prl_salary_review, SalaryReview>();
            Mapper.CreateMap<SalaryReview, prl_salary_review>();
        }

        private static void ConfigureRolePrivilege()
        {
            Mapper.CreateMap<RolePrivilege, prl_role_privilege>();
            Mapper.CreateMap<prl_role_privilege, RolePrivilege>();
        }

        private static void ConfigDBToUser()
        {
            Mapper.CreateMap<prl_users, Users>();
        }

        private static void ConfigUserToDB()
        {
            Mapper.CreateMap<Users, prl_users>();
        }
    }
}
