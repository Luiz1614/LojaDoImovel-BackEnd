using LojaDoImovel.Contracts.DTOs.Enterprise;

namespace LojaDoImovel.Application.Services.Interfaces
{
    public interface IEnterpriseService
    {
        Task<EnterpriseDto> AddEnterpriseAsync(CreateEnterpriseDto createEnterpriseDto);
        Task<IEnumerable<EnterpriseDto>> GetAllEnterpriseAsync();
        Task<EnterpriseDto> GetEnterpriseByIdAsync(int idEnterprise);
        Task<EnterpriseDto> GetEnterpriseByNameAsync(string name);
        Task<EnterpriseDto> UnactivateEnterpriseAsycn(UpdateEnterpriseDto updateEnterpriseDto);
        Task<EnterpriseDto> UpdateEnterpriseAsync(UpdateEnterpriseDto updateEnterpriseDto);
    }
}