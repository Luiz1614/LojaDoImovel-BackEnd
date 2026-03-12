using LojaDoImovel.Contracts.DTOs.Property;

namespace LojaDoImovel.Application.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyDto> AddPropertyAsync(CreatePropertyDto createPropertyDto);
        Task<bool> DeletePropertyAsync(int id);
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
        Task<PropertyDto> GetPropertyByCodeAsync(string code);
        Task<PropertyDto> GetPropertyByIdAsync(int id);
        Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto);
    }
}