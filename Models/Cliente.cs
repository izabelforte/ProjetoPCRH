using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }
        [Required]
        [DisplayName("Nome do Cliente")]
        public string NomeCliente { get; set; }
        [Required]
        [DisplayName("NIF")]
        public string Nif {  get; set; }
        [DisplayName("Morada")]
        public string Morada { get; set; }
        [Required]
        [DisplayName("E-mail")]
        public string Email { get; set; }
        
    }
}
