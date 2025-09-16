using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    [Table("Relatorios")]
    public class Relatorio
    {
        [Key]
        public int RelatorioId { get; set; }

        [DisplayName("Data")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataRelatorio { get; set; }

        [DisplayName("Valor")]
        public double Valor { get; set; }
        
        public int TempoTotalHoras { get; set; }

        [ForeignKey("ProjetoId")]
        [ValidateNever]
        public int ProjetoId { get; set; }
        [ValidateNever]
        public Projeto Projeto { get; set; }
    }
}
