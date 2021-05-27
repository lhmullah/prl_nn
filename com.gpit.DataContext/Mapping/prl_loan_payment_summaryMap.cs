using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_loan_payment_summaryMap : EntityTypeConfiguration<prl_loan_payment_summary>
    {
        public prl_loan_payment_summaryMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_loan_payment_summary", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.loan_entry_id).HasColumnName("loan_entry_id");
            this.Property(t => t.salary_process_id).HasColumnName("salary_process_id");
            this.Property(t => t.this_month_paid).HasColumnName("this_month_paid");
            this.Property(t => t.loan_realized).HasColumnName("loan_realized");
            this.Property(t => t.loan_balance).HasColumnName("loan_balance");

            // Relationships

            this.HasRequired(t => t.prl_loan_entry)
                .WithMany(t => t.prl_loan_payment_summary)
                .HasForeignKey(d => d.loan_entry_id);

            this.HasRequired(t => t.prl_salary_process)
                .WithMany(t => t.prl_loan_payment_summary)
                .HasForeignKey(d => d.salary_process_id);

        }
    }
}
