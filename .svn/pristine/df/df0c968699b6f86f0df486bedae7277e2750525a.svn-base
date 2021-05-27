using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_settlement_allowanceMap : EntityTypeConfiguration<prl_employee_settlement_allowance>
    {
        public prl_employee_settlement_allowanceMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_employee_settlement_allowance", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.settlement_id).HasColumnName("settlement_id");
            this.Property(t => t.allowance_id).HasColumnName("allowance_id");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.due_allowance_id).HasColumnName("due_allowance_id");
            this.Property(t => t.due_amount).HasColumnName("due_amount");

            // Relationships
            this.HasRequired(t => t.prl_employee_settlement)
                .WithMany(t => t.prl_employee_settlement_allowance)
                .HasForeignKey(d => d.settlement_id);

        }
    }
}
