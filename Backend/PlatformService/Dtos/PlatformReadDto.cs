// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable
namespace PlatformService.Dtos
{
    public record PlatformReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public string Cost { get; set; } = null!;


    }
}
