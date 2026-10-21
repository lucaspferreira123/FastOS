using FastOS.Domain.Entities;
using FastOS.Domain.Interfaces;
using FastOS.Domain.ValueObjects;

namespace FastOS.Application.Services
{
    public class OrdemServicoBusiness
    {
        private static readonly SemaphoreSlim CadastrarOrdemMutex = new(1, 1);
        private readonly IOrdemServicoRepository _repository;
        private readonly IRelatorioPdfStorage _relatorioStorage;

        public OrdemServicoBusiness(IOrdemServicoRepository repository, IRelatorioPdfStorage relatorioStorage)
        {
            _repository = repository;
            _relatorioStorage = relatorioStorage;
        }

        public async Task<OrdemServicoEntity> CadastrarOrdem(OrdemServicoEntity ordem, Stream? relatorioPdf = null, CancellationToken cancellationToken = default)
        {
            await CadastrarOrdemMutex.WaitAsync();
            try
            {
                if (ordem == null)
                {
                    throw new ArgumentException("Nao foi possivel cadastrar a ordem.");
                }

                string? nomeArquivo = null;
                try
                {
                    if (relatorioPdf is not null)
                    {
                        nomeArquivo = await _relatorioStorage.SalvarAsync(relatorioPdf, cancellationToken);
                        ordem.RelatorioPdfFileName = nomeArquivo;
                    }

                    return await _repository.CadastrarOrdem(ordem);
                }
                catch
                {
                    if (nomeArquivo is not null)
                        await _relatorioStorage.RemoverAsync(nomeArquivo, CancellationToken.None);
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar a ordem", ex);
            }
            finally
            {
                CadastrarOrdemMutex.Release();
            }
        }

        public async Task<Stream?> ObterRelatorioPdf(int idOrdem, CancellationToken cancellationToken = default)
        {
            var ordem = await _repository.GetByIdAsync(idOrdem);
            if (ordem is null || string.IsNullOrWhiteSpace(ordem.RelatorioPdfFileName)) return null;
            return await _relatorioStorage.AbrirAsync(ordem.RelatorioPdfFileName, cancellationToken);
        }

        public async Task<List<OrdemServicoDto>> ObterTodasOrdens()
        {
            try
            {
                var ordens = await _repository.ObterTodasOrdens();

                return ordens;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter as ordens", ex);
            }
        }

        public async Task<OrdemServicoDto> ObterOrdemDto(int idOrdem)
        {
            try
            {
                var ordem = await _repository.ObterOrdemDto(idOrdem);

                return ordem;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter a ordem", ex);
            }
        }

        public async Task<OrdemServicoEntity> AlterarOrdemServico(OrdemServicoEntity model)
        {
            try
            {
                var ordem = await _repository.AlterarOrdemServico(model);

                return ordem;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar a ordem", ex);
            }
        }

        public async Task AlterarStatusOrdem(int idOrdem, int idStatus)
        {
            try
            {
                var ordem = await _repository.GetByIdAsync(idOrdem);

                if (ordem == null)
                    return;

                if (idStatus == 5)
                {
                    ordem.Pago = true;
                }

                ordem.idStatus = idStatus;

                await _repository.UpdateAsync(ordem);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar a ordem", ex);
            }
        }
    }
}
