using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CosmosDb
{
    class CosmosDbService
    {
        private DocumentClient client;

        public CosmosDbService(string uri, string primaryKey)
        {
            client = new DocumentClient(new Uri(uri), primaryKey);
        }

        public async Task CreateDb()
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "OrganizationDB" });
            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("OrganizationDB"), new DocumentCollection { Id = "OrganizationCollection" });
            Console.WriteLine("Created database");

            var contoso = new Organization()
            {
                Id = "Contoso.LTD",
                Name = "Contoso",
                Age = 10,
                Workers = new Worker[]
                {
                    new Worker
                    {
                        FirstName = "John",
                        LastName = "Rain",
                        Age = 25,
                        Salary = 3000
                    },
                    new Worker
                    {
                        FirstName = "Harry",
                        LastName = "Francisco",
                        Age = 30,
                        Salary = 4000
                    },
                    new Worker
                    {
                        FirstName = "Jane",
                        LastName = "Knife",
                        Age = 40,
                        Salary = 5000
                    }
                }
            };

            await CreateOrganization("OrganizationDB", "OrganizationCollection", contoso);

            ExecuteLinqQuery();
        }

        public async Task UpdateOrganizationName(string organizationId, string newName)
        {
            var queryOptions = new FeedOptions { MaxItemCount = 1 };

            var organizations = this.client.CreateDocumentQuery<Organization>(
                    UriFactory.CreateDocumentCollectionUri("OrganizationDB", "OrganizationCollection"), queryOptions)
                    .Where(o => o.Id == organizationId);

            foreach (var organization in organizations)
            {
                organization.Name = newName;
                await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("OrganizationDB", "OrganizationCollection", organizationId), organization);
            }

            Console.WriteLine("Query after update:");
            ExecuteSqlQuery();
        }

        private async Task CreateOrganization(string databaseName, string collectionName, Organization organization)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, organization.Id));
                Console.WriteLine($"Organization {organization.Id} already exists");
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), organization);
                    Console.WriteLine($"Created organization {organization.Id}");
                }
                else
                {
                    Console.WriteLine($"Something went wrong. Organization {organization.Id} wasn't created.");
                }
            }
        }

        private void ExecuteLinqQuery()
        {
            var queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Organization> organizationQuery = this.client.CreateDocumentQuery<Organization>(
                    UriFactory.CreateDocumentCollectionUri("OrganizationDB", "OrganizationCollection"), queryOptions)
                    .Where(o => o.Name.Contains("Contoso"));

            Console.WriteLine("LINQ query:");

            foreach (var organization in organizationQuery)
            {
                Console.WriteLine("Read {0}", organization);
            }
        }

        private void ExecuteSqlQuery()
        {
            var queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Organization> organizationQuery = this.client.CreateDocumentQuery<Organization>(
                    UriFactory.CreateDocumentCollectionUri("OrganizationDB", "OrganizationCollection"),
                    "SELECT * FROM Organization",
                    queryOptions);

            Console.WriteLine("SQL query:");

            foreach (var organization in organizationQuery)
            {
                Console.WriteLine("Read {0}", organization);
            }
        }
    }
}
