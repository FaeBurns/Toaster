﻿namespace Toaster.Execution;

public readonly struct StackFrame
{
    public readonly ushort[] Registers;
    public readonly int ReturnLineIndex;

    public StackFrame(int registerCount, int returnLineIndex)
    {
        Registers = new ushort[registerCount];
        ReturnLineIndex = returnLineIndex;
    }
}