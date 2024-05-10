using Microsoft.AspNetCore.Mvc;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ModelContext _context;

        public RecipeController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
