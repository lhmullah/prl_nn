using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_loan_entryMap : EntityTypeConfiguration<prl_loan_entry>
    {
        public prl_loan_entryMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_loan_entry", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.deduction_name_id).HasColumnName("deduction_name_id");
            this.Property(t => t.loan_start_date).HasColumnName("loan_start_date");
            this.Property(t => t.loan_end_date).HasColumnName("loan_end_date");
            this.Property(t => t.principal_amount).HasColumnName("principal_amount");
            this.Property(t => t.monthly_installment).HasColumnName("monthly_installment");

            // Relationships

            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_loan_entry)
                .HasForeignKey(d => d.emp_id);

            this.HasRequired(t => t.prl_deduction_name)
               .WithMany(t => t.prl_loan_entry)
               .HasForeignKey(d => d.deduction_name_id);

        }
    }
}
