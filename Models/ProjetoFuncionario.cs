using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoPCRH.Models
{
    [Table("ProjetosFuncionarios")]
    public class ProjetoFuncionario
    {

        //configurar a chave composta no OnModelCreating no DbContext
        public int ProjetoId { get; set; }
        public int FuncionarioId { get; set; }

       
        public Projeto Projeto { get; set; }
        public Funcionario Funcionario { get; set; }
    }
}
