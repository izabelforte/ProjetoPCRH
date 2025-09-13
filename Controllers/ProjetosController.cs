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
        private readonly AppDbContext _context;

        public ProjetosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Projetos
        [AuthorizeRole("Admin", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Projetos.Include(p => p.Cliente);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Projetos/Details/5
        [AuthorizeRole("Admin", "GestorProjeto")]
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
        [AuthorizeRole("Admin", "GestorProjeto")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email");
            return View();
        }

        // POST: Projetos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "GestorProjeto")]
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
        [AuthorizeRole("Admin", "GestorProjeto")]
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
        [AuthorizeRole("Admin", "GestorProjeto")]
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
        [AuthorizeRole("Admin", "GestorProjeto")]
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
        [AuthorizeRole("Admin", "GestorProjeto")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto != null) _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ProjetoExists(int id)
        {
            return _context.Projetos.Any(e => e.ProjetoId == id);
        }
    }
}
