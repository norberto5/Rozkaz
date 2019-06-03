using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rozkaz.Models;

namespace Rozkaz.Utils
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static User GetUser(this ISession session)
        {
            string value = session.GetString("User");
            return value == null ? default(User) : JsonConvert.DeserializeObject<User>(value);
        }
    }
}
