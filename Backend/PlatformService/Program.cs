// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> Using InMemory DB");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemory"));
}
else
{
    Console.WriteLine("--> Using SQL server DB");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("--> Is dev: Setting up swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

Console.WriteLine($"--> CommandService running at {builder.Configuration["CommandService"]}");
PrepareDb.PrepPopulation(app, app.Environment.IsProduction());

app.MapGrpcService<GrpcPlatformService>();

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

app.Run();

