// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platform = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
                var platformModel = _mapper.Map<Platform>(platform);

                try
                {
                    if (!repo.ExternalPlatformExists(platformModel.Id))
                    {
                        repo.CreatePlatform(platformModel);
                        if (repo.SaveChanges())
                        {
                            Console.WriteLine($"--> Created Platform from Messagebus: {platformModel.Id}");
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"--> Error while saving platform: {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType?.Event)
            {
                case "Platform_Published":
                    Console.WriteLine($"--> Event is PlatformPublished");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine($"--> Could not determine event");
                    return EventType.Undefined;
            }
        }


    }
    enum EventType
    {
        PlatformPublished,
        Undefined
    }

}
