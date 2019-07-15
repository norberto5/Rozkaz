using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using Rozkaz.Models;
using WebApp_OpenIDConnect_DotNet.Services.GraphOperations;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using System;
using Rozkaz.Services;
using Microsoft.EntityFrameworkCore;

namespace Rozkaz.Controllers
{
    [Authorize, MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
    public class OrderController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IGraphApiOperations graphApiOperations;

        private readonly RozkazDatabaseContext db;
        private readonly OrderPdfService orderPdfService;

        public OrderController(ITokenAcquisition tokenAcquisition, IGraphApiOperations graphApiOperations, RozkazDatabaseContext rozkazDatabaseContext, OrderPdfService orderPdfService)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.graphApiOperations = graphApiOperations;
            db = rozkazDatabaseContext;
            this.orderPdfService = orderPdfService;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

            var orders = db.Orders.Where(o => o.Owner == currentUser).ToList();
            return View(orders);
        }

        // GET: Order/Show/5
        public async Task<ActionResult> Show(Guid id)
        {
            User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

            OrderEntry order = db.Orders.First(o => o.Uid == id && o.Owner == currentUser);

            string orderName = orderPdfService.CreateOrder(order.Order);

            byte[] bytes = System.IO.File.ReadAllBytes(orderName);
            System.IO.File.Delete(orderName);
            return File(bytes, "application/pdf");
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

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

        // GET: Order/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

            OrderEntry orderEntry = db.Orders.First(o => o.Uid == id && o.Owner == currentUser);

            return View(orderEntry);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, OrderModel model)
        {
            try
            {
                User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);
                OrderEntry orderEntry = db.Orders.Where(o => o.Uid == id && o.Owner == currentUser).Single();
                OrderModel order = orderEntry.Order;

                order.Info.OrderNumber = model.Info.OrderNumber;
                order.Info.Author = model.Info.Author;
                order.Info.City = model.Info.City;
                order.Info.Date = model.Info.Date;
                order.Info.Unit.NameFirstLine = model.Info.Unit.NameFirstLine;
                order.Info.Unit.NameSecondLine = model.Info.Unit.NameSecondLine;
                order.OccassionalIntro = model.OccassionalIntro;
                order.ExceptionsFromAnotherOrder = model.ExceptionsFromAnotherOrder;

                db.Entry(orderEntry).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

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