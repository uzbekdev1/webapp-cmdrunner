using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using CmdRunner.Helpers;
using System.Diagnostics;
using Newtonsoft.Json;
using CmdRunner.Models;

namespace CmdRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "ClLbxh7qmq7h_s"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "cmd",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($" [x] Received {message}");

                var payload = JsonConvert.DeserializeObject<CommandParam>(message);
                var result = await ProcessHelper.Execute(payload.File, new string[] { payload.Args }, TimeSpan.FromSeconds(3600));

                if (string.IsNullOrWhiteSpace(result.StandardError))
                {
                    Console.WriteLine(result.StandardOutput);
                }
                else
                {
                    Console.WriteLine(result.StandardError);
                }
            };
            channel.BasicConsume(queue: "cmd",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            Console.WriteLine("DONE!");
        }
    }
}
