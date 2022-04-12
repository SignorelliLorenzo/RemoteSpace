using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi.Data
{
    public class AppFileDbContext:DbContext 
    {
        public AppFileDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<FileElement> EleFiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileElement>()
            .HasMany(d => d.ChidFiles)
            .WithOne(d=>d.FatherDirectory)
            .HasForeignKey(e => e.FatherDirectoryId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
