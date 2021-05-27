using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_salary_deductionsMap : EntityTypeConfiguration<prl_salary_deductions>
    {
        public prl_salary_deductionsMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_salary_deductions", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.salary_process_id).HasColumnName("salary_process_id");
            this.Property(t => t.salary_month).HasColumnName("salary_month");
            this.Property(t => t.calculation_for_days).HasColumnName("calculation_for_days");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.deduction_name_id).HasColumnName("deduction_name_id");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.arrear_amount).HasColumnName("arrear_amount");

            // Relationships
            this.HasRequired(t => t.prl_deduction_name)
                .WithMany(t => t.prl_salary_deductions)
                .HasForeignKey(d => d.deduction_name_id);
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_salary_deductions)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_salary_process)
                .WithMany(t => t.prl_salary_deductions)
                .HasForeignKey(d => d.salary_process_id);

        }
    }
}
