using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_leave_without_pay_settingsMap : EntityTypeConfiguration<prl_leave_without_pay_settings>
    {
        public prl_leave_without_pay_settingsMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.Lwp_type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_leave_without_pay_settings", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.Lwp_type).HasColumnName("Lwp_type");
            this.Property(t => t.allowance_id).HasColumnName("allowance_id");
            this.Property(t => t.percentage_of_basic).HasColumnName("percentage_of_basic");
            this.Property(t => t.percentage_of_allowance).HasColumnName("percentage_of_allowance");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
