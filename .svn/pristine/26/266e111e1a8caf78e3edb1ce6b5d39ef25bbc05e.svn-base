using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_deduction_configurationMap : EntityTypeConfiguration<prl_deduction_configuration>
    {
        public prl_deduction_configurationMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.gender)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_deduction_configuration", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.deduction_name_id).HasColumnName("deduction_name_id");
            this.Property(t => t.is_active).HasColumnName("is_active");
            this.Property(t => t.is_monthly).HasColumnName("is_monthly");
            this.Property(t => t.is_taxable).HasColumnName("is_taxable");
            this.Property(t => t.is_individual).HasColumnName("is_individual");
            this.Property(t => t.gender).HasColumnName("gender");
            this.Property(t => t.is_confirmation_required).HasColumnName("is_confirmation_required");
            this.Property(t => t.depends_on_working_hour).HasColumnName("depends_on_working_hour");
            this.Property(t => t.activation_date).HasColumnName("activation_date");
            this.Property(t => t.deactivation_date).HasColumnName("deactivation_date");
            this.Property(t => t.project_rest_year).HasColumnName("project_rest_year");
            this.Property(t => t.once_off_deduction).HasColumnName("once_off_deduction");
            this.Property(t => t.flat_amount).HasColumnName("flat_amount");
            this.Property(t => t.percent_amount).HasColumnName("percent_amount");
            this.Property(t => t.max_amount).HasColumnName("max_amount");
            this.Property(t => t.min_amount).HasColumnName("min_amount");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_deduction_name)
                .WithMany(t => t.prl_deduction_configuration)
                .HasForeignKey(d => d.deduction_name_id);

        }
    }
}
