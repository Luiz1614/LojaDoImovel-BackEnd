using LojaDoImovel.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LojaDoImovel.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class IntegrationController : ControllerBase
{
    private readonly ICepService _cepService;
    private readonly ILogger<IntegrationController> _logger;

    public IntegrationController(ICepService cepService, ILogger<IntegrationController> logger)
    {
        _cepService = cepService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves address information for the specified CEP (Brazilian postal code).
    /// </summary>
    /// <param name="cep">The CEP (postal code) to look up. Must be a valid Brazilian postal code.</param>
    /// <returns>An HTTP 200 response containing the address data if the CEP is found; otherwise, a 400 Bad Request
    /// response if the CEP is invalid or not found.</returns>
    [HttpGet("{cep}")]
    public async Task<IActionResult> GetCep(string cep)
    {
        _logger.LogInformation("Buscando dados do CEP {Cep}.", cep);

        var address = await _cepService.GetCepDataAsync(cep);

        if (address == null)
        {
            _logger.LogWarning("CEP {Cep} não encontrado.", cep);
            return BadRequest("Endereço não encontrado, verifique os dados enviados.");
        }

        _logger.LogInformation("Dados do CEP {Cep} retornados com sucesso.", cep);
        return Ok(address);
    }
}
