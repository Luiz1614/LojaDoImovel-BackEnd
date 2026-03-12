using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Property;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LojaDoImovel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertyController> _logger;

    public PropertyController(IPropertyService propertyService, ILogger<PropertyController> logger)
    {
        _propertyService = propertyService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePropertyDto createPropertyDto)
    {
        var result = await _propertyService.AddPropertyAsync(createPropertyDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível adicionar o imóvel. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Imóvel adicionado com sucesso!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _propertyService.GetAllPropertiesAsync();

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _propertyService.GetPropertyByIdAsync(id);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [HttpGet("code")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await _propertyService.GetPropertyByCodeAsync(code);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [HttpPut]
    public async Task<IActionResult> Put(UpdatePropertyDto updatePropertyDto)
    {
        var result = await _propertyService.UpdatePropertyAsync(updatePropertyDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar o imóvel. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Imóvel atualizado com sucesso!");

    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _propertyService.DeletePropertyAsync(id);

        if (!result)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.NoContent, "Imóvel deletado com sucesso!");
    }
}
