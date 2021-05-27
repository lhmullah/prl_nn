using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_settlement_deductionMap : EntityTypeConfiguration<prl_employee_settlement_deduction>
    {
        public prl_employee_settlement_deductionMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_employee_settlement_deduction", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.settlement_id).HasColumnName("settlement_id");
            this.Property(t => t.deduction_id).HasColumnName("deduction_id");
            this.Property(t => t.amount).HasColumnName("amount");

            // Relationships
            this.HasOptional(t => t.prl_employee_settlement)
                .WithMany(t => t.prl_employee_settlement_deduction)
                .HasForeignKey(d => d.settlement_id);

        }
    }
}
