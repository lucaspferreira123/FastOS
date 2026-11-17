using System.Text.Json;

namespace FastOS.Application.Services;

public sealed record DiagnosticImportValidation(bool IsValid, string? Error, JsonElement? Root, string? Json);

public static class DiagnosticImportValidator
{
    public const int SupportedSchemaVersion = 1;
    public const int MaxPayloadBytes = 10 * 1024 * 1024;

    public static DiagnosticImportValidation Validate(string? json)
    {
        if (json is null) return Invalid("O arquivo JSON está vazio.");
        if (System.Text.Encoding.UTF8.GetByteCount(json) > MaxPayloadBytes) return Invalid("O JSON excede o limite de 10 MB.");
        if (string.IsNullOrWhiteSpace(json)) return Invalid("O arquivo JSON está vazio.");
        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 32, CommentHandling = JsonCommentHandling.Disallow });
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return Invalid("A raiz do JSON deve ser um objeto.");
            if (!root.TryGetProperty("schemaVersion", out var version) || version.ValueKind != JsonValueKind.Number || !version.TryGetInt32(out var schemaVersion) || schemaVersion != SupportedSchemaVersion)
                return Invalid("Versão do diagnóstico incompatível. Versão suportada: 1.");
            if (!root.TryGetProperty("diagnosticId", out var id) || id.ValueKind != JsonValueKind.String || !id.TryGetGuid(out _))
                return Invalid("diagnosticId ausente ou inválido.");
            if (!root.TryGetProperty("generatedAt", out var generated) || !generated.TryGetDateTime(out _))
                return Invalid("generatedAt ausente ou inválido.");
            foreach (var required in new[] { "appVersion", "computer", "operatingSystem", "hardware", "health", "warnings", "summary" })
                if (!root.TryGetProperty(required, out _)) return Invalid($"Campo obrigatório ausente: {required}.");
            var hardware = root.GetProperty("hardware");
            var health = root.GetProperty("health");
            if (root.GetProperty("appVersion").ValueKind != JsonValueKind.String || root.GetProperty("computer").ValueKind != JsonValueKind.Object || root.GetProperty("operatingSystem").ValueKind != JsonValueKind.Object || hardware.ValueKind != JsonValueKind.Object || health.ValueKind != JsonValueKind.Object || root.GetProperty("warnings").ValueKind != JsonValueKind.Array || root.GetProperty("summary").ValueKind != JsonValueKind.String ||
                !hardware.TryGetProperty("processor", out var processor) || processor.ValueKind != JsonValueKind.Object || !hardware.TryGetProperty("memory", out var memory) || memory.ValueKind != JsonValueKind.Object || !health.TryGetProperty("alerts", out var alerts) || alerts.ValueKind != JsonValueKind.Array)
                return Invalid("Estrutura inválida nos dados de computador, hardware, saúde ou avisos.");
            if (!root.GetProperty("computer").TryGetProperty("hostname", out var hostname) || hostname.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(hostname.GetString()))
                return Invalid("Nome do computador ausente.");

            var computer = root.GetProperty("computer");
            var operatingSystem = root.GetProperty("operatingSystem");
            if (hostname.GetString()!.Length > 200 || !TryString(root, "appVersion", 100) || !TryString(root, "summary", 2000) ||
                !TryString(operatingSystem, "name", 300) || !TryString(operatingSystem, "build", 100) || !TryString(operatingSystem, "architecture", 50) ||
                !TryString(processor, "name", 300) || !TryNullableNumber(processor, "usagePercent", 0, 100) || !TryNullableNumber(processor, "temperatureC", -50, 250) ||
                !TryNullableNumber(memory, "totalGb", 0, 1_000_000) || !TryNullableNumber(memory, "availableGb", 0, 1_000_000) || !TryNullableNumber(memory, "usagePercent", 0, 100))
                return Invalid("Há campos de computador, sistema ou hardware com tipos ou limites inválidos.");

            foreach (var arrayName in new[] { "disks", "graphics", "networkAdapters", "sensors", "topProcesses" })
                if (!TryArray(hardware, arrayName, out _)) return Invalid($"hardware.{arrayName} deve ser uma lista.");
            if (!hardware.TryGetProperty("motherboard", out var motherboard) || motherboard.ValueKind != JsonValueKind.Object)
                return Invalid("hardware.motherboard deve ser um objeto.");
            if (!TryString(health, "status", 20) || !TryNumber(health, "score", 0, 100) || !TryArray(health, "alerts", out alerts))
                return Invalid("Dados de saúde inválidos.");
            if (!new[] { "normal", "warning", "critical", "unknown" }.Contains(health.GetProperty("status").GetString(), StringComparer.OrdinalIgnoreCase))
                return Invalid("health.status contém um valor não suportado.");
            foreach (var disk in hardware.GetProperty("disks").EnumerateArray())
                if (disk.ValueKind != JsonValueKind.Object || !TryString(disk, "name", 200) || !TryNullableNumber(disk, "capacityGb", 0, 1_000_000) ||
                    !TryNullableNumber(disk, "freeGb", 0, 1_000_000) || !TryNullableNumber(disk, "usedPercent", 0, 100))
                    return Invalid("hardware.disks contém um item inválido.");
            foreach (var gpu in hardware.GetProperty("graphics").EnumerateArray())
                // GPU metadata differs by provider and driver (for example driver/vramGb
                // may be present only for the WMI fallback). Preserve the complete object;
                // the preview reads known fields defensively and the raw JSON is retained.
                if (gpu.ValueKind != JsonValueKind.Object)
                    return Invalid("hardware.graphics contém um item inválido.");
            foreach (var sensor in hardware.GetProperty("sensors").EnumerateArray())
                if (sensor.ValueKind != JsonValueKind.Object || !TryString(sensor, "hardware", 300) || !TryString(sensor, "name", 300) ||
                    !TryString(sensor, "type", 50) || !TryNullableNumber(sensor, "value", -1_000_000, 1_000_000) || !TryStringOrEmpty(sensor, "unit", 30))
                    return Invalid("hardware.sensors contém um item inválido.");
            foreach (var network in hardware.GetProperty("networkAdapters").EnumerateArray())
                if (network.ValueKind != JsonValueKind.Object || !TryString(network, "name", 300) || !TryString(network, "status", 50) ||
                    !TryStringOrEmpty(network, "addresses", 1000) || !TryStringOrEmpty(network, "gateway", 1000) || !TryString(network, "speed", 100))
                    return Invalid("hardware.networkAdapters contém um item inválido.");
            foreach (var process in hardware.GetProperty("topProcesses").EnumerateArray())
                if (process.ValueKind != JsonValueKind.Object || !TryString(process, "name", 300) || !TryNumber(process, "memoryMb", 0, 10_000_000))
                    return Invalid("hardware.topProcesses contém um item inválido.");
            foreach (var warning in root.GetProperty("warnings").EnumerateArray())
                if (warning.ValueKind != JsonValueKind.String || warning.GetString()!.Length > 2000)
                    return Invalid("Cada aviso deve ser texto com até 2000 caracteres.");
            foreach (var alert in alerts.EnumerateArray())
                if (alert.ValueKind != JsonValueKind.Object || !TryString(alert, "severity", 20) || !TryString(alert, "category", 100) ||
                    !TryString(alert, "title", 300) || !TryString(alert, "description", 2000) || !TryString(alert, "recommendation", 2000))
                    return Invalid("A lista health.alerts contém um item inválido.");
            return new(true, null, root.Clone(), root.GetRawText());
        }
        catch (JsonException) { return Invalid("JSON malformado ou com estrutura excessivamente profunda."); }
    }

    private static bool TryString(JsonElement parent, string name, int maxLength) =>
        parent.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String && value.GetString() is { Length: > 0 } text && text.Length <= maxLength;

    private static bool TryStringOrEmpty(JsonElement parent, string name, int maxLength) =>
        parent.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String && value.GetString() is { } text && text.Length <= maxLength;

    private static bool TryArray(JsonElement parent, string name, out JsonElement value) =>
        parent.TryGetProperty(name, out value) && value.ValueKind == JsonValueKind.Array;

    private static bool TryNumber(JsonElement parent, string name, double min, double max) =>
        parent.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number) && double.IsFinite(number) && number >= min && number <= max;

    private static bool TryNullableNumber(JsonElement parent, string name, double min, double max) =>
        parent.TryGetProperty(name, out var value) && (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number) && double.IsFinite(number) && number >= min && number <= max);

    private static DiagnosticImportValidation Invalid(string error) => new(false, error, null, null);
}
