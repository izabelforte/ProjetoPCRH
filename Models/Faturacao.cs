using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Faturas")]
    public class Faturacao
    {
        [Key]
        public int FaturaId { get; set; }
        [DisplayName("ID")]

        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFatura { get; set; }
        [DisplayName("Data da Fatura")]

        public double Valor { get; set; }
        [DisplayName("Valor")]

        [ForeignKey("ContratoId")]
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }

    }
}
