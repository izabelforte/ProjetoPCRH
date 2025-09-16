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
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }
        // GET: /Relatorios/RelatorioProjetoTerminado/5
        public async Task<IActionResult> RelatorioProjetoTerminado(int id)
        {
            var relatorio = await _context.Relatorios
                .Include(r => r.Projeto) // para aceder aos dados do projeto
                .FirstOrDefaultAsync(r => r.RelatorioId == id);

            if (relatorio == null)
            {
                return NotFound();
            }

            // Passa um único Relatorio para a view
            return View(relatorio);
        }

        

        // GET: Relatorios
        [AuthorizeRole("Administrador", "GestorProjeto")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Relatorios.Include(r => r.Projeto);
            return View(await appDbContext.ToListAsync());
        }
    }
}

