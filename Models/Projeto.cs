using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Projetos")]
    public class Projeto
    {
        [Key]
        public int ProjetoId { get; set; }
        [DisplayName("ID")]
        [Required]
        public string NomeProjeto { get; set; }
        [DisplayName("Nome do Projeto")]
        public string Descricao { get; set; }
        [DisplayName("Descrição")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataInicio { get; set; }
        [DisplayName("Data de Início")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataFim { get; set; }
        [DisplayName("Data de Fim")]

        public double Orcamento { get; set; }
        [DisplayName("Orçamento")]
        public string StatusProjeto { get; set; }
        [DisplayName("Estado do Projeto")]

        [ForeignKey("ClienteId")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
