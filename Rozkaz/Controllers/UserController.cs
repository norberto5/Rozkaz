using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using System.Threading.Tasks;
using System.Web;
using WebApp_OpenIDConnect_DotNet.Services.GraphOperations;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace Rozkaz.Controllers
{
    public class UserController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IGraphApiOperations graphApiOperations;

        public UserController(ITokenAcquisition tokenAcquisition, IGraphApiOperations graphApiOperations)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.graphApiOperations = graphApiOperations;
        }

        public IActionResult Login() => new LocalRedirectResult("/AzureAD/Account/SignIn");

        [Authorize]
        [MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<IActionResult> Index()
        {
            string accessToken = await tokenAcquisition.GetAccessTokenOnBehalfOfUser(HttpContext, new[] { Constants.ScopeUserRead });

            dynamic me = await graphApiOperations.GetUserInformation(accessToken);
            string photo = await graphApiOperations.GetPhotoAsBase64Async(accessToken);

            ViewData["Me"] = me;
            ViewData["Photo"] = photo;

            HttpContext.Session.SetString("Name", (me as JObject).Property("givenName").Value.ToString());

            return View();
        }

        [Authorize]
        public IActionResult Logout()
        {
            string redirectPage = Url.Action(nameof(LoggedOut), "User", null, Url.ActionContext.HttpContext.Request.Scheme);
            return new RedirectResult("https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=" + HttpUtility.UrlEncode(redirectPage));
        }

        public IActionResult LoggedOut() => View();
    }
}