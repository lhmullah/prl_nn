using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_allowance_nameMap : EntityTypeConfiguration<prl_allowance_name>
    {
        public prl_allowance_nameMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.allowance_name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.description)
                .HasMaxLength(250);

            this.Property(t => t.gl_code)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("prl_allowance_name", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.allowance_head_id).HasColumnName("allowance_head_id");
            this.Property(t => t.allowance_name).HasColumnName("allowance_name");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.gl_code).HasColumnName("gl_code");

            // Relationships
            this.HasMany(t => t.prl_grade)
                .WithMany(t => t.prl_allowance_name)
                .Map(m =>
                    {
                        m.ToTable("prl_allowance_grade_mapping", "payroll_system");
                        m.MapLeftKey("allowance_name_id");
                        m.MapRightKey("grade_id");
                    });

            this.HasRequired(t => t.prl_allowance_head)
                .WithMany(t => t.prl_allowance_name)
                .HasForeignKey(d => d.allowance_head_id);

        }
    }
}
