    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoPCRH.Models;

namespace ProjetoPCRH.Controllers
{
    [AuthorizeRole("Administrador")]
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .FromSqlRaw("EXEC sp_ListarClientes")
                .ToListAsync();
            return View(clientes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Buscar cliente pela SP
            var cliente = (await _context.Clientes
                .FromSqlRaw("EXEC sp_ObterClientePorId @p0", id)
                .ToListAsync())
                .FirstOrDefault();

            if (cliente == null) return NotFound();

            // Carregar contratos e faturações normalmente
            cliente.Contratos = await _context.Contratos
                .Where(c => c.ClienteId == cliente.ClienteId)
                .Include(c => c.Faturacoes)
                .ToListAsync();

            return View(cliente);
        }


        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeCliente,Nif,Morada,Email")] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                    foreach (var error in item.Value.Errors)
                        Console.WriteLine($"{item.Key}: {error.ErrorMessage}");
                return View(cliente);
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_InserirCliente @p0, @p1, @p2, @p3",
                    cliente.NomeCliente, cliente.Nif, cliente.Morada ?? "", cliente.Email);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao criar cliente: " + ex.Message);
                return View(cliente);
            }
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = (await _context.Clientes
                .FromSqlRaw("EXEC sp_ObterClientePorId @p0", id)
                .ToListAsync())
                .FirstOrDefault();

            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClienteId,NomeCliente,Nif,Morada,Email")] Cliente cliente)
        {
            if (id != cliente.ClienteId) return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                    foreach (var error in item.Value.Errors)
                        Console.WriteLine($"{item.Key}: {error.ErrorMessage}");
                return View(cliente);
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_AtualizarCliente @p0, @p1, @p2, @p3, @p4",
                    cliente.ClienteId, cliente.NomeCliente, cliente.Nif, cliente.Morada ?? "", cliente.Email);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao atualizar cliente: " + ex.Message);
                return View(cliente);
            }
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = (await _context.Clientes
                .FromSqlRaw("EXEC sp_ObterClientePorId @p0", id)
                .ToListAsync())
                .FirstOrDefault();

            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_EliminarCliente @p0", id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao eliminar cliente: " + ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
