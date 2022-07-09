// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepareDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!, isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {

            if (isProduction)
            {
                Console.WriteLine("--> Attempting to run migrations..");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Could not run migrations! {ex.Message}");
                }

            }

            if (!context.Platforms.Any())
            {
                try
                {
                    Console.WriteLine("--> Seeding default data...");
                    context.Platforms.AddRange(
                        new Platform() { Name = "Docker", Publisher = "Microsoft", Cost = "Free" },
                        new Platform() { Name = "Git", Publisher = "Gitsoft", Cost = "Free" },
                        new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                       );
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"--->ERROR {ex.Message}");
                }


            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}
