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
        [ValidateNever]
        public int RelatorioId { get; set; }

        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataRelatorio { get; set; }

        public double Valor { get; set; }
        
        public int TempoTotalHoras { get; set; }

        public int ProjetoId { get; set; }
        [ValidateNever]
        public Projeto Projeto { get; set; }
    }
}
