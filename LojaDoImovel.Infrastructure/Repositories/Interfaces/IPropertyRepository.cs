namespace LojaDoImovel.Infrastructure.Repositories.Interfaces
{
    public interface IPropertyRepository
    {
        Task<Property> AddPropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int idProperty);
        Task<IEnumerable<Property>> GetAllPropertiesAsync(int idEnterprise);
        Task<Property?> GetPropertyByCodeAsync(string code, int idEnterprise);
        Task<Property?> GetPropertyByIdAsync(int id, int idEnterprise);
        Task<Property?> UpdatePropertyAsync(Property property);
    }
}