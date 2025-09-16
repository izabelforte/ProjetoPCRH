using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    [Table("Contratos")]
    public class Contrato
    {
        [Key]
        public int ContratoId { get; set; }

        [DisplayName("Data de Início")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataInicio { get; set; }

        [DisplayName("Data de Fim")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFim { get; set; }

        [DisplayName("Valor")]
        public double Valor { get; set; }

        [DisplayName("Estado do Contrato")]
        public string StatusContrato { get; set; }
        

        [ForeignKey("ClienteId")]
        [ValidateNever]
        public int ClienteId { get; set; }
        [ValidateNever]
        public Cliente Cliente { get; set; } 


        [ForeignKey("ProjetoId")]
        [ValidateNever]
        public int ProjetoId { get; set; }
        [ValidateNever]
        public Projeto Projeto { get; set; }
        [ValidateNever]
        public virtual ICollection<Faturacao> Faturacoes { get; set; }
        
    }
}
