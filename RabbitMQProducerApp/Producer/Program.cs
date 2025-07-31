using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

class Program
{
    static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        IConfigurationSection rabbitConfig = config.GetSection("RabbitMQ");
        string host = rabbitConfig["HostName"] ?? "localhost";
        int port = int.Parse(rabbitConfig["Port"] ?? "5672");
        string username = rabbitConfig["Username"] ?? "guest";
        string password = rabbitConfig["Password"] ?? "guest";
        string queue = rabbitConfig["Queue"] ?? "default_queue";

        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password
        };

        IConnection connection = factory.CreateConnection();
        IModel channel = connection.CreateModel();

        channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);

        IBasicPublishBatch batch = channel.CreateBasicPublishBatch();

        for (int i = 1; i <= 10000; i++)
        {
            string message = $"Batch message {i} at {DateTime.Now}";
            byte[] body = Encoding.UTF8.GetBytes(message);

            IBasicProperties props = channel.CreateBasicProperties();
            props.Persistent = true;

            batch.Add(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                properties: props,
                body: new ReadOnlyMemory<byte>(body)
            );
        }

        batch.Publish();
        Console.WriteLine("Batch published.");
    }
}
