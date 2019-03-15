using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CosmosDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var cosmosDbService = new CosmosDbService(configuration.GetConnectionString("Uri"), configuration.GetConnectionString("PrimaryKey"));

            cosmosDbService.CreateDb().Wait();

            cosmosDbService.UpdateOrganizationName("Contoso.LTD", "ContosoNewName").Wait();

            Console.ReadKey();
        }
    }
}
