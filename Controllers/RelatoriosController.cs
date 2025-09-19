using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoPCRH.Models;

namespace ProjetoPCRH.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de relatórios de projetos.
    /// Permite criar, listar e consultar relatórios de projetos.
    /// O acesso é restrito a Administrador, GestorProjeto e Cliente, dependendo do método.
    /// </summary>
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa o controller de relatórios com o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da aplicação.</param>
        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Exibe o relatório de um projeto terminado.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <returns>View com os detalhes do projeto ou NotFound se não existir.</returns>
        public async Task<IActionResult> TerminarProjeto(int id)
        {
            var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.ProjetoId == id);

            if (projeto == null)
            {
                return NotFound();
            }

            ViewBag.Projeto = projeto;
            return View();
        }

        /// <summary>
        /// Cria um novo relatório de projeto.
        /// </summary>
        /// <param name="relatorio">Objeto Relatorio preenchido com dados do formulário.</param>
        /// <returns>Redireciona para Index se a criação for bem-sucedida, caso contrário retorna para a mesma view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RelatorioId, DataRelatorio, Valor, TempoTotalHoras, ProjetoId")] Relatorio relatorio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relatorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Relatorios");
        }

        /// <summary>
        /// Lista todos os relatórios de projetos, incluindo informações do projeto.
        /// Apenas acessível por Administrador e GestorProjeto.
        /// </summary>
        /// <returns>View com a lista de relatórios.</returns>
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Relatorios.Include(r => r.Projeto);
            return View(await appDbContext.ToListAsync());
        }

        /// <summary>
        /// Lista os relatórios de projetos do cliente autenticado.
        /// Apenas acessível a utilizadores com papel de Cliente.
        /// </summary>
        /// <returns>View com a lista de relatórios do cliente ou redireciona para login se não houver sessão.</returns>
        [AuthorizeRole("Cliente")]
        public async Task<IActionResult> MeusRelatorios()
        {
            var utilizador = HttpContext.Session.GetString("UtilizadorId");
            var utilizadorId = Convert.ToInt32(utilizador);

            if (string.IsNullOrEmpty(utilizador))
            {
                return RedirectToAction("Login", "Account");
            }

            var cliente = _context.Utilizadores
                .Where(u => u.UtilizadorId == utilizadorId)
                .Single();
            var clienteId = Convert.ToInt32(cliente.ClienteId);

            var relatorios = await _context.Relatorios
                .Include(r => r.Projeto.Cliente)
                .Where(r => r.Projeto.Cliente.ClienteId == clienteId)
                .ToListAsync();

            return View(relatorios);
        }
    }
}

