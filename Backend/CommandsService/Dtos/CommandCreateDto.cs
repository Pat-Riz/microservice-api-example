// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CommandsService.Dtos
{
    public record CommandCreateDto
    {
        public string HowTo { get; init; } = null!;

        public string CommandLine { get; init; } = null!;
    }
}
