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
        DbSet<FileElement> EleFiles { get; set; }
    }
}
