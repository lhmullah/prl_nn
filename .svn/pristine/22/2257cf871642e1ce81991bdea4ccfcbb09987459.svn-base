using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_settlementMap : EntityTypeConfiguration<prl_employee_settlement>
    {
        public prl_employee_settlementMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_settlement", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.pf_own_amount).HasColumnName("pf_own_amount");
            this.Property(t => t.pf_company_amount).HasColumnName("pf_company_amount");
            this.Property(t => t.gf_amount).HasColumnName("gf_amount");
            this.Property(t => t.fractional_salary_earning).HasColumnName("fractional_salary_earning");
            this.Property(t => t.bonus_earning_amount).HasColumnName("bonus_earning_amount");
            this.Property(t => t.ot_amount).HasColumnName("ot_amount");
            this.Property(t => t.other_allowances).HasColumnName("other_allowances");
            this.Property(t => t.total_employee_earnings).HasColumnName("total_employee_earnings");
            this.Property(t => t.fractional_salary_deduction).HasColumnName("fractional_salary_deduction");
            this.Property(t => t.bonus_deduction).HasColumnName("bonus_deduction");
            this.Property(t => t.other_deductions).HasColumnName("other_deductions");
            this.Property(t => t.total_company_earnings).HasColumnName("total_company_earnings");
            this.Property(t => t.settlement_date).HasColumnName("settlement_date");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_settlement)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
