using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_over_time_amountMap : EntityTypeConfiguration<prl_over_time_amount>
    {
        public prl_over_time_amountMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_over_time_amount", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.over_time_config_id).HasColumnName("over_time_config_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.month_year).HasColumnName("month_year");
            this.Property(t => t.time_card_upload_id).HasColumnName("time_card_upload_id");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.actual_total).HasColumnName("actual_total");
            this.Property(t => t.pay_total).HasColumnName("pay_total");
            this.Property(t => t.arrear_amount).HasColumnName("arrear_amount");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_over_time_amount)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_over_time_configuration)
                .WithMany(t => t.prl_over_time_amount)
                .HasForeignKey(d => d.over_time_config_id);
            this.HasOptional(t => t.prl_upload_time_card_entry)
                .WithMany(t => t.prl_over_time_amount)
                .HasForeignKey(d => d.time_card_upload_id);

        }
    }
}
