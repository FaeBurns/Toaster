using System;
using System.Collections.Generic;
using Toaster.Parsing;

namespace Toaster.Execution;

public class Interpreter : IExecutionContext
{
    private readonly ExecutionConfig _config;
    private readonly TokenProgram _tokenProgram;
    private readonly FlowController _flowController;
    private readonly RegisterController _registerController;
    private readonly Stack<StackFrame> _stack = new Stack<StackFrame>();

    /// <summary>
    /// Gets the <see cref="ErrorCollection"/> that is populated by errors achieved during execution.
    /// </summary>
    public ErrorCollection InstructionErrorCollection { get; } = new ErrorCollection();

    /// <summary>
    /// Gets the <see cref="PinController"/> instance used to manage incoming and outgoing pin values.
    /// </summary>
    public PinController PinController { get; }

    /// <summary>
    /// Gets the index of the line currently being executed.
    /// </summary>
    public int CurrentLineIndex => _flowController.CurrentLineIndex;

    /// <summary>
    /// Gets a value indicating whether the interpreter has errors.
    /// </summary>
    public bool HasErrors => InstructionErrorCollection.HasErrors;

    /// <summary>
    /// Gets a value indicating whether the interpreter has reached the end of the program.
    /// </summary>
    public bool Finished => _flowController.NextLineIndex < 0;

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
            throw new ArgumentException($"{nameof(TokenProgram)} failed validation check. {programValidator.ErrorCollection}", nameof(tokenProgram));

        _config = config;
        _tokenProgram = tokenProgram;
        _flowController = new FlowController(tokenProgram);
        _registerController = new RegisterController(config);
        PinController = new PinController(config.PinCount);
    }

    /// <summary>
    /// Advances execution of the program by one instruction
    /// </summary>
    public void Step()
    {
        if (HasErrors)
            throw new InvalidOperationException($"Cannot step interpreter while it has errors. {InstructionErrorCollection}");

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
        if (StackDepth >= _config.MaxStackDepth)
            throw new StackOverflowException($"Cannot exceed {nameof(ExecutionConfig.MaxStackDepth)} as defined by {nameof(ExecutionConfig)}");

        int returnIndex = _flowController.CurrentLineIndex + 1;
        StackFrame frame = new StackFrame(_config.StackRegisterCount, returnIndex);

        // set return address for this frame
        _registerController.SetRegister("ra", (ushort)returnIndex);

        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            // save current stack register values
            frame.Registers[i] = _registerController.GetRegister("s" + i);

            // clear current stack register values
            _registerController.SetRegister("s" + i, 0);
        }
        _stack.Push(frame);
    }

    public int PopFrame()
    {
        if (StackDepth == 0)
            throw new InvalidOperationException("Cannot pop frame from stack when stack is empty");

        StackFrame frame = _stack.Pop();
        for (int i = 0; i < _config.StackRegisterCount; i++)
        {
            _registerController.SetRegister("s" + i, frame.Registers[i]);
        }

        // set visible return address for the current frame
        _registerController.SetRegister("ra", (ushort)frame.ReturnLineIndex);

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
        PinController.SetOutputPin(pin, value);
    }

    public void SetPins(int startPin, bool[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            PinController.SetOutputPin(startPin + i, values[i]);
        }
    }

    public bool GetPin(int pin)
    {
        return PinController.GetPinValue(pin);
    }

    public bool[] GetPins(int startPin, int count)
    {
        bool[] result = new bool[count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = PinController.GetPinValue(startPin + i);
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