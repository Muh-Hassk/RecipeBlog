using Microsoft.AspNetCore.Mvc;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly ModelContext _context;
        public UserController(ModelContext? context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            return View(user);
        }
        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("UserId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }


    }
}