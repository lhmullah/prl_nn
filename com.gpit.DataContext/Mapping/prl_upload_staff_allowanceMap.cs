using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_upload_staff_allowanceMap : EntityTypeConfiguration<prl_upload_staff_allowance>
    {
        public prl_upload_staff_allowanceMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_upload_staff_allowance", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.allowance_name_id).HasColumnName("allowance_name_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.salary_month_year).HasColumnName("salary_month_year");
            this.Property(t => t.no_of_entry).HasColumnName("no_of_entry");
            this.Property(t => t.amount_or_percentage).HasColumnName("amount_or_percentage");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_allowance_name)
                .WithMany(t => t.prl_upload_staff_allowance)
                .HasForeignKey(d => d.allowance_name_id);
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_upload_staff_allowance)
                .HasForeignKey(d => d.emp_id);
        }
    }
}
