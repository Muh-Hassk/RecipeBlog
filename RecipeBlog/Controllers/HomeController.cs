using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;
using System.Diagnostics;
using System.Linq;

namespace RecipeBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;

        public HomeController(ILogger<HomeController> logger, ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            
                // Retrieve RecipeCategories and Recipes from the database
                var recipeCategories = _context.Recipecategories.Include(rc => rc.Recipes).ToList();

                // Pass the data to the view
                ViewBag.RecipeCategories = recipeCategories;

         
                // Retrieve the first three chefs from the database
                var chefs = _context.Users.Where(x=> x.Roleid == 2 && x.Imagepath != null).ToList();

            var testimonials = _context.Testimonials.Where(u => u.Isapproved == "Yes")
                                           .Include(t => t.User)
                                           .ToList();

            ViewBag.Testomnials = testimonials;
            var AboutUs = _context.Aboutuspages.FirstOrDefault();
            ViewBag.AboutUs = AboutUs;


            // Pass the data to the view
            ViewBag.Chefs = chefs;


            return View();
            }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class Chef
    {

        public User chef { get; set; }
        public int RecipeCount { get; set; }
    }

}
