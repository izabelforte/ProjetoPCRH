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

        /// <summary>
        /// Exibe os detalhes de um cliente específico.
        /// Primeiro obtém os dados básicos do cliente através de uma stored procedure (sp_ObterClientePorId).
        /// Em seguida, carrega os contratos do cliente e, para cada contrato, as suas faturações.
        /// </summary>
        /// <param name="id">Identificador do cliente a ser pesquisado.</param>
        /// <returns>
        /// Retorna a view com as informações completas do cliente,
        /// incluindo contratos e faturações, ou NotFound se o cliente não existir.
        /// </returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Utiliza FromSqlRaw para executar "sp_ObterClientePorId" e obter os dados básicos
            var cliente = (await _context.Clientes
                .FromSqlRaw("EXEC sp_ObterClientePorId @p0", id) // @p0 é substituído pelo parâmetro 'id'
                .ToListAsync())
                .FirstOrDefault();

            // Se a SP não encontrar nenhum cliente, retorna 404
            if (cliente == null) return NotFound();

            // Aqui não foi usado SP, apenas LINQ normal para incluir as faturações
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
        /// <summary>
        /// Cria um novo cliente na base de dados.
        /// Os dados são validados pelo ModelState e inseridos através da stored procedure "sp_InserirCliente".
        /// </summary>
        /// <param name="cliente">
        /// Objeto <see cref="Cliente"/> contendo Nome, NIF, Morada e Email informados no formulário.
        /// </param>
        /// <returns>
        /// Redireciona para a lista de clientes (Index) em caso de sucesso,
        /// ou retorna a mesma view com mensagens de erro se ocorrer falha na validação ou inserção.
        /// </returns>
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
        /// <summary>
        /// GET: Edit
        /// Obtém um cliente específico para edição, usando a stored procedure
        /// <c>sp_ObterClientePorId</c>.
        /// </summary>
        /// <param name="id">ID do cliente a ser editado.</param>
        /// <returns>Retorna a view de edição com os dados do cliente, 
        /// ou NotFound se não for encontrado.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Busca o cliente através da stored procedure.
            var cliente = (await _context.Clientes
                .FromSqlRaw("EXEC sp_ObterClientePorId @p0", id)
                .ToListAsync())
                .FirstOrDefault();

            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Edit
        /// <summary>
        /// POST: Edit
        /// Atualiza os dados de um cliente existente utilizando a stored procedure
        /// <c>sp_AtualizarCliente</c>.
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado.</param>
        /// <param name="cliente">Objeto cliente com os dados submetidos no formulário.</param>
        /// <returns>
        /// Redireciona para o Index se a atualização for bem-sucedida,
        /// ou retorna a view com mensagens de erro em caso de falha.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClienteId,NomeCliente,Nif,Morada,Email")] Cliente cliente)
        {
            // Verifica se o ID da rota corresponde ao ID do cliente do formulário.
            if (id != cliente.ClienteId) return NotFound();

            try
            {
                // Chama a stored procedure para atualizar o cliente na base de dados.
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_AtualizarCliente @p0, @p1, @p2, @p3, @p4",
                    cliente.ClienteId, cliente.NomeCliente, cliente.Nif, cliente.Morada ?? "", cliente.Email);

                return RedirectToAction(nameof(Index));// Redireciona para a lista de clientes.
            }
            catch (Exception ex)
            {
                // Caso ocorra erro, adiciona mensagem ao ModelState.
                ModelState.AddModelError("", "Erro ao atualizar cliente: " + ex.Message);
                return View(cliente);
            }
        }

        // GET: Delete
        /// <summary>
        /// GET: Delete
        /// Obtém os dados de um cliente para exibição antes da exclusão,
        /// utilizando a stored procedure <c>sp_ObterClientePorId</c>.
        /// </summary>
        /// <param name="id">ID do cliente a ser eliminado.</param>
        /// <returns>Retorna a view de confirmação de exclusão ou NotFound se não existir.</returns>
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
        /// <summary>
        /// POST: DeleteConfirmed
        /// Elimina definitivamente um cliente da base de dados
        /// através da stored procedure <c>sp_EliminarCliente</c>.
        /// </summary>
        /// <param name="id">ID do cliente a eliminar.</param>
        /// <returns>Redireciona para o Index após a eliminação.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Executa a stored procedure para apagar o cliente.
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
