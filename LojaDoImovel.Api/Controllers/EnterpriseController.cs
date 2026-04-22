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

    /// <summary>
    /// Creates a new enterprise using the provided data.
    /// </summary>
    /// <param name="createEnterpriseDto">The data transfer object containing the information required to create a new enterprise. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IActionResult indicating the
    /// outcome of the creation request: returns a 201 Created status if successful, or a 400 Bad Request status if the
    /// input data is invalid.</returns>
    [HttpPost]
    public async Task<IActionResult> Post(CreateEnterpriseDto createEnterpriseDto)
    {
        var result = await _enterpriseService.AddEnterpriseAsync(createEnterpriseDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível adicionar o empreendimento. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Empreendimento adicionado com sucesso!");
    }

    /// <summary>
    /// Retrieves all enterprise records.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the list of all enterprises if found; otherwise, a response with
    /// status code 404 (Not Found) if no enterprises exist.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _enterpriseService.GetAllEnterpriseAsync();

        if(result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum empreendimento encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Retrieves the enterprise details for the specified enterprise identifier.
    /// </summary>
    /// <param name="idEnterprise">The unique identifier of the enterprise to retrieve. Must be a valid enterprise ID.</param>
    /// <returns>A 200 OK response containing the enterprise details if found; otherwise, a 404 Not Found response if no
    /// enterprise exists with the specified identifier.</returns>
    [HttpGet("id")]
    public async Task<IActionResult> GetById(int idEnterprise)
    {
        var result = await _enterpriseService.GetEnterpriseByIdAsync(idEnterprise);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum empreendimento encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Retrieves an enterprise by its name.
    /// </summary>
    /// <remarks>This method returns a 404 Not Found status if no enterprise with the specified name
    /// exists.</remarks>
    /// <param name="name">The name of the enterprise to retrieve. Cannot be null or empty.</param>
    /// <returns>A 200 OK response containing the enterprise if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("name")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _enterpriseService.GetEnterpriseByNameAsync(name);

        if (result == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhum empreendimento encontrado.");

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    /// <summary>
    /// Updates the details of an existing enterprise using the specified data transfer object.
    /// </summary>
    /// <param name="updateEnterpriseDto">An object containing the updated information for the enterprise. All required fields must be provided and valid.</param>
    /// <returns>A status code indicating the result of the update operation. Returns 204 (No Content) if the update is
    /// successful; otherwise, returns 400 (Bad Request) if the update could not be performed due to invalid data.</returns>
    [HttpPut]
    public async Task<IActionResult> Put(UpdateEnterpriseDto updateEnterpriseDto)
    {
        var result = await _enterpriseService.UpdateEnterpriseAsync(updateEnterpriseDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar o empreendimento. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Empreendimento atualizado com sucesso!");
    }

    /// <summary>
    /// Deactivates an enterprise based on the provided update information.
    /// </summary>
    /// <param name="updateEnterpriseDto">An object containing the data required to identify and update the enterprise to be deactivated. Cannot be null.</param>
    /// <returns>A status code indicating the result of the operation. Returns 204 (No Content) if the enterprise was
    /// successfully deactivated; returns 400 (Bad Request) if the operation could not be completed due to invalid
    /// input.</returns>
    [HttpPut("unactivate")]
    public async Task<IActionResult> Unactivate(UpdateEnterpriseDto updateEnterpriseDto)
    {
        var result = await _enterpriseService.UnactivateEnterpriseAsycn(updateEnterpriseDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível desativar o empreendimento. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Empreendimento desativado com sucesso!");
    }
}

