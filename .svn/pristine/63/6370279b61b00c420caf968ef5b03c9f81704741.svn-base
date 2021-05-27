using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_tax_processMap : EntityTypeConfiguration<prl_employee_tax_process>
    {
        public prl_employee_tax_processMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_tax_process", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.salary_process_id).HasColumnName("salary_process_id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.salary_month).HasColumnName("salary_month");
            this.Property(t => t.yearly_tax).HasColumnName("yearly_tax");
            this.Property(t => t.monthly_tax).HasColumnName("monthly_tax");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_tax_process)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_fiscal_year)
                .WithMany(t => t.prl_employee_tax_process)
                .HasForeignKey(d => d.fiscal_year_id);
            this.HasRequired(t => t.prl_salary_process)
                .WithMany(t => t.prl_employee_tax_process)
                .HasForeignKey(d => d.salary_process_id);

        }
    }
}
