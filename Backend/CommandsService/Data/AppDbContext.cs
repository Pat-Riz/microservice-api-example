// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>().HasMany(p => p.Commands).WithOne(p => p.Platform).HasForeignKey(p => p.PlatformId);
            modelBuilder.Entity<Command>().HasOne(c => c.Platform).WithMany(c => c.Commands).HasForeignKey(c => c.PlatformId);
        }
    }
}
