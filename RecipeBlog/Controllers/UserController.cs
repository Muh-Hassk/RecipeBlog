using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Retrieve RecipeCategories and Recipes from the database
            var recipeCategories = _context.Recipecategories.Include(rc => rc.Recipes.Where(x => x.Isaccepted == "Yes")).ToList();

            // Pass the data to the view
            ViewBag.RecipeCategories = recipeCategories;


            // Retrieve the first three chefs from the database
            var chefs = _context.Users.Where(x => x.Roleid == 2 && x.Imagepath != null).ToList();


            // Pass the data to the view
            ViewBag.Chefs = chefs;

            return View(user);
        }

        public IActionResult ChefRecipe(int chefId)
        {
            var id = HttpContext.Session.GetInt32("UserId");
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
            ViewBag.Chef = _context.Users.Where(x => x.Userid == chefId).SingleOrDefault();
            // Pass chefId to the view


            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ChefRecipe(int chefId, string search)
        {
            var id = HttpContext.Session.GetInt32("UserId");
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
            var id = HttpContext.Session.GetInt32("UserId");
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
            var id = HttpContext.Session.GetInt32("UserId");
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



        // Other actions...
        
        public IActionResult Buy(int recipeId)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var recipes = _context.Recipes.Where(x => x.Recipeid == recipeId).ToList();
            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            var categories = _context.Recipecategories.ToList();
            var recipeInfoList = from r in recipes
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };


            ViewBag.Recipes = recipeInfoList;
            // Pass chefId to the view


            return View(user);
        }
        public IActionResult ProcessPurchase(int recipeid)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var recipe = _context.Recipes.FirstOrDefault(x => x.Recipeid == recipeid);
            if (recipe == null)
            {
                // Handle the case where the recipe is not found
                return RedirectToAction("RecipeNotFound");
            }
            var payments = _context.Payments.FirstOrDefault(x => x.Userid == id && x.Recipeid == recipeid);
        
            if (payments != null)
            {
                return RedirectToAction("PaymentSuccess");
            }

            ViewBag.Recipe = recipe;

            // Return the view with the Recipe object
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ProcessPurchase(Cardinfo card, int recipeid)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var recipe = _context.Recipes.FirstOrDefault(x => x.Recipeid == recipeid);
            if (recipe == null)
            {
                // Handle the case where the recipe is not found
                return RedirectToAction("RecipeNotFound");
            }

            ViewBag.Recipe = recipe;
            // Validate the Card details
            var DBcard = await _context.Cardinfos.FirstOrDefaultAsync(x => x.Cvv == card.Cvv && x.Cardnumber == card.Cardnumber && x.Expirydate == card.Expirydate && x.Cardholdername == card.Cardholdername);


            if (DBcard != null) { 
                // Check if the user's balance is sufficient to make the payment
                if (DBcard.Balance < recipe.Price)
                {
                    // If balance is insufficient, redirect to an error page
                    return RedirectToAction("PaymentError", User);
                }

                // Deduct the payment amount from the user's balance
                card.Balance -= (decimal)recipe.Price;

                // Create a payment record in the Payment table
                var payment = new Payment
                {
                    Userid = userId,
                    Recipeid = recipe.Recipeid,
                    Paymentamount = recipe.Price,
                    Paymentdate = DateTime.Now,
                    Paymentstatus = "Paid" // Assuming payment is successful
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(); // Use asynchronous SaveChangesAsync()

                // Redirect the user to a page indicating a successful payment
                return RedirectToAction("PaymentSuccess");
            }
            return View(card);

        }

        public IActionResult PaymentError()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult PaymentSuccess()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        
        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("UserId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }

        // PaymentViewModel.cs

        public class PaymentViewModel
        {
            public Recipe Recipe { get; set; }
            // Add other properties as needed for card information
        }

      

        public class RecipeInfo
        {
            public Recipe Recipe { get; set; }
            public User Chef { get; set; }
            public Recipecategory Category { get; set; }
        }
    }

}