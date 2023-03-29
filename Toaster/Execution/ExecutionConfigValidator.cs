using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Toaster.Execution;

public class ExecutionConfigValidator
{
    private readonly Regex _validNameRegex = new Regex("^([a-zA-Z]+[0-9]*)$");

    private readonly HashSet<string> _registerNameSet;
    private readonly ExecutionConfig _config;

    private readonly List<ValidationError> _errors = new List<ValidationError>();

    public IReadOnlyList<ValidationError> Errors => _errors;

    public bool HasErrors => _errors.Count > 0;

    public bool IsOk => !HasErrors;

    public ExecutionConfigValidator(ExecutionConfig config)
    {
        _config = config;
        _registerNameSet = new HashSet<string>(config.NamedRegisters);
    }

    public bool Validate()
    {
        if (_registerNameSet.Count != _config.NamedRegisters.Count)
            Raise(new ValidationError("duplicate register names found in config"));

        ThrowIfBuiltin("acc");
        ThrowIfBuiltin("t");
        ThrowIfBuiltin("rv");
        ThrowIfBuiltin("ra");

        foreach (string registerName in _config.NamedRegisters)
        {
            Match regexMatch = _validNameRegex.Match(registerName);
            if (!regexMatch.Success || regexMatch.Length != registerName.Length)
                Raise(new ValidationError($"Invalid register name \"{registerName}\""));

            // check for stack register
            // or pin mimics
            if (registerName.Length >= 2)
            {
                bool endsWithNumber = int.TryParse(registerName[1].ToString(), out int _);

                if (registerName[0] == 's' && endsWithNumber)
                    Raise(new ValidationError("register cannot attempt to mimic or replace stack register"));

                if (registerName[0] == 'p' && endsWithNumber)
                    Raise(new ValidationError("register cannot attempt to mimic pin accessors"));

                if (registerName[0] == 'r' && endsWithNumber)
                    Raise(new ValidationError("register cannot attempt to mimic or replace numbered \"r\" register"));
            }
        }

        if (_config.PinCount < 0)
            Raise(new ValidationError("pin count found to be negative in config"));

        if (_config.MaxStackDepth < 0)
            Raise(new ValidationError("max stack depth found to be negative in config"));

        if (_config.StackRegisterCount < 0)
            Raise(new ValidationError("stack register count found to be negative in config"));

        if (_config.BasicRegisterCount < 0)
            Raise(new ValidationError("basic register count found to be negative in config"));

        return IsOk;
    }

    private void ThrowIfBuiltin(string name)
    {
        if (_registerNameSet.Contains(name))
            Raise(new ValidationError($"built-in register name \"${name}\" found in config"));
    }

    private void Raise(ValidationError message)
    {
        _errors.Add(message);
    }
}

[ExcludeFromCodeCoverage]
public class ValidationError
{
    public string Message { get; }

    public ValidationError(string message)
    {
        Message = message;
    }
}