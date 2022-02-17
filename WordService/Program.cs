using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace WordService
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(WordService), new Uri($"http://{Settings.Default.ServiceHost}:{Settings.Default.ServicePort}/{Settings.Default.ServiceName}"));
            host.Description.Behaviors.Add(new ServiceMetadataBehavior
            {
                HttpGetEnabled = true,
                MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 },                
            });

            host.Description.Behaviors.Remove(
                typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(
                new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });

            host.AddServiceEndpoint(typeof(IWordService), new BasicHttpBinding(), "basic");

            try
            {
                host.Open();
            }
            catch
            {
                Console.WriteLine("Сервис уже запущен");
                return;
            }


            WordService service = new WordService();

            var factory = new ConnectionFactory()
            {
                //HostName = "localhost",
                Uri = new Uri(Settings.Default.MQUri)
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "suggestions",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    try
                    {
                        service.InsertWord(message, false);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to insert a word");
                    }
                    Console.WriteLine("A word is inserted");
                };
                channel.BasicConsume(queue: "suggestions",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Server is running...");
                Console.ReadLine();
            }

            Console.WriteLine("Server is running...");
            Console.ReadLine();
        }
    }
}
