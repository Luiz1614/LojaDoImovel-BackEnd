using LojaDoImovel.Application.Integrations.Interfaces;
using LojaDoImovel.Contracts.DTOs.Integrations;
using Microsoft.Extensions.Logging;

namespace LojaDoImovel.Application.Services;

public class CepService : ICepService
{
    private readonly ICepIntegration _cepIntegration;
    private readonly ILogger<CepService> _logger;

    public CepService(ICepIntegration cepIntegration, ILogger<CepService> logger)
    {
        _cepIntegration = cepIntegration;
        _logger = logger;
    }

    public async Task<CepResponseDto> GetCepDataAsync(string cep)
    {
        _logger.LogInformation("Consultando CEP {Cep} na integração externa.", cep);

        var response = await _cepIntegration.GetCep(cep);

        if (response != null && response.IsSuccessStatusCode)
        {
            _logger.LogInformation("CEP {Cep} encontrado com sucesso.", cep);
            return response.Content!;
        }

        _logger.LogWarning("CEP {Cep} não encontrado ou integração retornou erro. StatusCode: {StatusCode}",
            cep, response?.StatusCode);
        return null;
    }
}
