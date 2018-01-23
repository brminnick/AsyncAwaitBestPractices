using Newtonsoft.Json;

namespace AsyncAwaitBestPractices
{
    public abstract class CosmosDbModel<T>  where T : CosmosDbModel<T>
    {
        public static string CollectionId => typeof(T).Name;
        public static string DatabaseId => "CosmosDbSampleAppDatabase";

        public string TypeName => typeof(T).Name;

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
