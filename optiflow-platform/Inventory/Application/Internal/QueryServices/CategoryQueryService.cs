using optiflow_platform.Inventory.Application.Services;
using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Domain.Model.Queries;
using optiflow_platform.Inventory.Domain.Repositories;

namespace optiflow_platform.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Application service for handling category queries.
/// </summary>
/// <param name="categoryRepository">Repository for accessing category data.</param>
public class CategoryQueryService(ICategoryRepository categoryRepository) : ICategoryQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery query,
        CancellationToken cancellationToken = default) =>
        await categoryRepository.ListAsync(cancellationToken);
}
