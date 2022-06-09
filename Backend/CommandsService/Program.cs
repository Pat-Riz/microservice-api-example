// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.EventProcessing;
using CommandsService.Models;
using CommandsService.SyndDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemory"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
builder.Services.AddHostedService<MessageBusSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("--> Is dev: Setting up swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (app.Environment.IsProduction())
{
    //TODO: grpcClient.GetAllPlatforms(); don't work locally? Fail to connect to grpcServer
    PrepareDb.PrepPopulation(app);
}


app.MapGet("/c/platforms", (ICommandRepo repo, IMapper mapper) =>
{
    var platforms = repo.GetAllPlatforms();
    return Results.Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
}).Produces<CommandReadDto>();

app.MapGet("/c/platforms/{plaftormId}/commands", (ICommandRepo repo, IMapper mapper, int plaftormId) =>
{
    Console.WriteLine($"--> Gettings commands for id: {plaftormId}");

    if (!repo.PlatformExists(plaftormId))
    {
        return Results.NotFound("Platform not found");
    }

    var commands = repo.GetCommandsForPlatform(plaftormId);
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
}).Produces<CommandReadDto>();

app.MapPost("/c/platforms/{plaftormId}/commands", (ICommandRepo repo, IMapper mapper, int plaftormId, CommandCreateDto commandDto) =>
{
    if (!repo.PlatformExists(plaftormId)) return Results.NotFound("Platform not found");

    var command = mapper.Map<Command>(commandDto);
    repo.CreateCommand(plaftormId, command);
    if (repo.SaveChanges())
    {
        var readDto = mapper.Map<CommandReadDto>(command);
        return Results.CreatedAtRoute("GetCommand", new { plaftormId = plaftormId, commandId = readDto.Id }, readDto);
    }

    return Results.Conflict("Couldent create command");
}).Produces<CommandReadDto>();

app.MapGet("/c/platforms/{plaftormId}/commands/{commandId}", (ICommandRepo repo, IMapper mapper, int plaftormId, int commandId) =>
{
    if (!repo.PlatformExists(plaftormId)) return Results.NotFound("Platform not found");

    var command = repo.GetCommand(plaftormId, commandId);

    if (command == null) return Results.NotFound("Command not found");

    return Results.Ok(mapper.Map<CommandReadDto>(command));
}).WithName("GetCommand").Produces<CommandReadDto>();



app.Run();
