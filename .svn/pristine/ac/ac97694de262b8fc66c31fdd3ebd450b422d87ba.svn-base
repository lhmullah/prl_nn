using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_income_tax_adjustmentMap : EntityTypeConfiguration<prl_income_tax_adjustment>
    {
        public prl_income_tax_adjustmentMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_income_tax_adjustment", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.fiscal_year).HasColumnName("fiscal_year");
            this.Property(t => t.month_year).HasColumnName("month_year");
            this.Property(t => t.adjustment_amount).HasColumnName("adjustment_amount");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_income_tax_adjustment)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_fiscal_year)
                .WithMany(t => t.prl_income_tax_adjustment)
                .HasForeignKey(d => d.fiscal_year);

        }
    }
}
