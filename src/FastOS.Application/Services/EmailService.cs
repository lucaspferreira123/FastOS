using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace FastOS.Application.Services;

public class EmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _remetente;
    private readonly string _nomeRemetente;
    private readonly bool _useSsl;

    public EmailService(IConfiguration configuration)
    {
        var section = configuration.GetSection("Email");

        _host = section["Host"] ?? throw new InvalidOperationException("Configuração de e-mail ausente: Email:Host.");

        _port = int.TryParse(section["Port"], out var port)
            ? port
            : 587;

        _username = section["Username"] ?? throw new InvalidOperationException("Configuração de e-mail ausente: Email:Username.");
        _password = section["Password"] ?? throw new InvalidOperationException("Configuração de e-mail ausente: Email:Password.");
        _remetente = section["Remetente"] ?? _username;
        _nomeRemetente = section["NomeRemetente"] ?? "FastOS";
        _useSsl = bool.TryParse(section["UseSsl"], out var useSsl) ? useSsl : true;
    }

    public async Task EnviarReciboAsync(string destinatario, string nomeCliente, int idOrdem, byte[] pdfBytes)
    {
        using var client = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = _useSsl
        };

        using var mensagem = new MailMessage();
        mensagem.From = new MailAddress(_remetente, _nomeRemetente);
        mensagem.To.Add(new MailAddress(destinatario, nomeCliente));
        mensagem.Subject = $"Recibo de Cobrança - Ordem de Serviço Nº {idOrdem}";
        mensagem.IsBodyHtml = true;
        mensagem.Body = $@"
            <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                <div style='background:#c0392b;padding:20px;text-align:center;'>
                    <h2 style='color:#fff;margin:0;'>UTI do PC Informatica</h2>
                    <p style='color:#fff;margin:4px 0 0;font-size:13px;'>Assistência Técnica em Informática</p>
                </div>

                <div style='padding:24px;background:#f9f9f9;'>
                    <p style='font-size:15px;'>Olá, <strong>{nomeCliente}</strong>!</p>
                    <p>Segue em anexo o <strong>recibo de cobrança</strong> referente à
                       <strong>Ordem de Serviço Nº {idOrdem}</strong>.</p>
                    <p>O recibo contém o <strong>QR Code PIX</strong> para pagamento.
                       Basta escanear com o aplicativo do seu banco para realizar o pagamento.</p>

                    <div style='background:#fff;border:1px solid #ddd;border-radius:8px;padding:16px;margin:20px 0;'>
                        <p style='margin:0;font-size:13px;color:#555;'>
                            <strong>Formas de pagamento aceitas:</strong><br/>
                            💳 Cartão de Débito e Crédito<br/>
                            💵 Dinheiro<br/>
                            📱 PIX (QR Code no anexo)
                        </p>
                    </div>

                    <p style='font-size:13px;color:#777;'>
                        Em caso de dúvidas, entre em contato conosco.<br/>
                        <strong>(19) 99900-0000</strong> | contato@utipc.com.br
                    </p>
                </div>

                <div style='background:#eee;padding:12px;text-align:center;font-size:11px;color:#999;'>
                    UTI do PC Informatica — Rua Americana, 100 - Americana/SP
                </div>
            </div>";

        using var stream = new MemoryStream(pdfBytes);
        using var anexo = new Attachment(stream, $"Recibo_OS_{idOrdem}.pdf", "application/pdf");
        mensagem.Attachments.Add(anexo);

        await client.SendMailAsync(mensagem);
    }
}
