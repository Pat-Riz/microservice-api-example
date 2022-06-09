// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommandsService.SyndDataServices.Grpc;

namespace CommandsService.Data
{
    public static class PrepareDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>()!,
                    serviceScope.ServiceProvider.GetService<IPlatformDataClient>()!);
            }
        }

        private static void SeedData(ICommandRepo repo, IPlatformDataClient grpcClient)
        {
            var platforms = grpcClient.GetAllPlatforms();
            Console.WriteLine($"--> Creating {platforms.Count()} platforms");
            foreach (var plat in platforms)
            {
                if (!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);

                }
            }
            repo.SaveChanges();
        }
    }
}
