using com.linde.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.linde.DataContext.Mapping
{
    public class prl_certificate_uploadMap : EntityTypeConfiguration<prl_certificate_upload>
    {
        public prl_certificate_uploadMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            // Table & Column Mappings
            this.ToTable("prl_certificate_upload", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_id).HasColumnName("emp_id");
            this.Property(t => t.income_year).HasColumnName("income_year");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.file_path).HasColumnName("file_path");
            this.Property(t => t.number_of_car).HasColumnName("number_of_car");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships

            this.HasRequired(t => t.prl_employee)
                .WithMany(t => t.prl_certificate_upload)
                .HasForeignKey(d => d.emp_id);
        }
    }
}
