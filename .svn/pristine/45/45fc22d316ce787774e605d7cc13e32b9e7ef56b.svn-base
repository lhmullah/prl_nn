using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_settlement_over_timeMap : EntityTypeConfiguration<prl_employee_settlement_over_time>
    {
        public prl_employee_settlement_over_timeMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_employee_settlement_over_time", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.settlement_id).HasColumnName("settlement_id");
            this.Property(t => t.overtime_id).HasColumnName("overtime_id");
            this.Property(t => t.amount).HasColumnName("amount");
        }
    }
}
