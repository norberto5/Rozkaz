using Newtonsoft.Json;
using System;

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

    public override bool Equals(object obj) => obj is User user ? Id == user.Id : false;
    public override int GetHashCode() => HashCode.Combine(Id);
    public static bool operator ==(User user1, User user2) => user1 is null ? user2 is null : user1.Equals(user2);
    public static bool operator !=(User user1, User user2) => !(user1 == user2);
  }
}
