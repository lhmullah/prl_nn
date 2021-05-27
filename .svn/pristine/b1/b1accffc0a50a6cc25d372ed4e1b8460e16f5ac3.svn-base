using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_salary_holdMap : EntityTypeConfiguration<prl_salary_hold>
    {
        public prl_salary_holdMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.month)
                .IsRequired()
                .HasMaxLength(12);

            this.Property(t => t.year)
                .IsRequired()
                .HasMaxLength(4);

            this.Property(t => t.is_holded)
                .HasMaxLength(65532);

            this.Property(t => t.hold_reason)
                .HasMaxLength(250);

            this.Property(t => t.unhold_reason)
                .HasMaxLength(200);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_salary_hold", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.month).HasColumnName("month");
            this.Property(t => t.year).HasColumnName("year");
            this.Property(t => t.is_holded).HasColumnName("is_holded");
            this.Property(t => t.hold_reason).HasColumnName("hold_reason");
            this.Property(t => t.hold_from).HasColumnName("hold_from");
            this.Property(t => t.hold_to).HasColumnName("hold_to");
            this.Property(t => t.with_salary).HasColumnName("with_salary");
            this.Property(t => t.without_salary).HasColumnName("without_salary");
            this.Property(t => t.pf_continue).HasColumnName("pf_continue");
            this.Property(t => t.gf_continue).HasColumnName("gf_continue");
            this.Property(t => t.unhold_date).HasColumnName("unhold_date");
            this.Property(t => t.unhold_reason).HasColumnName("unhold_reason");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_salary_hold)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
