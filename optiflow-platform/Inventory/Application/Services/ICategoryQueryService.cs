using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Model.Queries;

namespace optiflow_platform.Inventory.Application.Services;

/// <summary>
///     Contract for category query operations.
/// </summary>
public interface ICategoryQueryService
{
    /// <summary>Returns all product categories.</summary>
    Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken = default);
}
