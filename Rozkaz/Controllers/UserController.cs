using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rozkaz.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Index() => View();
    }
}