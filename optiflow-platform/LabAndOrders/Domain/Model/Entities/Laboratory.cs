namespace optiflow_platform.LabAndOrders.Domain.Model.Entities;

/// <summary>
///     Represents an external optical laboratory that produces work orders.
/// </summary>
public class Laboratory
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Laboratory()
    {
        Name = null!;
        ContactInfo = null!;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string ContactInfo { get; private set; }
}
