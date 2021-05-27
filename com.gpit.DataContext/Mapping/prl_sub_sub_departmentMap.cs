using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_sub_sub_departmentMap : EntityTypeConfiguration<prl_sub_sub_department>
    {
        public prl_sub_sub_departmentMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties

            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_sub_sub_department", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.sub_department_id).HasColumnName("sub_department_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");

            this.HasRequired(t => t.prl_sub_department)
                .WithMany(t => t.prl_sub_sub_department)
                .HasForeignKey(d => d.sub_department_id);
        }
    }
}
