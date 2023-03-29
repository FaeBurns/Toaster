using System;
using System.Collections.Generic;
using System.Linq;
using Toaster.Parsing;

namespace Toaster.Execution;

public class Interpreter : IExecutionContext
{
    private readonly ExecutionConfig _config;
    private readonly TokenProgram _tokenProgram;
    private readonly Dictionary<string, ushort> _registerValues = new Dictionary<string, ushort>();
    private readonly Stack<ushort[]> _savedStackRegisters = new Stack<ushort[]>();

    /// <summary>
    /// Gets the index of the next line to be executed. A value less than 0 means that no execution will occur.
    /// </summary>
    public int NextLineIndex { get; private set; }

    /// <summary>
    /// Gets the index of the currently executing/just executed line of the program.
    /// </summary>
    public int CurrentLineIndex { get; private set; }

    /// <summary>
    /// Gets the current depth of the stack.
    /// </summary>
    public int StackDepth => _savedStackRegisters.Count;

    public Interpreter(ExecutionConfig config, TokenProgram tokenProgram)
    {
        _config = config;
        _tokenProgram = tokenProgram;

        ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
        validator.Validate();

        if (validator.HasErrors)
            throw new ArgumentException($"validation of argument {config} shows errors", nameof(config));
    }

    /// <summary>
    /// Advances execution of the program by one instruction
    /// </summary>
    public void Step()
    {
        // if next line index is invalid
        // either EOF or invalid jump occured so exit
        if (NextLineIndex < 0)
            return;

        CurrentLineIndex = NextLineIndex;

        TokenLine tokenLine = _tokenProgram.Lines[CurrentLineIndex];
    }

    public void PushFrame()
    {
        ushort[] frame = new ushort[_config.StackRegisterCount];
        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            frame[i] = _registerValues["s" + i];
        }
        _savedStackRegisters.Push(frame);
    }

    public void PopFrame()
    {
        ushort[] frame = _savedStackRegisters.Pop();
        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            _registerValues["s" + i] = frame[i];
        }
    }

    public void Jump(string label)
    {
        Jump(TryFindLabel(label));
    }

    public void Jump(int lineNumber)
    {
        NextLineIndex = GetNextExecutingLineIndex(lineNumber);
    }

    /// <summary>
    /// Gets the value of the specified register.
    /// </summary>
    /// <param name="registerName">The name of the register to find.</param>
    /// <returns>The value in the register</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ushort GetRegisterValue(string registerName)
    {
        if (!_registerValues.ContainsKey(registerName))
            throw new InvalidOperationException("Cannot get value of register that does not exist");

        return _registerValues[registerName];
    }

    public void SetRegisterValue(string registerName, ushort value)
    {
        if (!_registerValues.ContainsKey(registerName))
            throw new InvalidOperationException($"Cannot set value of register that does not exist");

        _registerValues[registerName] = value;
    }

    /// <summary>
    /// Gets a readonly view of the registers and their values
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, ushort> GetRegisterValues()
    {
        return _registerValues;
    }

    /// <summary>
    /// Tries to find a label in the program.
    /// </summary>
    /// <param name="label">The label to find.</param>
    /// <returns>The line index of the first instruction after the label. -1 if no match was found.</returns>
    public int TryFindLabel(string label)
    {
        foreach (TokenLine line in _tokenProgram.Lines)
        {
            if (line.IsLabel && line.Tokens[0].RegexResult.Groups[1].Value == label)
                return GetNextExecutingLineIndex(line.LineIndex);
        }

        return -1;
    }

    private int GetNextExecutingLineIndex(int startLineIndex)
    {
        if (startLineIndex < 0)
            return -1;

        if (startLineIndex > _tokenProgram.LastInstructionIndex)
            return -1;

        for (int i = startLineIndex; i <= _tokenProgram.LastInstructionIndex; i++)
        {
            if (_tokenProgram.Lines[i].IsInstruction)
                return i;
        }

        return -1;
    }
}