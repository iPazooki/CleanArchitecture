using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Application.Entities.Users.Queries.Get;

internal class GetUserQueryHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<GetUserQuery>> validators) 
    : BaseRequestHandler<GetUserQuery, UserResponse>(validators)
{
    protected override async Task<Result<UserResponse>> HandleRequest(GetUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await applicationUnitOfWork.Users.FindAsync(keyValues: [request.Id], cancellationToken);

        return user is null
            ? Result<UserResponse>.Failure("User Not Found.")
            : Result<UserResponse>.Success(user.ToResponse());
    }
}