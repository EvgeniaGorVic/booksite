using Microsoft.AspNetCore.Mvc;
using booksite.Data;
using booksite.Models;
using Microsoft.EntityFrameworkCore;

namespace booksite.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var featuredBooks = await _context.Books
                    .Where(b => b.IsFeatured)
                    .Take(6)
                    .ToListAsync();

                var bestsellers = await _context.Books
                    .Where(b => b.IsBestseller)
                    .Take(6)
                    .ToListAsync();

                ViewBag.FeaturedBooks = featuredBooks;
                ViewBag.Bestsellers = bestsellers;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в Index: {ex.Message}");
                return View(new List<Book>());
            }
        }

        public async Task<IActionResult> Product(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        public async Task<IActionResult> Catalog()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}