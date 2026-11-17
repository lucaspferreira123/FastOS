using FastOS.Application.Services;
using Xunit;

namespace FastOS.Tests;

public sealed class DiagnosticImportValidatorTests
{
    private static string ValidJson(int schemaVersion = 1) => $$"""
        {
          "schemaVersion": {{schemaVersion}},
          "diagnosticId": "1afdf6c5-70e1-4c27-8a3c-993b405b957e",
          "generatedAt": "2026-10-12T14:30:00Z",
          "appVersion": "1.0.0",
          "computer": { "hostname": "PC-CLIENTE" },
          "operatingSystem": { "name": "Windows", "build": "10", "architecture": "X64", "bootTime": null },
          "hardware": { "processor": { "name": "CPU", "usagePercent": null, "temperatureC": null }, "memory": { "totalGb": 16, "usedGb": 8, "availableGb": 8, "usagePercent": null }, "disks": [], "graphics": [], "motherboard": {}, "networkAdapters": [], "sensors": [], "topProcesses": [] },
          "health": { "status": "normal", "score": 100, "alerts": [] },
          "warnings": [],
          "summary": "Coleta de teste"
        }
        """;

    [Fact]
    public void AcceptsWellFormedVersionOneDiagnosticAndKeepsNullSensorValues()
    {
        var result = DiagnosticImportValidator.Validate(ValidJson());
        Assert.True(result.IsValid);
        Assert.Equal("PC-CLIENTE", result.Root!.Value.GetProperty("computer").GetProperty("hostname").GetString());
        Assert.Equal(System.Text.Json.JsonValueKind.Null, result.Root.Value.GetProperty("hardware").GetProperty("processor").GetProperty("temperatureC").ValueKind);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(-1)]
    public void RejectsUnsupportedSchema(int version)
    {
        var result = DiagnosticImportValidator.Validate(ValidJson(version));
        Assert.False(result.IsValid);
        Assert.Contains("incompatível", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("[]")]
    [InlineData("null")]
    public void RejectsMalformedOrWrongRoot(string json) => Assert.False(DiagnosticImportValidator.Validate(json).IsValid);

    [Fact]
    public void RejectsPayloadLargerThanTenMegabytes()
    {
        var result = DiagnosticImportValidator.Validate(new string(' ', DiagnosticImportValidator.MaxPayloadBytes + 1));
        Assert.False(result.IsValid);
        Assert.Contains("10 MB", result.Error);
    }

    [Fact]
    public void RejectsMissingRequiredFields()
    {
        var result = DiagnosticImportValidator.Validate("{\"schemaVersion\":1}");
        Assert.False(result.IsValid);
        Assert.Contains("diagnosticId", result.Error);
    }

    [Fact]
    public void RejectsInvalidHardwareArrayItems()
    {
        var json = ValidJson().Replace("\"disks\": []", "\"disks\": [null]", StringComparison.Ordinal);
        var result = DiagnosticImportValidator.Validate(json);
        Assert.False(result.IsValid);
        Assert.Contains("hardware.disks", result.Error);
    }

    [Theory]
    [InlineData("{ \"name\": \"GPU\" }")]
    [InlineData("{ \"name\": \"GPU\", \"driver\": \"Driver 1\", \"vramGb\": 8, \"usagePercent\": null, \"temperatureC\": null }")]
    [InlineData("{ \"name\": \"GPU\", \"driver\": \"Driver 1\", \"vramGb\": null, \"usagePercent\": 34.5, \"temperatureC\": 47 }")]
    public void AcceptsGraphicsItemsFromDiagnosticExporter(string graphicsItem)
    {
        var json = ValidJson().Replace("\"graphics\": []", $"\"graphics\": [{graphicsItem}]", StringComparison.Ordinal);

        var result = DiagnosticImportValidator.Validate(json);

        Assert.True(result.IsValid, result.Error);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("\"GPU\"")]
    public void RejectsMalformedGraphicsItems(string graphicsItem)
    {
        var json = ValidJson().Replace("\"graphics\": []", $"\"graphics\": [{graphicsItem}]", StringComparison.Ordinal);

        var result = DiagnosticImportValidator.Validate(json);

        Assert.False(result.IsValid);
        Assert.Contains("hardware.graphics", result.Error);
    }

    [Fact]
    public void RejectsMalformedAlertsAndAcceptsUnavailableOptionalSensorValues()
    {
        var valid = DiagnosticImportValidator.Validate(ValidJson());
        Assert.True(valid.IsValid);

        var json = ValidJson().Replace("\"alerts\": []", "\"alerts\": [ { \"severity\": 7 } ]", StringComparison.Ordinal);
        var result = DiagnosticImportValidator.Validate(json);
        Assert.False(result.IsValid);
        Assert.Contains("health.alerts", result.Error);
    }
}
