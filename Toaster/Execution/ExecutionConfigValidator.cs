using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Toaster.Execution;

public class ExecutionConfigValidator
{
    private readonly Regex _validNameRegex = new Regex("^([a-zA-Z]+[0-9]*)$");

    private readonly HashSet<string> _registerNameSet;
    private readonly ExecutionConfig _config;

    private readonly List<ConfigValidationError> _validationErrors = new List<ConfigValidationError>();

    public IReadOnlyList<ConfigValidationError> ValidationErrors => _validationErrors;

    public bool HasErrors => _validationErrors.Count > 0;

    public bool IsOk => !HasErrors;

    public ExecutionConfigValidator(ExecutionConfig config)
    {
        _config = config;
        _registerNameSet = new HashSet<string>(config.NamedRegisters);
    }

    public bool Validate()
    {
        if (_registerNameSet.Count != _config.NamedRegisters.Count)
            Raise(new ConfigValidationError("duplicate register names found in config"));

        ThrowIfBuiltin("acc");
        ThrowIfBuiltin("t");
        ThrowIfBuiltin("rv");
        ThrowIfBuiltin("ra");

        foreach (string registerName in _config.NamedRegisters)
        {
            Match regexMatch = _validNameRegex.Match(registerName);
            if (!regexMatch.Success || regexMatch.Length != registerName.Length)
                Raise(new ConfigValidationError($"Invalid register name \"{registerName}\""));

            // check for stack register
            // or pin mimics
            if (registerName.Length >= 2)
            {
                bool endsWithNumber = int.TryParse(registerName[1].ToString(), out int _);

                if (registerName[0] == 's' && endsWithNumber)
                    Raise(new ConfigValidationError("register cannot attempt to mimic or replace stack register"));

                if (registerName[0] == 'p' && endsWithNumber)
                    Raise(new ConfigValidationError("register cannot attempt to mimic pin accessors"));

                if (registerName[0] == 'r' && endsWithNumber)
                    Raise(new ConfigValidationError("register cannot attempt to mimic or replace numbered \"r\" register"));
            }
        }

        if (_config.PinCount < 0)
            Raise(new ConfigValidationError("pin count found to be negative in config"));

        if (_config.MaxStackDepth < 0)
            Raise(new ConfigValidationError("max stack depth found to be negative in config"));

        if (_config.StackRegisterCount < 0)
            Raise(new ConfigValidationError("stack register count found to be negative in config"));

        if (_config.BasicRegisterCount < 0)
            Raise(new ConfigValidationError("basic register count found to be negative in config"));

        if (_config.StepsPerInstruction < 1)
            Raise(new ConfigValidationError("per-instruction step threshold cannot be less than 1"));

        return IsOk;
    }

    private void ThrowIfBuiltin(string name)
    {
        if (_registerNameSet.Contains(name))
            Raise(new ConfigValidationError($"built-in register name \"${name}\" found in config"));
    }

    private void Raise(ConfigValidationError message)
    {
        _validationErrors.Add(message);
    }
}

[ExcludeFromCodeCoverage]
public class ConfigValidationError
{
    public string Message { get; }

    public ConfigValidationError(string message)
    {
        Message = message;
    }
}