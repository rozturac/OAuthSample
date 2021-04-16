using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OAuthSample.ConsoleClient
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            #region Resources Owner
            var discoRO = await DiscoveryClient.GetAsync("http://localhost:5050/");
            if (discoRO.IsError)
            {
                Console.WriteLine(discoRO.Error);
                return;
            }

            //Grab a bearer token using ResourceOwnerPassword Grant Type
            var tokenClientRO = new TokenClient(discoRO.TokenEndpoint, "ro.client", "secret");
            var tokenResponseRO = await tokenClientRO.RequestResourceOwnerPasswordAsync("Manish", "password", "OAuthSampleApi");
            if (tokenResponseRO.IsError)
            {
                Console.WriteLine(tokenResponseRO.Error);
                return;
            }

            Console.WriteLine(tokenResponseRO.Json);
            Console.WriteLine("\r\n");

            #endregion

            #region ClientCredentials
            //discover all the endpoints using metadata of identity server
            var disco = await DiscoveryClient.GetAsync("http://localhost:5050/");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //Grab a bearer token using Client Credential Grant Type
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("OAuthSampleApi");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\r\n");

            //Consume our costumer api
            using (var client = new HttpClient())
            {
                client.SetBearerToken(tokenResponse.AccessToken);

                var customerInfo = new StringContent(JsonConvert.SerializeObject(new
                {
                    Id = 18,
                    FirstName = "Rıdvan",
                    LastName = "OZTURAC"
                }), Encoding.UTF8, "application/json");

                var createCustomerResponse = await client.PostAsync("http://localhost:59143/api/customers", customerInfo);

                if (!createCustomerResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(createCustomerResponse.StatusCode);
                }

                var getCustomersResponse = await client.GetAsync("http://localhost:59143/api/customers");

                if (!createCustomerResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(createCustomerResponse.StatusCode);
                }
                else
                {
                    var content = await getCustomersResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
            }
            #endregion

            Console.ReadKey();
        }
    }
}
