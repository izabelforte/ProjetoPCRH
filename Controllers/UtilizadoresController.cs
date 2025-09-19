using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoPCRH.Models;
using ProjetoPCRH.Models.ViewModels;

namespace ProjetoPCRH.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de utilizadores.
    /// Permite listar, criar, editar, detalhar e eliminar utilizadores.
    /// Apenas acessível a utilizadores com o papel de Administrador.
    /// </summary>
    [AuthorizeRole("Administrador")]
    public class UtilizadoresController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa o controller de utilizadores com o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da aplicação.</param>
        public UtilizadoresController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os utilizadores da aplicação.
        /// </summary>
        /// <returns>View com a lista de utilizadores.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizadores.ToListAsync());
        }

        /// <summary>
        /// Mostra os detalhes de um utilizador específico.
        /// </summary>
        /// <param name="id">Identificador do utilizador.</param>
        /// <returns>View com os detalhes ou NotFound se não existir.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);

            if (utilizador == null) return NotFound();

            return View(utilizador);
        }

        /// <summary>
        /// Mostra o formulário para criar um novo utilizador.
        /// Carrega listas de funcionários e clientes disponíveis.
        /// </summary>
        /// <returns>View para criação de utilizador.</returns>
        public IActionResult Create()
        {
            var funcionariosDisponiveis = _context.Funcionarios
                .Where(f => !_context.Utilizadores.Any(u => u.FuncionarioId == f.FuncionarioId))
                .ToList();

            var clientesDisponiveis = _context.Clientes
                .Where(c => !_context.Utilizadores.Any(u => u.ClienteId == c.ClienteId))
                .ToList();

            var vm = new UtilizadorCreateViewModel
            {
                Utilizador = new Utilizador(),
                Funcionarios = funcionariosDisponiveis,
                Clientes = clientesDisponiveis
            };

            return View(vm);
        }

        /// <summary>
        /// Processa a criação de um novo utilizador.
        /// </summary>
        /// <param name="model">ViewModel com os dados do utilizador a criar.</param>
        /// <returns>Redireciona para Index em caso de sucesso ou recarrega a View em caso de falha.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtilizadorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Utilizador.Tipo == "Funcionario") model.Utilizador.ClienteId = null;
                else if (model.Utilizador.Tipo == "Cliente") model.Utilizador.FuncionarioId = null;
                else { model.Utilizador.ClienteId = null; model.Utilizador.FuncionarioId = null; }

                _context.Add(model.Utilizador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// Mostra o formulário para edição de um utilizador existente.
        /// </summary>
        /// <param name="id">Identificador do utilizador a editar.</param>
        /// <returns>View com os dados do utilizador ou NotFound se não existir.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utilizador = await _context.Utilizadores
                .Include(u => u.Funcionario)
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.UtilizadorId == id);

            if (utilizador == null) return NotFound();

            var funcionariosDisponiveis = _context.Funcionarios
                .Where(f => !_context.Utilizadores.Any(u => u.FuncionarioId == f.FuncionarioId) || f.FuncionarioId == utilizador.FuncionarioId)
                .ToList();

            var clientesDisponiveis = _context.Clientes
                .Where(c => !_context.Utilizadores.Any(u => u.ClienteId == c.ClienteId) || c.ClienteId == utilizador.ClienteId)
                .ToList();

            var vm = new UtilizadorCreateViewModel
            {
                Utilizador = utilizador,
                Funcionarios = funcionariosDisponiveis,
                Clientes = clientesDisponiveis
            };

            return View(vm);
        }

        /// <summary>
        /// Processa a edição de um utilizador existente.
        /// </summary>
        /// <param name="id">Identificador do utilizador.</param>
        /// <param name="model">ViewModel com os dados atualizados.</param>
        /// <returns>Redireciona para Index em caso de sucesso ou recarrega a View em caso de falha.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtilizadorCreateViewModel model)
        {
            if (id != model.Utilizador.UtilizadorId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Utilizador.Tipo == "Funcionario") model.Utilizador.ClienteId = null;
                    else if (model.Utilizador.Tipo == "Cliente") model.Utilizador.FuncionarioId = null;
                    else { model.Utilizador.ClienteId = null; model.Utilizador.FuncionarioId = null; }

                    _context.Update(model.Utilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Utilizadores.Any(u => u.UtilizadorId == model.Utilizador.UtilizadorId)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            model.Funcionarios = _context.Funcionarios.ToList();
            model.Clientes = _context.Clientes.ToList();

            var errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new {
                    Field = ms.Key,
                    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                });

            foreach (var err in errors)
            {
                Console.WriteLine($"Campo: {err.Field}");
                foreach (var e in err.Errors)
                {
                    Console.WriteLine($"  Erro: {e}");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Mostra o formulário de confirmação para eliminar um utilizador.
        /// </summary>
        /// <param name="id">Identificador do utilizador a eliminar.</param>
        /// <returns>View de confirmação ou NotFound se não existir.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);

            if (utilizador == null) return NotFound();

            return View(utilizador);
        }

        /// <summary>
        /// Elimina o utilizador após confirmação.
        /// </summary>
        /// <param name="id">Identificador do utilizador a eliminar.</param>
        /// <returns>Redireciona para a lista de utilizadores.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador != null) _context.Utilizadores.Remove(utilizador);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um utilizador existe na base de dados.
        /// </summary>
        /// <param name="id">Identificador do utilizador.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadores.Any(e => e.UtilizadorId == id);
        }
    }
}

