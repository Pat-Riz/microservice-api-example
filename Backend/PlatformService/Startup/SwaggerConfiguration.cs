// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace PlatformService.Startup;

public static class SwaggerConfiguration
{
    public static WebApplication ConfigureSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("--> Is dev: Setting up swagger");
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
