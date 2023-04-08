using System;
using System.Collections.Generic;
using System.Linq;
using Toaster.Definition;
using Toaster.Execution;
using Toaster.Instructions;
using Toaster.Parsing.TokenValidators;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Parsing;

public class TokenProgramValidator
{
    public ErrorCollection ErrorCollection { get; } = new ErrorCollection();

    public void Validate(TokenProgram program, ExecutionConfig validationTarget)
    {
        // get all register names and create validationContext
        string[] registers = RegisterController.GetRegisterNames(validationTarget);
        TokenValidationContext validationContext = new TokenValidationContext(registers);

        PreprocessLabels(program, validationContext);

        foreach (TokenLine tokenLine in program.Lines)
        {
            ValidateLine(tokenLine, validationContext);
        }
    }

    private void ValidateLine(TokenLine tokenLine, TokenValidationContext validationContext)
    {
        foreach (Token token in tokenLine.Tokens)
        {
            // validate if instruction
            if (token.Id == TokenId.INSTRUCTION)
                ValidateSignature(tokenLine);

            // get validator for token
            TokenValidator validator = TokenValidatorSelector.GetValidator(token.Id);

            // set error target
            // and validate
            validator.ErrorCollection = ErrorCollection;
            validator.Validate(token, validationContext);
        }
    }

    private void PreprocessLabels(TokenProgram program, TokenValidationContext validationContext)
    {
        // validate all labels
        foreach (TokenLine tokenLine in program.Lines)
        {
            if (tokenLine.IsLabel)
            {
                TokenValidator validator = new LabelTokenValidator();
                validator.ErrorCollection = ErrorCollection;
                validator.Validate(tokenLine.Tokens[0], validationContext);
            }
        }
    }

    /// <summary>
    /// Validates a <see cref="TokenLine"/> holding an instruction line. Only performs validation on the signature of the instruction line, does not check if arguments are valid.
    /// </summary>
    /// <param name="instructionLine"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Input was not an instruction line.</exception>
    private void ValidateSignature(TokenLine instructionLine)
    {
        if (!instructionLine.IsInstruction)
            throw new ArgumentException($"input {nameof(TokenLine)} must be an instruction line", nameof(instructionLine));

        StringValueExtractor instructionExtractor = new StringValueExtractor();
        string instructionName = instructionExtractor.ExtractValue(instructionLine.Tokens[0]);

        // get all instructions but the first
        IEnumerable<Token> argumentTokens = instructionLine.Tokens.Skip(1);

        // try and get instruction
        Instruction instruction = InstructionManager.TryFetchInstructionBySignature(instructionName, argumentTokens.Select(t => t.Id).ToArray());

        // return okay if instruction was found
        if (instruction != null)
            return;

        if (!InstructionManager.GetHasInstructionWithName(instructionName))
        {
            ErrorCollection.RaiseError($"Cannot find instruction with name {instructionName}", instructionLine.LineIndex, 0, instructionLine.FullLine.Length);
            return;
        }

        IEnumerable<InstructionDefinition> definitions = InstructionManager.GetDefinitions(instructionName);

        string validDefinitionsString = "";

        foreach (InstructionDefinition definition in definitions)
        {
            validDefinitionsString += definition.ToString();
        }

        ErrorCollection.RaiseError($"Could not find valid override for {instructionName}. Valid overrides:\n{validDefinitionsString}", instructionLine.LineIndex, 0, instructionLine.FullLine.Length);
    }
}