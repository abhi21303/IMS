using AutoMapper;
using IMS.Models;
using IMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ImsContext _db;
        private readonly IMapper _mapper;
        public ProductsController(ImsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<Product> obj = _db.Products.ToList();  
            return View(obj);
        }
        [HttpPost]
        public IActionResult Index(string searchInput)
        {
            List<Product> products = _db.Products.Where(p=>p.Name.Contains(searchInput)).ToList();
            return View(products);
        }

        public IActionResult Add()
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public IActionResult Add(ProductDTO obj)
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            if(ModelState.IsValid)
            {
                var data = _mapper.Map<Product>(obj);
                _db.Products.Add(data);
                _db.SaveChanges();
                return RedirectToAction("Index","Products");
            }
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            if (id==0 || id==null) 
            {
                return NotFound();  
            }
            var data = _db.Products.Find(id);
            
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Edit(int id,ProductDTO obj) 
        {
            var old_product_data = _db.Products.Find(id);

            if (ModelState.IsValid)
            {
                _mapper.Map(obj, old_product_data);
                _db.SaveChanges();
                return RedirectToAction("Index","Products"); 
            }
            return View(obj);
        }

        public IActionResult Delete(int? id) 
        {
            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var data = _db.Products.Find(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {

            if (id == 0 )
            {
                return NotFound();
            }
            var data = _db.Products.Find(id);
            if(ModelState.IsValid)
            { 
                _db.Products.Remove(data);
                _db.SaveChanges();
                return RedirectToAction("Index", "Products");
            }
            return View(data);
        }
        public IActionResult Show(int? id)
        {

            var category = _db.Categories.ToList();
            ViewBag.Category = new SelectList(category, "CategoryId", "CategoryName");
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var data = _db.Products.Find(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
    }
}
