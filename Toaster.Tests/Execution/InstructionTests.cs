using System.Collections.Generic;
using NUnit.Framework;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Tests.Execution;

[TestFixture]
public class InterpreterTests
{
    private void TestRegister(Interpreter interpreter, string name, ushort value)
    {
        IReadOnlyDictionary<string, ushort> registerValues = interpreter.GetRegisterValues();

        Assert.AreEqual(value, registerValues[name]);
    }

    private ExecutionConfig GetGenericConfig()
    {
        return new ExecutionConfig()
        {
            NamedRegisters =
            {
                "named0",
                "named1",
                "named2",
                "named3",
            },
            PinCount = 16,
            MaxStackDepth = 5,
            StackRegisterCount = 4,
            BasicRegisterCount = 4,
        };
    }

    [Test]
    public void AddSubtract()
    {
        string program = "add 3\nsub 2";

        TokenProgram parsedProgram = new Parser().Tokenize(program);

        Interpreter interpreter = new Interpreter(GetGenericConfig(), parsedProgram);

        TestRegister(interpreter, "acc", 0);

        interpreter.Step();
        TestRegister(interpreter, "acc", 3);

        interpreter.Step();
        TestRegister(interpreter, "acc", 1);
    }
}