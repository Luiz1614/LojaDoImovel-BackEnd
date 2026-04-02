using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Enterprise;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LojaDoImovel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnterpriseController : ControllerBase
{
    private readonly IEnterpriseService _enterpriseService;
    private readonly ILogger<EnterpriseController> _logger;

    public EnterpriseController(IEnterpriseService enterpriseService, ILogger<EnterpriseController> logger)
    {
        _enterpriseService = enterpriseService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateEnterpriseDto createEnterpriseDto)
    {
        var result = await _enterpriseService.AddEnterpriseAsync(createEnterpriseDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível adicionar o empreendimento. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Empreendimento adicionado com sucesso!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _enterpriseService.GetAllEnterpriseAsync();

        if(result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum empreendimento encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(int idEnterprise)
    {
        var result = await _enterpriseService.GetEnterpriseByIdAsync(idEnterprise);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum empreendimento encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [HttpGet("name")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _enterpriseService.GetEnterpriseByNameAsync(name);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum empreendimento encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [HttpPut]
    public async Task<IActionResult> Put(UpdateEnterpriseDto updateEnterpriseDto)
    {
        var result = await _enterpriseService.UpdateEnterpriseAsync(updateEnterpriseDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar o empreendimento. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Empreendimento atualizado com sucesso!");
    }

    [HttpPut("unactivate")]
    public async Task<IActionResult> Unactivate(UpdateEnterpriseDto updateEnterpriseDto)
    {
        var result = await _enterpriseService.UnactivateEnterpriseAsycn(updateEnterpriseDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível desativar o empreendimento. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Empreendimento desativado com sucesso!");
    }
}

