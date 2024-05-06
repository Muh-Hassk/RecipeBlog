using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        public AdminController(ModelContext? context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            return View(user);
        }
        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("AdminId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }


    }
}
