using FastOS.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FastOS.Infrastructure.Storage;

public sealed class FileSystemRelatorioPdfStorage : IRelatorioPdfStorage
{
    private readonly string _directory;

    public FileSystemRelatorioPdfStorage(IWebHostEnvironment environment, IConfiguration configuration)
    {
        var configuredPath = configuration["FileStorage:ReportsPath"];
        var userDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _directory = Path.GetFullPath(string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(string.IsNullOrWhiteSpace(userDataPath) ? environment.ContentRootPath : userDataPath, "FastOS", "order-reports")
            : Path.IsPathRooted(configuredPath) ? configuredPath : Path.Combine(environment.ContentRootPath, configuredPath));
    }

    public async Task<string> SalvarAsync(Stream arquivo, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_directory);
        var fileName = $"{Guid.NewGuid():N}.pdf";
        var path = ObterCaminho(fileName);
        try
        {
            await using var destination = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous);
            await arquivo.CopyToAsync(destination, cancellationToken);
            return fileName;
        }
        catch
        {
            if (File.Exists(path)) File.Delete(path);
            throw;
        }
    }

    public Task RemoverAsync(string nomeArquivo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ObterCaminho(nomeArquivo);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    public Task<Stream?> AbrirAsync(string nomeArquivo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ObterCaminho(nomeArquivo);
        Stream? stream = File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan) : null;
        return Task.FromResult(stream);
    }

    private string ObterCaminho(string nomeArquivo)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo) || Path.GetFileName(nomeArquivo) != nomeArquivo)
            throw new InvalidOperationException("Nome de arquivo inválido.");
        var path = Path.GetFullPath(Path.Combine(_directory, nomeArquivo));
        if (!path.StartsWith(_directory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Caminho de arquivo inválido.");
        return path;
    }
}
