using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_income_tax_challanMap : EntityTypeConfiguration<prl_income_tax_challan>
    {
        public prl_income_tax_challanMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.challan_no)
                .HasMaxLength(50);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_income_tax_challan", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.company_id).HasColumnName("company_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.challan_no).HasColumnName("challan_no");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.challan_date).HasColumnName("challan_date");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
