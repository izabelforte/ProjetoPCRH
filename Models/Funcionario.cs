using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    [Table("Funcionarios")]
    public class Funcionario
    {
        [Key]
        public int FuncionarioId { get; set; }
      
        [Required]
        [DisplayName("Nome do Funcionário")]
        public string NomeFuncionario { get; set; }
      
        [Required]
        [DisplayName("NIF")]

        public string Nif { get; set; }

        [DisplayName("Cargo")]
        public string Cargo { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Data de Admissão")]
        [Column(TypeName = "date")]  
        public DateTime DataAdmissao { get; set; }

       


        public bool Ativo { get; set; } 
        [ValidateNever]
        public ICollection<Projeto> Projetos { get; set; }
        [ValidateNever]
        public ICollection<FuncionarioProjeto> FuncionarioProjetos { get; set; }
    }

}
