using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_workers_allowancesMap : EntityTypeConfiguration<prl_workers_allowances>
    {
        public prl_workers_allowancesMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_workers_allowances", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.allowance_process_id).HasColumnName("allowance_process_id");
            this.Property(t => t.salary_month).HasColumnName("salary_month");
            this.Property(t => t.calculation_for_days).HasColumnName("calculation_for_days");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.allowance_name_id).HasColumnName("allowance_name_id");
            this.Property(t => t.allowance_name).HasColumnName("allowance_name");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.arrear_amount).HasColumnName("arrear_amount");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_allowance_name)
                .WithMany(t => t.prl_workers_allowances)
                .HasForeignKey(d => d.allowance_name_id);
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_workers_allowances)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
