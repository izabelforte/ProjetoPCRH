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
    [AuthorizeRole("Administrador", "GestorProjeto")]
    public class ContratosController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa o controller de contratos com o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da aplicação.</param>
        public ContratosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os contratos existentes, incluindo cliente e projeto associados.
        /// </summary>
        /// <returns>View com a lista de contratos.</returns>
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Contratos.Include(c => c.Cliente).Include(c => c.Projeto);
            return View(await appDbContext.ToListAsync());
        }

        /// <summary>
        /// Mostra os detalhes de um contrato específico.
        /// </summary>
        /// <param name="id">Identificador do contrato.</param>
        /// <returns>View com os detalhes do contrato ou NotFound se não existir.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _context.Contratos
                .Include(c => c.Cliente)
                .Include(c => c.Projeto)
                .FirstOrDefaultAsync(m => m.ContratoId == id);
            if (contrato == null)
            {
                return NotFound();
            }

            return View(contrato);
        }

        /// <summary>
        /// Exibe o formulário para criar um novo contrato.
        /// </summary>
        /// <returns>View para criação de contrato.</returns>
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email");
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto");
            return View();
        }

        /// <summary>
        /// Cria um novo contrato e guarda-o na base de dados.
        /// </summary>
        /// <param name="contrato">Objeto contrato com os dados preenchidos.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContratoId,DataInicio,DataFim,Valor,StatusContrato,ClienteId,ProjetoId")] Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contrato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", contrato.ClienteId);
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto", contrato.ProjetoId);
            return View(contrato);
        }

        /// <summary>
        /// Exibe o formulário de edição para um contrato específico.
        /// </summary>
        /// <param name="id">Identificador do contrato.</param>
        /// <returns>View para edição do contrato ou NotFound se não existir.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", contrato.ClienteId);
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto", contrato.ProjetoId);
            return View(contrato);
        }

        /// <summary>
        /// Atualiza os dados de um contrato existente.
        /// </summary>
        /// <param name="id">Identificador do contrato.</param>
        /// <param name="contrato">Objeto contrato atualizado.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContratoId,DataInicio,DataFim,Valor,StatusContrato,ClienteId,ProjetoId")] Contrato contrato)
        {
            if (id != contrato.ContratoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contrato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContratoExists(contrato.ContratoId))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", contrato.ClienteId);
            ViewData["ProjetoId"] = new SelectList(_context.Projetos, "ProjetoId", "NomeProjeto", contrato.ProjetoId);
            return View(contrato);
        }

        /// <summary>
        /// Exibe a página de confirmação para exclusão de um contrato.
        /// </summary>
        /// <param name="id">Identificador do contrato.</param>
        /// <returns>View de confirmação de exclusão ou NotFound se não existir.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _context.Contratos
                .Include(c => c.Cliente)
                .Include(c => c.Projeto)
                .FirstOrDefaultAsync(m => m.ContratoId == id);
            if (contrato == null)
            {
                return NotFound();
            }

            return View(contrato);
        }

        /// <summary>
        /// Confirma e executa a exclusão de um contrato.
        /// </summary>
        /// <param name="id">Identificador do contrato.</param>
        /// <returns>Redireciona para Index após exclusão.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato != null)
            {
                _context.Contratos.Remove(contrato);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um contrato existe na base de dados.
        /// </summary>
        /// <param name="id">Identificador do contrato.</param>
        /// <returns>True se o contrato existir, False caso contrário.</returns>
        private bool ContratoExists(int id)
        {
            return _context.Contratos.Any(e => e.ContratoId == id);
        }
    }
}
