namespace Toaster.Parsing.TokenReaders;

public abstract class TokenValidator
{
    public ErrorCollection Errors { get; } = new ErrorCollection();

    public abstract void Validate(Token token, TokenValidationContext context);
}