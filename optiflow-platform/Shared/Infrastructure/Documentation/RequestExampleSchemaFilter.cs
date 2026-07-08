using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using optiflow_platform.IAM.Interfaces.REST.Resources;
using optiflow_platform.Inventory.Interfaces.REST.Resources;
using optiflow_platform.LabAndOrders.Interfaces.REST.Resources;

namespace optiflow_platform.Shared.Infrastructure.Documentation;

/// <summary>
///     Attaches example request bodies to the most-used request payloads so the Swagger
///     "Try it out" panel is prefilled with realistic values, making the API easier to debug.
///     Keys are the request resource types; the JSON uses the camelCase names the API serializes.
/// </summary>
public class RequestExampleSchemaFilter : ISchemaFilter
{
    private static readonly Dictionary<Type, string> Examples = new()
    {
        [typeof(SignInResource)] = """
            { "email": "admin@optiflow.com", "password": "Passw0rd!" }
            """,
        [typeof(SignUpResource)] = """
            { "email": "owner@myoptics.com", "password": "Passw0rd!", "userType": "admin" }
            """,
        [typeof(UpdateUserEmailResource)] = """
            { "newEmail": "new.address@myoptics.com" }
            """,
        [typeof(UpdateUserPasswordResource)] = """
            { "currentPassword": "Passw0rd!", "newPassword": "N3wPassw0rd!" }
            """,
        [typeof(CreateWorkOrderResource)] = """
            {
              "saleId": 1,
              "recipeId": 1,
              "labId": 1,
              "patientName": "María Fernández",
              "laboratoryName": "Laboratorio Óptico Central",
              "lensType": "Progresivas",
              "lensProductId": 12,
              "frame": "Ray-Ban RB5154",
              "frameProductId": 34,
              "prescription": "OD -1.25 OS -1.50 ADD +2.00",
              "priority": "normal",
              "deliveryDate": "2026-07-20",
              "deposit": 100.0,
              "total": 450.0
            }
            """,
        [typeof(RegisterProductResource)] = """
            {
              "category": "Frames",
              "supplierId": 1,
              "supplierName": "Distribuidora Óptica S.A.",
              "sku": "FR-RB5154-BLK",
              "name": "Ray-Ban RB5154 Clubmaster",
              "brand": "Ray-Ban",
              "model": "RB5154",
              "price": 320.0,
              "stock": 15,
              "minimumStockThreshold": 3
            }
            """,
        [typeof(RestockProductResource)] = """
            { "quantity": 10, "author": "Carlos Ramírez" }
            """,
    };

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        // Only the concrete OpenApiSchema exposes a settable Example; the interface is read-only.
        if (schema is OpenApiSchema concrete && Examples.TryGetValue(context.Type, out var json))
            concrete.Example = JsonNode.Parse(json);
    }
}
