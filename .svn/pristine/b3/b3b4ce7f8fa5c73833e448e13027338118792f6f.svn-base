using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_leave_without_payMap : EntityTypeConfiguration<prl_employee_leave_without_pay>
    {
        public prl_employee_leave_without_payMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_leave_without_pay", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.setting_id).HasColumnName("setting_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.no_of_days).HasColumnName("no_of_days");
            this.Property(t => t.strat_date).HasColumnName("strat_date");
            this.Property(t => t.end_date).HasColumnName("end_date");
            this.Property(t => t.effective_month).HasColumnName("effective_month");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_leave_without_pay)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_leave_without_pay_settings)
                .WithMany(t => t.prl_employee_leave_without_pay)
                .HasForeignKey(d => d.setting_id);

        }
    }
}
