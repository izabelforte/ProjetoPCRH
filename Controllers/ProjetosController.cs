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

        // NOVO MÉTODO: MeusProjetos
        [AuthorizeRole("Funcionario")]
        public async Task<IActionResult> MeusProjetos()
        {
            // 1. Pegar o ID do utilizador logado
            var userId = HttpContext.Session.GetInt32("UtilizadorId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Buscar o utilizador e verificar se tem FuncionarioId
            var utilizador = await _context.Utilizadores
                .Include(u => u.Funcionario)
                    .ThenInclude(f => f.FuncionarioProjetos)
                        .ThenInclude(fp => fp.Projeto)
                .FirstOrDefaultAsync(u => u.UtilizadorId == userId);

            if (utilizador?.Funcionario == null)
            {
                return NotFound("Este utilizador não está associado a nenhum funcionário.");
            }

            // 3. Pegar os projetos associados
            var projetos = utilizador.Funcionario.FuncionarioProjetos
                .Select(fp => fp.Projeto)
                .ToList();

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
           
            // Funcionários (todos)
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "FuncionarioId", "NomeFuncionario");

            ViewBag.StatusProjeto = new SelectList(new List<string> { "Planeado", "Em andamento" });
            return View();
        }

        //// POST: Projetos/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizeRole("Administrador", "GestorProjeto")]
        //public async Task<IActionResult> Create([Bind("ProjetoId,NomeProjeto,Descricao,DataInicio,DataFim,Orcamento,StatusProjeto,ClienteId")] Projeto projeto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(projeto);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", projeto.ClienteId);
        //    return View(projeto);
        //}

        // POST: Projetos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjetoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1️⃣ Criar projeto
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
                await _context.SaveChangesAsync(); // Salvar primeiro para gerar ProjetoId

                // 2️⃣ Associar funcionários na tabela de junção
                if (model.FuncionariosSelecionados != null)
                {
                    foreach (var funcId in model.FuncionariosSelecionados)
                    {
                        var relacao = new FuncionarioProjeto
                        {
                            ProjetoId = projeto.ProjetoId, // agora já existe
                            FuncionarioId = funcId
                        };
                        _context.FuncionarioProjetos.Add(relacao);
                    }

                    await _context.SaveChangesAsync(); // Salvar as relações
                }

                return RedirectToAction(nameof(Index));
            }

            // Se deu erro de validação, recarregar selects
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Email", model.ClienteId);
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "FuncionarioId", "NomeFuncionario");
            return View(model);
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

           

            // Redirecionar para a view RelatorioProjetoTerminado
            // Certifica-te que a view espera um único Relatorio
            return RedirectToAction("TerminarProjeto", "Relatorios", new { id = projeto.ProjetoId });
        }


        private bool ProjetoExists(int id)
        {
            return _context.Projetos.Any(e => e.ProjetoId == id);
        }
    }
}
