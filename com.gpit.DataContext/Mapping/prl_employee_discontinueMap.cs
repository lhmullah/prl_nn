using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_employee_discontinueMap : EntityTypeConfiguration<prl_employee_discontinue>
    {
        public prl_employee_discontinueMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.is_active)
                .HasMaxLength(65532);

            this.Property(t => t.discontination_type)
                .HasMaxLength(65532);

            this.Property(t => t.with_salary)
                .HasMaxLength(65532);

            this.Property(t => t.without_salary)
                .HasMaxLength(65532);


            this.Property(t => t.remarks)
                .HasMaxLength(200);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_discontinue", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.is_active).HasColumnName("is_active");
            this.Property(t => t.discontinue_date).HasColumnName("discontinue_date");
            this.Property(t => t.continution_date).HasColumnName("continution_date");
            this.Property(t => t.discontination_type).HasColumnName("discontination_type");
            this.Property(t => t.with_salary).HasColumnName("with_salary");
            this.Property(t => t.without_salary).HasColumnName("without_salary");
            this.Property(t => t.remarks).HasColumnName("remarks");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_discontinue)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
