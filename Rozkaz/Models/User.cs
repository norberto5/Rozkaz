using Newtonsoft.Json;

namespace Rozkaz.Models
{
    public class User
    {
        public string Id { get; set; }

        [JsonProperty("givenName")]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Mail { get; set; }

        public UnitModel Unit { get; set; }
    }
}
