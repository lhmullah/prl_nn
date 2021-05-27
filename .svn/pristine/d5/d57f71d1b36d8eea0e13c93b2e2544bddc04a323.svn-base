using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_bank_branchMap : EntityTypeConfiguration<prl_bank_branch>
    {
        public prl_bank_branchMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.branch_name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.branch_code)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_bank_branch", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.bank_id).HasColumnName("bank_id");
            this.Property(t => t.branch_name).HasColumnName("branch_name");
            this.Property(t => t.branch_code).HasColumnName("branch_code");

            // Relationships
            this.HasRequired(t => t.prl_bank)
                .WithMany(t => t.prl_bank_branch)
                .HasForeignKey(d => d.bank_id);

        }
    }
}
