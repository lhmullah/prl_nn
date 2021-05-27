using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_salary_process_detailMap : EntityTypeConfiguration<prl_salary_process_detail>
    {
        public prl_salary_process_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_salary_process_detail", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.salary_process_id).HasColumnName("salary_process_id");
            this.Property(t => t.salary_month).HasColumnName("salary_month");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.calculation_for_days).HasColumnName("calculation_for_days");
            this.Property(t => t.current_basic).HasColumnName("current_basic");
            this.Property(t => t.this_month_basic).HasColumnName("this_month_basic");
            this.Property(t => t.total_allowance).HasColumnName("total_allowance");
            this.Property(t => t.totla_arrear_allowance).HasColumnName("totla_arrear_allowance");
            this.Property(t => t.pf_amount).HasColumnName("pf_amount");
            this.Property(t => t.pf_arrear).HasColumnName("pf_arrear");
            this.Property(t => t.total_deduction).HasColumnName("total_deduction");
            this.Property(t => t.total_arrear_deduction).HasColumnName("total_arrear_deduction");
            this.Property(t => t.total_monthly_tax).HasColumnName("total_monthly_tax");
            this.Property(t => t.total_overtime).HasColumnName("total_overtime");
            this.Property(t => t.total_overtime_arrear).HasColumnName("total_overtime_arrear");
            this.Property(t => t.total_bonus).HasColumnName("total_bonus");
            this.Property(t => t.net_pay).HasColumnName("net_pay");

            // Relationships
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_salary_process_detail)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_salary_process)
                .WithMany(t => t.prl_salary_process_detail)
                .HasForeignKey(d => d.salary_process_id);

        }
    }
}
