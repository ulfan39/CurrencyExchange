using System;
using System.ServiceModel;
using CurrencyService;

namespace CurrencyHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8080/CurrencyService");

            ServiceHost host = new ServiceHost(typeof(CurrencyServiceImpl), baseAddress);
            host.AddServiceEndpoint(typeof(ICurrencyService), new BasicHttpBinding(), "");

            host.Open();
            Console.WriteLine("Service is running at: " + baseAddress);
            Console.WriteLine("Press Enter to stop...");
            Console.ReadLine();
            host.Close();
        }
    }
}