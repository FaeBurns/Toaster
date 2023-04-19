using System;
using System.Collections.Generic;

namespace Toaster.Execution;

/// <summary>
/// A class containing information about the setup of an <see cref="Interpreter"/> and what functions it can support.
/// </summary>
public class ExecutionConfig
{
    /// <summary>
    /// Gets a collection of the names of all additional registers.
    /// </summary>
    public List<string> NamedRegisters { get; } = new List<string>();

    /// <summary>
    /// Gets or Sets the amount of ($s0) stack registers on the interpreter.
    /// </summary>
    public int StackRegisterCount { get; set; }

    /// <summary>
    /// Gets or Sets the maximum stack depth of the interpreter.
    /// </summary>
    public int MaxStackDepth { get; set; }

    /// <summary>
    /// Gets or Sets the amount of pins the interpreter has access to.
    /// </summary>
    public int PinCount { get; set; }

    /// <summary>
    /// Gets or Sets the amount of ($r0) basic registers the interpreter has.
    /// </summary>
    public int BasicRegisterCount { get; set; }
}