using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_salary_processMap : EntityTypeConfiguration<prl_salary_process>
    {
        public prl_salary_processMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.batch_no)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.gender)
                .HasMaxLength(65532);

            this.Property(t => t.is_disbursed)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_salary_process", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.batch_no).HasColumnName("batch_no");
            this.Property(t => t.salary_month).HasColumnName("salary_month");
            this.Property(t => t.process_date).HasColumnName("process_date");
            this.Property(t => t.payment_date).HasColumnName("payment_date");
            this.Property(t => t.company_id).HasColumnName("company_id");
            this.Property(t => t.division_id).HasColumnName("division_id");
            this.Property(t => t.department_id).HasColumnName("department_id");
            this.Property(t => t.grade_id).HasColumnName("grade_id");
            this.Property(t => t.gender).HasColumnName("gender");
            this.Property(t => t.is_disbursed).HasColumnName("is_disbursed");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
