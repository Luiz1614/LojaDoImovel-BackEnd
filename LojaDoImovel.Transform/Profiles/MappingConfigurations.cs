using LojaDoImovel.Contracts.DTOs.ApplicationUser;
using LojaDoImovel.Contracts.DTOs.Enterprise;
using LojaDoImovel.Contracts.DTOs.Property;
using LojaDoImovel.Contracts.DTOs.PropertyDtos;
using LojaDoImovel.Contracts.DTOs.PropertyImageDtos;
using LojaDoImovel.Infrastructure.Identity;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace LojaDoImovel.Transform.Profiles;

public static class MappingConfigurations
{
    public static void RegisterMaps(this IServiceCollection services)
    {
        // --- Property Mappings ---

        TypeAdapterConfig<ApplicationUser, PendingUserDto>
            .NewConfig();

        // CreatePropertyDto -> Property
        TypeAdapterConfig<CreatePropertyDto, Property>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Enterprise)
            .Map(dest => dest.Images, src => src.ImageUrls != null
                ? src.ImageUrls.Adapt<ICollection<PropertyImage>>()
                : null)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt);

        // UpdatePropertyDto -> Property
        TypeAdapterConfig<UpdatePropertyDto, Property>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Enterprise)
            .Ignore(dest => dest.CreatedAt);

        // Property -> PropertyDto
        TypeAdapterConfig<Property, PropertyDto>
            .NewConfig()
            .Map(dest => dest.Images, src => src.Images != null
                ? src.Images.Adapt<List<PropertyImageDto>>()
                : null);

        // PropertyImage -> PropertyImageDto
        TypeAdapterConfig<PropertyImage, PropertyImageDto>
            .NewConfig();

        // PropertyImageDto -> PropertyImage
        TypeAdapterConfig<PropertyImageDto, PropertyImage>
            .NewConfig();

        // --- Enterprise Mappings ---
        // CreateEnterpriseDto -> Enterprise
        TypeAdapterConfig<CreateEnterpriseDto, Enterprise>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Properties);

        // UpdateEnterpriseDto -> Enterprise
        TypeAdapterConfig<UpdateEnterpriseDto, Enterprise>
            .NewConfig()
            .Ignore(dest => dest.Properties);

        // Enterprise -> EnterpriseDto
        TypeAdapterConfig<Enterprise, EnterpriseDto>
            .NewConfig();

        // EnterpriseDto -> Enterprise (se necessário para testes/updates)
        TypeAdapterConfig<EnterpriseDto, Enterprise>
            .NewConfig();
    }
}