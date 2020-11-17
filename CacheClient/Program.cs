using CacheCow.Client;
using CacheCow.Client.RedisCacheStore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CacheClient
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var client = ClientExtensions.CreateClient(new RedisStore("localhost:6379"));
            var baseAddress = new Uri("http://localhost:1337");
            client.BaseAddress = baseAddress;

            const string quitCommand = "doneyo";
            bool quit = false;

            while (!quit)
            {
                Console.WriteLine("Hit Enter to Call the API");
                Console.ReadLine();

                var response = await client.GetAsync("/cache/time");
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Headers.CacheControl.ToString());
                Console.WriteLine(content);
                Console.WriteLine($"Type '{quitCommand}' to quit");
                
                quit = (Console.ReadLine() == quitCommand);
            }
        }
    }
}
