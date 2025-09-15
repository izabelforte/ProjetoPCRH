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
    public class UtilizadoresController : Controller
    {
        private readonly AppDbContext _context;

        public UtilizadoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Utilizadors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizadores.ToListAsync());
        }

        // GET: Utilizadors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        //get: utilizadors/create
        //public iactionresult create()
        //{
        //    return view();
        //}

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


        // POST: Utilizadors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UtilizadorId,Username,Password,Tipo")] Utilizador utilizador)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(utilizador);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(utilizador);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtilizadorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Utilizador.Tipo == "Funcionario")
                {
                    model.Utilizador.ClienteId = null;
                }
                else if (model.Utilizador.Tipo == "Cliente")
                {
                    model.Utilizador.FuncionarioId = null;
                }
                else
                {
                    model.Utilizador.ClienteId = null;
                    model.Utilizador.FuncionarioId = null;
                }

                _context.Add(model.Utilizador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        //// GET: Utilizadors/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var utilizador = await _context.Utilizadores.FindAsync(id);
        //    if (utilizador == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(utilizador);
        //}

        //// POST: Utilizadors/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("UtilizadorId,Username,Password,Tipo")] Utilizador utilizador)
        //{
        //    if (id != utilizador.UtilizadorId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(utilizador);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UtilizadorExists(utilizador.UtilizadorId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(utilizador);
        //}

        // GET: Utilizadors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .Include(u => u.Funcionario)
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.UtilizadorId == id);

            if (utilizador == null)
            {
                return NotFound();
            }

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

        // POST: Utilizadors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtilizadorCreateViewModel model)
        {
            if (id != model.Utilizador.UtilizadorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Utilizador.Tipo == "Funcionario")
                    {
                        model.Utilizador.ClienteId = null;
                    }
                    else if (model.Utilizador.Tipo == "Cliente")
                    {
                        model.Utilizador.FuncionarioId = null;
                    }
                    else
                    {
                        model.Utilizador.ClienteId = null;
                        model.Utilizador.FuncionarioId = null;
                    }

                    _context.Update(model.Utilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Utilizadores.Any(u => u.UtilizadorId == model.Utilizador.UtilizadorId))
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

            // Se falhar a validação, volta a carregar as listas
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


        // GET: Utilizadors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.UtilizadorId == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // POST: Utilizadors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador != null)
            {
                _context.Utilizadores.Remove(utilizador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadores.Any(e => e.UtilizadorId == id);
        }
    }
}
