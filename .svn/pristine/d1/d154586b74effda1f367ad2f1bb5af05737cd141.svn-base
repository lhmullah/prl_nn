using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
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

            this.Property(t => t.present_address)
                .HasMaxLength(250);

            this.Property(t => t.permanent_address)
                .HasMaxLength(250);

            this.Property(t => t.phone)
                .HasMaxLength(20);

            this.Property(t => t.email)
                .HasMaxLength(50);

            this.Property(t => t.gender)
                .HasMaxLength(65532);

            this.Property(t => t.marital_status)
                .HasMaxLength(65532);

            this.Property(t => t.father_name)
                .HasMaxLength(100);

            this.Property(t => t.mother_name)
                .HasMaxLength(100);

            this.Property(t => t.account_no)
                .HasMaxLength(20);

            this.Property(t => t.payment_mode)
                .HasMaxLength(65532);

            this.Property(t => t.tin)
                .HasMaxLength(100);

            this.Property(t => t.picture)
                .HasMaxLength(100);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_employee", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_no).HasColumnName("emp_no");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.present_address).HasColumnName("present_address");
            this.Property(t => t.permanent_address).HasColumnName("permanent_address");
            this.Property(t => t.phone).HasColumnName("phone");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.religion_id).HasColumnName("religion_id");
            this.Property(t => t.gender).HasColumnName("gender");
            this.Property(t => t.marital_status).HasColumnName("marital_status");
            this.Property(t => t.company_id).HasColumnName("company_id");
            this.Property(t => t.father_name).HasColumnName("father_name");
            this.Property(t => t.mother_name).HasColumnName("mother_name");
            this.Property(t => t.bank_id).HasColumnName("bank_id");
            this.Property(t => t.bank_branch_id).HasColumnName("bank_branch_id");
            this.Property(t => t.account_no).HasColumnName("account_no");
            this.Property(t => t.payment_mode).HasColumnName("payment_mode");
            this.Property(t => t.dob).HasColumnName("dob");
            this.Property(t => t.joining_date).HasColumnName("joining_date");
            this.Property(t => t.tin).HasColumnName("tin");
            this.Property(t => t.confirmation_date).HasColumnName("confirmation_date");
            this.Property(t => t.is_confirmed).HasColumnName("is_confirmed");
            this.Property(t => t.is_pf_member).HasColumnName("is_pf_member");
            this.Property(t => t.is_gf_member).HasColumnName("is_gf_member");
            this.Property(t => t.termination_date).HasColumnName("termination_date");
            this.Property(t => t.picture).HasColumnName("picture");
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
            this.HasOptional(t => t.prl_company)
                .WithMany(t => t.prl_employee)
                .HasForeignKey(d => d.company_id);
            this.HasRequired(t => t.prl_religion)
                .WithMany(t => t.prl_employee)
                .HasForeignKey(d => d.religion_id);

        }
    }
}
