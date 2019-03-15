using Newtonsoft.Json;

namespace CosmosDb
{
    class Organization
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Worker[] Workers { get; set; }

        public override string ToString()
            => JsonConvert.SerializeObject(this);
    }
}
