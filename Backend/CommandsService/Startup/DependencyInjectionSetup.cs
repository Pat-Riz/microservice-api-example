// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CommandsService.Startup;

using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.SyndDataServices.Grpc;
using Microsoft.EntityFrameworkCore;
public static class DependencyInjectionSetup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemory"));
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ICommandRepo, CommandRepo>();
        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddScoped<IPlatformDataClient, PlatformDataClient>();
        services.AddHostedService<MessageBusSubscriber>();


        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
        });

        return services;
    }
}
