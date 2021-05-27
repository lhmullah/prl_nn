using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_role_privilegeMap : EntityTypeConfiguration<prl_role_privilege>
    {
        public prl_role_privilegeMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.role)
                .HasMaxLength(100);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_role_privilege", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.menu_id).HasColumnName("menu_id");
            this.Property(t => t.sub_menu_id).HasColumnName("sub_menu_id");
            this.Property(t => t.role).HasColumnName("role");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
