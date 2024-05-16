
using System.ComponentModel.DataAnnotations;

namespace Pgotchi.Functions;

public sealed class AzureIotHubOptions
{
    public const string SectionName = "AzureIotHub";
    
    [Required]
    public required string ConnectionString { get; set; }
}