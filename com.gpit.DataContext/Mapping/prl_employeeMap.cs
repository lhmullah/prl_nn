using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_employeeMap : EntityTypeConfiguration<prl_employee>
    {
        public prl_employeeMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.emp_no)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(100);

            //this.Property(t => t.present_address)
            //    .HasMaxLength(250);

            //this.Property(t => t.permanent_address)
            //    .HasMaxLength(250);

            this.Property(t => t.official_contact_no)
                .HasMaxLength(20);

            this.Property(t => t.personal_contact_no)
                .HasMaxLength(20);

            this.Property(t => t.email)
                .HasMaxLength(50);

            this.Property(t => t.personal_email)
                .HasMaxLength(50);

            this.Property(t => t.gender)
                .HasMaxLength(65532);

            this.Property(t => t.account_no)
                .HasMaxLength(20);

            this.Property(t => t.routing_no)
                .HasMaxLength(20);

            this.Property(t => t.tin)
                .HasMaxLength(100);

            

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee", "payroll_system_nn");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_no).HasColumnName("emp_no");
            this.Property(t => t.name).HasColumnName("name");

            this.Property(t => t.official_contact_no).HasColumnName("official_contact_no");
            this.Property(t => t.personal_contact_no).HasColumnName("personal_contact_no");


            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.personal_email).HasColumnName("personal_email");
            this.Property(t => t.religion_id).HasColumnName("religion_id");
            this.Property(t => t.gender).HasColumnName("gender");

            this.Property(t => t.bank_id).HasColumnName("bank_id");
            this.Property(t => t.bank_branch_id).HasColumnName("bank_branch_id");
            this.Property(t => t.account_no).HasColumnName("account_no");
            this.Property(t => t.routing_no).HasColumnName("routing_no");
            this.Property(t => t.dob).HasColumnName("dob");
            this.Property(t => t.joining_date).HasColumnName("joining_date");
            this.Property(t => t.tin).HasColumnName("tin");
            this.Property(t => t.confirmation_date).HasColumnName("confirmation_date");
            this.Property(t => t.is_confirmed).HasColumnName("is_confirmed");
            
            this.Property(t => t.is_active).HasColumnName("is_active");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");

            // Relationships
            this.HasOptional(t => t.prl_bank)
                .WithMany(t => t.prl_employee)
                .HasForeignKey(d => d.bank_id);
            this.HasOptional(t => t.prl_bank_branch)
                .WithMany(t => t.prl_employee)
                .HasForeignKey(d => d.bank_branch_id);
            this.HasRequired(t => t.prl_religion)
                .WithMany(t => t.prl_employee)
                .HasForeignKey(d => d.religion_id);

        }
    }
}
