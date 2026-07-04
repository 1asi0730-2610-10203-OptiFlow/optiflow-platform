using optiflow_platform.LabAndOrders.Domain.Model.Commands;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.LabAndOrders.Interfaces.REST.Transform;

/// <summary>
///     Assembles a LinkSaleCommand from a work order id and a LinkSaleResource.
/// </summary>
public static class LinkSaleCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a work order id and a LinkSaleResource to a LinkSaleCommand.
    /// </summary>
    public static LinkSaleCommand ToCommandFromResource(int workOrderId, LinkSaleResource resource) =>
        new(workOrderId, resource.SaleId);
}
