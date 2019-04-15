using System.Diagnostics;
using Rozkaz.Services;
using Microsoft.AspNetCore.Mvc;
using Rozkaz.Models;

namespace Rozkaz.Controllers
{
	public class HomeController : Controller
	{
        public IActionResult Index() => View();

        public FileContentResult CreatePdf()
        {
            var orderPdfService = new OrderPdfService();
            string orderName = orderPdfService.CreateSampleOrder();

            byte[] bytes = System.IO.File.ReadAllBytes(orderName);
            System.IO.File.Delete(orderName);
            return File(bytes, "application/pdf");
        }

        public IActionResult About() => View();

        public IActionResult Contact() => View();

		public IActionResult Privacy() => View();

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
