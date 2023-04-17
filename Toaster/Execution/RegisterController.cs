using System;
using System.Collections.Generic;

namespace Toaster.Execution;

/// <summary>
/// Handles the getting and setting of string-ushort pairs.
/// </summary>
public class RegisterController
{
    private readonly Dictionary<string, ushort> _allRegisters = new Dictionary<string, ushort>();

    /// <summary>
    /// Creates a new <see cref="RegisterController"/> from an (assumed valid) <see cref="ExecutionConfig"/>.
    /// </summary>
    /// <param name="config"></param>
    public RegisterController(ExecutionConfig config)
    {
        // get all names for register
        string[] registerNames = GetRegisterNames(config);

        // add entry in registers for each discovered name
        foreach (string name in registerNames)
        {
            _allRegisters.Add(name, 0);
        }
    }

    /// <summary>
    /// Sets the specified register to the provided value.
    /// </summary>
    /// <param name="register">The register to set.</param>
    /// <param name="value">The value to set the register to.</param>
    /// <exception cref="InvalidOperationException">Register does not exist.</exception>
    public void SetRegister(string register, ushort value)
    {
        if (!_allRegisters.ContainsKey(register))
            throw new InvalidOperationException("Cannot set value for register that does not exist");

        _allRegisters[register] = value;
    }

    /// <summary>
    /// Gets a value from the specified register.
    /// </summary>
    /// <param name="register">The register to get the value from.</param>
    /// <exception cref="InvalidOperationException">Register does not exist.</exception>
    /// <returns>The value held in the register.</returns>
    public ushort GetRegister(string register)
    {
        if (!_allRegisters.ContainsKey(register))
            throw new InvalidOperationException($"Cannot get value from register {register} as it does not exist");

        return _allRegisters[register];
    }

    /// <summary>
    /// Gets a collection of all register names used by this instance.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetUsedRegisterNames()
    {
        return _allRegisters.Keys;
    }

    /// <summary>
    /// Gets an array of all register names from an (assumed valid) <see cref="ExecutionConfig"/>.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static string[] GetRegisterNames(ExecutionConfig config)
    {
        // 4 is built-in register count
        int totalRegisterCount = config.NamedRegisters.Count + config.BasicRegisterCount + 4;
        totalRegisterCount += config.StackRegisterCount;

        string[] result = new string[totalRegisterCount];

        int workingIndex = 0;

        foreach (string name in config.NamedRegisters)
        {
            result[workingIndex] = name;
            workingIndex++;
        }

        for (int i = 0; i < config.BasicRegisterCount; i++)
        {
            result[workingIndex] = "r" + i;
            workingIndex++;
        }

        for (int i = 0; i < config.StackRegisterCount; i++)
        {
            result[workingIndex] = "s" + i;
            workingIndex++;
        }

        result[workingIndex + 0] = "acc";
        result[workingIndex + 1] = "t";
        result[workingIndex + 2] = "ra";
        result[workingIndex + 3] = "rv";

        return result;
    }
}