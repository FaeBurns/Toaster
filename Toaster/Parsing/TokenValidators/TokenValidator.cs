namespace Toaster.Parsing.TokenValidators;

public abstract class TokenValidator
{
    public ErrorCollection Errors { get; set; } = new ErrorCollection();

    public abstract void Validate(Token token, TokenValidationContext context);
}