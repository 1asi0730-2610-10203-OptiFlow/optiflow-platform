using optiflow_platform.Inventory.Domain.Model.Entities;
using optiflow_platform.Inventory.Interfaces.REST.Resources;

namespace optiflow_platform.Inventory.Interfaces.REST.Transform;

/// <summary>
///     Assembles a CategoryResource from a Category entity.
/// </summary>
public static class CategoryResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a Category entity to a CategoryResource.
    /// </summary>
    public static CategoryResource ToResourceFromEntity(Category category) =>
        new(category.Id, category.Name);
}
