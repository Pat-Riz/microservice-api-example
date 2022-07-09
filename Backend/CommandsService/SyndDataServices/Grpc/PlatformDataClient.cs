// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyndDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        public IEnumerable<Platform> GetAllPlatforms()
        {
            var grpcAddress = _config["GrpcPlatform"];
            Console.WriteLine($"--> Calling gRPC Service {grpcAddress}");
            var channel = GrpcChannel.ForAddress(grpcAddress);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Failed talking to gRPC platform service: {ex.Message}");
                return new List<Platform>();
            }
        }
    }
}
