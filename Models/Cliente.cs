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
        public string NomeCliente { get; set; }
        [Required]
        public string Nif {  get; set; }

        public string Morada { get; set; }

        public string Email { get; set; }
        
    }
}
