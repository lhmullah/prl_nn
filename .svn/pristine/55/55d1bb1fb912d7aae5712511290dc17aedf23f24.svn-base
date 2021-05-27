using System.Data.Entity.ModelConfiguration;
using com.gpit.Model;

namespace com.gpit.DataContext.Mapping
{
    public class prl_upload_time_card_entryMap : EntityTypeConfiguration<prl_upload_time_card_entry>
    {
        public prl_upload_time_card_entryMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.emp_no)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.month)
                .HasMaxLength(10);

            this.Property(t => t.approver_id)
                .HasMaxLength(50);

            this.Property(t => t.hrms_comment)
                .HasMaxLength(300);

            this.Property(t => t.payroll_comments)
                .HasMaxLength(300);

            this.Property(t => t.created_by)
                .HasMaxLength(50);

            this.Property(t => t.updated_by)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("prl_upload_time_card_entry", "payroll_system");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.emp_no).HasColumnName("emp_no");
            this.Property(t => t.month).HasColumnName("month");
            this.Property(t => t.year).HasColumnName("year");
            this.Property(t => t.for_month).HasColumnName("for_month");
            this.Property(t => t.submission_date).HasColumnName("submission_date");
            this.Property(t => t.double_rate_hour).HasColumnName("double_rate_hour");
            this.Property(t => t.triple_rate_hour).HasColumnName("triple_rate_hour");
            this.Property(t => t.four_times_rate_hour).HasColumnName("four_times_rate_hour");
            this.Property(t => t.total_ot).HasColumnName("total_ot");
            this.Property(t => t.holiday).HasColumnName("holiday");
            this.Property(t => t.normal_days).HasColumnName("normal_days");
            this.Property(t => t.total_days).HasColumnName("total_days");
            this.Property(t => t.evening_shift).HasColumnName("evening_shift");
            this.Property(t => t.night_shift).HasColumnName("night_shift");
            this.Property(t => t.weekend).HasColumnName("weekend");
            this.Property(t => t.total_shift).HasColumnName("total_shift");
            this.Property(t => t.on_call_days).HasColumnName("on_call_days");
            this.Property(t => t.approver_id).HasColumnName("approver_id");
            this.Property(t => t.approve_date).HasColumnName("approve_date");
            this.Property(t => t.one_way).HasColumnName("one_way");
            this.Property(t => t.two_way).HasColumnName("two_way");
            this.Property(t => t.hrms_comment).HasColumnName("hrms_comment");
            this.Property(t => t.payroll_comments).HasColumnName("payroll_comments");
            this.Property(t => t.created_by).HasColumnName("created_by");
            this.Property(t => t.created_date).HasColumnName("created_date");
            this.Property(t => t.updated_by).HasColumnName("updated_by");
            this.Property(t => t.updated_date).HasColumnName("updated_date");
        }
    }
}
