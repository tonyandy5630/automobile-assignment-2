using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace eStore.Controllers
{
    public class OrderController : Controller
    {
        // GET: HomeController1

        IOrderRepository orderRepository = null;
        IOrderDetailRepository orderDetailRepository = null;    
        IProductRepository productRepository = null;
        public OrderController() {
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
            productRepository = new ProductRepository();
        } 
        public ActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("id");
            var orderList = orderRepository.GetOrders();
            if (id != null && id != 1)
            {
                orderList = orderList.Where(m => m.MemberId == id.Value);
            }
            return View(orderList);
        }

        // GET: HomeController1/Details/5
        public ActionResult Details(int id)
        {
            var orders = orderRepository.GetOrders();
            OrderObject order = orders.SingleOrDefault(o => o.OrderId == id);
            ViewBag.order = order;
            var orderDetails = orderDetailRepository.GetOrdersByOrderID(order.OrderId);
            var product = productRepository.GetProducts();
            var result = orderDetails.Join(product, a => a.ProductId, b => b.ProductId, (a, b) => b).Distinct();
            return View(result);
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
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var orders = orderRepository.GetOrders();
            OrderObject order = orders.FirstOrDefault(o => o.OrderId == id.Value);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                orderDetailRepository.DeleteListOrderDetail(id);
                orderRepository.DeleteOrder(id);
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
