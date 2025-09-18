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
                // Guardar na sessão com os nomes corretos
                HttpContext.Session.SetInt32("UtilizadorId", utilizador.UtilizadorId);
                HttpContext.Session.SetString("Utilizadores", utilizador.Username);
                HttpContext.Session.SetString("UtilizadorId", utilizador.UtilizadorId.ToString());
                HttpContext.Session.SetString("TipoUtilizadores", utilizador.Tipo); // Administrador, GestorProjeto, Funcionario, Cliente


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

        public IActionResult acessoNegado()
        {
            return View();
        }

    }
}