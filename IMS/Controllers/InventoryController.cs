using AutoMapper;
using IMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IMS.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ImsContext _db;
        private readonly IMapper _mapper;

        public InventoryController(ImsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            //List<Product> productList = _db.Products.ToList();
            List<Product> productList = _db.Products.Include(p => p.Category).ToList();
            return View(productList);
        }
        [HttpPost]
        public IActionResult Index(string searchInput)
        {
            List<Product> productList = _db.Products.Include(p=>p.Category).Where(p => p.Name.Contains(searchInput)).ToList();
            return View(productList);
        }

        public IActionResult Purchase(int? id)
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var data = _db.Products.Find(id);
            data.Quantity = 1;
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Purchase(Product obj)
        {
            var product_price = obj.Price;
            var product_quantity = obj.Quantity;
            var amount = product_price * product_quantity;

            TempData["ProductName"] = obj.Name;
            TempData["ProductPrice"] = obj.Price;
            TempData["Quantity"] = product_quantity;
            TempData["Amount"] = amount;

            return RedirectToAction("Purchase_confirm", "Inventory", new
            {
                id = obj.ProductId,
                productName = obj.Name,
                productPrice = obj.Price,
                quantity = product_quantity,
                amount = amount
            });
        }


        public IActionResult Purchase_confirm(int? id, string productName, decimal? productPrice, int? quantity, decimal? amount)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var data = _db.Products.Find(id);

            ViewData["Amount"] = amount;
            ViewData["ProductName"] = productName;
            ViewData["Quantity"] = quantity;
            ViewData["ProductPrice"] = productPrice;

            return View(data);
        }

        [HttpPost]
        public IActionResult Purchase_confirm(Product confirmation)
        {
            DateTime dateTime = DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
             
                try
                {
                int? userID = (int)HttpContext.Session.GetInt32("UserId"); // Implement this method to get the logged-in user's ID
                if (userID == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                var order = new Order
                    {
                        UserId = userID,
                        OrderDate = dateOnly,
                    };

                    _db.Orders.Add(order);
                    _db.SaveChanges();

                    // Create order details
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = confirmation.ProductId,
                        Quantity = confirmation.Quantity,
                        UnitPrice = confirmation.Price // Assuming you have a unit price available
                    };

                    _db.OrderDetails.Add(orderDetail);
                    _db.SaveChanges();

                    // Redirect to a thank you page or any other appropriate action
                    return RedirectToAction("ThankYou", "Inventory");
                }
                catch
                {
                    
                    return RedirectToAction("Index", "Login");
                }
           
        }

        public IActionResult Sell()
        {
            List<Product> products = _db.Products.Include(p=>p.Category).ToList();
            return View(products);

        }

        public IActionResult SellingPage(int? id)
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var data = _db.Products.Find(id);
            data.Quantity = 1;
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult SellingPage(Product obj)
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            var data = _db.Products.Find(obj.ProductId);
            if(data == null)
            {
                TempData["ProductNotFound"] = "Product Not Found in Inventory";
                return View(data);
            }
            if (obj.Quantity > data.Quantity)
            {
                TempData["OutOfStock"] = "Quantity Not In Stock";
                return View(data);
            }
            var product_price = obj.Price;
            var product_quantity = obj.Quantity;
            var amount = product_price * product_quantity;

            TempData["SelledProductName"] = obj.Name;
            TempData["SelledProductPrice"] = obj.Price;
            TempData["SelledQuantity"] = product_quantity;
            TempData["SelledAmount"] = amount;

            return RedirectToAction("Sell_Confirm", "Inventory", new
            {
                id = obj.ProductId,
                productName = obj.Name,
                productPrice = obj.Price,
                quantity = product_quantity,
                amount = amount
            });

        }
        public IActionResult Sell_Confirm(int id, string productName, decimal? productPrice, int? quantity, decimal? amount) 
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var data = _db.Products.Find(id);

            ViewData["SelledAmount"] = amount;
            ViewData["SelledProductName"] = productName;
            ViewData["SelledQuantity"] = quantity;
            ViewData["SelledProductPrice"] = productPrice;

            return View(data);
        }
        [HttpPost]
        public IActionResult Sell_Confirm(string CustomerName, int CustomerContact, string GstNo, Product product)
        {
            DateTime dateTime = DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);

            try
            {
                int? userID = (int)HttpContext.Session.GetInt32("UserId"); 
                if (userID == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var data = _db.Products.Find(product.ProductId);
                var sale = new Sale
                {
                    CustomerName = CustomerName,
                    CustomerContact = CustomerContact,
                    ProductId = product.ProductId,
                    GstNo = GstNo,
                    SellDate = dateOnly,
                    ProductQuantinty = product.Quantity,
                    TotalAmount = data.Price * product.Quantity 
                };

                _db.Sales.Add(sale);
                _db.SaveChanges();

                // Redirect to a thank you page or any other appropriate action
                return RedirectToAction("ThankYou", "Inventory");
            }
            catch
            {

                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult ThankYou()
        {
            return View();  
        }
        
        
    }
}
