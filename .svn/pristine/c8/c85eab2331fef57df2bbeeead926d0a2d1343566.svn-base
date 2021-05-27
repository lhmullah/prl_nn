using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_deduction_nameMap : EntityTypeConfiguration<prl_deduction_name>
    {
        public prl_deduction_nameMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.gl_code)
                .HasMaxLength(20);

            this.Property(t => t.deduction_name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_deduction_name", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.deduction_head_id).HasColumnName("deduction_head_id");
            this.Property(t => t.gl_code).HasColumnName("gl_code");
            this.Property(t => t.deduction_name).HasColumnName("deduction_name");

            // Relationships
            this.HasMany(t => t.prl_grade)
                .WithMany(t => t.prl_deduction_name)
                .Map(m =>
                    {
                        m.ToTable("prl_deduction_grade_mapping", "payroll_system");
                        m.MapLeftKey("deduction_name_id");
                        m.MapRightKey("grade_id");
                    });

            this.HasRequired(t => t.prl_deduction_head)
                .WithMany(t => t.prl_deduction_name)
                .HasForeignKey(d => d.deduction_head_id);

        }
    }
}
