using Microsoft.AspNetCore.Mvc;

namespace ProjetoPCRH.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Exemplo simples: utilizador fixo
            if (username == "admin" && password == "1234")
            {
                // Guardar login em sessão
                HttpContext.Session.SetString("User", username);

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
