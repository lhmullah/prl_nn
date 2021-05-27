using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_locationMap : EntityTypeConfiguration<prl_location>
    {
        public prl_locationMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.location_name)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("prl_location", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.location_name).HasColumnName("location_name");
        }
    }
}
