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

        /// NOVO MÉTODO: MeusProjetos
        [AuthorizeRole("Funcionario")]
        public async Task<IActionResult> MeusProjetos()
        {
            // Pegar o ID do utilizador logado
            var userId = HttpContext.Session.GetInt32("UtilizadorId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Buscar o utilizador e verificar se tem FuncionarioId
            var utilizador = await _context.Utilizadores
                .Include(u => u.Funcionario)
                    .ThenInclude(f => f.FuncionarioProjetos)
                        .ThenInclude(fp => fp.Projeto)
                .FirstOrDefaultAsync(u => u.UtilizadorId == userId);

            if (utilizador?.Funcionario == null)
            {
                return NotFound("Este utilizador não está associado a nenhum funcionário.");
            }

            // Pegar os projetos associados
            var projetos = utilizador.Funcionario.FuncionarioProjetos
                .Select(fp => fp.Projeto)
                .ToList();

            return View(projetos);
        }


        // GET: Projetos/Details/5
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



        // GET: Projetos/Create
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email");

            
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios.Where(f => f.Ativo), 
                "FuncionarioId",
                "NomeFuncionario"
            );

            ViewBag.StatusProjeto = new SelectList(new List<string> { "Planeado", "Em andamento" });
            return View();
        }

      

        // POST: Projetos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjetoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Criar projeto
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

                // Associar funcionários na tabela de junção
                if (model.FuncionariosSelecionados != null)
                {
                    foreach (var funcId in model.FuncionariosSelecionados)
                    {
                        var relacao = new FuncionarioProjeto
                        {
                            ProjetoId = projeto.ProjetoId, 
                            FuncionarioId = funcId
                        };
                        _context.FuncionarioProjetos.Add(relacao);
                    }

                    await _context.SaveChangesAsync(); 
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", model.ClienteId);
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "FuncionarioId", "NomeFuncionario");
            return View(model);
        }



        // GET: Projetos/Edit/5
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


        // POST: Projetos/Edit/5
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

            return RedirectToAction("TerminarProjeto", "Relatorios", new { id = projeto.ProjetoId });
        }


        private bool ProjetoExists(int id)
        {
            return _context.Projetos.Any(e => e.ProjetoId == id);
        }
    }
}
