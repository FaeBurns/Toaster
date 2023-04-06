namespace Toaster.Parsing.TokenValidators;

public class RegisterTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        string register = token.RegexResult.Groups[1].Value;
        if (!context.Registers.Contains(register))
        {
            Errors.RaiseError($"Unknown register {register}", token.Position);
        }
    }
}