using LojaDoImovel.Contracts.DTOs.Integrations;
using Refit;

namespace LojaDoImovel.Application.Integrations.Interfaces;

public interface ICepIntegration
{
    [Get("/ws/{cep}/json/ ")]
    Task<ApiResponse<CepResponseDto>> GetCep(string cep);
}
