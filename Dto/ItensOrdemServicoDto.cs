namespace TesteMVC.Dto
{
    namespace TesteMVC.Dto
    {
        public class ItensOrdemServicoDto
        {
            public int idOrdemServico { get; set; }
            public int idItemOrdemServico { get; set; }
            public int idProduto { get; set; }
            public string nomeProduto { get; set; }
            public string quantidade { get; set; }
            public string DescricaoServico { get; set; }
            public decimal valorUnitario { get; set; }
        }
    }
}
