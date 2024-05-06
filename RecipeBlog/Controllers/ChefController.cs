using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class ChefController : Controller
    {
        private readonly ModelContext _context;

        public ChefController(ModelContext? context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            return View(user);
        }
        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("ChefId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }


    }
}

