namespace FastOS.Domain.Interfaces;

public interface IRelatorioPdfStorage
{
    Task<string> SalvarAsync(Stream arquivo, CancellationToken cancellationToken = default);
    Task RemoverAsync(string nomeArquivo, CancellationToken cancellationToken = default);
    Task<Stream?> AbrirAsync(string nomeArquivo, CancellationToken cancellationToken = default);
}
