using AutoMapper;
using IMS.Models;
using IMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers
{
    public class CategoryController : Controller
    {
        public readonly ImsContext _db;
        public readonly IMapper _mapper;
        public CategoryController(ImsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<Category> categories = _db.Categories.ToList();
            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(CategoryDTO obj)
        {
            if (ModelState.IsValid)
            {
                var data = _mapper.Map<Category>(obj);
                _db.Categories.Add(data);
                _db.SaveChanges();
                return RedirectToAction("Index","Category");
            }
            return View(obj);
        }
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var data = _db.Categories.Find(id);
            if(data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Edit(int id,CategoryDTO obj)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Categories.Find(id);

                _mapper.Map(obj, data);
                _db.SaveChanges();
                return RedirectToAction("Index","Category");    
            }
            return View(obj);
        }
        public IActionResult Delete(int? id) 
        {
            if(id==0 || id == null)
            {
                return NotFound();
            }
            var data = _db.Categories.Find(id);
            if(data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var data = _db.Categories.Find(id);
            if (ModelState.IsValid)
            {
                _db.Categories.Remove(data);
                _db.SaveChanges();
                return RedirectToAction("Index","Category");    
            }
            return View();
        }
        
    }
}
