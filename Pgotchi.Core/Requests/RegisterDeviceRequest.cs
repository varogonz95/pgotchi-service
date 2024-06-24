using FluentValidation;
using Pgotchi.Shared.Models;

namespace Pgotchi.Shared.Requests;

public sealed class RegisterDeviceRequest
{
    public required string DeviceId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IDictionary<string, DevicePropertyValue>? Properties { get; set; }
}

public class RegisterDeviceRequestValidator : AbstractValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(32);
        RuleFor(x => x.Description).MaximumLength(128);
        RuleFor(x => x.Properties)
            .Must(x => x?.Count > 0)
            .When(x => x.Properties is not null)
            .WithMessage("Must have at least one item");
    }
}