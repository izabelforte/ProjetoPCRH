using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Funcionarios")]
    public class Funcionario
    {
        [Key]
        public int FuncionarioId { get; set; }
        [DisplayName("ID")]
        [Required]
        public string NomeFuncionario { get; set; }
        [DisplayName("Nome do Funcionário")]
        [Required]
        public string Nif { get; set; }
        [DisplayName("NIF")]

        public string Cargo { get; set; }
        [DisplayName("Cargo")]

        public string Email { get; set; }
        [DisplayName("E-mail")]
        [Column(TypeName = "date")]  // para guardar so a data sem horas
        public DateTime DataAdmissao { get; set; }
        [DisplayName("Data de Admissão")]

        public bool Ativo { get; set; }  //O EF vai mapear para bit no SQL Server (que armazena 0 ou 1).
    }
}
