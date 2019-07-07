using Newtonsoft.Json;
using System;

namespace Rozkaz.Models
{
    public class User : IComparable, IEquatable<User>
    {
        public string Id { get; set; }

        [JsonProperty("givenName")]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Mail { get; set; }

        public UnitModel Unit { get; set; }


        public int CompareTo(object obj) => Id.CompareTo(obj);

        public bool Equals(User other) => Id == other.Id;
    }
}
