// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IEventProcessor _eventProcessor;
        private IConnection? _connection;
        private IModel? _channel;
        private string? _queueName;
        private const string EXCHANGE = "trigger";

        public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor)
        {
            _config = config;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: EXCHANGE, type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, EXCHANGE, routingKey: "");

                Console.WriteLine("--> Connected on the MessageBus...");
                _connection.ConnectionShutdown += RabbitMq_ConnectionShutDown;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Dude, where is my bus?: {ex.Message}");
            }
        }

        private void RabbitMq_ConnectionShutDown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Bus Connection shutting down..");
        }

        public override void Dispose()
        {
            if (_channel is not null && _channel.IsOpen)
            {
                _channel.Close();
                if (_connection is not null)
                {
                    _connection.Close();
                    _connection.ConnectionShutdown -= RabbitMq_ConnectionShutDown;
                }
            }
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                var notificationMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer);
            return Task.CompletedTask;
        }



    }
}
