namespace CleanArchitecture.Domain.Errors;

public class UserErrors
{
    public static readonly Error FirstNameIsRequired = new("The name is invalid.", "UserNameInvalid");
    
    public static readonly Error LastNameIsRequired = new("The last name is invalid.", "UserLastNameInvalid");
}