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
        public LoginAndRegisterController(ModelContext context)
        {
            _context = context;
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

            // Your login logic here to authenticate the user
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
                    HttpContext.Session.SetInt32("ChefId", (int)user.Userid);
                    return RedirectToAction("Index", "Chef");
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
    }
}
