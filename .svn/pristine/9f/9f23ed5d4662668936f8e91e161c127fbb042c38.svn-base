using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_bonus_configurationMap : EntityTypeConfiguration<prl_bonus_configuration>
    {
        public prl_bonus_configurationMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.confirmed_emp)
                .HasMaxLength(65532);

            this.Property(t => t.is_festival)
                .HasMaxLength(65532);

            this.Property(t => t.gender_dependant)
                .HasMaxLength(65532);

            this.Property(t => t.is_taxable)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_bonus_configuration", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.bonus_name_id).HasColumnName("bonus_name_id");
            this.Property(t => t.confirmed_emp).HasColumnName("confirmed_emp");
            this.Property(t => t.is_festival).HasColumnName("is_festival");
            this.Property(t => t.gender_dependant).HasColumnName("gender_dependant");
            this.Property(t => t.is_taxable).HasColumnName("is_taxable");
            this.Property(t => t.flat_amount).HasColumnName("flat_amount");
            this.Property(t => t.percentage_of_basic).HasColumnName("percentage_of_basic");
            this.Property(t => t.number_of_basic).HasColumnName("number_of_basic");
            this.Property(t => t.basic_of_days).HasColumnName("basic_of_days");
            this.Property(t => t.effective_from).HasColumnName("effective_from");
            this.Property(t => t.effective_to).HasColumnName("effective_to");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_bonus_name)
                .WithMany(t => t.prl_bonus_configuration)
                .HasForeignKey(d => d.bonus_name_id);

        }
    }
}
