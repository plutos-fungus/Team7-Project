using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfsUp.Data;
using System;
using System.Linq;

namespace SurfsUp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SurfsUpContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfsUpContext>>()))
            {
                // Look for any movies.
                if (context.Surfboard.Any())
                {
                    return;   // DB has been seeded
                }

                context.Surfboard.AddRange(
                    new Surfboard
                    {
                        Name = "Jonathan",
                        BoardType = 1,
                        Length = 1,
                        Width = 2,
                        Thickness = 10,
                        Volume = 1,
                        Price = 30,
                        EquipmentTypes = "Kasper"

                    }
                );
                context.SaveChanges();
            }
        }
    }
}