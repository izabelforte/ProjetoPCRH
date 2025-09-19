using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoPCRH.Models;

namespace ProjetoPCRH.Controllers
{
    /// <summary>
    /// Controller respons�vel pelas p�ginas principais da aplica��o (Home).
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Inicializa o controller Home com o servi�o de logging.
        /// </summary>
        /// <param name="logger">Inst�ncia de ILogger para registrar logs.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// P�gina inicial da aplica��o.
        /// Verifica se h� utilizadores na sess�o; caso contr�rio, redireciona para a p�gina de login.
        /// </summary>
        /// <returns>View da p�gina inicial ou redirecionamento para a p�gina de login.</returns>
        public IActionResult Index()
        {
            var utilizadores = HttpContext.Session.GetString("Utilizadores");
            if (string.IsNullOrEmpty(utilizadores))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        /// <summary>
        /// P�gina de privacidade da aplica��o.
        /// </summary>
        /// <returns>View da pol�tica de privacidade.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// P�gina de erro da aplica��o.
        /// Exibe informa��es sobre a requisi��o em caso de erro.
        /// </summary>
        /// <returns>View de erro com o modelo ErrorViewModel.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

