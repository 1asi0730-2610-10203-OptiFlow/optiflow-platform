using optiflow_platform.IAM.Domain.Model.Commands;
using optiflow_platform.Shared.Domain.Model;

namespace optiflow_platform.IAM.Domain.Model.Aggregates;

public class Account : IAuditableEntity
{
    protected Account()
    {
        Name = null!;
    }

    public Account(CreateAccountCommand command) : this()
    {
        ArgumentNullException.ThrowIfNull(command);
        Id = Guid.NewGuid();
        Name = command.Name;
        OwnerUserId = command.OwnerUserId;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid? OwnerUserId { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
