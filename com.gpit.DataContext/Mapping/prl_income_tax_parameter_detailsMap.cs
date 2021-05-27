using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_income_tax_parameter_detailsMap : EntityTypeConfiguration<prl_income_tax_parameter_details>
    {
        public prl_income_tax_parameter_detailsMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.assesment_year)
                .HasMaxLength(20);

            this.Property(t => t.gender)
                .HasMaxLength(65532);

            // Table & Column Mappings
            this.ToTable("prl_income_tax_parameter_details", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.income_tax_parameter_id).HasColumnName("income_tax_parameter_id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.assesment_year).HasColumnName("assesment_year");
            this.Property(t => t.gender).HasColumnName("gender");
            this.Property(t => t.max_tax_age).HasColumnName("max_tax_age");
            this.Property(t => t.max_investment_amount).HasColumnName("max_investment_amount");
            this.Property(t => t.max_investment_percentage).HasColumnName("max_investment_percentage");
            this.Property(t => t.max_inv_exempted_percentage).HasColumnName("max_inv_exempted_percentage");
            this.Property(t => t.max_amount_for_max_exemption_percent).HasColumnName("max_amount_for_max_exemption_percent");
            this.Property(t => t.min_inv_exempted_percentage).HasColumnName("min_inv_exempted_percentage");
            this.Property(t => t.min_tax_amount).HasColumnName("min_tax_amount");
            this.Property(t => t.max_house_rent_percentage).HasColumnName("max_house_rent_percentage");
            this.Property(t => t.house_rent_not_exceding).HasColumnName("house_rent_not_exceding");
            this.Property(t => t.max_conveyance_allowance).HasColumnName("max_conveyance_allowance");
            this.Property(t => t.free_car).HasColumnName("free_car");
            this.Property(t => t.lfa_exemption_percentage).HasColumnName("lfa_exemption_percentage");
            this.Property(t => t.medical_exemption_percentage).HasColumnName("medical_exemption_percentage");

            // Relationships
            this.HasOptional(t => t.prl_fiscal_year)
                .WithMany(t => t.prl_income_tax_parameter_details)
                .HasForeignKey(d => d.fiscal_year_id);

        }
    }
}
