using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoPCRH.Models;

namespace ProjetoPCRH.Controllers
{
    /// <summary>
    /// Controller responsável pelas páginas principais da aplicação (Home).
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Inicializa o controller Home com o serviço de logging.
        /// </summary>
        /// <param name="logger">Instância de ILogger para registrar logs.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Página inicial da aplicação.
        /// Verifica se há utilizadores na sessão; caso contrário, redireciona para a página de login.
        /// </summary>
        /// <returns>View da página inicial ou redirecionamento para a página de login.</returns>
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
        /// Página de privacidade da aplicação.
        /// </summary>
        /// <returns>View da política de privacidade.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Página de erro da aplicação.
        /// Exibe informações sobre a requisição em caso de erro.
        /// </summary>
        /// <returns>View de erro com o modelo ErrorViewModel.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

