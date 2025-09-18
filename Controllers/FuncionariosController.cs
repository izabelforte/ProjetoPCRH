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
    [AuthorizeRole("Administrador")]
    public class FuncionariosController : Controller
    {
        private readonly AppDbContext _context;

        public FuncionariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Funcionarios.ToListAsync());
        }

        // GET: Funcionarios/Details/5
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

        // GET: Funcionarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Funcionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Funcionarios/Edit/5
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

        // POST: Funcionarios/Edit/5
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


        [HttpGet]
        public async Task<IActionResult> VerificarProjetosEmAndamento(int id)
        {
            var temProjetos = await _context.FuncionarioProjetos
                .Include(fp => fp.Projeto)
                .AnyAsync(fp => fp.FuncionarioId == id && fp.Projeto.StatusProjeto == "Em Andamento");

            return Json(new { temProjetos });
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionarios.Any(e => e.FuncionarioId == id);
        }


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
