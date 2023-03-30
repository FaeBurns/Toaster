﻿namespace Toaster.Parsing.TokenReaders;

public class LabelArgumentTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        string label = token.RegexResult.Groups[1].Value;
        if (!context.Labels.Contains(label))
        {
            Errors.RaiseError($"label \"{label}\" could not be found", token.Position);
        }
    }
}