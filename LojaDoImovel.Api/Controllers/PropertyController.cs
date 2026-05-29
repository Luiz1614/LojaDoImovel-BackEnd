using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.Property;
using LojaDoImovel.Contracts.DTOs.PropertyDtos;
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

    /// <summary>
    /// Creates a new property using the specified property details.
    /// </summary>
    /// <remarks>This action requires the caller to be authorized with the 'userapproved' role.</remarks>
    /// <param name="createPropertyDto">The data transfer object containing the details of the property to create. Cannot be null.</param>
    /// <returns>An IActionResult indicating the result of the operation. Returns a 201 Created status if the property is added
    /// successfully; otherwise, returns a 400 Bad Request status if the property could not be added.</returns>
    [HttpPost]
    [Authorize(Roles = "userapproved")]
    public async Task<IActionResult> Post([FromBody] CreatePropertyDto createPropertyDto)
    {
        var result = await _propertyService.AddPropertyAsync(createPropertyDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível adicionar o imóvel. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Imóvel adicionado com sucesso!");
    }

    /// <summary>
    /// Retrieves all properties associated with the specified enterprise.
    /// </summary>
    /// <param name="idEnterprise">The unique identifier of the enterprise for which to retrieve properties.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of properties if found; otherwise, a response with status
    /// code 404 if no properties are found.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(int idEnterprise)
    {
        var result = await _propertyService.GetAllPropertiesAsync(idEnterprise);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Retrieves the property details for the specified property and enterprise identifiers.
    /// </summary>
    /// <param name="idProperty">The unique identifier of the property to retrieve.</param>
    /// <param name="idEnterprise">The unique identifier of the enterprise associated with the property.</param>
    /// <returns>An <see cref="IActionResult"/> containing the property details if found; otherwise, a response with status code
    /// 404 (Not Found).</returns>
    [HttpGet("id")]
    public async Task<IActionResult> GetById(int idProperty, int idEnterprise)
    {
        var result = await _propertyService.GetPropertyByIdAsync(idProperty, idEnterprise);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Retrieves a property by its unique code and enterprise identifier.  
    /// </summary>
    /// <param name="code">The unique code that identifies the property to retrieve. Cannot be null or empty.</param>
    /// <param name="idEnterprise">The identifier of the enterprise to which the property belongs.</param>
    /// <returns>An IActionResult containing the property data if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("code")]
    public async Task<IActionResult> GetByCode(string code, int idEnterprise)
    {
        var result = await _propertyService.GetPropertyByCodeAsync(code, idEnterprise);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Updates an existing property with the specified data.
    /// </summary>
    /// <remarks>This action requires the caller to be authorized with the 'userapproved' role. The update
    /// operation is performed asynchronously.</remarks>
    /// <param name="updatePropertyDto">An object containing the updated property information. Must not be null and should include all required fields
    /// for the update operation.</param>
    /// <returns>An IActionResult indicating the result of the update operation. Returns status code 204 (No Content) if the
    /// update is successful; otherwise, returns status code 400 (Bad Request) if the update fails due to invalid data.</returns>
    [HttpPut]
    [Authorize(Roles = "userapproved")]
    public async Task<IActionResult> Put([FromBody] UpdatePropertyDto updatePropertyDto)
    {
        var result = await _propertyService.UpdatePropertyAsync(updatePropertyDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar o imóvel. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Imóvel atualizado com sucesso!");

    }

    /// <summary>
    /// Deletes the property with the specified identifier.
    /// </summary>
    /// <param name="idProperty">The unique identifier of the property to delete. Must be a valid property ID.</param>
    /// <returns>An IActionResult indicating the result of the delete operation. Returns status code 204 (No Content) if the
    /// property was deleted successfully, or 404 (Not Found) if no property with the specified identifier exists.</returns>
    [HttpDelete]
    [Authorize(Roles = "userapproved")]
    public async Task<IActionResult> Delete(int idProperty)
    {
        var result = await _propertyService.DeletePropertyAsync(idProperty);

        if (!result)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum imóvel encontrado.");

        return StatusCode((int)HttpStatusCode.NoContent, "Imóvel deletado com sucesso!");
    }
}
