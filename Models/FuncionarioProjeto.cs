using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    public class FuncionarioProjeto
    {
        [Column("FuncionariosFuncionarioId")]
        public int FuncionarioId { get; set; }

        [Column("ProjetosProjetoId")]
        public int ProjetoId { get; set; }

        [ValidateNever]
        public Funcionario Funcionario { get; set; }
        [ValidateNever]
        public Projeto Projeto { get; set; }
    }
}
