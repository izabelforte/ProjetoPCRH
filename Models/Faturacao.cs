using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjetoPCRH.Models
{
    [Table("Faturas")]
    public class Faturacao
    {
        [Key]
        public int FaturaId { get; set; }
      
        [DisplayName("Data da Fatura")]
        [Column(TypeName = "date")]  
        public DateTime DataFatura { get; set; }

        [DisplayName("Valor")]
        public double Valor { get; set; }

        

        [ForeignKey("ContratoId")]
        [ValidateNever]
        public int ContratoId { get; set; }
        [ValidateNever]
        public Contrato Contrato { get; set; }

    }
}
