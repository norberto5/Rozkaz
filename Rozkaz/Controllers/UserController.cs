using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Rozkaz.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login() => new LocalRedirectResult("/AzureAD/Account/SignIn");

        [Authorize]
        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Logout()
        {
            string redirectPage = Url.Action(nameof(LoggedOut), "User", null, Url.ActionContext.HttpContext.Request.Scheme);
            return new RedirectResult("https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=" + HttpUtility.UrlEncode(redirectPage));
        }

        public IActionResult LoggedOut() => View();
    }
}