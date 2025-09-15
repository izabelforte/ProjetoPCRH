using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace ProjetoPCRH.Models.ViewModels
{
    public class UtilizadorCreateViewModel
    {
        // O Utilizador que vamos criar
        public Utilizador Utilizador { get; set; }

        // Listas de apoio para popular os dropdowns
        [ValidateNever]
        public IEnumerable<Funcionario> Funcionarios { get; set; }
        [ValidateNever]
        public IEnumerable<Cliente> Clientes { get; set; }
       
    }
}
