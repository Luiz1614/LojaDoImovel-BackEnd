using LojaDoImovel.Application.Integrations.Interfaces;
using LojaDoImovel.Contracts.DTOs.Integrations;

namespace LojaDoImovel.Application.Services;

public class CepService : ICepService
{
    private readonly ICepIntegration _cepIntegration;

    public CepService(ICepIntegration cepIntegration)
    {
        _cepIntegration = cepIntegration;
    }

    public async Task<CepResponseDto> GetCepDataAsync(string cep)
    {
        var response = await _cepIntegration.GetCep(cep);

        if (response != null && response.IsSuccessStatusCode)
        {
            return response.Content!;
        }

        return null;
    }
}
