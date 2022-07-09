// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommandsService.SyndDataServices.Grpc;

namespace CommandsService.Data
{
    public static class PrepareDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            Console.WriteLine("--> Seeding data");
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>()!;
                if (isProduction)
                {
                    FetchPlatformsFromPlatformService(repo, serviceScope.ServiceProvider.GetService<IPlatformDataClient>()!);
                }
                else
                {
                    CreateDefaultSeed(repo);
                }
            }
        }


        private static void CreateDefaultSeed(ICommandRepo repo)
        {
            Console.WriteLine("--> Creating default platforms and commands");
            repo.CreatePlatform(new Models.Platform() { Name = "Docker", ExternalId = 1 });
            repo.CreatePlatform(new Models.Platform() { Name = "Git", ExternalId = 2 });
            repo.CreatePlatform(new Models.Platform() { Name = "Kubernetes", ExternalId = 3 });
            repo.SaveChanges();

            repo.CreateCommand(1, new Models.Command() { HowTo = "Build an image", CommandLine = "docker build -t dockerhub/imageName ." });
            repo.CreateCommand(1, new Models.Command() { HowTo = "Push an image or a repository to a registry", CommandLine = "docker push imageName" });


            repo.CreateCommand(2, new Models.Command() { HowTo = "Initialize a new Git Repo", CommandLine = "git init" });
            repo.CreateCommand(2, new Models.Command() { HowTo = "Add file contents to the index", CommandLine = "git add" });
            repo.CreateCommand(2, new Models.Command() { HowTo = "Record changes to the repository", CommandLine = "git commit -m 'Initial commit'" });
            repo.CreateCommand(2, new Models.Command() { HowTo = "Add new tracked repository", CommandLine = "git remote add origin git@github.com:Pat-Riz/microservice-api-example.git" });
            repo.CreateCommand(2, new Models.Command() { HowTo = "Update remote refs and setup upstream", CommandLine = "git push -u origin master" });

            repo.CreateCommand(3, new Models.Command() { HowTo = "Creating objects", CommandLine = "kubectl apply -f ./my-manifest.yaml" });
            repo.CreateCommand(3, new Models.Command() { HowTo = "Viewing resources ", CommandLine = "kubectl get services \n kubectl get pods --all-namespaces" });
            repo.SaveChanges();
        }

        private static void FetchPlatformsFromPlatformService(ICommandRepo repo, IPlatformDataClient grpcClient)
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

