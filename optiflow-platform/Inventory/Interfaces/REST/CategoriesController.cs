using System.Net.Mime;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Interfaces.REST.Resources;
using optiflow_platform.Inventory.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.Inventory.Interfaces.REST;

/// <summary>
///     Categories controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Categories")]
public class CategoriesController(ICategoryQueryService categoryQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets all product categories.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all product categories",
        Description = "Returns all registered product categories",
        OperationId = "GetAllCategories")]
    [SwaggerResponse(200, "List of categories", typeof(IEnumerable<CategoryResource>))]
    public async Task<ActionResult> GetAllCategories(CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesQuery();
        var result = await categoryQueryService.Handle(query, cancellationToken);
        var resources = result.Select(CategoryResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
