using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: HomeController1
        IProductRepository productRepository = null;
        IOrderDetailRepository orderDetailRepository = null;
        IOrderRepository orderRepository = null;
        public const string CARTKEY = "cart";
        const string MEMBER_KEY = "Member";
        const int SHIP_TIME = 7;
        const int FREIGHT = 10;
        public ProductController()
        {
            productRepository = new ProductRepository();
            orderDetailRepository = new OrderDetailRepository();
            orderRepository = new OrderRepository();
        }
        public ActionResult Index(string txtsearch, string price)
        {
            var productList = productRepository.GetProducts();
            if(txtsearch == null && price == null)
            {
                return View(productList);
            }

            ViewBag.txtsearch = txtsearch;
            List<ProductObject> resultList = new List<ProductObject>();
            if (price != null)
            {
                int min = 0;
                int max = 0;
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
                var tmp = productList.Where(pro => pro.UnitPrice >= min && pro.UnitPrice <= max).ToList();
                resultList.AddRange(tmp);
            }
            if (txtsearch != null)
            {
                var tmp = productList.Where(pro => pro.ProductName.Contains(txtsearch)).ToList();
                resultList.AddRange(tmp);
            }
            resultList = resultList.OrderBy(o => o.ProductId).ToList();
            return View(resultList);
        }


        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {
            try
            {
                var product = productRepository.GetProductByID(productid);
                if (product == null)
                    return NotFound("Không có sản phẩm");

                // Xử lý đưa vào Cart ...
                var cart = GetCartItems();
                var cartitem = cart.Find(p => p.product.ProductId == productid);
                if (cartitem != null)
                {
                    // Đã tồn tại, tăng thêm 1
                    cartitem.quantity++;
                }
                else
                {
                    //  Thêm mới
                    cart.Add(new CartItem() { quantity = 1, product = product });
                }

                // Lưu cart vào Session
                SaveCartSession(cart);
                // Chuyển đến trang hiện thị Cart
                return RedirectToAction(nameof(Cart));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }

        // GET: HomeController1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = productRepository.GetProductByID(id.Value);
            if (product == null)
            {

                return NotFound();
            }
            return View(product);
        }


        List<CartItem> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }
        MemberObject getMember()
        {
            MemberObject member = null;
            var session = HttpContext.Session;
            string jsonMember = session.GetString(MEMBER_KEY);
            if (jsonMember != null)
            {
                return JsonConvert.DeserializeObject<MemberObject>(jsonMember);
            }
            return member;
        }


        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }
        public int getMaxIdOrder()
        {
            int result = 0;
            var orderList = orderRepository.GetOrders();
            foreach (var order in orderList)
            {
                if(order.OrderId > result)
                {
                    result = order.OrderId;
                }
            }

            return result;
        }

        public IActionResult Checkout()
        {
            try
            {
                MemberObject member = getMember();
                if(member == null)
                {
                    return RedirectToAction("Index","Login");
                }
                //add new order
                int orderID = getMaxIdOrder() + 1;
                int memberID = member.MemberId;
                DateTime orderDate = DateTime.Now;
                DateTime requiredDate = orderDate;
                DateTime shipDate = orderDate.AddDays(SHIP_TIME);
                int freight = FREIGHT;
                OrderObject orderObject = new OrderObject
                {
                    OrderId = orderID,
                    Freight = freight,
                    ShippedDate = shipDate,
                    MemberId = memberID,
                    OrderDate = orderDate,
                    RequiredDate = requiredDate,
                };
                orderRepository.InsertOrder(orderObject);

                //add new orderdetails
                var cartItems = GetCartItems();
                foreach (var item in cartItems)
                {
                    int productID = item.product.ProductId;
                    int quantity = item.quantity;
                    decimal unitPrice = item.product.UnitPrice;
                    double discount = 0;
                    OrderDetailObject orderDetail = new OrderDetailObject
                    {
                        OrderId = orderID,
                        ProductId = productID,
                        UnitPrice = unitPrice,
                        Discount = discount,
                        Quantity = quantity,

                    };
                    orderDetailRepository.InsertOrderDetail(orderDetail);
                }
                ClearCart();
                return RedirectToAction("Index","Order");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            
        }

        // GET: HomeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductObject product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    productRepository.InsertProduct(product);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(product);
            }
        }

        // GET: HomeController1/Edit/5
        [Route("Product/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = productRepository.GetProductByID(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: HomeController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAction(int id, ProductObject product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    productRepository.UpdateProduct(product);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(product);
            }
        }

        // GET: HomeController1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductObject product = productRepository.GetProductByID(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: HomeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                productRepository.DeleteProduct(id);
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
