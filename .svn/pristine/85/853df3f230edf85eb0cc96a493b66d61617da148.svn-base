using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_tax_process_detailMap : EntityTypeConfiguration<prl_employee_tax_process_detail>
    {
        public prl_employee_tax_process_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.allowance_head_name)
                .HasMaxLength(100);

            this.Property(t => t.bonus_name)
                .HasMaxLength(100);

            this.Property(t => t.tax_item)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("prl_employee_tax_process_detail", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.tax_process_id).HasColumnName("tax_process_id");
            this.Property(t => t.salary_process_id).HasColumnName("salary_process_id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.allowance_head_id).HasColumnName("allowance_head_id");
            this.Property(t => t.allowance_head_name).HasColumnName("allowance_head_name");
            this.Property(t => t.bonus_id).HasColumnName("bonus_id");
            this.Property(t => t.bonus_name).HasColumnName("bonus_name");
            this.Property(t => t.tax_item).HasColumnName("tax_item");
            this.Property(t => t.gross_annual_income).HasColumnName("gross_annual_income");
            this.Property(t => t.less_exempted).HasColumnName("less_exempted");
            this.Property(t => t.total_taxable_income).HasColumnName("total_taxable_income");

            // Relationships
            this.HasOptional(t => t.prl_employee_tax_process)
                .WithMany(t => t.prl_employee_tax_process_detail)
                .HasForeignKey(d => d.tax_process_id);

        }
    }
}
