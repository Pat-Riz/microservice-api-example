// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Startup;
using PlatformService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.RegisterServices();

var app = builder.Build();

app.ConfigureSwagger();

app.UseHttpsRedirection();
app.UseCors();

Console.WriteLine($"--> CommandService running at {builder.Configuration["CommandService"]}");
PrepareDb.PrepPopulation(app, app.Environment.IsProduction());

app.MapGrpcService<GrpcPlatformService>();

app.MapPlatformEndpoints();

app.Run();

