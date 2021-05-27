using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_gradeMap : EntityTypeConfiguration<prl_grade>
    {
        public prl_gradeMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.grade)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("prl_grade", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.grade).HasColumnName("grade");
            this.Property(t => t.upper_basic).HasColumnName("upper_basic");
            this.Property(t => t.lower_basic).HasColumnName("lower_basic");

            // Relationships
            this.HasMany(t => t.prl_bonus_name)
                .WithMany(t => t.prl_grade)
                .Map(m =>
                    {
                        m.ToTable("prl_bonus_grade_mapping", "payroll_system");
                        m.MapLeftKey("grade_id");
                        m.MapRightKey("bonus_name_id");
                    });


        }
    }
}
