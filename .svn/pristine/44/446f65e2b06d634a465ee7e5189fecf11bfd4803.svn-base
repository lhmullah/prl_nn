using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_companyMap : EntityTypeConfiguration<prl_company>
    {
        public prl_companyMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.description)
                .HasMaxLength(250);

            this.Property(t => t.address)
                .HasMaxLength(250);

            this.Property(t => t.primary_phone)
                .HasMaxLength(20);

            this.Property(t => t.secondary_phone)
                .HasMaxLength(20);

            this.Property(t => t.email)
                .HasMaxLength(100);

            this.Property(t => t.web)
                .HasMaxLength(50);

            this.Property(t => t.logo)
                .HasMaxLength(200);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_company", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.address).HasColumnName("address");
            this.Property(t => t.primary_phone).HasColumnName("primary_phone");
            this.Property(t => t.secondary_phone).HasColumnName("secondary_phone");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.web).HasColumnName("web");
            this.Property(t => t.logo).HasColumnName("logo");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
