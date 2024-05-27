using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;
using System.Net.Mail;
using System.Text;

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

            var testimonials = _context.Testimonials.Where(u=> u.Isapproved == "Yes")
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


        public IActionResult Profile()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }


            return View(user);
        }
        public IActionResult AddTestomonial()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }



            return View();
        }

        [HttpPost]
        public IActionResult AddTestimonial(Testimonial model)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.SingleOrDefault(x => x.Userid == id);
            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }
            if (ModelState.IsValid)
            {
                model.Userid = id;
                model.Dateadded = DateTime.Now; // Set the date added
                model.Isapproved = "No";
                _context.Testimonials.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            // In case of invalid state, load the user again for the view
            return View(model);
        }



        public IActionResult EditProfile()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }


            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
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


        public IActionResult YourRecipes()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // I wanna List the Recipe that user Bought only 
            var recipes = _context.Recipes.Where(x => x.Isaccepted == "Yes").ToList();
            var chefs = _context.Users.Where(x => x.Roleid == 2).ToList();
            var categories = _context.Recipecategories.ToList();
            var payments = _context.Payments.Where(x => x.Userid == id);
            // Assuming id is the user's ID
            var recipeInfoList = from r in recipes
                                 join p in payments on r.Recipeid equals p.Recipeid
                                 where p.Userid == id
                                 join c in chefs on r.Chefid equals c.Userid
                                 join cat in categories on r.Categoryid equals cat.Categoryid
                                 select new RecipeInfo { Recipe = r, Chef = c, Category = cat };

            ViewBag.Recipes = recipeInfoList;

            return View();

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
                return RedirectToAction("PaymentSuccess", new { recipeid = recipe.Recipeid });
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
                return RedirectToAction("PaymentSuccess", new { recipeid = recipe.Recipeid });
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

        public async Task<IActionResult> PaymentSuccess(int recipeid)
        {
            var recipe = _context.Recipes.FirstOrDefault(x => x.Recipeid == recipeid);
            if (recipe == null)
            {
                // Handle the case where the recipe is not found
                return RedirectToAction("RecipeNotFound");
            }

            ViewBag.Recipe = recipe;

            // Retrieve the user's email address
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user == null)
            {
                // Handle the case where the user is not found
                return RedirectToAction("Index", "Home");
            }

            // Send recipe details to the user's email
            await SendRecipeEmail(recipeid);

            return View();
        }

        // Method to send recipe details to user's email
        // Method to send recipe details to user's email
        private async Task SendRecipeEmail(int recipeId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            var recipe = await _context.Recipes.FirstOrDefaultAsync(u => u.Recipeid == recipeId);


            StringBuilder emailContent = new StringBuilder();
            emailContent.AppendLine("<html>");
            emailContent.AppendLine("<head>");
            emailContent.AppendLine("<style>");
            emailContent.AppendLine("body { font-family: Arial, sans-serif; background-color: #f4f4f4; }");
            emailContent.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; border-radius: 5px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1); }");
            emailContent.AppendLine("h2 { color: #333; }");
            emailContent.AppendLine("p { color: #666; }");
            emailContent.AppendLine(".invoice { margin-top: 20px; border-top: 1px solid #ddd; padding-top: 10px; }");
            emailContent.AppendLine(".invoice p { margin: 5px 0; }");
            emailContent.AppendLine("</style>");
            emailContent.AppendLine("</head>");
            emailContent.AppendLine("<body>");
            emailContent.AppendLine("<div class='container'>");
            emailContent.AppendLine("<h2>Recipe Details</h2>");
            emailContent.AppendLine("<p><strong>Name:</strong> " + recipe.Recipename + "</p>");
            emailContent.AppendLine("<p><strong>Ingredients:</strong> " + recipe.Ingredients + "</p>");
            emailContent.AppendLine("<p><strong>Instructions:</strong> " + recipe.Instructions + "</p>");
            emailContent.AppendLine("<div class='invoice'>");
            emailContent.AppendLine("<h2>Invoice Details</h2>");
            emailContent.AppendLine("<p><strong>Total Price:</strong> $" + recipe.Price + "</p>");
            // Add more invoice details here if needed
            emailContent.AppendLine("</div>");
            emailContent.AppendLine("</div>");
            emailContent.AppendLine("</body>");
            emailContent.AppendLine("</html>");

            using (MailMessage mail = new MailMessage())
            {
                
                mail.From = new MailAddress("recipeblogtahaluf@gmail.com"); // Sender's email address
                mail.To.Add(user.Email); // Recipient's email address
                mail.Subject = "Recipe Details and Invoice"; // Email subject
                mail.Body = emailContent.ToString(); // Email body content
                mail.IsBodyHtml = true; // Set to true if the email body contains HTML content

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; // Your SMTP host
                smtp.Port = 587; // Your SMTP port
                smtp.Credentials = new System.Net.NetworkCredential("recipeblogtahaluf@gmail.com", "onbkrbaamxlevbev"); // Your SMTP credentials
                smtp.EnableSsl = true; // Enable SSL/TLS

                // Send the email
                await smtp.SendMailAsync(mail);
            }
        }




        public IActionResult Logout()
        {
            // Remove the specific session variable "ChefId"
            HttpContext.Session.Remove("UserId");

            // Redirect to the login page or any other desired page
            return RedirectToAction("Index", "Home");
        }

        // PaymentViewModel.cs


        public class RecipeInfo
        {
            public Recipe Recipe { get; set; }
            public User Chef { get; set; }
            public Recipecategory Category { get; set; }
        }
    }

}