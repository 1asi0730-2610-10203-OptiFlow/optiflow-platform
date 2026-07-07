using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.Subscription.Interfaces.REST.Resources;

public record CreatePlanResource(
    [StringLength(100, ErrorMessage = "Name must be at most 100 characters")] string Name,
    [StringLength(50, ErrorMessage = "Tier must be at most 50 characters")] string Tier,
    [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1000000")] decimal Price,
    [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters")] string Description);
