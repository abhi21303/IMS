using AutoMapper;
using IMS.Models;
using IMS.Models.DTOs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf;
using System.Reflection.Metadata;
using Document = iTextSharp.text.Document;
using PdfDocument = PdfSharp.Pdf.PdfDocument;


namespace IMS.Controllers
{
    public class OrderController : Controller
    {
        public readonly ImsContext _db;
        public readonly IMapper _mapper;
        public OrderController(ImsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<Order> orders = _db.Orders.Include(o => o.OrderDetails)
                                           .ThenInclude(od => od.Product)
                                           .OrderByDescending(o => o.Status == "Pending")
                                           .ToList();

            return View(orders);
        }

        [HttpPost]

        public IActionResult Index(string searchInput)
        {
            List<Order> orders = _db.Orders.Include(o => o.OrderDetails)
                                           .ThenInclude(od => od.Product)
                                           .Where(o => o.OrderDetails.Any(od => od.Product.Name.Contains(searchInput)))
                                           .OrderByDescending(o => o.Status == "Pending")
                                           .ToList();

            return View(orders);
        }

        public IActionResult OrderDetail(int id)
        {
            var data = _db.OrderDetails.Include(o => o.Product).FirstOrDefault(o => o.OrderDetailId == id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult ChangeStatus(Order obj)
        {
            try
            {

                var Old_data = _db.Orders.Find(obj.OrderId);
                if (Old_data == null)
                {
                    return NotFound();
                }
                if (Old_data.Status == "Pending" && obj.Status == "Completed")
                {

                    Old_data.Status = "Completed";
                    _db.Orders.Update(Old_data);
                    _db.SaveChanges();
                }
                var orderDetail = _db.OrderDetails.Find(obj.OrderId);
                if (orderDetail == null)
                {
                    return NotFound();
                }
                var quantity = orderDetail.Quantity;

                var product = _db.Products.Find(orderDetail.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                product.Quantity = product.Quantity + quantity;
                _db.Products.Update(product);
                _db.SaveChanges();

                DateTime dateTime = DateTime.Now;
                DateOnly dateOnly = DateOnly.FromDateTime(dateTime);

                var transactionDTO = new InventoryTransactionDTO();
                transactionDTO.TransactionDate = dateOnly;
                transactionDTO.ProductId = orderDetail.ProductId;
                transactionDTO.Quantity = quantity;
                var transaction = _mapper.Map<InventoryTransaction>(transactionDTO);
                _db.InventoryTransactions.Add(transaction);
                _db.SaveChanges();

                return RedirectToAction("Index", "Order");
            }
            catch
            {
                TempData["StatusChangeError"] = "Something Went Wrong! Please Try Again";
                return RedirectToAction("Index", "Order");
            }
        }

        public IActionResult GeneratePDF(int id)
        {
            var orderDetail = _db.OrderDetails.Include(p => p.Product).Include(o => o.Order).FirstOrDefault(o => o.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            string fileName = $"bill_{orderDetail.OrderId}.pdf"; // Generate a unique file name
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filePath = Path.Combine(downloadsPath, "Downloads", fileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (Document document = new Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Add order details to the PDF document
                document.Add(new Paragraph("Order Details"));
                document.Add(new Paragraph($"Order ID: {orderDetail.OrderId}"));
                document.Add(new Paragraph($"Product ID: {orderDetail.ProductId}"));
                document.Add(new Paragraph($"Product Name: {orderDetail.Product.Name}"));
                document.Add(new Paragraph($"Product Description: {orderDetail.Product.Description}"));
                document.Add(new Paragraph($"Quantity: {orderDetail.Quantity}"));
                document.Add(new Paragraph($"Price per Unit: {orderDetail.UnitPrice}"));
                document.Add(new Paragraph($"Total Order Amount: {orderDetail.Quantity * orderDetail.UnitPrice}"));

                // Close the document
                document.Close();
            }

            // Return the file for download
            return PhysicalFile(filePath, "application/pdf", fileName);
        }
    }
}

