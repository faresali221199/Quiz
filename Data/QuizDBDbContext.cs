using Microsoft.EntityFrameworkCore;
using Quiz.Models;

namespace Quiz.Data;

public partial class QuizDBDbContext : DbContext
{
    public QuizDBDbContext() { }
    public QuizDBDbContext(DbContextOptions<QuizDBDbContext> options) : base(options) { }

    public virtual DbSet<Admin> Admins { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<Lecture> Lectures { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Result> Results { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserAnswer> UserAnswers { get; set; }
    public virtual DbSet<Year> Years { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2A0QBS7\\SQLEXPRESS;Database=QuizDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تثبيت علاقة النتيجة بالطالب ومنع UserId1
        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId);
            entity.HasOne(d => d.User).WithMany(p => p.Results).HasForeignKey(d => d.UserId);
            entity.HasOne(d => d.Lecture).WithMany(p => p.Results).HasForeignKey(d => d.LectureId);
        });

        // تثبيت علاقة الطالب بالسنة
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasOne(d => d.Year).WithMany(p => p.Users).HasForeignKey(d => d.YearId);
        });

        // تثبيت علاقة الإجابات بالنتيجة
        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.UserAnswerId);
            entity.HasOne(d => d.Result).WithMany(p => p.UserAnswers).HasForeignKey(d => d.ResultId);
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}