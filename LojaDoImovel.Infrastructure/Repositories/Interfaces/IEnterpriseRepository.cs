namespace LojaDoImovel.Infrastructure.Repositories.Interfaces
{
    public interface IEnterpriseRepository
    {
        Task<Enterprise> AddEnterpriseAsync(Enterprise enterprise);
        Task<IEnumerable<Enterprise>> GetAllEnterprisesAsync();
        Task<Enterprise?> GetEnterpriseByIdAsync(int idEnterprise);
        Task<Enterprise?> GetEnterpriseByNameAsync(string name);
        Task<Enterprise?> UnactivateEnterpriseAsync(Enterprise enterprise);
        Task<Enterprise?> UpdateEnterpriseAsync(Enterprise enterprise);
    }
}