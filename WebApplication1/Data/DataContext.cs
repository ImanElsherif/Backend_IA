using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<Employer> Employer { get; set; }
        public DbSet<JobSeeker> JobSeeker { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<IdentityCard> IdentityCard { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Proposal> Proposal { get; set; }
        public DbSet<SavedJob> SavedJob { get; set; }
  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employer>().ToTable("Employers");
            modelBuilder.Entity<JobSeeker>().ToTable("JobSeekers");
            modelBuilder.Entity<Job>().ToTable("Jobs");
            modelBuilder.Entity<Proposal>().ToTable("Proposals");
            modelBuilder.Entity<SavedJob>().ToTable("SavedJobs");


            modelBuilder.Entity<Employer>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employer>(e => e.Id);

            modelBuilder.Entity<JobSeeker>()
                .HasOne(js => js.User)
                .WithOne()
                .HasForeignKey<JobSeeker>(js => js.Id);
            
            // Configure one-to-many relationship between Employer and Job
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Employer)
                .WithMany(e => e.Jobs) // One Employer can have many Jobs
                .HasForeignKey(j => j.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-one relationship between Proposal and Employer
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Employer)
                .WithMany(e => e.Proposals)
                .HasForeignKey(p => p.EmpId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-one relationship between Proposal and Job
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Job)
                .WithMany(j => j.Proposals)
                .HasForeignKey(p => p.JobId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure many-to-one relationship between Proposal and Job
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.JobSeeker)
                .WithMany(js => js.Proposals)
                .HasForeignKey(p => p.JobSeekerId)
                .OnDelete(DeleteBehavior.Restrict);
           
            modelBuilder.Entity<SavedJob>()
               .HasOne(sj => sj.JobSeeker)
               .WithMany(js => js.SavedJobs)
               .HasForeignKey(sj => sj.JobSeekerId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.Job)
                .WithMany(j => j.SavedJobs)
                .HasForeignKey(sj => sj.JobId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
