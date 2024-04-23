using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using WebApp.Models;

namespace WebApp.Services
{
    public class RabbitService
    {

        public void Send(CommandParam command)
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

            var message = JsonConvert.SerializeObject(command);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "cmd",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($" [x] Sent {message}");

        }

    }
}
