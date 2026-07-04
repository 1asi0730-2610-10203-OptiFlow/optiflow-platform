namespace optiflow_platform.IAM.Domain.Model.Commands;

public record CreateAccountCommand(string Name, Guid OwnerUserId);
