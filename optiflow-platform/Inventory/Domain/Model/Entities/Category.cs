namespace optiflow_platform.Inventory.Domain.Model.Entities;

/// <summary>
///     Represents a product category used to classify catalog items.
/// </summary>
public class Category
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Category()
    {
        Name = null!;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
}
