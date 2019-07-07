using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using Rozkaz.Models;
using WebApp_OpenIDConnect_DotNet.Services.GraphOperations;
using WebApp_OpenIDConnect_DotNet.Infrastructure;

namespace Rozkaz.Controllers
{
    public class OrderController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IGraphApiOperations graphApiOperations;

        private readonly RozkazDatabaseContext db;

        public OrderController(ITokenAcquisition tokenAcquisition, IGraphApiOperations graphApiOperations, RozkazDatabaseContext rozkazDatabaseContext)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.graphApiOperations = graphApiOperations;
            db = rozkazDatabaseContext;
        }

        // GET: Order
        [Authorize]
        [MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<IActionResult> Index()
        {
            var currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

            var orders = db.Orders.Where(o => o.Owner == currentUser).Select(o => o.Order).ToList();
            return View(orders);
        }

        //// GET: Order/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Order/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Order/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Order/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Order/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Order/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Order/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}