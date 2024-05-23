using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ModelContext? context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;   
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
            ViewBag.TotalSales = _context.Payments.Count(x => x.Paymentstatus == "Paid");
            ViewBag.TotalProfit = _context.Payments
    .Where(x => x.Paymentstatus == "Paid")
    .Sum(x => x.Paymentamount);




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

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            return View(user);
        }
        public IActionResult Home()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var Home = _context.Homepages.FirstOrDefault();

            ViewBag.HomePage = Home;

            return View(user);
        }

        public IActionResult EditHomePage()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var Home = _context.Homepages.FirstOrDefault();

            ViewBag.HomePage = Home;

            return View(Home);
        }

        [HttpPost]
        public async Task<IActionResult> EditHomePage(Homepage model, IFormFile imageFile)
        {
            var homePage = _context.Homepages.FirstOrDefault();
            if (homePage == null)
            {
                return NotFound();
            }

            // Store the existing image path
            string existingImagePath = homePage.Imagepath;

            // Check if a new image is provided
            if (imageFile != null && imageFile.Length > 0)
            {
                // If a new image is provided, handle the file upload and update the image path
                string wwwrootpath = _webHostEnvironment.WebRootPath;
                string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string fullPath = Path.Combine(wwwrootpath, "images", imageName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                homePage.Imagepath = imageName;
            }
            else
            {
                // If no new image is provided, retain the existing image path
                homePage.Imagepath = existingImagePath;
            }

            // Validate the model without considering the ImageFile property if no new image is provided
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                // Update other properties of the homepage as needed
                homePage.Pagecontent = model.Pagecontent;

                _context.SaveChanges();

                return RedirectToAction("Home"); // Redirect to a success page
            }

            return View(model); // If the model state is invalid, return the view with validation errors 
        }


        public IActionResult AboutUs()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var AboutUs = _context.Aboutuspages.FirstOrDefault();

            ViewBag.About = AboutUs;

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> EditAboutUs(Aboutuspage model, IFormFile imageFile)
        {
            var AboutUs = _context.Aboutuspages.FirstOrDefault();
            if (AboutUs == null)
            {
                return NotFound();
            }

            // Store the existing image path
            string existingImagePath = AboutUs.Imagepath;

            // Check if a new image is provided
            if (imageFile != null && imageFile.Length > 0)
            {
                // If a new image is provided, handle the file upload and update the image path
                string wwwrootpath = _webHostEnvironment.WebRootPath;
                string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string fullPath = Path.Combine(wwwrootpath, "images", imageName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                AboutUs.Imagepath = imageName;
            }
            else
            {
                // If no new image is provided, retain the existing image path
                AboutUs.Imagepath = existingImagePath;
            }

            // Validate the model without considering the ImageFile property if no new image is provided
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                // Update other properties of the homepage as needed
                AboutUs.Pagecontent = model.Pagecontent;

                _context.SaveChanges();

                return RedirectToAction("Home"); // Redirect to a success page
            }

            return View(model); // If the model state is invalid, return the view with validation errors 
        }

        public IActionResult ContactUs()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var Contactus = _context.Contactus.FirstOrDefault();

            ViewBag.Contact = Contactus;

            return View(user);
        }
        public IActionResult Testomonials()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var testimonials = _context.Testimonials
                                           .Include(t => t.User)
                                           .ToList();

            ViewBag.Testomnials = testimonials;



            return View(user);
        }
        [HttpPost]
        public IActionResult ApproveTest(int Testimonialid)
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            var test = _context.Testimonials.FirstOrDefault(x => x.Testimonialid == Testimonialid);


            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Update user's information
            test.Isapproved = "Yes";

            _context.SaveChanges();

            return RedirectToAction("Testomonials");
        }

        [HttpPost]
        public IActionResult RejectTest(int Testimonialid)
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            var test = _context.Testimonials.FirstOrDefault(x => x.Testimonialid == Testimonialid);


            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Update user's information
            test.Isapproved = "No";

            _context.SaveChanges();

            return RedirectToAction("Testomonials");
        }


        [HttpPost]
        public IActionResult Profile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // Update user's information
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;

            _context.SaveChanges();

            return RedirectToAction("Profile");
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
        public async Task<IActionResult> Reports(DateTime? startDate, DateTime? endDate)
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var users = _context.Users.Where(x => x.Roleid == 3).ToList();
            var payments = _context.Payments.ToList();
            var categories = _context.Recipecategories.ToList();
            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();

            // Filter recipe payments based on search start and end dates if provided
            var recipePaymentsList = (from p in payments
                                      join r in recipes on p.Recipeid equals r.Recipeid
                                      join u in users on p.Userid equals u.Userid
                                      join rc in categories on r.Categoryid equals rc.Categoryid
                                      where p.Paymentstatus == "Paid" &&
                                            (!startDate.HasValue || p.Paymentdate >= startDate.Value.Date) &&
                                            (!endDate.HasValue || p.Paymentdate <= endDate.Value.Date)
                                      select new RecipePayments
                                      {
                                          Recipe = r,
                                          User = u,
                                          Category = rc,
                                          Payments = p
                                      }).ToList();


            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();

            // Filter recipe info based on search start and end dates if provided
            var recipeInfoList = (from r in recipes
                                  join c in chefs on r.Chefid equals c.Userid
                                  join cat in categories on r.Categoryid equals cat.Categoryid
                                  where (!startDate.HasValue || r.Createdate >= startDate.Value) &&
                                        (!endDate.HasValue || r.Createdate <= endDate.Value)
                                  select new RecipeInfo { Recipe = r, Chef = c, Category = cat }).ToList();

            ViewBag.RecipeInfo = recipeInfoList;

            ViewBag.RequstedInfo = recipePaymentsList;

            return View();
        }
        [HttpGet] 
        public IActionResult Reports()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.FirstOrDefault(x => x.Userid == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var users = _context.Users.Where(x => x.Roleid == 3).ToList();
            var payments = _context.Payments.ToList();
            var categories = _context.Recipecategories.ToList();
            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();

            // Filter recipe payments based on search start and end dates if provided
            var recipePaymentsList = (from p in payments
                                      join r in recipes on p.Recipeid equals r.Recipeid
                                      join u in users on p.Userid equals u.Userid
                                      join rc in categories on r.Categoryid equals rc.Categoryid
                                      where p.Paymentstatus == "Paid" 
                                      select new RecipePayments
                                      {
                                          Recipe = r,
                                          User = u,
                                          Category = rc,
                                          Payments = p
                                      }).ToList();


            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();

            // Filter recipe info based on search start and end dates if provided
            var recipeInfoList = (from r in recipes
                                  join c in chefs on r.Chefid equals c.Userid
                                  join cat in categories on r.Categoryid equals cat.Categoryid
                                  select new RecipeInfo { Recipe = r, Chef = c, Category = cat }).ToList();

            ViewBag.RecipeInfo = recipeInfoList;

            ViewBag.RequstedInfo = recipePaymentsList;

            return View();
        }

        public IActionResult Recipes()
        {
            var id = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }
            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();
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
        public IActionResult UnActivateRecipe(int recipeId)
        {
            // Find the recipe in the database
            var recipe = _context.Recipes.FirstOrDefault(r => r.Recipeid == recipeId);

            if (recipe != null)
            {
                // Update the IsAccepted property
                recipe.Isaccepted = "No"; 

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to the same page
                return RedirectToAction("Recipes");
            }

            // Handle error
            return NotFound();
        }

       
        [HttpPost]
        public IActionResult AcceptRecipe(int recipeId)
        {
            // Find the recipe in the database
            var recipe = _context.Recipes.FirstOrDefault(r => r.Recipeid == recipeId);

            if (recipe != null)
            {
                // Update the IsAccepted property
                recipe.Isaccepted = "Yes";

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to the same page
                return RedirectToAction("ApproveRecipe");
            }

           
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
            // Remove the specific session variable "Admin"
            HttpContext.Session.Remove("AdminId");

            // Redirect to the home page
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
