using AutoMapper;
using IMS.Models;
using IMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Globalization;
using System.Linq;

namespace IMS.Controllers
{
    public class SalesController : Controller
    {
        private readonly ImsContext _db;
        private readonly IMapper _mapper;

        public SalesController(ImsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            List<Sale> list = _db.Sales.Include(s => s.Product).OrderByDescending(s=>s.Status=="Pending").ToList();
            return View(list);
        }
        [HttpPost]
        public IActionResult Index(string searchInput)
        {

            List<Sale> productList = _db.Sales.Include(s => s.Product).Where(sp => sp.Product.Name.Contains(searchInput) || sp.CustomerName.Contains(searchInput) ).ToList();
            return View(productList);
        }
        [HttpPost]
        public IActionResult ChangeStatus(int id , string status)
        {
            try
            {
                var old_data = _db.Sales.Find(id);
                if(old_data == null)
                {
                    return NotFound();
                }
                if (old_data.Status=="Pending" || status=="Completed")
                {
                    old_data.Status = "Completed";
                    _db.Sales.Update(old_data);
                    _db.SaveChanges();
                }
                var quantity = old_data.ProductQuantinty;

                var product = _db.Products.Find(old_data.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                product.Quantity = product.Quantity - quantity;
                _db.Products.Update(product);
                _db.SaveChanges();

                DateTime dateTime = DateTime.Now;
                DateOnly dateOnly = DateOnly.FromDateTime(dateTime);

                var transactionDTO = new InventoryTransactionDTO();
                transactionDTO.TransactionDate = dateOnly;
                transactionDTO.ProductId = old_data.ProductId;
                transactionDTO.Quantity = - old_data.ProductQuantinty;
                var transaction = _mapper.Map<InventoryTransaction>(transactionDTO);
                _db.InventoryTransactions.Add(transaction);
                _db.SaveChanges();

                return RedirectToAction("Index","Sales");

            }
            catch
            {
                TempData["StatusChangeError"] = "Something Went Wrong! Please Try Again";
                return RedirectToAction("Index","Sales");
            }
        }

    }
}
