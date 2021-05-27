using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_gf_settingMap : EntityTypeConfiguration<prl_gf_setting>
    {
        public prl_gf_settingMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_gf_setting", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.service_length_from).HasColumnName("service_length_from");
            this.Property(t => t.service_length_to).HasColumnName("service_length_to");
            this.Property(t => t.number_of_basic).HasColumnName("number_of_basic");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
