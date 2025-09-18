using System.Security.Policy;

namespace ProjetoPCRH.Models.ViewModels
{
    public class ProjetoCreateViewModel
    {
        public int ProjetoId { get; set; }
        public string NomeProjeto { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public double Orcamento { get; set; }
        public string StatusProjeto { get; set; }
        public int ClienteId { get; set; }

        // IDs dos funcionários escolhidos
        public List<int> FuncionariosSelecionados { get; set; } = new List<int>();
    }
}
