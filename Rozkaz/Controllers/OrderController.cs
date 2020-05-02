using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Client;
using Rozkaz.Models;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using System;
using Rozkaz.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rozkaz.Controllers
{
    [Authorize, MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
    public class OrderController : Controller
    {
        private readonly UserResolver userResolveService;

        private readonly RozkazDatabaseContext db;
        private readonly OrderPdfService orderPdfService;
        private User currentUser;

        public OrderController(UserResolver userResolveService, RozkazDatabaseContext rozkazDatabaseContext, OrderPdfService orderPdfService)
        {
            this.userResolveService = userResolveService;
            db = rozkazDatabaseContext;
            this.orderPdfService = orderPdfService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
          currentUser = userResolveService.GetUser().Result;

          base.OnActionExecuting(context);
        }

        // GET: Order
        public IActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Owner).Where(o => o.Owner == currentUser).OrderBy(o => o.CreatedTime).ToList();
            return View(orders);
        }

        // GET: Order/Show/5
        public IActionResult Show(Guid id)
        {
            OrderEntry orderEntry = db.Orders.Include(o => o.Owner).SingleOrDefault(o => o.Uid == id);

            if(!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
            {
                return RedirectToAction(nameof(Index));
            }

            string orderName = orderPdfService.CreateOrder(orderEntry.Order);

            byte[] bytes = System.IO.File.ReadAllBytes(orderName);
            System.IO.File.Delete(orderName);
            return File(bytes, "application/pdf");
        }

        // GET: Order/Create
        public IActionResult Create() => View();

        // POST: Order/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(OrderModel model)
        {
            try
            {
                db.Orders.Add(
                    new OrderEntry()
                    {
                        Order = model,
                        Owner = currentUser,
                        CreatedTime = DateTime.Now,
                        LastModifiedTime = DateTime.Now
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
        public IActionResult Edit(Guid id)
        {
            OrderEntry orderEntry = db.Orders.Include(o => o.Owner).SingleOrDefault(o => o.Uid == id);

            if(!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
            {
                return View();
            }

            return View(orderEntry);
        }

        // POST: Order/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, OrderModel model)
        {
            try
            {
                OrderEntry orderEntry = db.Orders.Include(o => o.Owner).Where(o => o.Uid == id).SingleOrDefault();

                if (!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
                {
                    return View();
                }

                orderEntry.LastModifiedTime = DateTime.Now;

                OrderModel order = orderEntry.Order;
                order.Info = model?.Info;
                order.OccassionalIntro = model?.OccassionalIntro;
                order.ExceptionsFromAnotherOrder = model?.ExceptionsFromAnotherOrder;

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
        public IActionResult Delete(Guid id)
        {
            OrderEntry orderEntry = db.Orders.Include(o => o.Owner).Where(o => o.Uid == id).SingleOrDefault();

            if (!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
            {
                return View();
            }

            return View(orderEntry);
        }

        // POST: Order/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id, IFormCollection formCollection)
        {
            try
            {
                OrderEntry orderEntry = db.Orders.Include(o => o.Owner).Where(o => o.Uid == id).SingleOrDefault();

                if (!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
                {
                    return View();
                }

                orderEntry.LastModifiedTime = DateTime.Now;
                orderEntry.Deleted = true;

                db.Entry(orderEntry).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool IsOrderFoundAndCurrentUserHavePermission(OrderEntry orderEntry)
        {
            if (orderEntry == null || orderEntry.Deleted)
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