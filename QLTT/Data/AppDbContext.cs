using Microsoft.EntityFrameworkCore;
using QLTT.Models;

namespace QLTT.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Khai báo DbSet cho từng bảng
        public DbSet<User> Users { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AdminAction> AdminActions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đặt tên bảng
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Campaign>().ToTable("Campaigns");
            modelBuilder.Entity<Donation>().ToTable("Donations");
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Report>().ToTable("Reports");
            modelBuilder.Entity<Follow>().ToTable("Follows");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            modelBuilder.Entity<AdminAction>().ToTable("AdminActions");

            // ======= Comment =======
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa User khi xóa Comment

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Campaign)
                .WithMany()
                .HasForeignKey(c => c.CampaignId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Comment khi Campaign bị xóa

            // ======= Donation =======
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa User khi xóa Donation

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Campaign)
                .WithMany()
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Donation khi Campaign bị xóa

            // ======= Follow =======
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade path

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Campaign)
                .WithMany()
                .HasForeignKey(f => f.CampaignId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Follow khi Campaign bị xóa

            // ======= Notification =======
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Notification khi User bị xóa

            // ======= AuditLog =======
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa AuditLog khi User bị xóa

            // ======= AdminAction =======
            modelBuilder.Entity<AdminAction>()
                .HasOne(a => a.Admin)
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa Admin khi xóa AdminAction

            // ======= Campaign =======
            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa User khi xóa Campaign

        }
    }
}
