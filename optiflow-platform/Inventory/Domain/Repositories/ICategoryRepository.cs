using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Shared.Domain.Repositories;

namespace optiflow_platform.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for <see cref="Category"/> persistence.
/// </summary>
public interface ICategoryRepository : IBaseRepository<Category>
{
}
