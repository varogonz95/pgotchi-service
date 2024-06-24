using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Pgotchi.Shared.Models;

namespace Pgotchi.Shared;

public class DataProfile : Profile
{
    public DataProfile()
    {
        CreateMap<Device, DeviceSummary>()
            .ForMember(dst => dst.AuthenticationType, opt => opt.MapFrom(src => src.Authentication.Type))
            .ForMember(dst => dst.SymmetricPrimaryKey, opt => opt.MapFrom(src => src.Authentication.SymmetricKey.PrimaryKey))
            .ForMember(dst => dst.SymmetricSecondaryKey, opt => opt.MapFrom(src => src.Authentication.SymmetricKey.SecondaryKey))
            .ForMember(dst => dst.X509PrimaryThumbprint, opt => opt.MapFrom(src => src.Authentication.X509Thumbprint.PrimaryThumbprint))
            .ForMember(dst => dst.X509SecondaryThumbprint, opt => opt.MapFrom(src => src.Authentication.X509Thumbprint.SecondaryThumbprint))
            .IncludeAllDerived()
            ;

        CreateMap<Twin, DeviceTwinSummary>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.DeviceId))
            .ForMember(
                dst => dst.UserId, 
                opt => opt.MapFrom(src => src.Tags.Contains(TwinTags.UserId) ? src.Tags[TwinTags.UserId] : null))
            .ForMember(dst => dst.X509PrimaryThumbprint, opt => opt.MapFrom(src => src.X509Thumbprint.PrimaryThumbprint))
            .ForMember(dst => dst.X509SecondaryThumbprint, opt => opt.MapFrom(src => src.X509Thumbprint.SecondaryThumbprint))
            .ForMember(dst => dst.Properties, opt => opt.MapFrom(src => src.Properties.Desired))
            ;

    }
}
