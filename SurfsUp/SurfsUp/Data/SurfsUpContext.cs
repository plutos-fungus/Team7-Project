using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Models;

namespace SurfsUp.Data
{
    public class SurfsUpContext : DbContext
    {
        public SurfsUpContext (DbContextOptions<SurfsUpContext> options)
            : base(options)
        {
        }

        public DbSet<SurfsUp.Models.Surfboard> Surfboard { get; set; } = default!;

        public DbSet<SurfsUp.Models.Rental>? Rental { get; set; }
    }
}
