using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Shared.Application.Patterns;

namespace optiflow_platform.LabAndOrders.Interfaces.Acl;

/// <summary>
///     Implements <see cref="IInventoryContextFacade"/> by delegating to Inventory application services.
/// </summary>
/// <remarks>
///     LabAndOrders never imports Inventory domain types directly — only this facade does.
///     If Inventory internals change, only this class needs updating.
/// </remarks>
public class InventoryContextFacade(IProductCommandService productCommandService)
    : IInventoryContextFacade
{
    /// <inheritdoc />
    public async Task<bool> ConsumeStockAsync(int productId, int quantity, string author,
        CancellationToken cancellationToken = default)
    {
        var command = new ConsumeStockCommand(productId, quantity, author);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result.IsSuccess;
    }
}
