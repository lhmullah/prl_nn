using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_employee_yearly_investmentMap : EntityTypeConfiguration<prl_employee_yearly_investment>
    {
        public prl_employee_yearly_investmentMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.created_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_yearly_investment", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.fiscal_year_id).HasColumnName("fiscal_year_id");
            this.Property(t => t.invested_amount).HasColumnName("invested_amount");
           
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_yearly_investment)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_fiscal_year)
                .WithMany(t => t.prl_employee_yearly_investment)
                .HasForeignKey(d => d.fiscal_year_id);
        }
    }
}
