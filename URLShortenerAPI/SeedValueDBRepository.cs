namespace URLShortenerAPI
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    public class SeedValueDBRepository
    {
        private static CosmosClient cosmosClient;

        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("ENDPOINT_URI");
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PRIMARY_KEY");

        private static readonly string databaseId = Environment.GetEnvironmentVariable("DATABASE_ID");
        private static readonly string seedValueContainerID = Environment.GetEnvironmentVariable("CONTAINER_ID_FOR_SEED_VALUE");
        private static readonly string seedID = Environment.GetEnvironmentVariable("SEED_ID");

        private static readonly int initialSeedValue = new Random().Next(1000, 9999);

        public static void Initialize()
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Wait();
            Database database = cosmosClient.GetDatabase(databaseId);
            database.CreateContainerIfNotExistsAsync(seedValueContainerID, "/id").Wait();
            CreateSeedValueItemIfNotExistsAsync(initialSeedValue).Wait();
        }

        public static async Task CreateSeedValueItemIfNotExistsAsync(int seedValue)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container seedValueContainer = database.GetContainer(seedValueContainerID);
            Models.SeedValue seed = new Models.SeedValue
            {
                Id = seedID,
                Value = seedValue
            };
            try
            {
                ItemResponse<Models.SeedValue> seedValueResponse =
                    await seedValueContainer.ReadItemAsync<Models.SeedValue>(seed.Id, new PartitionKey(seed.Id));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<Models.SeedValue> seedValueResponse =
                    await seedValueContainer.CreateItemAsync(seed, new PartitionKey(seed.Id));
            }
        }

        public static async void UpdateSeedValueItemAsync(int seedValue)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container seedValueContainer = database.GetContainer(seedValueContainerID);
            Models.SeedValue newSeedValue = new Models.SeedValue
            {
                Id = seedID,
                Value = seedValue,
            };
            await seedValueContainer.ReplaceItemAsync(newSeedValue, newSeedValue.Id);
        }

        public static async Task<int> GetCurrentSeedValueAsync()
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container seedValueContainer = database.GetContainer(seedValueContainerID);
            ItemResponse<Models.SeedValue> seedValueResponse =
                await seedValueContainer.ReadItemAsync<Models.SeedValue>(seedID, new PartitionKey(seedID));
            return seedValueResponse.Resource.Value;
        }
    }
}
