using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using System.Threading.Tasks;
using System.Web;
using WebApp_OpenIDConnect_DotNet.Services.GraphOperations;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Rozkaz.Models;
using Rozkaz.Utils;

namespace Rozkaz.Controllers
{
    public class UserController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IGraphApiOperations graphApiOperations;

        private readonly RozkazDatabaseContext db;

        public UserController(ITokenAcquisition tokenAcquisition, IGraphApiOperations graphApiOperations, RozkazDatabaseContext rozkazDatabaseContext)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.graphApiOperations = graphApiOperations;
            db = rozkazDatabaseContext;
        }

        public IActionResult Login() => new LocalRedirectResult("/AzureAD/Account/SignIn");

        [Authorize]
        [MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<IActionResult> Index()
        {
            string accessToken = await tokenAcquisition.GetAccessTokenOnBehalfOfUser(HttpContext, new[] { Constants.ScopeUserRead });

            var userInformation = await graphApiOperations.GetUserInformation(accessToken) as JObject;

            User user = db.Users.Find(userInformation.Property("id").Value.ToString());

            if (user == null)
            {
                db.Add(userInformation.ToObject<User>());
                db.SaveChanges();
            }

            HttpContext.Session.SetString("Name", user.Name);
            HttpContext.Session.Set("User", user);

            return View(user);
        }

        [Authorize]
        public IActionResult Logout()
        {
            string redirectPage = Url.Action(nameof(LoggedOut), "User", null, HttpContext.Request.Scheme);
            return new RedirectResult("https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=" + HttpUtility.UrlEncode(redirectPage));
        }

        public IActionResult LoggedOut() => View();
    }
}