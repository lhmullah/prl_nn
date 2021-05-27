using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_upload_pf_dividend_yearlyMap : EntityTypeConfiguration<prl_upload_pf_dividend_yearly>
    {
        public prl_upload_pf_dividend_yearlyMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            
            this.ToTable("prl_upload_pf_dividend_yearly", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.dividend_month_year).HasColumnName("dividend_month_year");
            this.Property(t => t.own_contributed_amount).HasColumnName("own_contributed_amount");
            this.Property(t => t.company_contributed_amount).HasColumnName("company_contributed_amount");
            this.Property(t => t.own_dividend_amount).HasColumnName("own_dividend_amount");
            this.Property(t => t.company_dividend_amount).HasColumnName("company_dividend_amount");

            this.Property(t => t.principal_amount).HasColumnName("principal_amount");


            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships

            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_upload_pf_dividend_yearly)
                .HasForeignKey(d => d.emp_id);

        }
    }
}
