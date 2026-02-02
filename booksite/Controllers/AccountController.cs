using Microsoft.AspNetCore.Mvc;
using booksite.Data;
using booksite.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace booksite.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe = false, string? returnUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "Введите имя пользователя и пароль");
                    return View();
                }

                var hashedPassword = HashPassword(password);
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

                if (user == null || user.Password != hashedPassword || !user.IsActive)
                {
                    ModelState.AddModelError("", "Неверное имя пользователя или пароль");
                    return View();
                }

                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                if (user.IsAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                TempData["Success"] = "Вход выполнен успешно!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Произошла ошибка при входе");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(string Username, string Email, string Phone, string Password, string ConfirmPassword)
        {
            try
            {
                if (Password != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Пароли не совпадают");
                    return View("Login", new { register = "true" });
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == Username || u.Email == Email);

                if (existingUser != null)
                {
                    if (existingUser.Username == Username)
                        ModelState.AddModelError("Username", "Имя пользователя уже занято");
                    if (existingUser.Email == Email)
                        ModelState.AddModelError("Email", "Email уже зарегистрирован");

                    return View("Login", new { register = "true" });
                }

                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    Phone = Phone ?? string.Empty,
                    Password = HashPassword(Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["Success"] = "Регистрация успешна!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ошибка при регистрации");
                return View("Login", new { register = "true" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Вы успешно вышли";
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}