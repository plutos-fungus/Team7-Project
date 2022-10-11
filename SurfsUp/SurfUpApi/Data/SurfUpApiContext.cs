using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurfUpApi.Models;

namespace SurfUpApi.Data
{
    public class SurfUpApiContext : IdentityDbContext
    {
        public SurfUpApiContext (DbContextOptions<SurfUpApiContext> options)
            : base(options)
        {
        }

        public DbSet<SurfUpApi.Models.Rental> Rental { get; set; } = default!;

        public DbSet<SurfUpApi.Models.Surfboard> Surfboard { get; set; }
    }
}
