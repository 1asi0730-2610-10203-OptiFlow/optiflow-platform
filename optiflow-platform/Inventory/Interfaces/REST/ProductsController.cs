using System.Net.Mime;
using optiflow_platform.Inventory.Application.Errors;
using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Aggregates;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Interfaces.REST.Resources;
using optiflow_platform.Inventory.Interfaces.REST.Transform;
using optiflow_platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace optiflow_platform.Inventory.Interfaces.REST;

/// <summary>
///     Products controller.
/// </summary>
[ApiController]
[Route("/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Products")]
[Authorize]
public class ProductsController(
    IProductCommandService productCommandService,
    IProductQueryService productQueryService,
    ILogger<ProductsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Registers a new product.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Registers a product",
        Description = "Registers a new product in the catalog with its initial stock and minimum stock threshold",
        OperationId = "RegisterProduct")]
    [SwaggerResponse(201, "The product was registered", typeof(ProductResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(409, "A product with the given SKU already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RegisterProduct([FromBody] RegisterProductResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = RegisterProductCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await productCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Product, RegisterProductError>.Success success =>
                    CreatedAtAction(nameof(GetProductById),
                        new { id = success.Value.Id },
                        ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Product, RegisterProductError>.Failure { Error: RegisterProductError.DuplicateSku } =>
                    Problem(title: "Duplicate SKU",
                        detail: $"A product with SKU '{resource.Sku}' already exists.", statusCode: 409),
                _ => Problem(title: "Unexpected error", detail: "Could not register product.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while registering product with SKU {Sku}", resource.Sku);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering product with SKU {Sku}", resource.Sku);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while registering the product.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Gets all products.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all products",
        Description = "Returns all products in the catalog",
        OperationId = "GetAllProducts")]
    [SwaggerResponse(200, "List of products", typeof(IEnumerable<ProductResource>))]
    public async Task<ActionResult> GetAllProducts(CancellationToken cancellationToken = default)
    {
        var query = new GetAllProductsQuery();
        var result = await productQueryService.Handle(query, cancellationToken);
        var resources = result.Select(ProductResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets the products with low stock.
    /// </summary>
    [HttpGet("low-stock")]
    [SwaggerOperation(
        Summary = "Gets products with low stock",
        Description = "Returns the products whose stock is at or below their minimum stock threshold, backing the Critical Stock Monitor view",
        OperationId = "GetLowStockProducts")]
    [SwaggerResponse(200, "List of low stock products", typeof(IEnumerable<ProductResource>))]
    public async Task<ActionResult> GetLowStockProducts(CancellationToken cancellationToken = default)
    {
        var query = new GetLowStockProductsQuery();
        var result = await productQueryService.Handle(query, cancellationToken);
        var resources = result.Select(ProductResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a product by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets a product by id",
        Description = "Returns a single product for the given identifier",
        OperationId = "GetProductById")]
    [SwaggerResponse(200, "The product was found", typeof(ProductResource))]
    [SwaggerResponse(404, "The product was not found")]
    public async Task<ActionResult> GetProductById(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetProductByIdQuery(id);
        var result = await productQueryService.Handle(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    /// <summary>
    ///     Verifies whether a product has stock available to satisfy a supply request.
    /// </summary>
    [HttpGet("{id:int}/availability")]
    [SwaggerOperation(
        Summary = "Verifies a product's stock availability",
        Description = "Checks whether a product has stock available to satisfy a supply request from an external order",
        OperationId = "VerifyProductSupplyStock")]
    [SwaggerResponse(200, "The product has stock available", typeof(ProductResource))]
    [SwaggerResponse(404, "The product was not found")]
    [SwaggerResponse(409, "The product has no stock available", typeof(ProblemDetails))]
    public async Task<ActionResult> VerifySupplyStock(int id, CancellationToken cancellationToken)
    {
        var command = new VerifySupplyStockCommand(id);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Product, VerifySupplyStockError>.Success success =>
                Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Product, VerifySupplyStockError>.Failure { Error: VerifySupplyStockError.ProductNotFound } =>
                NotFound(),
            Result<Product, VerifySupplyStockError>.Failure { Error: VerifySupplyStockError.OutOfStock } =>
                Problem(title: "Out of stock",
                    detail: $"Product {id} has no stock available to satisfy the supply request.", statusCode: 409),
            _ => Problem(title: "Unexpected server error",
                detail: "Could not verify product stock availability.", statusCode: 500)
        };
    }

    /// <summary>
    ///     Updates the catalog details of a product.
    /// </summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates a product's catalog details",
        Description = "Updates the name, SKU, category, price and minimum stock threshold of a product",
        OperationId = "UpdateProduct")]
    [SwaggerResponse(200, "The product was updated", typeof(ProductResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "The product was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] UpdateProductResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateProductCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var result = await productCommandService.Handle(command, cancellationToken);
            return result switch
            {
                Result<Product, UpdateProductError>.Success success =>
                    Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
                Result<Product, UpdateProductError>.Failure { Error: UpdateProductError.ProductNotFound } =>
                    NotFound(),
                _ => Problem(title: "Unexpected server error",
                    detail: "Could not update product.", statusCode: 500)
            };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while updating product {ProductId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while updating product {ProductId}", id);
            return Problem(title: "Unexpected server error",
                detail: "An unexpected error occurred while updating the product.", statusCode: 500);
        }
    }

    /// <summary>
    ///     Sets a product's stock to an absolute level.
    /// </summary>
    [HttpPatch("{id:int}/stock")]
    [SwaggerOperation(
        Summary = "Updates a product's stock level",
        Description = "Sets the stock of a product to an absolute quantity, e.g. after a physical recount",
        OperationId = "UpdateProductStockLevel")]
    [SwaggerResponse(200, "The stock level was updated", typeof(ProductResource))]
    [SwaggerResponse(404, "The product was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateStockLevel(int id, [FromBody] UpdateStockLevelResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateStockLevelCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Product, UpdateStockLevelError>.Success success =>
                Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Product, UpdateStockLevelError>.Failure { Error: UpdateStockLevelError.ProductNotFound } =>
                NotFound(),
            _ => Problem(title: "Unexpected server error",
                detail: "Could not update product stock level.", statusCode: 500)
        };
    }

    /// <summary>
    ///     Restocks a product and records the corresponding audit log entry.
    /// </summary>
    [HttpPost("{id:int}/restock")]
    [SwaggerOperation(
        Summary = "Restocks a product",
        Description = "Adds replenished stock to a product, refreshes its last restock date, and logs the operation in the audit history",
        OperationId = "RestockProduct")]
    [SwaggerResponse(200, "The product was restocked", typeof(ProductResource))]
    [SwaggerResponse(400, "The restock quantity is invalid", typeof(string))]
    [SwaggerResponse(404, "The product was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RestockProduct(int id, [FromBody] RestockProductResource resource,
        CancellationToken cancellationToken)
    {
        var command = RestockProductCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Product, RestockProductError>.Success success =>
                Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Product, RestockProductError>.Failure { Error: RestockProductError.ProductNotFound } =>
                NotFound(),
            Result<Product, RestockProductError>.Failure { Error: RestockProductError.InvalidQuantity } =>
                BadRequest("Restock quantity must be greater than zero."),
            _ => Problem(title: "Unexpected server error",
                detail: "Could not restock product.", statusCode: 500)
        };
    }

    /// <summary>
    ///     Confirms a manual stock adjustment and records the corresponding audit log entry.
    /// </summary>
    [HttpPost("{id:int}/adjustments")]
    [SwaggerOperation(
        Summary = "Logs a manual stock adjustment",
        Description = "Confirms a manual correction of a product's stock backed by a justification, and logs the operation in the audit history",
        OperationId = "LogManualStockAdjustment")]
    [SwaggerResponse(200, "The adjustment was confirmed", typeof(ProductResource))]
    [SwaggerResponse(400, "The justification is missing", typeof(string))]
    [SwaggerResponse(404, "The product was not found")]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> LogManualAdjustment(int id, [FromBody] LogManualAdjustmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = LogManualAdjustmentCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Product, LogManualAdjustmentError>.Success success =>
                Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Product, LogManualAdjustmentError>.Failure { Error: LogManualAdjustmentError.ProductNotFound } =>
                NotFound(),
            Result<Product, LogManualAdjustmentError>.Failure { Error: LogManualAdjustmentError.JustificationRequired } =>
                BadRequest("A justification is required to confirm a stock adjustment."),
            _ => Problem(title: "Unexpected server error",
                detail: "Could not log manual stock adjustment.", statusCode: 500)
        };
    }

    /// <summary>
    ///     Consumes a product's stock and records the corresponding audit log entry.
    /// </summary>
    [HttpPost("{id:int}/consume")]
    [SwaggerOperation(
        Summary = "Consumes a product's stock",
        Description = "Deducts stock consumed to fulfill an external order, e.g. lab order material, and logs the operation in the audit history",
        OperationId = "ConsumeProductStock")]
    [SwaggerResponse(200, "The stock was consumed", typeof(ProductResource))]
    [SwaggerResponse(400, "The consumption quantity is invalid", typeof(string))]
    [SwaggerResponse(404, "The product was not found")]
    [SwaggerResponse(409, "The product does not have enough stock available", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ConsumeStock(int id, [FromBody] ConsumeStockResource resource,
        CancellationToken cancellationToken)
    {
        var command = ConsumeStockCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result switch
        {
            Result<Product, ConsumeStockError>.Success success =>
                Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<Product, ConsumeStockError>.Failure { Error: ConsumeStockError.ProductNotFound } =>
                NotFound(),
            Result<Product, ConsumeStockError>.Failure { Error: ConsumeStockError.InvalidQuantity } =>
                BadRequest("Consumption quantity must be greater than zero."),
            Result<Product, ConsumeStockError>.Failure { Error: ConsumeStockError.InsufficientStock } =>
                Problem(title: "Insufficient stock",
                    detail: $"Product {id} does not have enough stock to fulfill this consumption.", statusCode: 409),
            _ => Problem(title: "Unexpected server error",
                detail: "Could not consume product stock.", statusCode: 500)
        };
    }
}
