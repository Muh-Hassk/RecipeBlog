using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class ChefController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChefController(ModelContext? context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var testimonials = _context.Testimonials.Where(u => u.Isapproved == "Yes")
                                           .Include(t => t.User)
                                           .ToList();

            ViewBag.Testomnials = testimonials;


            var HomePage = _context.Homepages.FirstOrDefault();
            var AboutUs = _context.Aboutuspages.FirstOrDefault();

            ViewBag.HomePage = HomePage;
            ViewBag.AboutUs = AboutUs;








            var recipeCategories = _context.Recipecategories.Include(rc => rc.Recipes.Where(x => x.Isaccepted == "Yes")).ToList();

            // Pass the data to the view
            ViewBag.RecipeCategories = recipeCategories;


            // Retrieve the first three chefs from the database
            var chefs = _context.Users.Where(x => x.Roleid == 2 && x.Imagepath != null).ToList();


            // Pass the data to the view
            ViewBag.Chefs = chefs;

            return View(user);
        }

        public IActionResult AddRecipe()
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.SingleOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var newRecipe = new Recipe();

            ViewBag.Category = _context.Recipecategories.ToList();

            // Optionally, you can initialize properties of the new recipe here if needed

            return View(newRecipe);
        }
        [HttpPost]
        public async Task<IActionResult> AddRecipe(Recipe newRecipe, IFormFile imageFile)
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.SingleOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Process the image file if it's provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    string wwwrootpath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string fullPath = Path.Combine(wwwrootpath, "images", imageName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    newRecipe.Imagepath = imageName;
                }

                newRecipe.Isaccepted = "No";
                newRecipe.Chefid = user.Userid;

                // Save the new recipe to the database
                _context.Recipes.Add(newRecipe);
                await _context.SaveChangesAsync();

                return RedirectToAction("YourRecipes", "Chef"); // Redirect to your recipes page or any other page
            }

            // If the model state is not valid, re-populate ViewBag.Category and return the view with validation errors
            ViewBag.Category = _context.Recipecategories.ToList();
            return View(newRecipe);
        }




        public IActionResult ChefRecipe(int chefId)
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();
            var chefs = _context.Users.Where(x => x.Userid == chefId).ToList();
            var categories = _context.Recipecategories.ToList();
            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };


            ViewBag.Recipes = recipeInfoList;
            ViewBag.Chef  = _context.Users.Where(x => x.Userid == chefId).SingleOrDefault();
             // Pass chefId to the view


            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ChefRecipe(int chefId, string search)
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Recipe> recipesQuery = _context.Recipes.Where(x => x.Isaccepted == "Yes" && x.Chefid == chefId);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                recipesQuery = recipesQuery.Where(x => x.Recipename.ToLower().Contains(search));
            }

            var recipes = await recipesQuery.ToListAsync();
            var chefs = await _context.Users.Where(x => x.Userid == chefId).ToListAsync();
            var categories = await _context.Recipecategories.ToListAsync();

            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };

            ViewBag.Recipes = recipeInfoList.ToList();
            ViewBag.ChefId = chefId; // Pass chefId to the view
            ViewBag.Chef = _context.Users.Where(x => x.Userid == chefId).SingleOrDefault();


            return View(user);
        }





        public IActionResult ViewAll()
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();
            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            var categories = _context.Recipecategories.ToList();
            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };


            ViewBag.Recipes = recipeInfoList;

            return View();

        }
        [HttpGet]
        public IActionResult ViewAll(string search)
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.SingleOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Recipe> recipesQuery = _context.Recipes.Where(x => x.Isaccepted == "Yes");
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                recipesQuery = recipesQuery.Where(x => x.Recipename.ToLower().Contains(search));
            }

            var recipes = recipesQuery.ToList();
            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            var categories = _context.Recipecategories.ToList();

            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };

            ViewBag.Recipes = recipeInfoList.ToList();

            return View(user);
        }

        public IActionResult YourRecipes()
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var AcceptedRecipes = _context.Recipes.ToList();
            var chefs = _context.Users.Where(x => x.Userid == id).ToList();
            var categories = _context.Recipecategories.ToList();
            var recipeInfoList = from r in AcceptedRecipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };


            ViewBag.Recipes = recipeInfoList;

         


            return View();

        }
        [HttpGet]
        public IActionResult YourRecipes(string search)
        {
            var id = HttpContext.Session.GetInt32("ChefId");
            var user = _context.Users.SingleOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Recipe> recipesQuery = _context.Recipes;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                recipesQuery = recipesQuery.Where(x => x.Recipename.ToLower().Contains(search));
            }

            var recipes = recipesQuery.ToList();
            var chefs = _context.Users.Where(x => x.Userid == id).ToList();
            var categories = _context.Recipecategories.ToList();

            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };

            ViewBag.Recipes = recipeInfoList.ToList();

            return View(user);
        }

        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("ChefId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }




        public class RecipeInfo
        {
            public Recipe Recipe { get; set; }
            public User Chef { get; set; }
            public Recipecategory Category { get; set; }
        }

    }
}

