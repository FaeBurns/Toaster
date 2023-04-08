namespace Toaster.Parsing.TokenValidators;

public abstract class TokenValidator
{
    public ErrorCollection ErrorCollection { get; set; } = new ErrorCollection();

    public abstract void Validate(Token token, TokenValidationContext context);
}