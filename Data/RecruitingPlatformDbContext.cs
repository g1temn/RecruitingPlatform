using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.Data
{
    public class RecruitingPlatformDbContext : IdentityDbContext<User, UserRole, int>
    {
        public RecruitingPlatformDbContext(DbContextOptions<RecruitingPlatformDbContext> options) : base(options)
        {

        }

        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeDocument> ResumeDocuments { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<ResumeSkill> ResumeSkills { get; set; }
        public DbSet<VacancySkill> VacancySkills { get; set; }
        public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Industry> Industries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // rename those two tables to match the database schema
            builder.Entity<User>().ToTable("users");
            builder.Entity<UserRole>().ToTable("user_roles");

            // rename tables create by Identity
            builder.Entity<IdentityUserRole<int>>().ToTable("user_role_mappings");
            builder.Entity<IdentityUserClaim<int>>().ToTable("user_claims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("user_logins");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");
            builder.Entity<IdentityUserToken<int>>().ToTable("user_tokens");

            // global query filters to exclude soft deleted entities
            builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            builder.Entity<JobSeeker>().HasQueryFilter(j => !j.IsDeleted);
            builder.Entity<Company>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Resume>().HasQueryFilter(r => !r.IsDeleted);
            builder.Entity<Vacancy>().HasQueryFilter(v => !v.IsDeleted);

            // disable cascade delete for all relationships
            var cascadeFKs = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
            
            // tell ef to establish a 1:1 relationship between user and job seeker
            builder.Entity<User>()
                .HasOne(u => u.JobSeeker)
                .WithOne(j => j.User)
                .HasForeignKey<JobSeeker>(j => j.Id);
            
            // tell ef to establish a 1:1 relationship between user and company
            builder.Entity<User>()
                .HasOne(u => u.Company)
                .WithOne(c => c.User)
                .HasForeignKey<Company>(c => c.Id);
        }
    }
}
