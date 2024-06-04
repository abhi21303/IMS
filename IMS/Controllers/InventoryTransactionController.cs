using IMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Controllers
{
    public class InventoryTransactionController : Controller
    {
        private readonly ImsContext _db;

        public InventoryTransactionController(ImsContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<InventoryTransaction> it = _db.InventoryTransactions.Include(i=>i.Product).OrderByDescending(p=>p.TransactionDate).ToList();
            return View(it);
        }


        [HttpPost]
        public IActionResult Index(string searchInput)
        
        {
            List<InventoryTransaction> it = _db.InventoryTransactions.Include(i=>i.Product).Where(p=>p.Product.Name.Contains(searchInput)).ToList(); 
            return View(it);
        }
    }
}
