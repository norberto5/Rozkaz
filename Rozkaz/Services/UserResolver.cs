using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web.Client;
using Newtonsoft.Json.Linq;
using Rozkaz.Models;
using Rozkaz.Utils;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using WebApp_OpenIDConnect_DotNet.Services.GraphOperations;

namespace Rozkaz.Services
{
  public class UserResolver
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ITokenAcquisition tokenAcquisition;
    private readonly IGraphApiOperations graphApiOperations;

    private readonly RozkazDatabaseContext db;

    public UserResolver(IHttpContextAccessor httpContextAccessor, ITokenAcquisition tokenAcquisition, IGraphApiOperations graphApiOperations, RozkazDatabaseContext rozkazDatabaseContext)
    {
      this.httpContextAccessor = httpContextAccessor;
      this.tokenAcquisition = tokenAcquisition;
      this.graphApiOperations = graphApiOperations;
      db = rozkazDatabaseContext;
    }

    public async Task<User> GetUser()
    {
      User user = httpContextAccessor.HttpContext.Session.GetUser();
      if (user != null)
      {
        return user;
      }

      string accessToken = await tokenAcquisition.GetAccessTokenOnBehalfOfUser(httpContextAccessor.HttpContext, new[] { Constants.ScopeUserRead });

      var userInformation = await graphApiOperations.GetUserInformation(accessToken) as JObject;

      user = userInformation.ToObject<User>();

      User dbUser = db.Users.Find(user.Id);

      if (dbUser == null)
      {
        db.Users.Add(user);
        db.SaveChanges();
      }
      else
      {
        user = dbUser;
      }

      httpContextAccessor.HttpContext.Session.Set("User", user);

      return user;
    }
  }
}
