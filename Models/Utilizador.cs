using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    [Table("Utilizadores")]
    public class Utilizador
    {
        [Key]
        public int UtilizadorId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Tipo { get; set; }

        public int? FuncionarioId { get; set; }
        [ValidateNever]
        public virtual Funcionario Funcionario { get; set; }

        public int? ClienteId { get; set; }
        [ValidateNever]
        public virtual Cliente Cliente { get; set; }
    }
}
