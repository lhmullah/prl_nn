using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_over_timeMap : EntityTypeConfiguration<prl_over_time>
    {
        public prl_over_timeMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.unit)
                .IsRequired()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("prl_over_time", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.unit).HasColumnName("unit");
            this.Property(t => t.max_value).HasColumnName("max_value");
        }
    }
}
