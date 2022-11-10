using BusinessObject;
using DataAccess.Repository;
using eStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace eStore.Controllers
{
    public class HomeController : Controller
    {
        IMemberRepository memRepo = new MemberRepository();
        const string SessionName = "_Name";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            MemberObject member;
            try
            {
                if (ModelState.IsValid)
                {
                    member = memRepo.Login(email, password);
                    if (member != null)
                    {
                        HttpContext.Session.SetString(SessionName, member.Email);
                        HttpContext.Session.SetInt32("id", member.MemberId);
                        return View("../Home/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or password  incorrect");
                        ViewBag.Warning = "Username or password  incorrect";
                        return View("../Login/Index");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View("../Home/Index");

        }

        [Route("Home/Logout")]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("../Login/Index");
        }
    }
}
