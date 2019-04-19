using System.Diagnostics;
using Rozkaz.Services;
using Microsoft.AspNetCore.Mvc;
using Rozkaz.Models;

namespace Rozkaz.Controllers
{
    public class HomeController : Controller
    {
        private static OrderPdfService orderPdfService => _orderPdfService ?? (_orderPdfService = new OrderPdfService());
        private static OrderPdfService _orderPdfService;

        public IActionResult Index() => View();

        public FileContentResult SampleOrder()
        {
            string orderName = orderPdfService.CreateSampleOrder();

            byte[] bytes = System.IO.File.ReadAllBytes(orderName);
            System.IO.File.Delete(orderName);
            return File(bytes, "application/pdf");
        }

        public FileContentResult GenerateDemoOrder(OrderModel model)
        {
            string orderName = orderPdfService.CreateOrder(model);

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
