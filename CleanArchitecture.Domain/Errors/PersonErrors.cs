namespace CleanArchitecture.Domain.Errors;

public class PersonErrors
{
    public static readonly Error FirstNameIsRequired = new("The person name is invalid.", "PersonNameInvalid");
    
    public static readonly Error LastNameIsRequired = new("The person last name is invalid.", "PersonLastNameInvalid");
}