namespace Toaster.Parsing.TokenValidators;

public class RegisterTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        string register = token.RegexResult.Groups[1].Value;
        if (!context.Registers.Contains(register))
        {
            ErrorCollection.RaiseError($"Unknown register {register}", token.Position);
        }
    }
}