using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Enterprise;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using Mapster;

namespace LojaDoImovel.Application.Services;

public class EnterpriseService : IEnterpriseService
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public EnterpriseService(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<EnterpriseDto> AddEnterpriseAsync(CreateEnterpriseDto createEnterpriseDto)
    {
        var enterprise = createEnterpriseDto.Adapt<Enterprise>();

        enterprise.IsActive = true;
        enterprise.CreatedAt = DateTime.UtcNow;

        var created = await _enterpriseRepository.AddEnterpriseAsync(enterprise);

        return created.Adapt<EnterpriseDto>();
    }

    public async Task<EnterpriseDto> GetEnterpriseByIdAsync(int idEnterprise)
    {
        var enterprise = await _enterpriseRepository.GetEnterpriseByIdAsync(idEnterprise);

        return enterprise.Adapt<EnterpriseDto>();
    }

    public async Task<EnterpriseDto> GetEnterpriseByNameAsync(string name)
    {
        var enterprise = await _enterpriseRepository.GetEnterpriseByNameAsync(name);

        return enterprise.Adapt<EnterpriseDto>();
    }

    public async Task<IEnumerable<EnterpriseDto>> GetAllEnterpriseAsync()
    {
        var enterprises = await _enterpriseRepository.GetAllEnterprisesAsync();

        return enterprises.Adapt<IEnumerable<EnterpriseDto>>();
    }

    public async Task<EnterpriseDto> UpdateEnterpriseAsync(UpdateEnterpriseDto updateEnterpriseDto)
    {
        var enterprise = updateEnterpriseDto.Adapt<Enterprise>();

        var updated = await _enterpriseRepository.UpdateEnterpriseAsync(enterprise);

        return updated.Adapt<EnterpriseDto>();
    }

    public async Task<EnterpriseDto> UnactivateEnterpriseAsycn(UpdateEnterpriseDto updateEnterpriseDto)
    {
        var enterprise = updateEnterpriseDto.Adapt<Enterprise>();

        var unactivated = await _enterpriseRepository.UnactivateEnterpriseAsync(enterprise);

        return unactivated.Adapt<EnterpriseDto>();
    }
}
