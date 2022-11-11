using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace eStore.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        IMemberRepository memRepo = new MemberRepository();
        const string SessionName = "_Name";
        public ActionResult Index()
        {
            return View();
        }


        // GET: LoginController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginController/Login
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
                    if(member != null)
                    {
                        HttpContext.Session.SetString(SessionName, member.MemberId.ToString());
                    }

                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Index");
            
        }

        // GET: LoginController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
