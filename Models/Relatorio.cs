using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Relatorios")]
    public class Relatorio
    {
        [Key]
        public int RelatorioId { get; set; }
        [DisplayName("ID")]

        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataRelatorio { get; set; }
        [DisplayName("Data")]

        public double Valor { get; set; }
        [DisplayName("Valor")]
        public int TempoTotalHoras { get; set; }

        [ForeignKey("ProjetoId")]
        public int ProjetoId { get; set; }
        public Projeto Projeto { get; set; }
    }
}
