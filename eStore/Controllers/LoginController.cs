using BusinessObject;
using DataAccess.Repository;
using eStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace eStore.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        IMemberRepository memRepo = new MemberRepository();
        const string SESSION_NAME = "_Name";
        const string MEMBER_KEY = "Member";
        public const string CARTKEY = "cart";

        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
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
                        //ISession session = new Session()
                        HttpContext.Session.SetString(SESSION_NAME, member.Email);
                        HttpContext.Session.SetInt32("id", member.MemberId);
                        SaveMemberSession(member);

                        /*return View("../Product/Index");*/
                        return RedirectToAction("Index", "Product");
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
            return View("../Product/Index");

        }
        MemberObject GetMemberFromSession()
        {

            var session = HttpContext.Session;
            string member = session.GetString(MEMBER_KEY);
            if (member != null)
            {
                return JsonConvert.DeserializeObject<MemberObject>(member);
            }
            return null;
        }

        void SaveMemberSession(MemberObject mem)
        {
            var session = HttpContext.Session;
            string member = JsonConvert.SerializeObject(mem);
            session.SetString(MEMBER_KEY, member);
        }

        void ClearMember()
        {
            var session = HttpContext.Session;
            session.Remove(MEMBER_KEY);
            session.Remove(CARTKEY);
        }

        [Route("Login/Logout")]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("../Login/Index");
        }
    }
}
