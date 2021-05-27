using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_children_allowanceMap : EntityTypeConfiguration<prl_employee_children_allowance>
    {
        public prl_employee_children_allowanceMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_children_allowance", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.no_of_children).HasColumnName("no_of_children");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.effective_from).HasColumnName("effective_from");
            this.Property(t => t.is_active).HasColumnName("is_active");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_children_allowance)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
