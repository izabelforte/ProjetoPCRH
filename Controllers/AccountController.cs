using Microsoft.AspNetCore.Mvc;
using ProjetoPCRH.Models;

namespace ProjetoPCRH.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var utilizador = _context.Utilizadores
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (utilizador != null)
            {
                HttpContext.Session.SetString("User", utilizador.Username);
                HttpContext.Session.SetString("Tipo", utilizador.Tipo);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Utilizador ou password incorretos!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}