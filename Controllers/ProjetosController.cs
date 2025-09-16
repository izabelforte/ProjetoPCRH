using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoPCRH.Models;

namespace ProjetoPCRH.Controllers
{
    
    public class ProjetosController : Controller
    {
        public ICollection<Funcionario> Funcionarios { get; set; }


        private readonly AppDbContext _context;

        public ProjetosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Projetos
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Projetos.Include(p => p.Cliente);
            return View(await appDbContext.ToListAsync());
        }

        // NOVO MÉTODO: MeusProjetos
        [AuthorizeRole("Funcionario")]
        public async Task<IActionResult> MeusProjetos()
        {
            var username = HttpContext.Session.GetString("Utilizadores");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var projetos = await _context.Projetos
                .Include(p => p.Funcionarios)
                .Where(p => p.Funcionarios.Any(f => f.NomeFuncionario == username))
                .ToListAsync();

            return View(projetos);
        }


        // GET: Projetos/Details/5
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var projeto = await _context.Projetos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.ProjetoId == id);

            if (projeto == null) return NotFound();

            return View(projeto);
        }

        // GET: Projetos/Create
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email");
            return View();
        }

        // POST: Projetos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Create([Bind("ProjetoId,NomeProjeto,Descricao,DataInicio,DataFim,Orcamento,StatusProjeto,ClienteId")] Projeto projeto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projeto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", projeto.ClienteId);
            return View(projeto);
        }

        // GET: Projetos/Edit/5
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null) return NotFound();

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", projeto.ClienteId);
            return View(projeto);
        }

        // POST: Projetos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Edit(int id, [Bind("ProjetoId,NomeProjeto,Descricao,DataInicio,DataFim,Orcamento,StatusProjeto,ClienteId")] Projeto projeto)
        {
            if (id != projeto.ProjetoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projeto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjetoExists(projeto.ProjetoId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", projeto.ClienteId);
            return View(projeto);
        }

        // GET: Projetos/Delete/5
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var projeto = await _context.Projetos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.ProjetoId == id);

            if (projeto == null) return NotFound();

            return View(projeto);
        }

        // POST: Projetos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto != null) _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Projetos/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Terminar(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null)
            {
                return NotFound();
            }

            // Atualizar status do projeto
            projeto.StatusProjeto = "Terminado";
            _context.Update(projeto);
            await _context.SaveChangesAsync();

            // Calcular tempo total em horas (DataFim não é nullable)
            int horas = (int)(projeto.DataFim - projeto.DataInicio).TotalHours;

            // Criar relatório associado ao projeto
            var relatorio = new Relatorio
            {
                DataRelatorio = DateTime.Now,
                Valor = projeto.Orcamento,
                TempoTotalHoras = horas,
                ProjetoId = projeto.ProjetoId
            };

            _context.Relatorios.Add(relatorio);
            await _context.SaveChangesAsync();

            // Redirecionar para a view RelatorioProjetoTerminado
            // Certifica-te que a view espera um único Relatorio
            return RedirectToAction("RelatorioProjetoTerminado", "Relatorios", new { id = relatorio.RelatorioId });
        }


        private bool ProjetoExists(int id)
        {
            return _context.Projetos.Any(e => e.ProjetoId == id);
        }
    }
}
