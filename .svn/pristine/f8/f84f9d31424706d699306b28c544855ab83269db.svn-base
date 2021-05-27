using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_salary_reviewMap : EntityTypeConfiguration<prl_salary_review>
    {
        public prl_salary_reviewMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.increment_reason)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.description)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.is_arrear_calculated)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_salary_review", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.current_basic).HasColumnName("current_basic");
            this.Property(t => t.new_basic).HasColumnName("new_basic");
            this.Property(t => t.increment_reason).HasColumnName("increment_reason");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.effective_from).HasColumnName("effective_from");
            this.Property(t => t.is_arrear_calculated).HasColumnName("is_arrear_calculated");
            this.Property(t => t.arrear_calculated_date).HasColumnName("arrear_calculated_date");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_salary_review)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
