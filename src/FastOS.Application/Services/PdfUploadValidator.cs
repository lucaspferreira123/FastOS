namespace FastOS.Application.Services;

public static class PdfUploadValidator
{
    public static async Task<string?> ValidarAsync(string nomeArquivo, long tamanho, Stream conteudo, long limiteBytes, CancellationToken cancellationToken = default)
    {
        if (tamanho <= 5 || tamanho > limiteBytes)
            return $"O arquivo deve ter até {limiteBytes / 1024 / 1024} MB e conter um PDF válido.";
        if (!string.Equals(Path.GetExtension(nomeArquivo), ".pdf", StringComparison.OrdinalIgnoreCase))
            return "O arquivo selecionado deve ter extensão .pdf.";

        var header = new byte[5];
        var lidos = await conteudo.ReadAsync(header.AsMemory(), cancellationToken);
        return lidos == header.Length && header.AsSpan().SequenceEqual("%PDF-"u8)
            ? null
            : "O conteúdo enviado não é um PDF válido.";
    }
}
