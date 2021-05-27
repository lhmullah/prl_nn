using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_bonus_process_detailMap : EntityTypeConfiguration<prl_bonus_process_detail>
    {
        public prl_bonus_process_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.batch_no)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_bonus_process_detail", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.bonus_process_id).HasColumnName("bonus_process_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.batch_no).HasColumnName("batch_no");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.bonus_tax_amount).HasColumnName("bonus_tax_amount");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");

            // Relationships
            this.HasRequired(t => t.prl_bonus_process)
                .WithMany(t => t.prl_bonus_process_detail)
                .HasForeignKey(d => d.bonus_process_id);
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_bonus_process_detail)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
