using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_free_carMap : EntityTypeConfiguration<prl_employee_free_car>
    {
        public prl_employee_free_carMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.is_active)
                .HasMaxLength(65532);

            this.Property(t => t.is_projected)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_free_car", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.is_active).HasColumnName("is_active");
            this.Property(t => t.is_projected).HasColumnName("is_projected");
            this.Property(t => t.start_date).HasColumnName("start_date");
            this.Property(t => t.end_date).HasColumnName("end_date");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_free_car)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
