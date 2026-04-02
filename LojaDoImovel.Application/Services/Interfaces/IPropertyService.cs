using LojaDoImovel.Contracts.DTOs.Property;
using LojaDoImovel.Contracts.DTOs.PropertyDtos;

namespace LojaDoImovel.Application.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyDto> AddPropertyAsync(CreatePropertyDto createPropertyDto);
        Task<bool> DeletePropertyAsync(int idProperty);
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(int idEnterprise);
        Task<PropertyDto> GetPropertyByCodeAsync(string code, int idEnterprise);
        Task<PropertyDto> GetPropertyByIdAsync(int idProperty, int idEnterprise);
        Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto);
    }
}