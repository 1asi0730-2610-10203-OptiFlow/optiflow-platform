using Swashbuckle.AspNetCore.Annotations;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Resources;

[SwaggerSchema(Description = "A patient order resource combining work order and payment data")]
public record PatientOrderResource(
    [SwaggerParameter("Work order ID")]          int      WorkOrderId,
    [SwaggerParameter("Sale ID")]                int      SaleId,
    [SwaggerParameter("Order number")]           string   OrderNumber,
    [SwaggerParameter("Patient name")]           string   PatientName,
    [SwaggerParameter("Lens type")]              string   LensType,
    [SwaggerParameter("Frame")]                  string   Frame,
    [SwaggerParameter("Work order status")]      string   Status,
    [SwaggerParameter("Priority")]               string   Priority,
    [SwaggerParameter("Estimated delivery")]     string   DeliveryDate,
    [SwaggerParameter("Creation date")]          string   CreatedAt,
    [SwaggerParameter("Total amount")]           decimal  Total,
    [SwaggerParameter("Deposit paid")]           decimal  Deposit,
    [SwaggerParameter("Pending balance")]        decimal  PendingBalance,
    [SwaggerParameter("Is rework")]              bool     IsRework);