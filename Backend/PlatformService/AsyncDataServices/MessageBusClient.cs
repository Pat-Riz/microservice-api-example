// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private const string EXCHANGE = "trigger";

        public MessageBusClient(IConfiguration config)
        {
            Console.WriteLine("--> Creating messagebus");
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQHost"],
                Port = int.Parse(config["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: EXCHANGE, type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMq_ConnectionShutDown;
                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Dude, where is my bus?: {ex.Message}");
            }
        }

        private void RabbitMq_ConnectionShutDown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown...");
            Dispose();
        }

        public void Dispose()
        {
            Console.WriteLine("Messagebus Disposed");
            if (_channel is not null && _channel.IsOpen)
            {
                _channel.Close();
                if (_connection is not null)
                {
                    _connection.Close();
                    _connection.ConnectionShutdown -= RabbitMq_ConnectionShutDown;
                }
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platform)
        {
            var message = JsonSerializer.Serialize(platform);
            if (_connection is not null && _connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection Open, sending message");
                SendMessage(message);
            }
        }



        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(EXCHANGE, routingKey: "", basicProperties: null, body);
            Console.WriteLine($"--> We have sent {message}");
        }
    }
}
