using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using System.Threading.Tasks;
using System.Web;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using Microsoft.AspNetCore.Http;
using Rozkaz.Models;
using Rozkaz.Services;

namespace Rozkaz.Controllers
{
    public class UserController : Controller
    {
        private readonly UserResolver userResolveService;
        private readonly RozkazDatabaseContext db;

        public UserController(UserResolver userResolveService, RozkazDatabaseContext rozkazDatabaseContext)
        {
            this.userResolveService = userResolveService;
            db = rozkazDatabaseContext;
        }

        public IActionResult Login() => new LocalRedirectResult("/AzureAD/Account/SignIn");

        [Authorize, MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<IActionResult> Index() => base.View(await userResolveService.GetUser());

        [Authorize]
        public IActionResult Logout()
        {
            string redirectPage = Url.Action(nameof(LoggedOut), "User", null, HttpContext.Request.Scheme);
            return new RedirectResult("https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=" + HttpUtility.UrlEncode(redirectPage));
        }

        public IActionResult LoggedOut() => View();
    }
}