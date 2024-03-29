﻿#nullable disable
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CommandsService.Dtos
{
    public record CommandReadDto
    {
        public int Id { get; set; }

        public int PlatformId { get; set; }

        public string HowTo { get; set; }

        public string CommandLine { get; set; }

    }
}
