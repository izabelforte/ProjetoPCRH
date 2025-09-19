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
    /// <summary>
    /// Controller responsável pela gestão das faturas (Faturações).
    /// Apenas utilizadores com o papel de Administrador podem aceder.
    /// </summary>
    [AuthorizeRole("Administrador")]
    public class FaturacoesController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa o controller de faturações com o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da aplicação.</param>
        public FaturacoesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as faturas, incluindo a informação do contrato associado.
        /// </summary>
        /// <returns>View com a lista de faturas.</returns>
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Faturas.Include(f => f.Contrato);
            return View(await appDbContext.ToListAsync());
        }

        /// <summary>
        /// Mostra os detalhes de uma fatura específica.
        /// </summary>
        /// <param name="id">Identificador da fatura.</param>
        /// <returns>View com os detalhes da fatura ou NotFound se não existir.</returns>
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

        /// <summary>
        /// Exibe o formulário para criar uma nova fatura.
        /// </summary>
        /// <returns>View para criação de fatura.</returns>
        public IActionResult Create()
        {
            ViewData["ContratoId"] = new SelectList(_context.Contratos, "ContratoId", "ContratoId");
            return View();
        }

        /// <summary>
        /// Cria uma nova fatura e guarda-a na base de dados.
        /// </summary>
        /// <param name="faturacao">Objeto fatura com os dados preenchidos.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
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

        /// <summary>
        /// Exibe o formulário de edição para uma fatura específica.
        /// </summary>
        /// <param name="id">Identificador da fatura.</param>
        /// <returns>View para edição da fatura ou NotFound se não existir.</returns>
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

        /// <summary>
        /// Atualiza os dados de uma fatura existente.
        /// </summary>
        /// <param name="id">Identificador da fatura.</param>
        /// <param name="faturacao">Objeto fatura atualizado.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
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

        /// <summary>
        /// Exibe a página de confirmação para exclusão de uma fatura.
        /// </summary>
        /// <param name="id">Identificador da fatura.</param>
        /// <returns>View de confirmação de exclusão ou NotFound se não existir.</returns>
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

        /// <summary>
        /// Confirma e executa a exclusão de uma fatura.
        /// </summary>
        /// <param name="id">Identificador da fatura.</param>
        /// <returns>Redireciona para Index após exclusão.</returns>
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

        /// <summary>
        /// Verifica se uma fatura existe na base de dados.
        /// </summary>
        /// <param name="id">Identificador da fatura.</param>
        /// <returns>True se a fatura existir, False caso contrário.</returns>
        private bool FaturacaoExists(int id)
        {
            return _context.Faturas.Any(e => e.FaturaId == id);
        }
    }
}

