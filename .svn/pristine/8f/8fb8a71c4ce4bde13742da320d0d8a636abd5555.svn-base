using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_tax_slabMap : EntityTypeConfiguration<prl_employee_tax_slab>
    {
        public prl_employee_tax_slabMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.parameter)
                .HasMaxLength(100);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_tax_slab", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.tax_process_id).HasColumnName("tax_process_id");
            this.Property(t => t.salary_process_id).HasColumnName("salary_process_id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.salary_date).HasColumnName("salary_date");
            this.Property(t => t.salary_month).HasColumnName("salary_month");
            this.Property(t => t.salary_year).HasColumnName("salary_year");
            this.Property(t => t.current_rate).HasColumnName("current_rate");
            this.Property(t => t.parameter).HasColumnName("parameter");
            this.Property(t => t.taxable_income).HasColumnName("taxable_income");
            this.Property(t => t.tax_liability).HasColumnName("tax_liability");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");

            // Relationships
            this.HasOptional(t => t.prl_employee_tax_process)
                .WithMany(t => t.prl_employee_tax_slab)
                .HasForeignKey(d => d.tax_process_id);

        }
    }
}
