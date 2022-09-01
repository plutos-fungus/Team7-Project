using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SurfsUp.Data;

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
                        ID = 0,
                        Name = "The boof",
                        BoardType = Surfboard.BoardTypes.SUP,
                        Length = 5,
                        Width = 10,
                        Thickness = 2,
                        Volume = 100,
                        Price = 400
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
