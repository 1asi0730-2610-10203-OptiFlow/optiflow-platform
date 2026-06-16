using System.Net.Mime;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Interfaces.REST.Resources;
using optiflow_platform.Inventory.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST;

/// <summary>
///     Suppliers controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Suppliers")]
public class SuppliersController(ISupplierQueryService supplierQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets all suppliers.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all suppliers",
        Description = "Returns all registered suppliers",
        OperationId = "GetAllSuppliers")]
    [SwaggerResponse(200, "List of suppliers", typeof(IEnumerable<SupplierResource>))]
    public async Task<ActionResult> GetAllSuppliers(CancellationToken cancellationToken = default)
    {
        var query = new GetAllSuppliersQuery();
        var result = await supplierQueryService.Handle(query, cancellationToken);
        var resources = result.Select(SupplierResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
