using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class LoginAndRegisterController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(x => x.Username == user.Username);

            if (existingUser == null)
            {
                if (ModelState.IsValid)
                {
                    // Get the selected role from the dropdown
                    switch (user.Roleid)
                    {
                        case 2:
                            // Chef
                            user.Roleid = 2;
                            break;
                        case 3:
                            // User
                            user.Roleid = 3;
                            break;
                        default:
                            // Default to User role if none selected
                            user.Roleid = 3;
                            break;
                    }

                    _context.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "LoginAndRegister"); // Redirect to login page after successful registration
                }
            }
            else
            {
                ModelState.AddModelError("Username", "User already exists.");
            }

            return View(user); // Return view with model to display validation errors
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            // Check if Username and Password are provided
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "Username and Password are required.");
                return View();
            }

            //  login logic 
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                // Check the role and set session accordingly
                if (user.Roleid == 1)
                {
                    HttpContext.Session.SetInt32("AdminId", (int)user.Userid);
                    return RedirectToAction("Index", "Admin");
                }
                else if (user.Roleid == 2)
                {
                    // Check if the user has an image path
                    if (!string.IsNullOrEmpty(user.Imagepath))
                    {
                        HttpContext.Session.SetInt32("ChefId", (int)user.Userid);
                        return RedirectToAction("Index", "Chef");
                    }
                    else
                    {
                        // Redirect to make him upload image
                        HttpContext.Session.SetInt32("imgId", (int)user.Userid);
                        return RedirectToAction("UploadImage", "LoginAndRegister");
                    }
                }
                else if (user.Roleid == 3)
                {
                    HttpContext.Session.SetInt32("UserId", (int)user.Userid);
                    return RedirectToAction("Index", "User");
                }
            }

            // If authentication fails, return to login page with error message
            ModelState.AddModelError("", "Username or Password is incorrect.");
            return View();
        }

        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            var id = HttpContext.Session.GetInt32("imgId");
            var user = _context.Users.Where(x => x.Userid == id).SingleOrDefault();

            if (imageFile != null && imageFile.Length > 0)
            {
                string wwwrootpath = _webHostEnvironment.WebRootPath;
                string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string fullPath = Path.Combine(wwwrootpath, "images", imageName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                user.Imagepath = imageName;

                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("imgId");
                HttpContext.Session.SetInt32("ChefId", (int)user.Userid);
                return RedirectToAction("Index", "Chef");

            }

            return View();
        }


    }
}
