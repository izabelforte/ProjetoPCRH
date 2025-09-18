using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    [Table("Projetos")]
    public class Projeto
    {
        [Key]
        public int ProjetoId { get; set; }
        
        [Required]
        [DisplayName("Nome do Projeto")]
        public string NomeProjeto { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Data de Início")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataInicio { get; set; }

        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFim { get; set; }

        [DisplayName("Orçamento")]
        public double Orcamento { get; set; }
        [DisplayName("Estado do Projeto")]
        public string StatusProjeto { get; set; }
       

        [ForeignKey("ClienteId")]
        [ValidateNever]
        public int ClienteId { get; set; }
        [ValidateNever]
        public Cliente Cliente { get; set; }
        [ValidateNever]
        public ICollection<Funcionario> Funcionarios { get; set; }
    }
}
