namespace TesteMVC.Dto
{
    public class OrdemServicoDto
    {
        public int idOrdemServico { get; set; }
        public string ClienteNome { get; set; }
        public int idCliente { get; set; }
        public string StatusDescricao { get; set; }
        public int idStatus { get; set; }
        public bool Pago { get; set; }
        public string DescricaoServico { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime PrevisaoEntrega { get; set; }
    }
}
