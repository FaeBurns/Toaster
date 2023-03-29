namespace Toaster.Execution;

public interface IExecutionContext
{
    public void PushFrame();
    public void PopFrame();
    public void Jump(string label);
    public void Jump(int lineIndex);
}