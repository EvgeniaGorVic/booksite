using Microsoft.AspNetCore.Mvc;

namespace booksite.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(int bookId)
        {
            return Json(new
            {
                success = true,
                message = "Книга добавлена в корзину! (демо-режим)"
            });
        }

        [HttpGet]
        public IActionResult GetCount()
        {
            return Json(new { count = 0 });
        }
    }
}