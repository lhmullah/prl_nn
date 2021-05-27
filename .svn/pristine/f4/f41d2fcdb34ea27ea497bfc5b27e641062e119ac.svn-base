using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_sub_menuMap : EntityTypeConfiguration<prl_sub_menu>
    {
        public prl_sub_menuMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.sub_menu)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.view_name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.controller_name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.module)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_sub_menu", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.menu_id).HasColumnName("menu_id");
            this.Property(t => t.sub_menu).HasColumnName("sub_menu");
            this.Property(t => t.view_name).HasColumnName("view_name");
            this.Property(t => t.controller_name).HasColumnName("controller_name");
            this.Property(t => t.parent_id).HasColumnName("parent_id");
            this.Property(t => t.level).HasColumnName("level");
            this.Property(t => t.module).HasColumnName("module");

            // Relationships
            this.HasRequired(t => t.prl_menu)
                .WithMany(t => t.prl_sub_menu)
                .HasForeignKey(d => d.menu_id);

        }
    }
}
