// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace PlatformService.Startup;
using AutoMapper;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

public static class MapEndpoints
{
    public static WebApplication MapPlatformEndpoints(this WebApplication app)
    {
        app.MapGet("/protos/platforms.proto", async (HttpContext context) =>
        {
            await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
        });

        app.MapGet("/platforms", (IPlatformRepo repo, IMapper mapper) =>
        {
            var platforms = repo.GetAllPlatforms();
            return Results.Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }).Produces<IEnumerable<PlatformReadDto>>();

        app.MapGet("/platforms/{id}", (IPlatformRepo repo, IMapper mapper, int id) =>
        {
            var platform = repo.GetPlatformById(id);
            if (platform is null) return Results.NotFound();

            return Results.Ok(mapper.Map<PlatformReadDto>(platform));
        }).WithName("GetPlatformById").Produces<PlatformReadDto>();

        app.MapPost("/platforms", async (IPlatformRepo repo, IMapper mapper, ICommandDataClient httpClient, PlatformCreateDto dto, IMessageBusClient messageBus) =>
        {
            var platformModel = mapper.Map<Platform>(dto);
            repo.CreatePlatform(platformModel);
            if (repo.SaveChanges())
            {
                var readDto = mapper.Map<PlatformReadDto>(platformModel);
                try
                {
                    await httpClient.SendPlatformToCommand(readDto);
                    var dtoToPublish = mapper.Map<PlatformPublishedDto>(readDto);
                    dtoToPublish.Event = "Platform_Published";
                    messageBus.PublishNewPlatform(dtoToPublish);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not talk to CommandService: {ex.Message}");

                }

                return Results.CreatedAtRoute("GetPlatformById", new { id = readDto.Id }, readDto);
            }
            return Results.BadRequest();
        }).Produces<PlatformReadDto>();

        return app; ;
    }
}
