﻿using System.Diagnostics;
using System.IO;
using Rozkaz.Services;
using Microsoft.AspNetCore.Mvc;
using Rozkaz.Models;

namespace Rozkaz.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index() => CreatePdf();

        public FileStreamResult CreatePdf()
        {
            var orderPdfService = new OrderPdfService();
            string orderName = orderPdfService.CreateSampleOrder();

            var stream = new FileStream(orderName, FileMode.Open);
            return File(stream, "application/pdf");
        }

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
