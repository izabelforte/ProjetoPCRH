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
    public class FaturacoesController : Controller
    {
        private readonly AppDbContext _context;

        public FaturacoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Faturacaos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Faturas.Include(f => f.Contrato);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Faturacaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faturacao = await _context.Faturas
                .Include(f => f.Contrato)
                .FirstOrDefaultAsync(m => m.FaturaId == id);
            if (faturacao == null)
            {
                return NotFound();
            }

            return View(faturacao);
        }

        // GET: Faturacaos/Create
        public IActionResult Create()
        {
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "ContratoId", "ContratoId");
            return View();
        }

        // POST: Faturacaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FaturaId,DataFatura,Valor,ContratoId")] Faturacao faturacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faturacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "ContratoId", "ContratoId", faturacao.ContratoId);
            return View(faturacao);
        }

        // GET: Faturacaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faturacao = await _context.Faturas.FindAsync(id);
            if (faturacao == null)
            {
                return NotFound();
            }
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "ContratoId", "ContratoId", faturacao.ContratoId);
            return View(faturacao);
        }

        // POST: Faturacaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FaturaId,DataFatura,Valor,ContratoId")] Faturacao faturacao)
        {
            if (id != faturacao.FaturaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faturacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaturacaoExists(faturacao.FaturaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "ContratoId", "ContratoId", faturacao.ContratoId);
            return View(faturacao);
        }

        // GET: Faturacaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faturacao = await _context.Faturas
                .Include(f => f.Contrato)
                .FirstOrDefaultAsync(m => m.FaturaId == id);
            if (faturacao == null)
            {
                return NotFound();
            }

            return View(faturacao);
        }

        // POST: Faturacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faturacao = await _context.Faturas.FindAsync(id);
            if (faturacao != null)
            {
                _context.Faturas.Remove(faturacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaturacaoExists(int id)
        {
            return _context.Faturas.Any(e => e.FaturaId == id);
        }
    }
}
