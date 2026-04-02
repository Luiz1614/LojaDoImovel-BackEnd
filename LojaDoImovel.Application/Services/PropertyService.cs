using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Property;
using LojaDoImovel.Contracts.DTOs.PropertyDtos;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using Mapster;

namespace LojaDoImovel.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDto> AddPropertyAsync(CreatePropertyDto createPropertyDto)
    {
        var property = createPropertyDto.Adapt<Property>();

        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;

        var created = await _propertyRepository.AddPropertyAsync(property);

        return created.Adapt<PropertyDto>();
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(int idEnterprise)
    {
        var properties = await _propertyRepository.GetAllPropertiesAsync(idEnterprise);

        return properties.Adapt<IEnumerable<PropertyDto>>();
    }

    public async Task<PropertyDto> GetPropertyByIdAsync(int idProperty, int idEnterprise)
    {
        var property = await _propertyRepository.GetPropertyByIdAsync(idProperty, idEnterprise);

        return property.Adapt<PropertyDto>();
    }

    public async Task<PropertyDto> GetPropertyByCodeAsync(string code, int idEnterprise)
    {
        var property = await _propertyRepository.GetPropertyByCodeAsync(code, idEnterprise);

        return property.Adapt<PropertyDto>();
    }

    public async Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto)
    {
        var property = updatePropertyDto.Adapt<Property>();

        property.UpdatedAt = DateTime.UtcNow;

        var updated = await _propertyRepository.UpdatePropertyAsync(property);

        return updated.Adapt<PropertyDto>();
    }

    public async Task<bool> DeletePropertyAsync(int idProperty)
    {
        var property = await _propertyRepository.DeletePropertyAsync(idProperty);

        return property;
    }
}
