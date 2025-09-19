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
    /// Controller responsável pela gestão de projetos.
    /// Controla ações como listar, criar, editar, eliminar, consultar e terminar projetos.
    /// O acesso é restrito a Administrador e GestorProjeto, exceto para o método MeusProjetos.
    /// </summary>
    public class ProjetosController : Controller
    {
        /// <summary>
        /// Coleção de funcionários associada (utilizada em Views).
        /// </summary>
        public ICollection<Funcionario> Funcionarios { get; set; }

        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa o controller de projetos com o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da aplicação.</param>
        public ProjetosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os projetos, incluindo informação do cliente.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <returns>View com a lista de projetos.</returns>
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Projetos.Include(p => p.Cliente);
            return View(await appDbContext.ToListAsync());
        }

        /// <summary>
        /// Lista os projetos associados ao funcionário autenticado.
        /// Apenas acessível a utilizadores com papel de Funcionário.
        /// </summary>
        /// <returns>View com os projetos do funcionário autenticado ou redireciona para login se não houver sessão.</returns>
        [AuthorizeRole("Funcionario")]
        public async Task<IActionResult> MeusProjetos()
        {
            var userId = HttpContext.Session.GetInt32("UtilizadorId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var utilizador = await _context.Utilizadores
                .Include(u => u.Funcionario)
                    .ThenInclude(f => f.FuncionarioProjetos)
                        .ThenInclude(fp => fp.Projeto)
                .FirstOrDefaultAsync(u => u.UtilizadorId == userId);

            if (utilizador?.Funcionario == null)
            {
                return NotFound("Este utilizador não está associado a nenhum funcionário.");
            }

            var projetos = utilizador.Funcionario.FuncionarioProjetos
                .Select(fp => fp.Projeto)
                .ToList();

            return View(projetos);
        }

        /// <summary>
        /// Mostra os detalhes de um projeto específico.
        /// Inclui informações do cliente e funcionários associados.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <returns>View com detalhes do projeto ou NotFound se não existir.</returns>
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var projeto = await _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.FuncionarioProjetos)
                    .ThenInclude(fp => fp.Funcionario)
                .FirstOrDefaultAsync(m => m.ProjetoId == id);

            if (projeto == null)
                return NotFound();

            return View(projeto);
        }

        /// <summary>
        /// Exibe o formulário para criar um novo projeto.
        /// Inclui listas de clientes, funcionários ativos e status.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <returns>View para criação de projeto.</returns>
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email");
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios.Where(f => f.Ativo), "FuncionarioId", "NomeFuncionario");
            ViewBag.StatusProjeto = new SelectList(new List<string> { "Planeado", "Em andamento" });
            return View();
        }

        /// <summary>
        /// Cria um novo projeto e associa funcionários selecionados.
        /// </summary>
        /// <param name="model">ViewModel com dados do projeto e funcionários selecionados.</param>
        /// <returns>Redireciona para Index se bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjetoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var projeto = new Projeto
                {
                    NomeProjeto = model.NomeProjeto,
                    Descricao = model.Descricao,
                    DataInicio = model.DataInicio,
                    DataFim = model.DataFim,
                    Orcamento = model.Orcamento,
                    StatusProjeto = model.StatusProjeto,
                    ClienteId = model.ClienteId
                };

                _context.Projetos.Add(projeto);
                await _context.SaveChangesAsync();

                if (model.FuncionariosSelecionados != null)
                {
                    foreach (var funcId in model.FuncionariosSelecionados)
                    {
                        _context.FuncionarioProjetos.Add(new FuncionarioProjeto
                        {
                            ProjetoId = projeto.ProjetoId,
                            FuncionarioId = funcId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", model.ClienteId);
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "FuncionarioId", "NomeFuncionario");
            return View(model);
        }

        /// <summary>
        /// Exibe o formulário de edição de um projeto existente, incluindo funcionários associados.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <returns>View para edição ou NotFound se não existir.</returns>
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var projeto = await _context.Projetos
                .Include(p => p.FuncionarioProjetos)
                    .ThenInclude(fp => fp.Funcionario)
                .FirstOrDefaultAsync(p => p.ProjetoId == id);

            if (projeto == null) return NotFound();

            var vm = new ProjetoCreateViewModel
            {
                ProjetoId = projeto.ProjetoId,
                NomeProjeto = projeto.NomeProjeto,
                Descricao = projeto.Descricao,
                DataInicio = projeto.DataInicio,
                DataFim = projeto.DataFim,
                Orcamento = projeto.Orcamento,
                StatusProjeto = projeto.StatusProjeto,
                ClienteId = projeto.ClienteId,
                FuncionariosSelecionados = projeto.FuncionarioProjetos?.Select(fp => fp.FuncionarioId).ToList()
            };

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", projeto.ClienteId);
            ViewBag.Funcionarios = new MultiSelectList(_context.Funcionarios, "FuncionarioId", "NomeFuncionario", vm.FuncionariosSelecionados);

            return View(vm);
        }

        /// <summary>
        /// Atualiza os dados de um projeto existente e suas associações de funcionários.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <param name="model">ViewModel com dados atualizados do projeto.</param>
        /// <returns>Redireciona para Index se bem-sucedido, ou retorna view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Edit(int id, ProjetoCreateViewModel model)
        {
            if (id != model.ProjetoId) return NotFound();

            if (ModelState.IsValid)
            {
                var projeto = await _context.Projetos
                    .Include(p => p.FuncionarioProjetos)
                    .FirstOrDefaultAsync(p => p.ProjetoId == id);

                if (projeto == null) return NotFound();

                projeto.NomeProjeto = model.NomeProjeto;
                projeto.Descricao = model.Descricao;
                projeto.DataInicio = model.DataInicio;
                projeto.DataFim = model.DataFim;
                projeto.Orcamento = model.Orcamento;
                projeto.StatusProjeto = model.StatusProjeto;
                projeto.ClienteId = model.ClienteId;

                projeto.FuncionarioProjetos.Clear();
                if (model.FuncionariosSelecionados != null)
                {
                    foreach (var funcId in model.FuncionariosSelecionados)
                    {
                        projeto.FuncionarioProjetos.Add(new FuncionarioProjeto
                        {
                            ProjetoId = projeto.ProjetoId,
                            FuncionarioId = funcId
                        });
                    }
                }

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

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", model.ClienteId);
            ViewBag.Funcionarios = new MultiSelectList(_context.Funcionarios, "FuncionarioId", "NomeFuncionario", model.FuncionariosSelecionados);

            return View(model);
        }

        /// <summary>
        /// Exibe a confirmação de exclusão de um projeto.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <returns>View de confirmação ou NotFound se não existir.</returns>
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

        /// <summary>
        /// Executa a exclusão de um projeto existente.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <param name="id">Identificador do projeto a ser removido.</param>
        /// <returns>Redireciona para a lista de projetos após exclusão.</returns>
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

        /// <summary>
        /// Marca um projeto como terminado.
        /// Apenas acessível por Administrador e Gestor de Projeto.
        /// </summary>
        /// <param name="id">Identificador do projeto a ser terminado.</param>
        /// <returns>Redireciona para a página de relatório do projeto terminado.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Terminar(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null) return NotFound();

            projeto.StatusProjeto = "Terminado";
            _context.Update(projeto);
            await _context.SaveChangesAsync();

            return RedirectToAction("TerminarProjeto", "Relatorios", new { id = projeto.ProjetoId });
        }

        /// <summary>
        /// Verifica se um projeto existe na base de dados pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do projeto.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        private bool ProjetoExists(int id)
        {
            return _context.Projetos.Any(e => e.ProjetoId == id);
        }
    }
}

