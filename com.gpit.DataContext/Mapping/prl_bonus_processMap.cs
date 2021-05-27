using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_bonus_processMap : EntityTypeConfiguration<prl_bonus_process>
    {
        public prl_bonus_processMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.month)
                .IsRequired()
                .HasMaxLength(12);

            this.Property(t => t.year)
                .IsRequired()
                .HasMaxLength(4);

            this.Property(t => t.batch_no)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.is_festival)
                .HasMaxLength(65532);

            this.Property(t => t.is_pay_with_salary)
                .HasMaxLength(65532);

            this.Property(t => t.is_available_in_payslip)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_bonus_process", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.bonus_name_id).HasColumnName("bonus_name_id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.month).HasColumnName("month");
            this.Property(t => t.year).HasColumnName("year");
            this.Property(t => t.batch_no).HasColumnName("batch_no");
            this.Property(t => t.process_date).HasColumnName("process_date");
            this.Property(t => t.festival_date).HasColumnName("festival_date");
            this.Property(t => t.is_festival).HasColumnName("is_festival");
            this.Property(t => t.religion_id).HasColumnName("religion_id");
            this.Property(t => t.department_id).HasColumnName("department_id");
            this.Property(t => t.is_pay_with_salary).HasColumnName("is_pay_with_salary");
            this.Property(t => t.is_available_in_payslip).HasColumnName("is_available_in_payslip");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_bonus_name)
                .WithMany(t => t.prl_bonus_process)
                .HasForeignKey(d => d.bonus_name_id);

        }
    }
}
