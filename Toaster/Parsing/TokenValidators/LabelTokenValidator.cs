namespace Toaster.Parsing.TokenValidators;

public class LabelTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        string label = token.RegexResult.Groups[1].Value;
        if (context.Labels.Contains(label))
        {
            Errors.RaiseError($"label \"{label}\" already exists", token.Position);
        }
        else
        {
            context.Labels.Add(label);
        }
    }
}