using IMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS.Controllers
{
    public class SignUpController : Controller
    {
        private readonly ImsContext _db;

        public SignUpController(ImsContext db)
        {
            _db = db;            
        }
        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
            return View();
        }
        [HttpPost]
        public IActionResult Index(User obj)
        {
            var roles = _db.Roles.ToList();
            try
            {
                if (obj.Password != obj.ConfirmPassword)
                {
                    TempData["error"] = "Password Not Matched With Confirm Password";
                    var roles1 = _db.Roles.ToList();
                    ViewBag.Roles = new SelectList(roles1, "RoleId", "RoleName", obj.RoleId);
                    return View();
                }
                else
                {
                    if(obj.RoleId == -1)
                    {
                        return View(obj);
                        TempData["error"] = "Please Select Role";

                    }
                    _db.Users.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index","Login");
                }
            }
            catch
            {
                var roles1 = _db.Roles.ToList();
                ViewBag.Roles = new SelectList(roles1, "RoleId", "RoleName", obj.RoleId);
                TempData["error"] = "Something Went Wrong Try Later";
                return View(obj);
            }
        }
    }
}
