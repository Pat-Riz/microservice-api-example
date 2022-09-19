// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace PlatformService.Startup;

using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

public static class DependencyInjectionSetup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IPlatformRepo, PlatformRepo>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        services.AddGrpc();

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
