using System.ComponentModel.DataAnnotations;

namespace Pgotchi.Shared.Options;

public sealed class AzureIotHubEventHubOptions
{
    public const string SectionName = "AzureIotHubEventHub";

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string ConnectionString { get; set; }

    public string? ApiVersion { get; set; } = string.Empty;
}