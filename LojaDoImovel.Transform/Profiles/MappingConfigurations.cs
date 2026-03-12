using LojaDoImovel.Contracts.DTOs.Property;
using LojaDoImovel.Contracts.DTOs.PropertyImage;
using Microsoft.Extensions.DependencyInjection;
using Mapster;

namespace LojaDoImovel.Transform.Profiles;

public static class MappingConfigurations
{
    public static void RegisterMaps(this IServiceCollection services)
    {
        // CreatePropertyDto -> Property
        TypeAdapterConfig<CreatePropertyDto, Property>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Enterprise)
            .Ignore(dest => dest.Images)
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
            .Map(dest => dest.Images, src => src.Images.Adapt<List<PropertyImageDto>>());

        // PropertyImage -> PropertyImageDto
        TypeAdapterConfig<PropertyImage, PropertyImageDto>
            .NewConfig();

        // PropertyImageDto -> PropertyImage (caso precise para upload/atualização)
        TypeAdapterConfig<PropertyImageDto, PropertyImage>
            .NewConfig();
    }
}