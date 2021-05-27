using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_menuMap : EntityTypeConfiguration<prl_menu>
    {
        public prl_menuMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.menue_name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.remarks)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.module)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_menu", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.menue_name).HasColumnName("menue_name");
            this.Property(t => t.remarks).HasColumnName("remarks");
            this.Property(t => t.module).HasColumnName("module");
        }
    }
}
