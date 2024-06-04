using IMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly ImsContext _db;

        public LoginController(ImsContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "RoleId","RoleName");
            return View();
        }
        [HttpPost]
        public IActionResult Index(User obj)
        {
            var roles = _db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
            try
            {
                var checkEmail = _db.Users.FirstOrDefault(x => x.Email == obj.Email);
                if (checkEmail == null)
                {
                    TempData["error"] = "Incorrect Email / If not Registerd then Sign Up";
                    return View();
                }
                var checkPass = _db.Users.FirstOrDefault(x => x.Email == obj.Email && x.Password != obj.Password && x.RoleId == obj.RoleId);
                if (checkPass != null)
                {
                    TempData["error"] = "Incorrect Password";
                    return View();
                }
                var user = _db.Users.FirstOrDefault(x=>x.Email == obj.Email && x.Password == obj.Password && x.RoleId == obj.RoleId);
                if (user == null)
                {
                    TempData["error"] = "NO Data Found! Please Sign Up First";  
                    return View();
                }
                HttpContext.Session.SetInt32("UserId",user.UserId);
                HttpContext.Session.SetString("user", user.UserName);
                HttpContext.Session.SetInt32("role", user.RoleId);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["error"] = "Error! Please try again";
                return View();
            }
        }

        public IActionResult Logout()
        {
            
            return View();
        }
        public IActionResult LogoutConfirm()
        {
            HttpContext.Session.Remove("user");
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(User user)
        {
            var old_data = _db.Users.FirstOrDefault(x => x.Email == user.Email);
            if(old_data == null)
            {
                TempData["ForgotPasswordError"] = "Enter Valid Email";
                return View();
            }
            try
            {
                old_data.Password = user.Password;
                _db.Users.Update(old_data);
                _db.SaveChanges();
                return RedirectToAction("Index", "Login");
            }
            catch
            {
                TempData["ForgotPasswordError"] = "Somethinng Went Wrong! Please Try Again";
                return RedirectToAction("Index","Login");
            }
        }
    }
}