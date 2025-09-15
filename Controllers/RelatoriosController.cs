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
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Relatorios
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Relatorios.Include(r => r.Projeto);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Relatorios/Details/5
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var relatorio = await _context.Relatorios
                .Include(r => r.Projeto)
                .FirstOrDefaultAsync(m => m.RelatorioId == id);

            if (relatorio == null) return NotFound();

            return View(relatorio);
        }

        // GET: Relatorios/Create
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public IActionResult Create()
        {
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto");
            return View();
        }

        // POST: Relatorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Create([Bind("RelatorioId,DataRelatorio,Valor,TempoTotalHoras,ProjetoId")] Relatorio relatorio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relatorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto", relatorio.ProjetoId);
            return View(relatorio);
        }

        // GET: Relatorios/Edit/5
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var relatorio = await _context.Relatorios.FindAsync(id);
            if (relatorio == null) return NotFound();

            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto", relatorio.ProjetoId);
            return View(relatorio);
        }

        // POST: Relatorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Edit(int id, [Bind("RelatorioId,DataRelatorio,Valor,TempoTotalHoras,ProjetoId")] Relatorio relatorio)
        {
            if (id != relatorio.RelatorioId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relatorio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelatorioExists(relatorio.RelatorioId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto", relatorio.ProjetoId);
            return View(relatorio);
        }

        // GET: Relatorios/Delete/5
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var relatorio = await _context.Relatorios
                .Include(r => r.Projeto)
                .FirstOrDefaultAsync(m => m.RelatorioId == id);

            if (relatorio == null) return NotFound();

            return View(relatorio);
        }

        // POST: Relatorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relatorio = await _context.Relatorios.FindAsync(id);
            if (relatorio != null)
            {
                _context.Relatorios.Remove(relatorio);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelatorioExists(int id)
        {
            return _context.Relatorios.Any(e => e.RelatorioId == id);
        }
    }
}

