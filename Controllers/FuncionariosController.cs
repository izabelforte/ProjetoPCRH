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
    /// Controller responsável pela gestão de funcionários.
    /// Apenas utilizadores com o papel de Administrador podem aceder.
    /// </summary>
    [AuthorizeRole("Administrador")]
    public class FuncionariosController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa o controller de funcionários com o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da aplicação.</param>
        public FuncionariosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os funcionários existentes.
        /// </summary>
        /// <returns>View com a lista de funcionários.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Funcionarios.ToListAsync());
        }

        /// <summary>
        /// Mostra os detalhes de um funcionário específico, incluindo os projetos associados.
        /// </summary>
        /// <param name="id">Identificador do funcionário.</param>
        /// <returns>View com os detalhes do funcionário ou NotFound se não existir.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios
                .Include(f => f.FuncionarioProjetos)
                    .ThenInclude(fp => fp.Projeto)
                .FirstOrDefaultAsync(m => m.FuncionarioId == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        /// <summary>
        /// Exibe o formulário para criar um novo funcionário.
        /// </summary>
        /// <returns>View para criação de funcionário.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Cria um novo funcionário e guarda-o na base de dados.
        /// </summary>
        /// <param name="funcionario">Objeto funcionário com os dados preenchidos.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FuncionarioId,NomeFuncionario,Nif,Cargo,Email,DataAdmissao,Ativo")] Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funcionario);
        }

        /// <summary>
        /// Exibe o formulário de edição para um funcionário específico.
        /// </summary>
        /// <param name="id">Identificador do funcionário.</param>
        /// <returns>View para edição do funcionário ou NotFound se não existir.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }
            return View(funcionario);
        }

        /// <summary>
        /// Atualiza os dados de um funcionário existente.
        /// </summary>
        /// <param name="id">Identificador do funcionário.</param>
        /// <param name="funcionario">Objeto funcionário atualizado.</param>
        /// <returns>Redireciona para Index se for bem-sucedido, caso contrário retorna a mesma view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FuncionarioId,NomeFuncionario,Nif,Cargo,Email,DataAdmissao,Ativo")] Funcionario funcionario)
        {
            if (id != funcionario.FuncionarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 🔹 Verificação antes de salvar
                if (!funcionario.Ativo)
                {
                    var temProjetos = await _context.FuncionarioProjetos
                        .Include(fp => fp.Projeto)
                        .AnyAsync(fp => fp.FuncionarioId == funcionario.FuncionarioId
                                        && fp.Projeto.StatusProjeto == "Em andamento");

                    if (temProjetos)
                    {
                        ModelState.AddModelError("", "Este funcionário está em projetos em andamento e não pode ser desativado.");
                        return View(funcionario);
                    }
                }

                try
                {
                    _context.Update(funcionario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionarioExists(funcionario.FuncionarioId))
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
            return View(funcionario);
        }

        /// <summary>
        /// Verifica se o funcionário possui projetos em andamento.
        /// </summary>
        /// <param name="id">Identificador do funcionário.</param>
        /// <returns>JSON indicando se o funcionário possui projetos em andamento.</returns>
        [HttpGet]
        public async Task<IActionResult> VerificarProjetosEmAndamento(int id)
        {
            var temProjetos = await _context.FuncionarioProjetos
                .Include(fp => fp.Projeto)
                .AnyAsync(fp => fp.FuncionarioId == id && fp.Projeto.StatusProjeto == "Em Andamento");

            return Json(new { temProjetos });
        }

        /// <summary>
        /// Verifica se um funcionário existe na base de dados.
        /// </summary>
        /// <param name="id">Identificador do funcionário.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.FuncionarioId == id);
        }

        /// <summary>
        /// Exibe os projetos associados ao funcionário autenticado.
        /// Apenas disponível para utilizadores com papel de Funcionário.
        /// </summary>
        /// <returns>View com os projetos do funcionário autenticado.</returns>
        [AuthorizeRole("Funcionario")]
        public async Task<IActionResult> MeusProjetos()
        {
            // Username do funcionário autenticado
            var username = User.Identity.Name;

            // Buscar projetos que tenham esse funcionário
            var meusProjetos = await _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.Funcionarios)
                .Where(p => p.Funcionarios.Any(f => f.NomeFuncionario == username))
                .ToListAsync();

            return View(meusProjetos);
        }
    }
}

