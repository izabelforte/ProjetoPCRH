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
        /// <summary>
        /// Realiza a autenticação do utilizador com base no username e password informados.
        /// Se as credenciais forem válidas, guarda informações importantes na sessão
        /// (ID, nome de utilizador e tipo) e redireciona para a página inicial.
        /// Caso contrário, retorna a view de login com mensagem de erro.
        /// </summary>
        /// <param name="username">Nome de utilizador digitado no formulário de login.</param>
        /// <param name="password">Password digitada no formulário de login.</param>
        /// <returns>Redireciona para Home/Index se login for bem-sucedido, ou devolve a view de login em caso de erro.</returns>
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