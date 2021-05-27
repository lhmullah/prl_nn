using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_deduction_headMap : EntityTypeConfiguration<prl_deduction_head>
    {
        public prl_deduction_headMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_deduction_head", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
        }
    }
}
