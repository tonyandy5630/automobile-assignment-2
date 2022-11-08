using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace eStore.Controllers
{
    public class MemberController : Controller
    {
        // GET: HomeController1
        IMemberRepository memberRepository = null;
        public MemberController() => memberRepository = new MemberRepository();
        public ActionResult Index()
        {
            var memberList = memberRepository.GetMembers();
            return View(memberList);
        }

        // GET: HomeController1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MemberObject member = memberRepository.getMemberByID(id.Value);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // GET: HomeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MemberObject member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    memberRepository.InsertMember(member);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(member);
            }
        }

        [Route("Member/Edit")]
        // GET: HomeController1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = memberRepository.getMemberByID(id.Value);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: HomeController1/Edit/5
        public ActionResult EditAction(int id, MemberObject member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    memberRepository.UpdateMember(member);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(member);
            }
        }

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = memberRepository.getMemberByID(id.Value);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                memberRepository.DeleteMember(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
    }
}
