using Microsoft.EntityFrameworkCore;
using Muhit.Domain.Entities;

namespace Muhit.Persistence.Context
{
    public class MuhitDbContext : DbContext
    {
        public MuhitDbContext(DbContextOptions<MuhitDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<District> Districts => Set<District>();
        public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();
        public DbSet<NeighborhoodReview> NeighborhoodReviews => Set<NeighborhoodReview>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<NeighborhoodAiAnalysis> NeighborhoodAiAnalyses => Set<NeighborhoodAiAnalysis>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.PhoneNumber)
                .IsUnique()
                .HasFilter("[PhoneNumber] IS NOT NULL");

            modelBuilder.Entity<AppUser>()
                .HasOne(x => x.CurrentNeighborhood)
                .WithMany(x => x.Residents)
                .HasForeignKey(x => x.CurrentNeighborhoodId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NeighborhoodReview>()
                .HasOne(x => x.AppUser)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NeighborhoodReview>()
                .HasOne(x => x.Neighborhood)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.NeighborhoodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NeighborhoodAiAnalysis>()
                .HasOne(x => x.Neighborhood)
                .WithOne()
                .HasForeignKey<NeighborhoodAiAnalysis>(x => x.NeighborhoodId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}