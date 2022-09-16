﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SurfsUp.Areas.Identity.Data;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SurfsUp.Models
{

    public static class SeedData
    {
       
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {

                //Sætter "roleManager" variable til typen "RoleManager" med type parameter af "IdentityRole"
                var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
                var roleName = "Admin";
                IdentityResult result;

                //checker om "roleManager" har en existerende "Admin" bruger
                bool roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    //Ópretter en ny admin bruger, hvis ingen bruger har en admin rolle. 
                    result = await roleManager
                    .CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        var userManager = serviceProvider
                            .GetRequiredService<UserManager<IdentityUser>>();
                        var config = serviceProvider
                            .GetRequiredService<IConfiguration>();
                        var admin = await userManager
                            .FindByEmailAsync(config["AdminCredentials:Email"]);

                        if (admin == null)
                        {
                            admin = new IdentityUser()
                            {
                                UserName = config["AdminCredentials:Email"],
                                Email = config["AdminCredentials:Email"],
                                EmailConfirmed = true
                            };
                            result = await userManager
                                .CreateAsync(admin, config["AdminCredentials:Password"]);
                            if (result.Succeeded)
                            {
                                result = await userManager
                                    .AddToRoleAsync(admin, roleName);
                                if (!result.Succeeded)
                                {
                                    // todo: process errors
                                }
                            }
                        }
                        var user = await userManager
                           .FindByEmailAsync("test@test.dk");
                        if (user == null)
                        {
                            user = new IdentityUser()
                            {
                                UserName = "test@test.dk",
                                Email = "test@test.dk",
                                EmailConfirmed = true
                            };
                            result = await userManager
                                .CreateAsync(user, "Test12345!");
                        }
                    }
                }

                

                // Look for any movies.
                else if (context.Surfboard.Any())
                {
                    return;   // DB has been seeded
                }

                context.Surfboard.AddRange(
                    new Surfboard
                    {
                        Name = "The boof",
                        BoardType = Surfboard.BoardTypes.SUP,
                        Length = 5,
                        Width = 10,
                        Thickness = 2,
                        Volume = 100,
                        Price = 400
                    },
                    new Surfboard
                    {
                        Name = "The Minilog",
                        BoardType = Surfboard.BoardTypes.shortboard,
                        Length = 6,
                        Width = 21,
                        Thickness = 2.75,
                        Volume = 38.8,
                        Price = 565
                    },
                    new Surfboard
                    {
                        Name = "The Wide Glider",
                        BoardType = Surfboard.BoardTypes.funboard,
                        Length = 7.1,
                        Width = 21.75,
                        Thickness = 2.75,
                        Volume = 44.16,
                        Price = 685
                    },
                    new Surfboard
                    {
                        Name = "The Golden Ratio",
                        BoardType = Surfboard.BoardTypes.funboard,
                        Length = 6.3,
                        Width = 21.85,
                        Thickness = 2.9,
                        Volume = 43.22,
                        Price = 695
                    },
                    new Surfboard
                    {
                        Name = "Mahi Mahi",
                        BoardType = Surfboard.BoardTypes.fish,
                        Length = 5.4,
                        Width = 20.75,
                        Thickness = 2.3,
                        Volume = 29.39,
                        Price = 645
                    },
                    new Surfboard
                    {
                        Name = "The Emerald Glider",
                        BoardType = Surfboard.BoardTypes.longboard,
                        Length = 9.2,
                        Width = 22.8,
                        Thickness = 2.8,
                        Volume = 65.4,
                        Price = 895
                    },
                    new Surfboard
                    {
                        Name = "The Bomber",
                        BoardType = Surfboard.BoardTypes.shortboard,
                        Length = 5.5,
                        Width = 21,
                        Thickness = 2.5,
                        Volume = 33.7,
                        Price = 645
                    },
                    new Surfboard
                    {
                        Name = "Walden Magic",
                        BoardType = Surfboard.BoardTypes.longboard,
                        Length = 9.6,
                        Width = 19.4,
                        Thickness = 3,
                        Volume = 80,
                        Price = 1025
                    },
                    new Surfboard
                    {
                        Name = "Naish One",
                        BoardType = Surfboard.BoardTypes.SUP,
                        Length = 12.6,
                        Width = 30,
                        Thickness = 6,
                        Volume = 301,
                        Price = 854,
                        EquipmentTypes = "Paddle"
                    },
                    new Surfboard
                    {
                        Name = "Six Tourer",
                        BoardType = Surfboard.BoardTypes.SUP,
                        Length = 11.6,
                        Width = 32,
                        Thickness = 6,
                        Volume = 270,
                        Price = 611,
                        EquipmentTypes = "Fin, Paddle, Pump, Leash"
                    },
                    new Surfboard
                    {
                        Name = "Naish Maliko",
                        BoardType = Surfboard.BoardTypes.SUP,
                        Length = 14,
                        Width = 25,
                        Thickness = 6,
                        Volume = 330,
                        Price = 1304,
                        EquipmentTypes = "Fin, Paddle, Pump, Leash"
                    }
                );
                context.SaveChanges();
            }


        }
    }
}
