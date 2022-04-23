using Api_Pcto.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi.Data
{
    public class MyTokenDbContext: IdentityDbContext<UserModel>
    {
        public MyTokenDbContext(DbContextOptions<MyTokenDbContext> options) : base(options)
        {
        }

        public DbSet<UserToken> eletokens { get; set; }
    }
}
