using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_fiscal_yearMap : EntityTypeConfiguration<prl_fiscal_year>
    {
        public prl_fiscal_yearMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.fiscal_year)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.assesment_year)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("prl_fiscal_year", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.fiscal_year).HasColumnName("fiscal_year");
            this.Property(t => t.assesment_year).HasColumnName("assesment_year");
        }
    }
}
