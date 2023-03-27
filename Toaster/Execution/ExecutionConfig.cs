using System.Collections.Generic;

namespace Toaster.Execution;

public class ExecutionConfig
{
    public List<string> NamedRegisters { get; } = new List<string>();

    public int StackRegisterCount { get; set; }

    public int MaxStackDepth { get; set; }

    public int PinCount { get; set; }

    public int BasicRegisterCount { get; set; }
}