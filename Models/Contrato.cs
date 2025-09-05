using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Contratos")]
    public class Contrato
    {
        [Key]
        public int ContratoId { get; set; }

        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataInicio { get; set; }
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFim { get; set; }

        public double Valor { get; set; }
        public string StatusContrato { get; set; }


        [ForeignKey("ClienteId")]
        public int ClienteId { get; set; }
        public Cliente cliente { get; set; }


        [ForeignKey("ProjetoId")]
        public int ProjetoId { get; set; }
        public Projeto projeto { get; set; }
    }
}
