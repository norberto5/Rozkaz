using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using Rozkaz.Models;
using Rozkaz.Services;
using System.Threading.Tasks;
using WebApp_OpenIDConnect_DotNet.Infrastructure;

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
      HttpContext.Session.Clear();
      foreach (string cookie in Request.Cookies.Keys)
      {
        Response.Cookies.Delete(cookie);
      }
      return new RedirectToActionResult(nameof(LoggedOut), "User", string.Empty);
    }

    public IActionResult LoggedOut() => View();
  }
}