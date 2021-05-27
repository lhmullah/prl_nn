using System.Data.Entity.ModelConfiguration;
using com.linde.Model;

namespace com.linde.DataContext.Mapping
{
    public class prl_usersMap : EntityTypeConfiguration<prl_users>
    {
        public prl_usersMap()
        {
            // Primary Key
            this.HasKey(t => t.user_id);

            // Properties
            this.Property(t => t.user_name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.emp_id);

            this.Property(t => t.email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.role_name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.password)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.PasswordQuestion)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.PasswordAnswer)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.created_by)
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("prl_users", "payroll_system_nn");
            this.Property(t => t.user_id).HasColumnName("User_Id");
            this.Property(t => t.user_name).HasColumnName("User_Name");
            this.Property(t => t.emp_id).HasColumnName("Emp_Id");
            this.Property(t => t.email).HasColumnName("Email");
            this.Property(t => t.role_name).HasColumnName("Role_Name");
            this.Property(t => t.password).HasColumnName("Password");
            this.Property(t => t.PasswordQuestion).HasColumnName("PasswordQuestion");
            this.Property(t => t.PasswordAnswer).HasColumnName("PasswordAnswer");
            this.Property(t => t.created_date).HasColumnName("Created_Date");
            this.Property(t => t.created_by).HasColumnName("Created_By");
            // Relationships
            //

        }
    }
}
