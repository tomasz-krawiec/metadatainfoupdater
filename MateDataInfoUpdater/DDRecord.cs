using Newtonsoft.Json;

namespace MetadataInformationUpdater
{
    public class DDRecord
    {
        [JsonProperty(Order = 1)]
        public int Id { get; set; }
        [JsonProperty(Order = 2)]
        public string ContextName { get; set; }
        [JsonProperty(Order = 3)]
        public string DataKey { get; set; }
        [JsonProperty(Order = 4)]
        public string BusinessDescription { get; set; }
        [JsonProperty(Order = 5)]
        public string FieldBehaviour { get; set; }
     
        [JsonProperty(Order = 6)]
        public string ComponentUniqueName
        {  get; set;
        }

        [JsonIgnore]
        public string Section { get; set; }
        [JsonIgnore]
        public string FieldType { get; set; }
        [JsonIgnore]
        public string ColumnName { get; set; }

        [JsonIgnore]
        public string TableName { get; set; }

        [JsonIgnore]
        public bool Processed { get; set; }
    }
}
