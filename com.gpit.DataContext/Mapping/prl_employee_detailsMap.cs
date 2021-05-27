using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_employee_detailsMap : EntityTypeConfiguration<prl_employee_details>
    {
        public prl_employee_detailsMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee_details", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.emp_status).HasColumnName("emp_status");
            this.Property(t => t.employee_category).HasColumnName("employee_category");
            this.Property(t => t.job_level_id).HasColumnName("job_level_id");
            this.Property(t => t.grade_id).HasColumnName("grade_id");
            this.Property(t => t.division_id).HasColumnName("division_id");
            this.Property(t => t.department_id).HasColumnName("department_id");
            this.Property(t => t.sub_department_id).HasColumnName("sub_department_id");
            this.Property(t => t.sub_sub_department_id).HasColumnName("sub_sub_department_id");

            this.Property(t => t.designation_id).HasColumnName("designation_id");

            this.Property(t => t.cost_centre_id).HasColumnName("cost_centre_id");
            this.Property(t => t.basic_salary).HasColumnName("basic_salary");
            this.Property(t => t.marital_status).HasColumnName("marital_status");
            this.Property(t => t.blood_group).HasColumnName("blood_group");
            this.Property(t => t.parmanent_address).HasColumnName("parmanent_address");
            this.Property(t => t.present_address).HasColumnName("present_address");
            
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships

            this.HasOptional(t => t.prl_job_level)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.job_level_id);

            this.HasRequired(t => t.prl_department)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.department_id);

            this.HasOptional(t => t.prl_sub_department)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.sub_department_id);

            this.HasOptional(t => t.prl_sub_sub_department)
               .WithMany(t => t.prl_employee_details)
               .HasForeignKey(d => d.sub_sub_department_id);

            this.HasRequired(t => t.prl_designation)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.designation_id);

            this.HasRequired(t => t.prl_cost_centre)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.cost_centre_id);

            this.HasOptional(t => t.prl_division)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.division_id);

            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.emp_id);

            this.HasOptional(t => t.prl_grade)
                .WithMany(t => t.prl_employee_details)
                .HasForeignKey(d => d.grade_id);

        }
    }
}
