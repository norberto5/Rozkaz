using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using norberto5.Models;

namespace norberto5.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index() => View();

		public IActionResult About()
		{
			ViewData["Message"] = "Uczę się ASP.NET Core MVC, nie bijcie ;c";

			return View();
		}

		public IActionResult Contact() => View();

		public IActionResult Privacy() => View();

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
