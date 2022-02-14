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
            var host = new ServiceHost(typeof(WordServiceLibrary.WordService), new Uri($"http://localhost:8080/words"));
            host.Description.Behaviors.Add(new ServiceMetadataBehavior
            {
                HttpGetEnabled = true,
                MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 },                
            });

            host.Description.Behaviors.Remove(
                typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(
                new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });

            host.AddServiceEndpoint(typeof(WordServiceLibrary.IWordService), new BasicHttpBinding(), "basic");

            try
            {
                host.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сервис уже запущен");
                return;
            }
            Console.WriteLine("Server is running...");
            Console.ReadLine();
        }
    }
}
