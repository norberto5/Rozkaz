using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Client;
using Rozkaz.Models;
using Rozkaz.Services;
using System;
using System.Linq;
using System.Net;
using WebApp_OpenIDConnect_DotNet.Infrastructure;

namespace Rozkaz.Controllers
{
  [Authorize, MsalUiRequiredExceptionFilter(Scopes = new[] { Constants.ScopeUserRead })]
  public class OrderController : Controller
  {
    private readonly UserResolver userResolver;

    private readonly RozkazDatabaseContext db;
    private readonly OrderPdfService orderPdfService;
    private User currentUser;

    public OrderController(UserResolver userResolver, RozkazDatabaseContext rozkazDatabaseContext, OrderPdfService orderPdfService)
    {
      this.userResolver = userResolver;
      db = rozkazDatabaseContext;
      this.orderPdfService = orderPdfService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
      currentUser = userResolver.GetUser().Result;

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

      if (!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
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

      if (!IsOrderFoundAndCurrentUserHavePermission(orderEntry))
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