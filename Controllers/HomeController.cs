// using System.Diagnostics;
// using Microsoft.AspNetCore.Mvc;
// using LoginReg.Models;
// using LoginReg.Context;
// using Microsoft.AspNetCore.Identity;
// namespace LoginReg.Controllers
// {
//     public class HomeController : Controller
//     {
//         private readonly ILogger<HomeController> _logger;
//         private readonly AppDbContext _context;

//         public HomeController(ILogger<HomeController> logger, AppDbContext context)
//         {
//             _logger = logger;
//             _context = context;
//         }

//         public IActionResult Index()
//         {
//             return View();
//         }

//         public IActionResult Privacy()
//         {
//             return View();
//         }

//         [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//         public IActionResult Error()
//         {
//             return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//         }

//         [HttpGet]
//         public IActionResult Login()
//         {
//             return View();
//         }

//         [HttpPost]
//         public IActionResult Login(string Email, string Password)
//         {
//             var user = _context.Users.FirstOrDefault(u => u.Email == Email);
//             if (user != null && new PasswordHasher().VerifyHashedPassword(user.PasswordHash, Password) == PasswordVerificationResult.Success)
//             {
//                 HttpContext.Session.SetInt32("UserId", user.Id);
//                 return RedirectToAction("Success");
//             }
//             ModelState.AddModelError("LoginError", "Invalid login attempt.");
//             return View();
//         }

//         [HttpGet]
//         public IActionResult Register()
//         {
//             return View();
//         }

//         [HttpPost]
//         public IActionResult Register(User user, string ConfirmPassword)
//         {
//             if (ModelState.IsValid && user.Password == ConfirmPassword)
//             {
//                 if (!_context.Users.Any(u => u.Email == user.Email))
//                 {
//                     var hasher = new PasswordHasher<User>();
//                     user.PasswordHash = hasher.HashPassword(user, user.Password);
//                     user.CreatedAt = DateTime.Now;
//                     user.UpdatedAt = DateTime.Now;
//                     _context.Users.Add(user);
//                     _context.SaveChanges();
//                     HttpContext.Session.SetInt32("UserId", user.Id);
//                     return RedirectToAction("Success");
//                 }
//                 ModelState.AddModelError("Email", "Email already exists.");
//             }
//             return View();
//         }

//         public IActionResult Logout()
//         {
//             HttpContext.Session.Clear();
//             return RedirectToAction("Login");
//         }

//         public IActionResult Success()
//         {
//             if (HttpContext.Session.GetInt32("UserId") == null)
//             {
//                 return RedirectToAction("Login");
//             }
//             return View();
//         }
//     }

//     internal class PasswordHasher
//     {
//         public PasswordHasher()
//         {
//         }

//         internal PasswordVerificationResult VerifyHashedPassword(string passwordHash, string password)
//         {
//             throw new NotImplementedException();
//         }
//     }

//     public class AuthMiddleware
//     {
//         private readonly RequestDelegate _next;
//         public AuthMiddleware(RequestDelegate next) => _next = next;
//         public async Task InvokeAsync(HttpContext context)
//         {
//             if (context.Session.GetInt32("UserId") == null && context.Request.Path != "/Login" && context.Request.Path != "/Register")
//             {
//                 context.Response.Redirect("/Login");
//                 return;
//             }
//             await _next(context);
//         }
//     }
// }
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LoginReg.Models;
using LoginReg.Context;
using Microsoft.AspNetCore.Identity;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
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

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("LoginError", "All fields are required.");
                return View("Index");
            }

            if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                ModelState.AddModelError("LoginError", "Invalid email format.");
                return View("Index");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser == null)
            {
                ModelState.AddModelError("LoginError", "Email does not exist.");
                return View("Index");
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(existingUser, existingUser.PasswordHash, user.Password) != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("LoginError", "Invalid login attempt.");
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", existingUser.Id);
            return RedirectToAction("Success");
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.ConfirmPassword))
            {
                ModelState.AddModelError("RegisterError", "All fields are required.");
                return View("Index");
            }

            if (user.FirstName.Length < 2)
            {
                ModelState.AddModelError("RegisterError", "First name must be at least 2 characters.");
                return View("Index");
            }

            if (user.LastName.Length < 2)
            {
                ModelState.AddModelError("RegisterError", "Last name must be at least 2 characters.");
                return View("Index");
            }

            if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                ModelState.AddModelError("RegisterError", "Invalid email format.");
                return View("Index");
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("RegisterError", "Email already exists.");
                return View("Index");
            }

            if (user.Password.Length < 8)
            {
                ModelState.AddModelError("RegisterError", "Password must be at least 8 characters.");
                return View("Index");
            }

            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("RegisterError", "Password confirm must match password.");
                return View("Index");
            }

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, user.Password);
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Success");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Success()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

