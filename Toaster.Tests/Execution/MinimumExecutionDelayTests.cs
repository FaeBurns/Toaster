using NUnit.Framework;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Tests.Execution;

[TestFixture]
public class MinimumExecutionDelayTests
{
    private string GetTestProgram()
    {
        return ":loop\nadd 1\nblt loop $acc 3";
    }

    private Interpreter GetDelayInterpreter(int delay)
    {
        ExecutionConfig executionConfig = TestHelpers.GetGenericConfig();
        executionConfig.StepsPerInstruction = delay;

        return new Interpreter(executionConfig, new Parser().Tokenize(GetTestProgram()));
    }

    [Test]
    public void Delay([Range(1, 10)] int delay)
    {
        Interpreter interpreter = GetDelayInterpreter(delay);

        int totalSteps = 0;
        for (int i = 0; i < 3; i++)
        {
            int desiredLineIndex = 1;
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < delay; k++)
                {
                    interpreter.Step();
                    Assert.AreEqual(interpreter.CurrentLineIndex, desiredLineIndex);
                    totalSteps++;
                }
                desiredLineIndex++;
            }
        }
        Assert.AreEqual(3 * 2 * delay, totalSteps);
    }
}