using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_income_tax_parameterMap : EntityTypeConfiguration<prl_income_tax_parameter>
    {
        public prl_income_tax_parameterMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.assessment_year)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.gender)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_income_tax_parameter", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.assessment_year).HasColumnName("assessment_year");
            this.Property(t => t.gender).HasColumnName("gender");
            this.Property(t => t.slab_mininum_amount).HasColumnName("slab_mininum_amount");
            this.Property(t => t.slab_maximum_amount).HasColumnName("slab_maximum_amount");
            this.Property(t => t.slab_percentage).HasColumnName("slab_percentage");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_fiscal_year)
                .WithMany(t => t.prl_income_tax_parameter)
                .HasForeignKey(d => d.fiscal_year_id);

        }
    }
}
