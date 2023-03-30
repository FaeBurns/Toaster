namespace Toaster.Execution;

public interface IExecutionContext
{
    public void PushFrame();
    public int PopFrame();
    public void Jump(int lineIndex);
    public ushort GetLabelLineIndex(string label);
    public ushort GetRegisterValue(string register);
    public void SetRegisterValue(string register, ushort value);
    public void SetPins(int startPin, bool[] values);
    public bool[] GetPins(int startPin, int count);
}