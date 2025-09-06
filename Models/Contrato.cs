using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Contratos")]
    public class Contrato
    {
        [Key]
        public int ContratoId { get; set; }
        [DisplayName("ID")]

        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataInicio { get; set; }
        [DisplayName("Data de Início")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFim { get; set; }
        [DisplayName("Data de Fim")]

        public double Valor { get; set; }
        [DisplayName("Valor")]
        public string StatusContrato { get; set; }
        [DisplayName("Estado do Contrato")]


        [ForeignKey("ClienteId")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }


        [ForeignKey("ProjetoId")]
        public int ProjetoId { get; set; }
        public Projeto Projeto { get; set; }
    }
}
