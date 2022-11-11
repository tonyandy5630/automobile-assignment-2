using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Runtime.CompilerServices;

namespace eStore.Controllers
{
    public class SearchController : Controller
    {
        // GET: HomeController1
        IProductRepository productRepository = null;
        public SearchController() => productRepository = new ProductRepository();
        public ActionResult Index(string txtsearch , string price)
        {
            ViewBag.txtsearch = txtsearch;
            var productList = productRepository.GetProducts();
            int min = 0;
            int max = 0;
            if (price != null)
            {
                if (price.Equals("1"))
                {
                    min = 0;
                    max = 50;
                }
                if (price.Equals("2"))
                {
                    min = 50;
                    max = 200;
                }
                if (price.Equals("3"))
                {
                    min = 200;
                    max = 500;
                }
                productList = productList.Where(pro => pro.UnitPrice >= min && pro.UnitPrice <= max);
            }
            if (txtsearch != null)
            {
                productList = productList.Where(pro => pro.ProductName.Contains(txtsearch));
            }
            return View(productList);
        }

        // GET: HomeController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: HomeController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController1/Edit/5
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

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController1/Delete/5
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
        public ActionResult search(string searchKey,string priceRange)
        {
            return View();
        }
    }
}
