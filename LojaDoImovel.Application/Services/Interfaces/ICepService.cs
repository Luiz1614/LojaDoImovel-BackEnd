using LojaDoImovel.Contracts.DTOs.Integrations;

namespace LojaDoImovel.Application.Services
{
    public interface ICepService
    {
        Task<CepResponseDto> GetCepDataAsync(string cep);
    }
}