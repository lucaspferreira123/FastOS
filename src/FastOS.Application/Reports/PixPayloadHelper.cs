namespace FastOS.Application.Reports;

/// <summary>
/// Gera o payload EMV/PIX no padrão do Banco Central do Brasil.
/// Especificação: https://www.bcb.gov.br/content/estabilidadefinanceira/pix/Regulamento_Pix/II_ManualdePadroesparaIniciacaodoPix.pdf
/// </summary>
public static class PixPayloadHelper
{
    private const string PayloadFormatIndicator   = "01";
    private const string MerchantCategoryCode     = "0000";
    private const string TransactionCurrency      = "986";  // BRL
    private const string CountryCode              = "BR";

    public static string Gerar(string chavePix, string nomeBeneficiario, string cidade, decimal valor, string txId)
    {
        // Normaliza strings (remove acentos e caracteres especiais)
        nomeBeneficiario = Normalizar(nomeBeneficiario, 25);
        cidade           = Normalizar(cidade, 15);
        txId             = Normalizar(txId, 25);

        // 26 - Merchant Account Information
        var gui     = TLV("00", "BR.GOV.BCB.PIX");
        var chave   = TLV("01", chavePix);
        var mai     = TLV("26", gui + chave);

        // 54 - Transaction Amount
        var valorStr   = valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        var txAmount   = TLV("54", valorStr);

        // 62 - Additional Data Field (txid)
        var txIdField  = TLV("05", txId);
        var adf        = TLV("62", txIdField);

        // Monta payload sem CRC
        var payload =
            TLV("00", PayloadFormatIndicator) +
            mai +
            TLV("52", MerchantCategoryCode) +
            TLV("53", TransactionCurrency) +
            txAmount +
            TLV("58", CountryCode) +
            TLV("59", nomeBeneficiario) +
            TLV("60", cidade) +
            adf +
            "6304"; // tag CRC + 4 chars placeholder

        // Calcula CRC16-CCITT
        var crc = CalcularCrc16(payload);
        return payload + crc;
    }

    // ── helpers ──────────────────────────────────────────────────────────

    private static string TLV(string tag, string value)
    {
        var len = value.Length.ToString("D2");
        return $"{tag}{len}{value}";
    }

    private static string Normalizar(string input, int maxLen)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Remove acentos
        var normalized = input.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var c in normalized)
        {
            var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var result = sb.ToString().Normalize(System.Text.NormalizationForm.FormC);

        // Mantém apenas caracteres permitidos pelo padrão EMV
        var clean = new System.Text.StringBuilder();
        foreach (var c in result)
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '@' || c == '.' || c == '-' || c == '_')
                clean.Append(c);
        }

        var final = clean.ToString().Trim();
        return final.Length > maxLen ? final[..maxLen] : final;
    }

    private static string CalcularCrc16(string payload)
    {
        // CRC16-CCITT (polinômio 0x1021, valor inicial 0xFFFF)
        ushort crc = 0xFFFF;
        foreach (var c in payload)
        {
            crc ^= (ushort)(c << 8);
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0)
                    crc = (ushort)((crc << 1) ^ 0x1021);
                else
                    crc <<= 1;
            }
        }
        return crc.ToString("X4");
    }
}
