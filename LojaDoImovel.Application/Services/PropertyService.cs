using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Property;
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

        var created = await _propertyRepository.AddPropertyAsync(property);

        return created.Adapt<PropertyDto>();
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
    {
        var properties = await _propertyRepository.GetAllPropertiesAsync();

        return properties.Adapt<IEnumerable<PropertyDto>>();
    }

    public async Task<PropertyDto> GetPropertyByIdAsync(int id)
    {
        var property = await _propertyRepository.GetPropertyByIdAsync(id);

        return property.Adapt<PropertyDto>();
    }

    public async Task<PropertyDto> GetPropertyByCodeAsync(string code)
    {
        var property = await _propertyRepository.GetPropertyByCodeAsync(code);

        return property.Adapt<PropertyDto>();
    }

    public async Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto)
    {
        var property = updatePropertyDto.Adapt<Property>();

        var updated = await _propertyRepository.UpdatePropertyAsync(property);

        return updated.Adapt<PropertyDto>();
    }

    public async Task<bool> DeletePropertyAsync(int id)
    {
        var property = await _propertyRepository.DeletePropertyAsync(id);

        return property;
    }
}
