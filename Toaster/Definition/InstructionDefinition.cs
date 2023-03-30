using System.Collections.Generic;
using System.Linq;
using Toaster.Instructions;

namespace Toaster.Definition;

public class InstructionDefinition
{
    public InstructionDefinition(string name, IEnumerable<DefinitionParameterFlag> parameterFlags, Instruction instruction)
    {
        Name = name;
        Parameters = parameterFlags.ToArray();
        Instruction = instruction;
    }

    public readonly string Name;

    public readonly DefinitionParameterFlag[] Parameters;

    public readonly Instruction Instruction;
}