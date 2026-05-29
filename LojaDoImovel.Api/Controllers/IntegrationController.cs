using LojaDoImovel.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LojaDoImovel.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class IntegrationController : ControllerBase
{
    private readonly ICepService _cepService;
    public IntegrationController(ICepService cepService)
    {
        _cepService = cepService;
    }

    [HttpGet("{cep}")]
    public async Task<IActionResult> GetCep(string cep)
    {
        var address = await _cepService.GetCepDataAsync(cep);

        if (address == null)
        {
            return BadRequest("Endereço não encontrado, verifique os dados enviados.");
        }

        return Ok(address);
    }
}
