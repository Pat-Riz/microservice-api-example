// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommandsService.Models;

namespace CommandsService.SyndDataServices.Grpc
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> GetAllPlatforms();
    }
}
