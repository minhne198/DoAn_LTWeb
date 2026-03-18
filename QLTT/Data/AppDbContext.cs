using Microsoft.EntityFrameworkCore;
using QLTT.Models;

namespace QLTT.Data
{
    /// <summary>
    /// AppDbContext - Database Context cho ứng dụng QLTT (Quyên Góp Từ Tấm Lòng).
    /// 
    /// Mục đích:
    /// - Quản lý kết nối và giao dịch với database
    /// - Định nghĩa mối quan hệ giữa các entity
    /// - Cấu hình cascade delete và delete behavior
    /// - Thực hiện entity tracking
    /// 
    /// Entity Framework Core:
    /// - DbContext: Lớp cơ sở cho tất cả các DbSet
    /// - DbSet: Đại diện cho bảng trong database
    /// - OnModelCreating: Cấu hình mô hình dữ liệu advanced
    /// 
    /// Database Cascade DeleteBehavior:
    /// - Cascade: Xóa entity cha sẽ xóa tất cả entities con
    /// - Restrict: Không cho phép xóa entity cha nếu có entity con
    /// - SetNull: Xóa entity cha sẽ set FK thành NULL
    /// - NoAction: Không thực hiện hành động gì
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor - Nhận DbContextOptions từ Dependency Injection.
        /// </summary>
        /// <param name="options">Cấu hình options cho DbContext</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ==================== DbSets - Đại diện cho các bảng trong database ====================

        /// <summary>
        /// Bảng Users - Lưu thông tin tài khoản người dùng.
        /// Cột: UserId (PK), FullName, Email, PasswordHash, Role, IsLocked, CreatedAt
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Bảng Campaigns - Lưu thông tin chiến dịch quyên góp.
        /// Cột: CampaignId (PK), UserId (FK), Title, Content, TargetAmount, CurrentAmount, 
        ///      ImagePath, IsApproved, IsCompleted, StartDate, EndDate, CreatedAt
        /// </summary>
        public DbSet<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Bảng Donations - Lưu thông tin các khoản đóng góp.
        /// Cột: DonationId (PK), CampaignId (FK), UserId (FK), Amount, Message, Status, CreatedAt
        /// </summary>
        public DbSet<Donation> Donations { get; set; }

        /// <summary>
        /// Bảng Comments - Lưu bình luận trên các chiến dịch.
        /// Cột: CommentId (PK), CampaignId (FK), UserId (FK), Content, CreatedAt
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Bảng Reports - Lưu các báo cáo/tố cáo về chiến dịch vi phạm.
        /// Cột: ReportId (PK), CampaignId (FK), UserId (FK), Reason, IsHandled, ReportedAt
        /// </summary>
        public DbSet<Report> Reports { get; set; }

        /// <summary>
        /// Bảng Follows - Lưu quan hệ theo dõi giữa user và campaign.
        /// Cột: FollowId (PK), UserId (FK), CampaignId (FK), FollowedAt
        /// </summary>
        public DbSet<Follow> Follows { get; set; }

        /// <summary>
        /// Bảng Notifications - Lưu thông báo gửi đến người dùng.
        /// Cột: NotificationId (PK), UserId (FK), Content, IsRead, CreatedAt
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Bảng AuditLogs - Ghi lại lịch sử các hành động của người dùng (bảo mật).
        /// Cột: LogId (PK), UserId (FK), Action, Timestamp
        /// </summary>
        public DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        /// Bảng AdminActions - Ghi lại các hành động quản trị của admin.
        /// Cột: AdminActionId (PK), AdminId (FK), ActionType, TargetId, Notes, ActionAt
        /// </summary>
        public DbSet<AdminAction> AdminActions { get; set; }

        /// <summary>
        /// OnModelCreating - Cấu hình mô hình dữ liệu advanced (fluent API).
        /// 
        /// Tại đây, chúng ta:
        /// 1. Đặt tên bảng trong database
        /// 2. Cấu hình mối quan hệ One-to-Many (1-N) giữa các entity
        /// 3. Cấu hình cascade delete behavior (cách xử lý khi xóa entity)
        /// 4. Cấu hình các ràng buộc và index
        /// 
        /// DELETE BEHAVIOR CHI TIẾT:
        /// 
        /// Cascade: 
        ///   - Xóa entity cha sẽ tự động xóa tất cả entities con
        ///   - Ví dụ: Xóa Campaign sẽ xóa tất cả Donations, Comments
        ///   - Sử dụng khi: Dữ liệu con không có ý nghĩa khi entity cha bị xóa
        /// 
        /// Restrict:
        ///   - KHÔNG cho phép xóa entity cha nếu có entity con liên kết
        ///   - Ví dụ: KHÔNG thể xóa User nếu user đó tạo Campaign
        ///   - Sử dụng khi: Cần bảo vệ dữ liệu cha
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== Đặt tên bảng trong database ====================
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Campaign>().ToTable("Campaigns");
            modelBuilder.Entity<Donation>().ToTable("Donations");
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Report>().ToTable("Reports");
            modelBuilder.Entity<Follow>().ToTable("Follows");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            modelBuilder.Entity<AdminAction>().ToTable("AdminActions");

            // ==================== Cấu hình mối quan hệ Comment ====================
            // Comment → User (Many-to-One)
            // Mối quan hệ: 1 User có Many Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa User nếu có Comments từ user
                // Lý do: Cần giữ thông tin người viết comment ngay cả sau khi xóa account

            // Comment → Campaign (Many-to-One)
            // Mối quan hệ: 1 Campaign có Many Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Campaign)
                .WithMany()
                .HasForeignKey(c => c.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa Comment khi Campaign bị xóa
                // Lý do: Comments chỉ có ý nghĩa khi campaign tồn tại

            // ==================== Cấu hình mối quan hệ Donation ====================
            // Donation → User (Many-to-One)
            // Mối quan hệ: 1 User có Many Donations
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa User nếu có Donations từ user
                // Lý do: Cần giữ thông tin nhà quyên góp để kiểm tra và báo cáo

            // Donation → Campaign (Many-to-One)
            // Mối quan hệ: 1 Campaign có Many Donations
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Campaign)
                .WithMany()
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa Donation khi Campaign bị xóa
                // Lý do: Donations chỉ có ý nghĩa khi campaign tồn tại

            // ==================== Cấu hình mối quan hệ Follow ====================
            // Follow → User (Many-to-One)
            // Mối quan hệ: 1 User có Many Follows
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict để tránh multiple cascade path
                // Lý do EF Core không cho phép multiple cascade path từ cùng một entity

            // Follow → Campaign (Many-to-One)
            // Mối quan hệ: 1 Campaign có Many Follows
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Campaign)
                .WithMany()
                .HasForeignKey(f => f.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa Follow khi Campaign bị xóa
                // Lý do: Follows chỉ có ý nghĩa khi campaign tồn tại

            // ==================== Cấu hình mối quan hệ Notification ====================
            // Notification → User (Many-to-One)
            // Mối quan hệ: 1 User có Many Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa Notification khi User bị xóa
                // Lý do: Thông báo chỉ có ý nghĩa cho người nhận, không cần giữ sau khi user xóa

            // ==================== Cấu hình mối quan hệ AuditLog ====================
            // AuditLog → User (Many-to-One)
            // Mối quan hệ: 1 User có Many AuditLogs
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa AuditLog khi User bị xóa
                // Lý do: Audit logs của user không cần giữ sau khi user bị xóa (mặc dù thực tế có thể cần)
                // TODO: Xem xét lại policy này - có thể nên Restrict để giữ log vì mục đích kiểm tra

            // ==================== Cấu hình mối quan hệ AdminAction ====================
            // AdminAction → Admin/User (Many-to-One)
            // Mối quan hệ: 1 Admin có Many AdminActions
            modelBuilder.Entity<AdminAction>()
                .HasOne(a => a.Admin)
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa Admin nếu có AdminActions từ admin
                // Lý do: Cần giữ thông tin ai thực hiện hành động admin vì mục đích kiểm toán

            // ==================== Cấu hình mối quan hệ Campaign ====================
            // Campaign → User (Many-to-One)
            // Mối quan hệ: 1 User có Many Campaigns
            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa User nếu user tạo Campaign
                // Lý do: Cần giữ thông tin tác giả campaign cho mục đích truy vết và báo cáo

            // ==================== Cấu hình mối quan hệ Report ====================
            // Report → User (Many-to-One)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa nếu user có report

            // Report → Campaign (Many-to-One)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Campaign)
                .WithMany()
                .HasForeignKey(r => r.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa report khi campaign xóa
        }
    }
}
