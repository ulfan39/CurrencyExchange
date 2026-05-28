using System;
using System.ServiceModel;
using CurrencyService;

namespace CurrencyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            EndpointAddress address = new EndpointAddress("http://localhost:8080/CurrencyService");
            BasicHttpBinding binding = new BasicHttpBinding();

            ChannelFactory<ICurrencyService> factory =
                new ChannelFactory<ICurrencyService>(binding, address);

            ICurrencyService client = factory.CreateChannel();

            Console.WriteLine(client.GetExchangeRate("USD"));
            Console.WriteLine(client.GetExchangeRate("EUR"));
            Console.WriteLine(client.GetExchangeRate("GBP"));

            Console.ReadLine();
        }
    }
}