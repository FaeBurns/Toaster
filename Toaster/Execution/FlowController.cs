using System;
using Toaster.Parsing;

namespace Toaster.Execution;

public class FlowController
{
    private readonly TokenProgram _tokenProgram;
    private int _remainingSleep = 0;

    public bool Modified { get; private set; }
    public int CurrentLineIndex { get; private set; }
    public int NextLineIndex { get; private set; }

    public int RemainingSleep
    {
        get => _remainingSleep;
        private set => _remainingSleep = Math.Max(0, value);
    }

    public bool Sleeping => RemainingSleep > 0;

    public FlowController(TokenProgram tokenProgram)
    {
        _tokenProgram = tokenProgram;

        // set line index execution will start at
        NextLineIndex = GetNextExecutingLineIndex(0);
    }

    public void Jump(int targetLineNumber)
    {
        int targetLineIndex = targetLineNumber - 1;

        NextLineIndex = GetNextExecutingLineIndex(targetLineIndex + 1);
        Modified = true;
    }

    public void Jump(string label)
    {
        Jump(TryFindLabel(label));
    }

    public void Reset()
    {
        Modified = false;
    }

    public void UpdateCurrent()
    {
        CurrentLineIndex = NextLineIndex;
    }

    public void MoveNext()
    {
        NextLineIndex = GetNextExecutingLineIndex(CurrentLineIndex + 1);
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

    public void Sleep(int sleepTime)
    {
        RemainingSleep = sleepTime;
    }

    public void ProcessSleep()
    {
        RemainingSleep--;
    }
}