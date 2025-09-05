using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Projetos")]
    public class Projeto
    {
        [Key]
        public int ProjetoId { get; set; }
        [Required]
        public string ProjetoName { get; set; }
        public string Descricao { get; set; }
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataInicio { get; set; }
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFim { get; set; }

        public double Orcamento { get; set; }
        public string StatusProjeto { get; set; }

        [ForeignKey("ClienteId")]
        public int ClienteId { get; set; }
        public Cliente cliente { get; set; }
    }
}
