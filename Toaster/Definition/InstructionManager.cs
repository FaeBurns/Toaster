#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Toaster.Instructions;
using Toaster.Parsing;

namespace Toaster.Definition;

public static class InstructionManager
{
    private static readonly Dictionary<string, List<InstructionDefinition>> _definitions = new Dictionary<string, List<InstructionDefinition>>();

    /// <summary>
    /// Loads a set of instructions from json.
    /// </summary>
    /// <param name="jsonText">The json to load the signatures from.</param>
    public static void LoadSignatures(string jsonText)
    {
        JsonInstructionDefinitionCollection definitionCollection = JsonConvert.DeserializeObject<JsonInstructionDefinitionCollection>(jsonText)!;

        foreach (JsonInstructionDefinition jsonDefinition in definitionCollection.Definitions)
        {
            Instruction instruction = CreateInstructionFromType(definitionCollection.NamespacePrefix, jsonDefinition.TypePath, definitionCollection.TypePostfix);
            InstructionDefinition definition = new InstructionDefinition(jsonDefinition.Name, jsonDefinition.ParameterFlags, instruction);

            if (!_definitions.ContainsKey(definition.Name))
                _definitions[definition.Name] = new List<InstructionDefinition>();

            _definitions[definition.Name].Add(definition);
        }
    }

    /// <summary>
    /// Tries to find an instruction that matches the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the instruction.</param>
    /// <param name="argumentIds">The ids of the argument tokens.</param>
    /// <returns>An instruction if one was found, Null if not.</returns>
    public static Instruction? TryFetchInstructionBySignature(string name, TokenId[] argumentIds)
    {
        DefinitionParameterFlag[] signatureArgumentIds = new DefinitionParameterFlag[argumentIds.Length];
        for (int i = 0; i < argumentIds.Length; i++)
        {
            signatureArgumentIds[i] = ConvertArgumentIdToParameterFlag(argumentIds[i]);
        }

        // try and find in name-definition mapping
        if (_definitions.TryGetValue(name, out List<InstructionDefinition> definitionOverloads))
        {
            foreach (InstructionDefinition foundDefinition in definitionOverloads)
            {
                // skip to next overload if parameter count does not match
                if (foundDefinition.Parameters.Length != signatureArgumentIds.Length)
                    continue;

                for (int i = 0; i < foundDefinition.Parameters.Length; i++)
                {
                    DefinitionParameterFlag definitionFlag = foundDefinition.Parameters[i];
                    DefinitionParameterFlag argumentFlag = signatureArgumentIds[i];

                    // if the and result has at least one bit set
                    if ((definitionFlag & argumentFlag) != 0)
                        return foundDefinition.Instruction;
                }
            }
        }

        // nothing valid was found
        return null;
    }

    private static DefinitionParameterFlag ConvertArgumentIdToParameterFlag(TokenId tokenId)
    {
        return tokenId switch
        {
            TokenId.REGISTER => DefinitionParameterFlag.REGISTER,
            TokenId.LABEL_ARG => DefinitionParameterFlag.LABEL,
            TokenId.PIN_RANGE => DefinitionParameterFlag.PIN_MULTIPLE,
            TokenId.PIN_RANGE_LENGTH => DefinitionParameterFlag.PIN_MULTIPLE,
            TokenId.PIN => DefinitionParameterFlag.PIN_SINGLE,
            TokenId.BINARY => DefinitionParameterFlag.CONSTANT,
            TokenId.HEX => DefinitionParameterFlag.CONSTANT,
            TokenId.INTEGER => DefinitionParameterFlag.CONSTANT,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenId), tokenId, null),
        };
    }

    private static Instruction CreateInstructionFromType(string namespacePrefix, string type, string typePostfix)
    {
        // find type in assembly
        Type foundType = Assembly.GetExecutingAssembly().GetType(namespacePrefix + "." + type + typePostfix);

        // create instance of instruction
        Instruction instruction = (Instruction)Activator.CreateInstance(foundType);

        // return result
        return instruction;
    }
}