﻿using FFhub_backend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FFhub_backend.Database
{
    public class FFDbContext : DbContext
    {
        public DbSet<DBVideo> Videos { get; set; }
        public DbSet<DBTag> Tags { get; set; }
        public DbSet<DBVideoTag> VideoTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Update this with your actual database connection string
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure many-to-many relationship for VideoTags
            modelBuilder.Entity<DBVideoTag>()
                .HasKey(vt => new { vt.VideoId, vt.TagId }); // Composite primary key

            modelBuilder.Entity<DBVideoTag>()
                .HasOne(vt => vt.Video)
                .WithMany(v => v.VideoTags)
                .HasForeignKey(vt => vt.VideoId);

            modelBuilder.Entity<DBVideoTag>()
                .HasOne(vt => vt.Tag)
                .WithMany(t => t.VideoTags)
                .HasForeignKey(vt => vt.TagId);

            // Optional: Add unique constraint on TagName to enforce uniqueness
            modelBuilder.Entity<DBTag>()
                .HasIndex(t => t.TagName)
                .IsUnique();

            // Optional: Configure column sizes for NVARCHAR types
            modelBuilder.Entity<DBVideo>()
                .Property(v => v.Link)
                .HasMaxLength(2048);

            modelBuilder.Entity<DBVideo>()
                .Property(v => v.Title)
                .HasMaxLength(200);

            modelBuilder.Entity<DBTag>()
                .Property(t => t.TagName)
                .HasMaxLength(50);
        }
    }
}
