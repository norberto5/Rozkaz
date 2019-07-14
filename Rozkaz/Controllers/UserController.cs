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

        public static async Task<User> GetUser(HttpContext httpContext, ITokenAcquisition tokenAcquisition, IGraphApiOperations graphApiOperations, RozkazDatabaseContext db)
        {
            string accessToken = await tokenAcquisition.GetAccessTokenOnBehalfOfUser(httpContext, new[] { Constants.ScopeUserRead });

            var userInformation = await graphApiOperations.GetUserInformation(accessToken) as JObject;

            User user = userInformation.ToObject<User>();

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

            httpContext.Session.Set("User", user);

            return user;
        }


        public IActionResult Login() => new LocalRedirectResult("/AzureAD/Account/SignIn");

        [Authorize, MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

            //db.Orders.Add(
            //    new OrderEntry()
            //    {
            //        Owner = user,
            //        Order = new OrderModel()
            //        {
            //            Info = new OrderInfoModel()
            //            {
            //                Author = user.DisplayName,
            //                City = "Warszawa",
            //                Date = DateTime.Now,
            //                OrderNumber = 66,
            //                OrderType = OrderType.Normal,
            //                Unit = new UnitModel()
            //                {
            //                    NameFirstLine = "Harcerski Klub Ratowniczy",
            //                    NameSecondLine = "\"Szczecin\""
            //                }
            //            }
            //        }
            //    });
            //db.SaveChanges();

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