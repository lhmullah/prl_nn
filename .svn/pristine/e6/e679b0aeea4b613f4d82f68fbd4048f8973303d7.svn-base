using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_employee_detailsMap : EntityTypeConfiguration<prl_employee_details>
    {
        public prl_employee_detailsMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.emp_status)
                .HasMaxLength(65532);

            this.Property(t => t.is_hold)
                .HasMaxLength(65532);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_details", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.emp_status).HasColumnName("emp_status");
            this.Property(t => t.grade_id).HasColumnName("grade_id");
            this.Property(t => t.division_id).HasColumnName("division_id");
            this.Property(t => t.department_id).HasColumnName("department_id");
            this.Property(t => t.basic_salary).HasColumnName("basic_salary");
            this.Property(t => t.designation_id).HasColumnName("designation_id");
            this.Property(t => t.is_hold).HasColumnName("is_hold");
            this.Property(t => t.posting_location_id).HasColumnName("posting_location_id");
            this.Property(t => t.posting_date).HasColumnName("posting_date");
            this.Property(t => t.contract_start_date).HasColumnName("contract_start_date");
            this.Property(t => t.contract_end_date).HasColumnName("contract_end_date");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasOptional(t => t.prl_department)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.department_id);
            this.HasRequired(t => t.prl_designation)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.designation_id);
            this.HasOptional(t => t.prl_division)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.division_id);
            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.emp_id);
            this.HasRequired(t => t.prl_grade)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.grade_id);
            this.HasOptional(t => t.prl_location)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.posting_location_id);

        }
    }
}
