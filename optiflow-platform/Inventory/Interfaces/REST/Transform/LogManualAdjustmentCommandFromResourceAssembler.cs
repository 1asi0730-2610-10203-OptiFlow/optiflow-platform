using optiflow_platform.Inventory.Domain.Model.Commands;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a LogManualAdjustmentCommand from a product id and a LogManualAdjustmentResource.
/// </summary>
public static class LogManualAdjustmentCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a product id and a LogManualAdjustmentResource to a LogManualAdjustmentCommand.
    /// </summary>
    public static LogManualAdjustmentCommand ToCommandFromResource(int productId, LogManualAdjustmentResource resource) =>
        new(productId, resource.NewStock, resource.Justification, resource.Author);
}
