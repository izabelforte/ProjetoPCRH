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

        [DisplayName("Data de relatório")]
        public DateTime DataRelatorio { get; set; }

        [DisplayName("Gastos reais")]
        public double Valor { get; set; }
        
        [DisplayName("Horas gastas")]
        public int TempoTotalHoras { get; set; }

        public int ProjetoId { get; set; }
        [ValidateNever]
        public Projeto Projeto { get; set; }
    }
}
