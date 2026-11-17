using FastOS.Infrastructure.Repositories;
using FastOS.Domain.ValueObjects;
using FastOS.Domain.Interfaces;
using FastOS.Domain.Entities;

namespace FastOS.Application.Services
{
    public class RelatorioBusiness
    {
        private readonly OrdemServicoBusiness _ordemServicoBusiness;
        private readonly OrcamentoBusiness _orcamentoBusiness;
        private readonly IItemOrdemServicoRepository _itemOrdemServicoRepository;
        private readonly IOrdemServicoRepository _repository;

        private static readonly string[] NomesMeses = ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"];
        private static readonly string[] NomesDias = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];

        public RelatorioBusiness(OrdemServicoBusiness ordemServicoBusiness, IItemOrdemServicoRepository itemOrdemServicoRepository, OrcamentoBusiness orcamentoBusiness, IOrdemServicoRepository repository)
        {
            _ordemServicoBusiness = ordemServicoBusiness;
            _itemOrdemServicoRepository = itemOrdemServicoRepository;
            _orcamentoBusiness = orcamentoBusiness;
            _repository = repository;
        }

        public async Task<DashboardDto> ObterDadosDashboard()
        {
            var todasOrdens = await _ordemServicoBusiness.ObterTodasOrdens();
            var agora = DateTime.Now;

            // status 5 = Concluída/Aguardando Pagamento, 6 = Concluída/Pagamento Realizado
            var idsStatusConcluido = new[] { 5, 6 };
            var idsStatusAndamento = new[] { 2, 3, 4 };

            var total = todasOrdens.Count;
            var emAndamento = todasOrdens.Count(o => idsStatusAndamento.Contains(o.idStatus));
            var concluidas = todasOrdens.Count(o => idsStatusConcluido.Contains(o.idStatus));
            var atrasadas = todasOrdens.Count(o =>
                !idsStatusConcluido.Contains(o.idStatus) &&
                o.PrevisaoEntrega < agora);

            // Tempo médio de resolução (OS concluídas)
            var ordensConcluidas = todasOrdens.Where(o => idsStatusConcluido.Contains(o.idStatus)).ToList();
            double tempoMedio = 0;
            if (ordensConcluidas.Any())
                tempoMedio = ordensConcluidas.Average(o => (o.PrevisaoEntrega - o.DataAbertura).TotalDays);

            // OS por status
            var osPorStatus = todasOrdens
                .GroupBy(o => o.StatusDescricao)
                .Select(g => new OsPorStatusDto { Status = g.Key, Quantidade = g.Count() })
                .OrderByDescending(x => x.Quantidade)
                .ToList();

            // OS por mês (últimos 12 meses)
            var osPorMes = Enumerable.Range(0, 12)
                .Select(i => agora.AddMonths(-11 + i))
                .Select(mes => new OsPorMesDto
                {
                    Mes = $"{NomesMeses[mes.Month - 1]}/{mes.Year % 100:D2}",
                    Quantidade = todasOrdens.Count(o =>
                        o.DataAbertura.Year == mes.Year && o.DataAbertura.Month == mes.Month)
                })
                .ToList();

            // OS por cliente (top 10)
            var osPorCliente = todasOrdens
                .GroupBy(o => o.ClienteNome)
                .Select(g => new OsPorClienteDto { Cliente = g.Key, Quantidade = g.Count() })
                .OrderByDescending(x => x.Quantidade)
                .Take(10)
                .ToList();

            // OS por dia da semana
            var osPorDia = Enumerable.Range(0, 7)
                .Select(i => new OsPorDiaSemanaDto
                {
                    DiaSemana = NomesDias[i],
                    Quantidade = todasOrdens.Count(o => (int)o.DataAbertura.DayOfWeek == i)
                })
                .ToList();

            // Abertas vs Concluídas por mês (últimos 6 meses)
            var abertasVsConcluidas = Enumerable.Range(0, 6)
                .Select(i => agora.AddMonths(-5 + i))
                .Select(mes => new OsAbertasVsConcluidas
                {
                    Mes = $"{NomesMeses[mes.Month - 1]}/{mes.Year % 100:D2}",
                    Abertas = todasOrdens.Count(o =>
                        o.DataAbertura.Year == mes.Year && o.DataAbertura.Month == mes.Month &&
                        !idsStatusConcluido.Contains(o.idStatus)),
                    Concluidas = todasOrdens.Count(o =>
                        o.DataAbertura.Year == mes.Year && o.DataAbertura.Month == mes.Month &&
                        idsStatusConcluido.Contains(o.idStatus))
                })
                .ToList();

            // Tempo médio de resolução por mês (últimos 6 meses) — gráfico de linha
            var tempoMedioPorMes = Enumerable.Range(0, 6)
                .Select(i => agora.AddMonths(-5 + i))
                .Select(mes =>
                {
                    var ordensDoMes = ordensConcluidas.Where(o =>
                        o.DataAbertura.Year == mes.Year && o.DataAbertura.Month == mes.Month).ToList();
                    var media = ordensDoMes.Any()
                        ? Math.Round(ordensDoMes.Average(o => (o.PrevisaoEntrega - o.DataAbertura).TotalDays), 1)
                        : 0;
                    return new TempoMedioPorMesDto
                    {
                        Mes = $"{NomesMeses[mes.Month - 1]}/{mes.Year % 100:D2}",
                        TempoMedioDias = media
                    };
                })
                .ToList();

            // Ranking de clientes removido

            // Últimas 10 ordens
            var ultimasOrdens = todasOrdens
                .OrderByDescending(o => o.DataAbertura)
                .Take(10)
                .ToList();

            return new DashboardDto
            {
                TotalOS = total,
                OSEmAndamento = emAndamento,
                OSConcluidas = concluidas,
                OSAtrasadas = atrasadas,
                TempoMedioResolucaoDias = Math.Round(tempoMedio, 1),
                OsPorStatus = osPorStatus,
                OsPorMes = osPorMes,
                OsPorCliente = osPorCliente,
                OsPorDiaSemana = osPorDia,
                OsAbertasVsConcluidas = abertasVsConcluidas,
                TempoMedioPorMes = tempoMedioPorMes,
                UltimasOrdens = ultimasOrdens
            };
        }

        public async Task<int> ObterIdClienteDaOrdem(int idOrdem)
        {
            var ordem = await _repository.GetByIdAsync(idOrdem);
            if (ordem == null)
                throw new Exception("Ordem de serviço não encontrada.");
            return ordem.idCliente;
        }

        public async Task<RelatorioOrcamentoDto> PopularRelatorioOrcamento(int idOrdem)
        {
            try
            {
                if (idOrdem == 0)
                {
                    throw new ArgumentException("N�o foi poss�vel imprimir o orcamento.");
                }

                var ordem = await _ordemServicoBusiness.ObterOrdemDto(idOrdem);

                var itens = await _itemOrdemServicoRepository.ObterItensOrdemServico(idOrdem);

                var orcamento = await _orcamentoBusiness.ObterOrcamento(idOrdem);

                var itensOrdem = itens.Where(i => i.idOrdemServico == idOrdem).ToList();

                var relatorioDto = new RelatorioOrcamentoDto
                {
                    OrdemServico = ordem,
                    Itens = itensOrdem,
                    Orcamento = orcamento 
                };

                return relatorioDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao imprimir o orcamento", ex);
            }
        }
    }
}

