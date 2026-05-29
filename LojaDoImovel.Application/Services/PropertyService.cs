using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Property;
using LojaDoImovel.Contracts.DTOs.PropertyDtos;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace LojaDoImovel.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(IPropertyRepository propertyRepository, ILogger<PropertyService> logger)
    {
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    public async Task<PropertyDto> AddPropertyAsync(CreatePropertyDto createPropertyDto)
    {
        _logger.LogInformation("Iniciando criação de imóvel para o empreendimento {IdEnterprise}.", createPropertyDto.EnterpriseId);

        var property = createPropertyDto.Adapt<Property>();

        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;

        var created = await _propertyRepository.AddPropertyAsync(property);

        if (created == null)
        {
            _logger.LogWarning("Falha ao criar imóvel para o empreendimento {IdEnterprise}.", createPropertyDto.EnterpriseId);
            return null;
        }

        _logger.LogInformation("Imóvel criado com sucesso. Id: {Id}", created.Id);
        return created.Adapt<PropertyDto>();
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(int idEnterprise)
    {
        _logger.LogInformation("Buscando imóveis do empreendimento {IdEnterprise}.", idEnterprise);

        var properties = await _propertyRepository.GetAllPropertiesAsync(idEnterprise);

        if (properties == null)
        {
            _logger.LogWarning("Nenhum imóvel encontrado para o empreendimento {IdEnterprise}.", idEnterprise);
            return null;
        }

        return properties.Adapt<IEnumerable<PropertyDto>>();
    }

    public async Task<PropertyDto> GetPropertyByIdAsync(int idProperty, int idEnterprise)
    {
        _logger.LogInformation("Buscando imóvel {IdProperty} no empreendimento {IdEnterprise}.", idProperty, idEnterprise);

        var property = await _propertyRepository.GetPropertyByIdAsync(idProperty, idEnterprise);

        if (property == null)
        {
            _logger.LogWarning("Imóvel {IdProperty} não encontrado no empreendimento {IdEnterprise}.", idProperty, idEnterprise);
            return null;
        }

        return property.Adapt<PropertyDto>();
    }

    public async Task<PropertyDto> GetPropertyByCodeAsync(string code, int idEnterprise)
    {
        _logger.LogInformation("Buscando imóvel com código '{Code}' no empreendimento {IdEnterprise}.", code, idEnterprise);

        var property = await _propertyRepository.GetPropertyByCodeAsync(code, idEnterprise);

        if (property == null)
        {
            _logger.LogWarning("Imóvel com código '{Code}' não encontrado no empreendimento {IdEnterprise}.", code, idEnterprise);
            return null;
        }

        return property.Adapt<PropertyDto>();
    }

    public async Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto)
    {
        _logger.LogInformation("Atualizando imóvel {IdProperty}.", updatePropertyDto.Id);

        var property = updatePropertyDto.Adapt<Property>();

        property.UpdatedAt = DateTime.UtcNow;

        var updated = await _propertyRepository.UpdatePropertyAsync(property);

        if (updated == null)
        {
            _logger.LogWarning("Falha ao atualizar imóvel {IdProperty}.", updatePropertyDto.Id);
            return null;
        }

        _logger.LogInformation("Imóvel {IdProperty} atualizado com sucesso.", updatePropertyDto.Id);
        return updated.Adapt<PropertyDto>();
    }

    public async Task<bool> DeletePropertyAsync(int idProperty)
    {
        _logger.LogInformation("Deletando imóvel {IdProperty}.", idProperty);

        var result = await _propertyRepository.DeletePropertyAsync(idProperty);

        if (!result)
            _logger.LogWarning("Imóvel {IdProperty} não encontrado para exclusão.", idProperty);
        else
            _logger.LogInformation("Imóvel {IdProperty} deletado com sucesso.", idProperty);

        return result;
    }
}
