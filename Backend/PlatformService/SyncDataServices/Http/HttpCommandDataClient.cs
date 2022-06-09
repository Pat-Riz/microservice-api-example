// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(JsonSerializer.Serialize(platform), Encoding.UTF8, "application/json");
            //We dont POST anymore. CommandService recive this information from the Messagebus
            //var response = await _httpClient.PostAsync(_config["CommandService"], httpContent);

            var response = await _httpClient.GetAsync(_config["CommandService"]);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--> Sync POST to commandeservice was OK {res}");
            }
            else
            {
                Console.WriteLine($"--> Sync POST to commandeservice was NOT OK {response.Content.ToString}");

            }
        }
    }
}
