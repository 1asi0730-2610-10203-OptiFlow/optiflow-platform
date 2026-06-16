using System.Net.Mime;
using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Interfaces.REST.Resources;
using optiflow_platform.Inventory.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
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
public class SuppliersController(
    ISupplierCommandService supplierCommandService,
    ISupplierQueryService supplierQueryService,
    ILogger<SuppliersController> logger) : ControllerBase
{
    /// <summary>
    ///     Registers a new supplier.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Registers a supplier",
        Description = "Creates a new supplier with the given name and contact information",
        OperationId = "RegisterSupplier")]
    [SwaggerResponse(201, "The supplier was registered", typeof(SupplierResource))]
    [SwaggerResponse(400, "The request payload is invalid or the name is already taken", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RegisterSupplier([FromBody] RegisterSupplierResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = RegisterSupplierCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await supplierCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Supplier, CreateSupplierError>.Success success =>
                    CreatedAtAction(nameof(GetSupplierById),
                        new { id = success.Value.Id },
                        SupplierResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Supplier, CreateSupplierError>.Failure { Error: CreateSupplierError.DuplicateName } =>
                    BadRequest($"A supplier named '{resource.Name}' already exists."),
                Result<Supplier, CreateSupplierError>.Failure { Error: CreateSupplierError.InvalidContact } =>
                    BadRequest("The provided contact person, phone, or email is invalid."),
                _ => Problem(title: "Unexpected error", detail: "Could not register the supplier.", statusCode: 500)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering supplier {Name}", resource.Name);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while registering the supplier.", statusCode: 500);
        }
    }

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

    /// <summary>
    ///     Gets a supplier by id.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Gets a supplier by id",
        Description = "Returns a single supplier for the given identifier",
        OperationId = "GetSupplierById")]
    [SwaggerResponse(200, "The supplier was found", typeof(SupplierResource))]
    [SwaggerResponse(404, "The supplier was not found")]
    public async Task<ActionResult> GetSupplierById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetSupplierByIdQuery(id);
        var result = await supplierQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(SupplierResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
}
