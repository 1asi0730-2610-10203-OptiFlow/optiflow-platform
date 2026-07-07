using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

public record CreatePlanResource(
    string Name,
    string Tier,
    [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1000000")] decimal Price,
    string Description);
