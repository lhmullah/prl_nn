using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_over_time_configurationMap : EntityTypeConfiguration<prl_over_time_configuration>
    {
        public prl_over_time_configurationMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.formula)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("prl_over_time_configuration", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.over_time_id).HasColumnName("over_time_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.formula).HasColumnName("formula");

            // Relationships
            this.HasRequired(t => t.prl_over_time)
                .WithMany(t => t.prl_over_time_configuration)
                .HasForeignKey(d => d.over_time_id);

        }
    }
}
