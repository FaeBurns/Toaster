namespace Toaster.Parsing.TokenValidators;

public class LabelArgumentTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        string label = token.RegexResult.Groups[1].Value;
        if (!context.Labels.Contains(label))
        {
            ErrorCollection.RaiseError($"label \"{label}\" could not be found", token.Position);
        }
    }
}