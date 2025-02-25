using CleanArchitecture.Domain.Entities.Security;
using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Application.Entities.Users.Commands.Create;

public class CreateUserCommandHandler(IApplicationUnitOfWork applicationUnitOfWork, IPasswordHasher passwordHasher) : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Address? address = null;
        string? passwordHash = null;
        
        if (request.Address is not null)
        {
            address = new Address(request.Address.City, request.Address.Street, request.Address.PostalCode);   
        }
        
        if (!string.IsNullOrEmpty(request.Password))
        {
            passwordHash = passwordHasher.HashPassword(request.Password);
        }
        
        Result<User> userResult = User.Create(request.FirstName, request.LastName, request.Email, passwordHash, address, (Gender?)request.Gender);

        if (!userResult.IsSuccess)
        {
            return Result<Guid>.Failure(userResult.Errors.ToArray());
        }

        Role? role = await applicationUnitOfWork.Roles.SingleOrDefaultAsync(x => x.Id == (int)Roles.Member, cancellationToken);

        if (role is null)
        {
            return Result<Guid>.Failure(string.Format(GeneralErrors.NotFoundErrorMessage, nameof(Role)));
        }
        
        userResult.Value!.AddRole(role);

        await applicationUnitOfWork.Users.AddAsync(userResult.Value!, cancellationToken);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? Result<Guid>.Success(userResult.Value!.Id)
            : Result<Guid>.Failure(string.Format(GeneralErrors.GeneralCreateErrorMessage, nameof(User)));
    }
}