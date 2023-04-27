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
        TokenValidationContext validationContext = new TokenValidationContext(registers, validationTarget.PinCount);

        PreprocessLabels(program, validationContext);

        foreach (TokenLine tokenLine in program.Lines)
        {
            ValidateLine(tokenLine, validationContext);
        }
    }

    private void ValidateLine(TokenLine tokenLine, TokenValidationContext validationContext)
    {
        // skip if line is empty
        if (tokenLine.Tokens.Count == 0)
            return;

        // get first token
        Token firstToken = tokenLine.Tokens[0];

        // if first token is an invalid id
        if (firstToken.Id != TokenId.INSTRUCTION && firstToken.Id != TokenId.LABEL)
        {
            ErrorCollection.RaiseError("Line must start with an instruction or label.", tokenLine.LineIndex, 0, tokenLine.FullLine.Length - 1);
        }

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
        Token instructionToken = instructionLine.Tokens[0];
        string instructionName = instructionExtractor.ExtractValue(instructionToken);

        // get all instructions but the first
        IEnumerable<Token> argumentTokens = instructionLine.Tokens.Skip(1);

        // try and find instruction from tokens signature
        // avoid multiple enumeration first
        Token[] enumeratedArgumentTokens = argumentTokens as Token[] ?? argumentTokens.ToArray();
        Instruction instruction = InstructionManager.TryFetchInstructionBySignature(instructionName, enumeratedArgumentTokens.Select(t => t.Id).ToArray());

        // return okay if instruction was found
        if (instruction != null)
            return;

        if (!InstructionManager.GetHasInstructionWithName(instructionName))
        {
            ErrorCollection.RaiseError($"Cannot find instruction with name {instructionName}", instructionToken.Position);
            return;
        }

        IEnumerable<InstructionDefinition> definitions = InstructionManager.GetDefinitions(instructionName);

        string validDefinitionsString = "";

        foreach (InstructionDefinition definition in definitions)
        {
            validDefinitionsString += definition.ToString();
        }

        string invalidArgumentsString = "";
        foreach (Token argumentToken in enumeratedArgumentTokens)
        {
            invalidArgumentsString += " " + argumentToken.Id;
        }

        // get end column of last token and raise error
        int endColumn = instructionLine.Tokens.Last().Position.EndColumn;
        ErrorCollection.RaiseError($"Could not find valid override for {instructionName}{invalidArgumentsString}. Valid overrides:\n{validDefinitionsString}", instructionLine.LineIndex, 0, endColumn);
    }
}