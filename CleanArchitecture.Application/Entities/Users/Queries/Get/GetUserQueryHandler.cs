using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Application.Entities.Users.Queries.Get;

public class GetUserQueryHandler(IApplicationUnitOfWork applicationUnitOfWork) : IRequestHandler<GetUserQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await applicationUnitOfWork.Users.FindAsync(keyValues: [request.Id], cancellationToken);

        return user is null
            ? Result<UserResponse>.Failure("User Not Found.")
            : Result<UserResponse>.Success(user.ToResponse());
    }
}