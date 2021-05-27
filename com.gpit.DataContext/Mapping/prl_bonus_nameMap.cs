using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_bonus_nameMap : EntityTypeConfiguration<prl_bonus_name>
    {
        public prl_bonus_nameMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_bonus_name", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
        }
    }
}
