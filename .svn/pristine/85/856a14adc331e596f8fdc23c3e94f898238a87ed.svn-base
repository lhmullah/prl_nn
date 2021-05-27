using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_bankMap : EntityTypeConfiguration<prl_bank>
    {
        public prl_bankMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.bank_name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.bank_code)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_bank", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.bank_name).HasColumnName("bank_name");
            this.Property(t => t.bank_code).HasColumnName("bank_code");
        }
    }
}
