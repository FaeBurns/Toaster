using System;
using System.Collections.Generic;
using System.Linq;
using Toaster.Instructions;
using Toaster.Parsing;

namespace Toaster.Execution;

public class Interpreter : IExecutionContext
{
    private readonly ExecutionConfig _config;
    private readonly TokenProgram _tokenProgram;
    private readonly FlowController _flowController;
    private readonly RegisterController _registerController;
    private readonly PinController _pinController;
    private readonly Stack<StackFrame> _stack = new Stack<StackFrame>();

    /// <summary>
    /// Gets the <see cref="ErrorCollection"/> that is populated by errors achieved during execution.
    /// </summary>
    public ErrorCollection InstructionErrorCollection { get; } = new ErrorCollection();

    /// <summary>
    /// Gets the current depth of the stack.
    /// </summary>
    public int StackDepth => _stack.Count;

    public Interpreter(ExecutionConfig config, TokenProgram tokenProgram)
    {
        // validate config before setup
        ExecutionConfigValidator configValidator = new ExecutionConfigValidator(config);
        configValidator.Validate();

        // throw if configValidator found errors
        if (configValidator.HasErrors)
            throw new ArgumentException($"{nameof(ExecutionConfig)} failed validation check", nameof(config));

        // validate program before continuing setup
        TokenProgramValidator programValidator = new TokenProgramValidator();
        programValidator.Validate(tokenProgram, config);

        // throw if programValidator found errors
        if (programValidator.ErrorCollection.HasErrors)
            throw new ArgumentException($"{nameof(TokenProgram)} failed validation check", nameof(tokenProgram));

        _config = config;
        _tokenProgram = tokenProgram;
        _flowController = new FlowController(tokenProgram);
        _registerController = new RegisterController(config);
        _pinController = new PinController(config.PinCount);
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

            // if execution failed, copy all errors to InstructionErrorCollection
            foreach (Error error in executor.ErrorCollection.Errors)
            {
                InstructionErrorCollection.Raise(error);
            }
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
            frame.Registers[i] = _registerController.GetRegister("s" + i);
        }
        _stack.Push(frame);
    }

    public int PopFrame()
    {
        StackFrame frame = _stack.Pop();
        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            _registerController.SetRegister("s" + i, frame.Registers[i]);
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
    /// <returns>The value in the register.</returns>
    public ushort GetRegisterValue(string registerName)
    {
        return _registerController.GetRegister(registerName);
    }

    /// <summary>
    /// Sets the value of the specified register.
    /// </summary>
    /// <param name="registerName">The name of the register to find.</param>
    /// <param name="value">The value to set the register to.</param>
    public void SetRegisterValue(string registerName, ushort value)
    {
        _registerController.SetRegister(registerName, value);
    }

    public void SetPin(int pin, bool value)
    {
        _pinController.SetOutputPin(pin, value);
    }

    public void SetPins(int startPin, bool[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            _pinController.SetOutputPin(startPin + i, values[i]);
        }
    }

    public bool GetPin(int pin)
    {
        return _pinController.GetPinValue(pin);
    }

    public bool[] GetPins(int startPin, int count)
    {
        bool[] result = new bool[count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = _pinController.GetPinValue(startPin + i);
        }
        return result;
    }

    /// <summary>
    /// Gets a readonly view of the registers and their values
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, ushort> GetRegisterValues()
    {
        Dictionary<string, ushort> values = new Dictionary<string, ushort>();
        foreach (string name in _registerController.GetUsedRegisterNames())
        {
            values.Add(name, _registerController.GetRegister(name));
        }

        return values;
    }
}