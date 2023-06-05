using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<AppUser> Users {get;set;}
         public DbSet<UserLiked> Likes {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLiked>()
            .HasKey(k => new {k.SourceUserId,k.TargetUserId});

           modelBuilder.Entity<UserLiked>()
           .HasOne(s => s.SourceUser)
           .WithMany(l=> l.LikedUsers)
           .HasForeignKey(s => s.SourceUserId)
           .OnDelete(DeleteBehavior.Cascade);

           modelBuilder.Entity<UserLiked>()
           .HasOne(s => s.TargetUser)
           .WithMany(l=> l.LikedByUsers)
           .HasForeignKey(s => s.TargetUserId)
           .OnDelete(DeleteBehavior.Cascade);
        }
    }
}