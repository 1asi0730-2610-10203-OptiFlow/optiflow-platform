using System.Threading;
using System.Threading.Tasks;
using atelier_platform_aplicaciones_web.IAM.Application.QueryServices;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.Queries;
using atelier_platform_aplicaciones_web.IAM.Domain.Repositories;

namespace atelier_platform_aplicaciones_web.IAM.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByIdAsync(query.UserId.Value, cancellationToken);
    }

    public async Task<User?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByEmailAsync(query.Email, cancellationToken);
    }
}
