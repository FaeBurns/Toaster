using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing.TokenValidators;

namespace Toaster.Parsing;

public class TokenProgramValidator
{
    public ErrorCollection Errors { get; } = new ErrorCollection();

    public void Validate(TokenProgram program, ExecutionConfig validationTarget)
    {
        // get all register names and create validationContext
        List<string> registers = GetRegisterNames(validationTarget);
        TokenValidationContext validationContext = new TokenValidationContext(registers);

        PreprocessLabels(program, validationContext);

        foreach (TokenLine tokenLine in program.Lines)
        {
            // if line is an instruction line
            // then its tokens should be validated
            if (tokenLine.IsInstruction)
            {
                foreach (Token token in tokenLine.Tokens)
                {
                    // get validator for token
                    TokenValidator validator = TokenValidatorSelector.GetValidator(token.Id);

                    // set error target
                    // and validate
                    validator.Errors = Errors;
                    validator.Validate(token, validationContext);
                }
            }
        }
    }

    private static void PreprocessLabels(TokenProgram program, TokenValidationContext validationContext)
    {
        // validate all labels
        foreach (TokenLine tokenLine in program.Lines)
        {
            if (tokenLine.IsLabel)
            {
                TokenValidator validator = TokenValidatorSelector.GetValidator(tokenLine.Tokens[0].Id);
                validator.Validate(tokenLine.Tokens[0], validationContext);
            }
        }
    }

    private static List<string> GetRegisterNames(ExecutionConfig validationTarget)
    {
        List<string> registers = new List<string>(validationTarget.NamedRegisters)
        {
            "acc",
            "t",
            "ra",
            "rv",
        };
        for (int i = 0; i < validationTarget.BasicRegisterCount; i++)
        {
            registers.Add("r" + i);
        }

        for (int i = 0; i < validationTarget.StackRegisterCount; i++)
        {
            registers.Add("s" + i);
        }
        return registers;
    }
}