using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_upload_bonusMap : EntityTypeConfiguration<prl_upload_bonus>
    {
        public prl_upload_bonusMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.is_percentage)
                .IsRequired()
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_upload_bonus", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.bonus_name_id).HasColumnName("bonus_name_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.fiscal_year).HasColumnName("fiscal_year");
            this.Property(t => t.month_year).HasColumnName("month_year");
            this.Property(t => t.is_percentage).HasColumnName("is_percentage");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.effective_from).HasColumnName("effective_from");
            this.Property(t => t.effective_to).HasColumnName("effective_to");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_bonus_name)
                .WithMany(t => t.prl_upload_bonus)
                .HasForeignKey(d => d.bonus_name_id);
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_upload_bonus)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
