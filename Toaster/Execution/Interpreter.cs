using System;
using System.Collections.Generic;
using Toaster.Parsing;

namespace Toaster.Execution;

public class Interpreter : IExecutionContext
{
    private readonly ExecutionConfig _config;
    private readonly TokenProgram _tokenProgram;
    private readonly FlowController _flowController;
    private readonly bool[] _pins;
    private readonly Dictionary<string, ushort> _registerValues = new Dictionary<string, ushort>();
    private readonly Stack<StackFrame> _stack = new Stack<StackFrame>();

    /// <summary>
    /// Gets the current depth of the stack.
    /// </summary>
    public int StackDepth => _stack.Count;

    public Interpreter(ExecutionConfig config, TokenProgram tokenProgram)
    {
        _config = config;
        _tokenProgram = tokenProgram;
        _flowController = new FlowController(tokenProgram);
        _pins = new bool[config.PinCount];

        ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
        validator.Validate();

        if (validator.HasErrors)
            throw new ArgumentException($"validation of argument {config} shows errors", nameof(config));

        CreateRegisters();
    }

    /// <summary>
    /// Advances execution of the program by one instruction
    /// </summary>
    public void Step()
    {
        // if next line index is invalid
        // either EOF or invalid jump occured so exit
        if (_flowController.NextLineIndex < 0)
            return;

        // update the current line index
        _flowController.UpdateCurrent();

        // get the TokenLine for the current line
        TokenLine tokenLine = _tokenProgram.Lines[_flowController.CurrentLineIndex];

        // if this line is an instruction
        if (tokenLine.IsInstruction)
        {
            // execute the line
            LineExecutor executor = new LineExecutor(this);
            executor.Execute(tokenLine);
        }

        // if the flow controller was not modified
        // move to the next line
        if (!_flowController.Modified)
            _flowController.MoveNext();

        // reset flow controller information
        _flowController.Reset();
    }

    public void PushFrame()
    {
        int returnIndex = _flowController.CurrentLineIndex + 1;
        StackFrame frame = new StackFrame(_config.StackRegisterCount, returnIndex);

        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            frame.Registers[i] = _registerValues["s" + i];
        }
        _stack.Push(frame);
    }

    public int PopFrame()
    {
        StackFrame frame = _stack.Pop();
        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            _registerValues["s" + i] = frame.Registers[i];
        }

        return frame.ReturnLineIndex;
    }

    public ushort GetLabelLineIndex(string label)
    {
        return (ushort)_flowController.TryFindLabel(label);
    }

    public void Jump(int lineNumber)
    {
        _flowController.Jump(lineNumber);
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

    public void SetPin(int pin, bool value)
    {
        _pins[pin] = value;
    }

    public void SetPins(int startPin, bool[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            _pins[startPin + i] = values[i];
        }
    }

    public bool GetPin(int pin)
    {
        return _pins[pin];
    }

    public bool[] GetPins(int startPin, int count)
    {
        bool[] result = new bool[count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = _pins[startPin + i];
        }
        return result;
    }

    /// <summary>
    /// Gets a readonly view of the registers and their values
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, ushort> GetRegisterValues()
    {
        return _registerValues;
    }

    private void CreateRegisters()
    {
        // add guaranteed registers
        _registerValues.Add("acc", 0);
        _registerValues.Add("t", 0);
        _registerValues.Add("ra", 0);
        _registerValues.Add("rv", 0);

        // create from config
        for (int i = 0; i < _config.BasicRegisterCount; i++)
        {
            _registerValues.Add("r" + i, 0);
        }

        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            _registerValues.Add("s" + i, 0);
        }

        foreach (string namedRegister in _config.NamedRegisters)
        {
            _registerValues.Add(namedRegister, 0);
        }
    }
}