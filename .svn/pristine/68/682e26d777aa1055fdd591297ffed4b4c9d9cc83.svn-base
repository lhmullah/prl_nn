using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_settlement_detailMap : EntityTypeConfiguration<prl_employee_settlement_detail>
    {
        public prl_employee_settlement_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_employee_settlement_detail", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.settlement_id).HasColumnName("settlement_id");
            this.Property(t => t.bonus_name_id_earning).HasColumnName("bonus_name_id_earning");
            this.Property(t => t.bonus_earning_amount).HasColumnName("bonus_earning_amount");
            this.Property(t => t.bonus_name_id_deductions).HasColumnName("bonus_name_id_deductions");
            this.Property(t => t.bonus_deduction_amount).HasColumnName("bonus_deduction_amount");
            this.Property(t => t.no_of_days_earning).HasColumnName("no_of_days_earning");
            this.Property(t => t.no_of_days_deduction).HasColumnName("no_of_days_deduction");

            // Relationships
            this.HasRequired(t => t.prl_employee_settlement)
                .WithMany(t => t.prl_employee_settlement_detail)
                .HasForeignKey(d => d.settlement_id);

        }
    }
}
