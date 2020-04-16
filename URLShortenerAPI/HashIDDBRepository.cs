namespace URLShortenerAPI
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public static class HashIDDBRepository
    {
        private static CosmosClient cosmosClient;

        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("ENDPOINT_URI");
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PRIMARY_KEY");

        private static readonly string databaseId = Environment.GetEnvironmentVariable("DATABASE_ID");
        private static readonly string hashIDContainerID = Environment.GetEnvironmentVariable("CONTAINER_ID_FOR_HASH_ID");

        public static void Initialize()
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Wait();
            Database database = cosmosClient.GetDatabase(databaseId);
            database.CreateContainerIfNotExistsAsync(hashIDContainerID, "/id").Wait();
        }

        public static async Task<Models.ShortLink> GetHashIDItemAsync(string hashID)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container hashIDContainer = database.GetContainer(hashIDContainerID);
            int seedValue = HashIDGenerator.DecodeSeedValue(hashID);
            ItemResponse<Models.ShortLink> shortLinkResponse = 
                await hashIDContainer.ReadItemAsync<Models.ShortLink>(seedValue.ToString(), new PartitionKey(seedValue.ToString()));
            return shortLinkResponse;
        }

        public static async Task<Models.ShortLink> CreateHashIDItemIfNotExistsAsync(string url)
        {
            Database database = cosmosClient.GetDatabase(databaseId);
            Container hashIDContainer = database.GetContainer(hashIDContainerID);
            int currentSeedValue = await SeedValueDBRepository.GetCurrentSeedValueAsync();
            string hashID = HashIDGenerator.EncodeSeedValue(currentSeedValue);
            Models.ShortLink shortLinkResponse = hashIDContainer.GetItemLinqQueryable<Models.ShortLink>(true)
                                                                 .Where(shortLink => shortLink.Url == url)
                                                                 .AsEnumerable()
                                                                 .FirstOrDefault();
            if (shortLinkResponse == null)
            {
                Models.ShortLink newShortlink = new Models.ShortLink
                {
                    Id = currentSeedValue.ToString(),
                    Url = url,
                    HashID = hashID,
                    CreatedTime = DateTime.UtcNow,
                };
                shortLinkResponse =
                    await hashIDContainer.CreateItemAsync(newShortlink, new PartitionKey(newShortlink.Id));
                currentSeedValue = await SeedValueDBRepository.GetCurrentSeedValueAsync();
                SeedValueDBRepository.UpdateSeedValueItemAsync(currentSeedValue + 1);
            }
            return shortLinkResponse;
        }
    }
}
