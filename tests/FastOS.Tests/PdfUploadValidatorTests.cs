using FastOS.Application.Services;
using Xunit;

namespace FastOS.Tests;

public class PdfUploadValidatorTests
{
    private const long Limit = 20 * 1024 * 1024;

    [Fact]
    public async Task AcceptsPdfWithSignatureAndCaseInsensitiveExtension()
    {
        await using var stream = new MemoryStream("%PDF-1.7 sample"u8.ToArray());

        var error = await PdfUploadValidator.ValidarAsync("relatório diagnóstico.PDF", stream.Length, stream, Limit);

        Assert.Null(error);
    }

    [Theory]
    [InlineData("relatorio.exe", 20)]
    [InlineData("relatorio.pdf", 5)]
    [InlineData("relatorio.pdf", 20 * 1024 * 1024 + 1)]
    public async Task RejectsInvalidExtensionOrSize(string name, long size)
    {
        await using var stream = new MemoryStream("%PDF-1.7 sample"u8.ToArray());

        var error = await PdfUploadValidator.ValidarAsync(name, size, stream, Limit);

        Assert.NotNull(error);
    }

    [Fact]
    public async Task RejectsPdfExtensionWithNonPdfContents()
    {
        await using var stream = new MemoryStream("not a pdf"u8.ToArray());

        var error = await PdfUploadValidator.ValidarAsync("relatorio.pdf", stream.Length, stream, Limit);

        Assert.NotNull(error);
    }
}
