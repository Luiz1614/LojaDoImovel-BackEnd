using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Enterprise;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace LojaDoImovel.Application.Services;

public class EnterpriseService : IEnterpriseService
{
    private readonly IEnterpriseRepository _enterpriseRepository;
    private readonly ILogger<EnterpriseService> _logger;

    public EnterpriseService(IEnterpriseRepository enterpriseRepository, ILogger<EnterpriseService> logger)
    {
        _enterpriseRepository = enterpriseRepository;
        _logger = logger;
    }

    public async Task<EnterpriseDto> AddEnterpriseAsync(CreateEnterpriseDto createEnterpriseDto)
    {
        _logger.LogInformation("Criando empreendimento '{Name}'.", createEnterpriseDto.Name);

        var enterprise = createEnterpriseDto.Adapt<Enterprise>();

        enterprise.IsActive = true;
        enterprise.CreatedAt = DateTime.UtcNow;

        var created = await _enterpriseRepository.AddEnterpriseAsync(enterprise);

        if (created == null)
        {
            _logger.LogWarning("Falha ao criar empreendimento '{Name}'.", createEnterpriseDto.Name);
            return null;
        }

        _logger.LogInformation("Empreendimento '{Name}' criado com sucesso. Id: {Id}", created.Name, created.Id);
        return created.Adapt<EnterpriseDto>();
    }

    public async Task<EnterpriseDto> GetEnterpriseByIdAsync(int idEnterprise)
    {
        _logger.LogInformation("Buscando empreendimento {IdEnterprise}.", idEnterprise);

        var enterprise = await _enterpriseRepository.GetEnterpriseByIdAsync(idEnterprise);

        if (enterprise == null)
        {
            _logger.LogWarning("Empreendimento {IdEnterprise} não encontrado.", idEnterprise);
            return null;
        }

        return enterprise.Adapt<EnterpriseDto>();
    }

    public async Task<EnterpriseDto> GetEnterpriseByNameAsync(string name)
    {
        _logger.LogInformation("Buscando empreendimento com nome '{Name}'.", name);

        var enterprise = await _enterpriseRepository.GetEnterpriseByNameAsync(name);

        if (enterprise == null)
        {
            _logger.LogWarning("Empreendimento com nome '{Name}' não encontrado.", name);
            return null;
        }

        return enterprise.Adapt<EnterpriseDto>();
    }

    public async Task<IEnumerable<EnterpriseDto>> GetAllEnterpriseAsync()
    {
        _logger.LogInformation("Buscando todos os empreendimentos.");

        var enterprises = await _enterpriseRepository.GetAllEnterprisesAsync();

        if (enterprises == null)
        {
            _logger.LogWarning("Nenhum empreendimento encontrado.");
            return null;
        }

        return enterprises.Adapt<IEnumerable<EnterpriseDto>>();
    }

    public async Task<EnterpriseDto> UpdateEnterpriseAsync(UpdateEnterpriseDto updateEnterpriseDto)
    {
        _logger.LogInformation("Atualizando empreendimento {IdEnterprise}.", updateEnterpriseDto.Id);

        var enterprise = updateEnterpriseDto.Adapt<Enterprise>();

        var updated = await _enterpriseRepository.UpdateEnterpriseAsync(enterprise);

        if (updated == null)
        {
            _logger.LogWarning("Falha ao atualizar empreendimento {IdEnterprise}.", updateEnterpriseDto.Id);
            return null;
        }

        _logger.LogInformation("Empreendimento {IdEnterprise} atualizado com sucesso.", updateEnterpriseDto.Id);
        return updated.Adapt<EnterpriseDto>();
    }

    public async Task<EnterpriseDto> UnactivateEnterpriseAsycn(UpdateEnterpriseDto updateEnterpriseDto)
    {
        _logger.LogInformation("Desativando empreendimento {IdEnterprise}.", updateEnterpriseDto.Id);

        var enterprise = updateEnterpriseDto.Adapt<Enterprise>();

        var unactivated = await _enterpriseRepository.UnactivateEnterpriseAsync(enterprise);

        if (unactivated == null)
        {
            _logger.LogWarning("Falha ao desativar empreendimento {IdEnterprise}.", updateEnterpriseDto.Id);
            return null;
        }

        _logger.LogInformation("Empreendimento {IdEnterprise} desativado com sucesso.", updateEnterpriseDto.Id);
        return unactivated.Adapt<EnterpriseDto>();
    }
}
