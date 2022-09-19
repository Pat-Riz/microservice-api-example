// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommandsService.Data;
using CommandsService.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwagger();

app.UseHttpsRedirection();
app.UseCors();

PrepareDb.PrepPopulation(app, app.Environment.IsProduction());

app.MapCommandsEndpoints();

app.Run();
