namespace LojaDoImovel.Infrastructure.Repositories.Interfaces
{
    public interface IPropertyRepository
    {
        Task<Property> AddPropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int idProperty);
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByCodeAsync(string code);
        Task<Property?> GetPropertyByIdAsync(int id);
        Task<Property?> UpdatePropertyAsync(Property property);
    }
}