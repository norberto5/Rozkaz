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
using System.Net;
using Microsoft.AspNetCore.Http;

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
            OrderEntry order = db.Orders.SingleOrDefault(o => o.Uid == id);

            if(!IsOrderFoundAndUserHavePermission(order, currentUser))
            {
                return RedirectToAction(nameof(Index));
            }

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

        // POST: Order/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderModel model)
        {
            try
            {
                User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

                db.Orders.Add(
                    new OrderEntry()
                    {
                        Order = model,
                        Owner = currentUser
                    });
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);

            OrderEntry orderEntry = db.Orders.SingleOrDefault(o => o.Uid == id);

            if(!IsOrderFoundAndUserHavePermission(orderEntry, currentUser))
            {
                return View();
            }

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
                OrderEntry orderEntry = db.Orders.Where(o => o.Uid == id).SingleOrDefault();

                if (!IsOrderFoundAndUserHavePermission(orderEntry, currentUser))
                {
                    return View();
                }

                OrderModel order = orderEntry.Order;
                order.Info = model.Info;
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

        // GET: Order/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);
            OrderEntry orderEntry = db.Orders.Where(o => o.Uid == id).SingleOrDefault();

            if (!IsOrderFoundAndUserHavePermission(orderEntry, currentUser))
            {
                return View();
            }

            return View(orderEntry);
        }

        // POST: Order/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, IFormCollection formCollection)
        {
            try
            {
                User currentUser = await UserController.GetUser(HttpContext, tokenAcquisition, graphApiOperations, db);
                OrderEntry orderEntry = db.Orders.Where(o => o.Uid == id).SingleOrDefault();

                if (!IsOrderFoundAndUserHavePermission(orderEntry, currentUser))
                {
                    return View();
                }

                db.Orders.Remove(orderEntry);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool IsOrderFoundAndUserHavePermission(OrderEntry orderEntry, User currentUser)
        {
            if (orderEntry == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            if (orderEntry.Owner != currentUser)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }
            return true;
        }
    }
}