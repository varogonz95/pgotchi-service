
using System.ComponentModel.DataAnnotations;

namespace Pgotchi.Functions;

public sealed class AzureIotHubOptions
{
    public const string SectionName = "AzureIotHub";

    [Required]
    public required string HostName { get; set; }
    
    [Required]
    public required string ConnectionString { get; set; }

    public string? ApiVersion { get; set; } = string.Empty;
}