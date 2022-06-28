using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Hubtel.eCommerce.UnitTests
{
    public class IntegrationTest : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        protected IntegrationTest(out WebApplicationFactory<Startup> applicationFactory)
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                    });
                });

            _serviceProvider = appFactory.Services;
            applicationFactory = appFactory;
        }



        public void Dispose()
        {
        }
    }

    public static class IntegrationTestExtensions
    {
        public static async Task<string> GetJwtAsync(HttpClient httpClient)
        {
            var data = new
            {
                username = "Test",
                password = "Test"
            };
            var content = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/AuthenticationApi/login", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return (string)JsonConvert.DeserializeObject(responseString);
        }

        public static async Task<HttpClient> AuthenticateAsync(this HttpClient client)
        {
            var model = await GetJwtAsync(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.ToString());
            return client;
        }
        public static HttpClient Authenticate(this HttpClient client)
        {

            var model = IntegrationTestExtensions.GetJwtAsync(client).Result;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.ToString());
            return client;

        }
    }

    public class TestValues
    {
        public readonly IConfigurationRoot StubConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:HubtelECommerceDb", "UserID=postgres;Password=root;Host=localhost;Port=5432;Database=HubtelDbTests;"},
                {"Issuer:","https://localhost:44315"},
                {"Audience:", "https://localhost:44315"}
            }).Build();


    }

}
