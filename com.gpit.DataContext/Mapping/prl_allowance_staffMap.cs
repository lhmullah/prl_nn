using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_allowance_staffMap : EntityTypeConfiguration<prl_allowance_staff>
    {
        public prl_allowance_staffMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_allowance_staff", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.allowance_name_id).HasColumnName("allowance_name_id");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.percent_amount).HasColumnName("percent_amount");
            this.Property(t => t.grade_id).HasColumnName("grade_id");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasRequired(t => t.prl_allowance_name)
                .WithMany(t => t.prl_allowance_staff)
                .HasForeignKey(d => d.allowance_name_id);

            this.HasOptional(t => t.prl_grade)
                .WithMany(t => t.prl_allowance_staff)
                .HasForeignKey(d => d.grade_id);



        }
    }
}
