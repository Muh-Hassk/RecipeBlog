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
            ViewBag.RegUsers = _context.Users.Where(x => x.Roleid == 3).ToList();
            ViewBag.RegChefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            ViewBag.RegUsersCount = _context.Users.Count(x => x.Roleid == 3);
            ViewBag.RegChefsCount = _context.Users.Count(x => x.Roleid == 2);
            ViewBag.AcceptedRec = _context.Recipes.Count(x => x.Isaccepted == "Yes");
            ViewBag.RejectedRec = _context.Recipes.Count(x => x.Isaccepted == "No");

            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();
            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            var categories = _context.Recipecategories.ToList();

            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };

            ViewBag.RecipeInfo = recipeInfoList;

            var users = _context.Users.Where(x=> x.Roleid == 3).ToList();
            var Payments = _context.Payments.ToList();

            var recipePaymentsList = (from p in Payments
                                      join r in recipes on p.Recipeid equals r.Recipeid
                                      join u in users on p.Userid equals u.Userid
                                      join rc in categories on r.Categoryid equals rc.Categoryid
                                      where p.Paymentstatus == "Paid" // Filter for paid payments
                                      select new RecipePayments
                                      {
                                          Recipe = r,
                                          User = u,
                                          Category = rc,
                                          Payments = p
                                      });


            ViewBag.RequstedInfo = recipePaymentsList;


            return View(user);
        }

        public IActionResult ApproveRecipe()
        {
            var id = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }
            var recipes = _context.Recipes.Where(x => x.Isaccepted == "No").ToList();
            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            var categories = _context.Recipecategories.ToList();
            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };

            ViewBag.RecipeInfo = recipeInfoList;

            return View();
        }

        [HttpPost]
        public IActionResult AcceptRecipe(int recipeId)
        {
            // Find the recipe in the database
            var recipe = _context.Recipes.FirstOrDefault(r => r.Recipeid == recipeId);

            if (recipe != null)
            {
                // Update the IsAccepted property
                recipe.Isaccepted = "Yes"; // Assuming IsAccepted is a string property

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to the same page or another page
                return RedirectToAction("ApproveRecipe");
            }

            // Handle error if the recipe is not found
            return NotFound();
        }
        [HttpPost]
        public IActionResult RejectRecipe(int recipeId)
        {
            // Find the recipe in the database
            var recipe = _context.Recipes.FirstOrDefault(r => r.Recipeid == recipeId);

            if (recipe != null)
            {
                // Remove the recipe from the database
                _context.Recipes.Remove(recipe);

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to the same page or another page
                return RedirectToAction("ApproveRecipe");
            }

            // Handle error if the recipe is not found
            return NotFound();
        }

        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("AdminId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }


    }

    public class RecipeInfo
    {
        public Recipe Recipe { get; set; }
        public User Chef { get; set; }
        public Recipecategory Category { get; set; }
    }

    public class RecipePayments
    {
        public Recipe Recipe { get; set; }
        public User User { get; set; }
        public Recipecategory Category { get; set; }

        public Payment Payments { get; set; }
    }
}
