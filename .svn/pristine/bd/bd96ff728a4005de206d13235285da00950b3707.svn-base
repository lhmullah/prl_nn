using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AutoMapper;
using com.gpit.Model;
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
            ConfigLocation();

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


            ConfigDeductionUploadData();
            ConfigAllowanceUploadData();

            ConfigOvertime();
            ConfigOvertimeConf();
            ConfigTimeCard();
            ConfigSalaryReview();


            ConfigureEmployeeFreeCar();

            ConfigureEmployeeChildrenAllowance();

            ConfigureLeaveWithoutPaySetting();

            ConfigureEmployeeLeaveWithoutPay();

            ConfigureRolePrivilege();

            ConfigureBonusProcessDetail();

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

        private static void ConfigOvertime()
        {
            Mapper.CreateMap<Overtime, prl_over_time>();
            Mapper.CreateMap<prl_over_time, Overtime>();
        }

        private static void ConfigOvertimeConf()
        {
            Mapper.CreateMap<OTConfiguration, prl_over_time_configuration>();
            Mapper.CreateMap<prl_over_time_configuration, OTConfiguration>();
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
                .ForMember(s => s.is_taxable, m => m.MapFrom(x => (x.is_taxable != null) ? Convert.ToSByte(x.is_taxable) : Convert.ToSByte(false)))
                .ForMember(s => s.is_individual, m => m.MapFrom(x => (x.is_individual != null) ? Convert.ToSByte(x.is_individual) : Convert.ToSByte(false)))
                .ForMember(s => s.depends_on_working_hour, m => m.MapFrom(x => (x.depends_on_working_hour != null) ? Convert.ToSByte(x.depends_on_working_hour) : Convert.ToSByte(false)))
                .ForMember(s => s.project_rest_year, m => m.MapFrom(x => (x.project_rest_year != null) ? Convert.ToSByte(x.project_rest_year) : Convert.ToSByte(false)))
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
                .ForMember(s => s.is_pf_member, m => m.MapFrom(x => (x.is_pf_member != null) ? Convert.ToSByte(x.is_pf_member) : Convert.ToSByte(false)))
                .ForMember(s => s.is_gf_member, m => m.MapFrom(x => (x.is_gf_member != null) ? Convert.ToSByte(x.is_gf_member) : Convert.ToSByte(false)))
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
        private static void ConfigLocation()
        {
            Mapper.CreateMap<prl_location, PostingLocation>();
            Mapper.CreateMap<PostingLocation, prl_location>();
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
    }
}
