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
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }
        // GET: /Relatorios/RelatorioProjetoTerminado/5

        public async Task<IActionResult> TerminarProjeto(int id)
        {
            var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.ProjetoId == id);

            if (projeto == null)
            {
                return NotFound();
            }
            ViewBag.Projeto = projeto;
            // Passa um único Relatorio para a view
            return View();
        }
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

        // GET: Relatorios
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Relatorios.Include(r => r.Projeto);
            return View(await appDbContext.ToListAsync());
        }
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
                .Where(u => u.UtilizadorId == utilizadorId).Single();
            var clienteId = Convert.ToInt32(cliente.ClienteId);
                
             
            var relatorios = await _context.Relatorios
                .Include(r => r.Projeto.Cliente)
                .Where(r => r.Projeto.Cliente.ClienteId == clienteId)
                .ToListAsync();
            return View(relatorios);
        }

    }
}
