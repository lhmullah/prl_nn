using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_cost_centreMap : EntityTypeConfiguration<prl_cost_centre>
    {
        public prl_cost_centreMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.cost_centre_name)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("prl_cost_centre", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.cost_centre_name).HasColumnName("cost_centre_name");
        }
    }
}
