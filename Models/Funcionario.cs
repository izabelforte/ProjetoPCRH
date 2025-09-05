using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Funcionarios")]
    public class Funcionario
    {
        [Key]
        public int FuncionarioId { get; set; }
        [Required]
        public string NomeFuncionario { get; set; }
        [Required]
        public string Nif { get; set; }

        public string Cargo { get; set; }

        public string Email { get; set; }
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataAdmissao { get; set; }

        public bool Ativo { get; set; }  //O EF vai mapear para bit no SQL Server (que armazena 0 ou 1).
    }
}
