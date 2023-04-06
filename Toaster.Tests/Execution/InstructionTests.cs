using System;
using System.Collections.Generic;
using NUnit.Framework;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Tests.Execution;

[TestFixture]
public class InstructionTests
{
    private void AssertStep(Interpreter interpreter)
    {
        interpreter.Step();
        Assert.IsTrue(interpreter.InstructionErrorCollection.IsOk, "interpreter encountered errors.\n" + interpreter.InstructionErrorCollection);
    }

    private void TestRegister(Interpreter interpreter, string name, ushort value)
    {
        IReadOnlyDictionary<string, ushort> registerValues = interpreter.GetRegisterValues();

        Assert.AreEqual(value, registerValues[name]);
    }

    private Interpreter InterpretProgram(string program)
    {
        TokenProgram parsedProgram = new Parser().Tokenize(program);

        return new Interpreter(TestHelpers.GetGenericConfig(), parsedProgram);
    }

    [Test]
    public void AddSubtract()
    {
        string program = "add 3\nsub 2";

        TokenProgram parsedProgram = new Parser().Tokenize(program);

        Interpreter interpreter = new Interpreter(TestHelpers.GetGenericConfig(), parsedProgram);

        TestRegister(interpreter, "acc", 0);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 3);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 1);
    }

    [Test]
    public void Add_Overflow()
    {
        string program = "add " + UInt16.MaxValue + "\nadd " + 5;

        TokenProgram parsedProgram = new Parser().Tokenize(program);

        Interpreter interpreter = new Interpreter(TestHelpers.GetGenericConfig(), parsedProgram);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", UInt16.MaxValue);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 4);
    }

    [Test]
    public void Subtract_TwoArgs()
    {
        Interpreter interpreter = InterpretProgram("sub 10 99");

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 89);
    }

    [Test]
    public void Subtract_Underflow()
    {
        Interpreter interpreter = InterpretProgram("sub 10");

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 65526);
    }

    [Test]
    public void Divide_SingleArg()
    {
        Interpreter interpreter = InterpretProgram("mov $acc 10\ndiv 3");

        // mov
        interpreter.Step();

        // div
        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 3);
    }

    [Test]
    public void Divide_MultipleArg()
    {
        Interpreter interpreter = InterpretProgram("div 10 3");

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 3);
    }

    [Test]
    public void Multiply_SingleArg()
    {
        Interpreter interpreter = InterpretProgram("mov $acc 10\nmul 3");

        // mov
        interpreter.Step();

        // mul
        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 30);
    }

    [Test]
    public void Multiply_MultipleArg()
    {
        Interpreter interpreter = InterpretProgram("mul 10 3");

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 30);
    }

    [Test]
    public void Multiply_Overflow()
    {
        Interpreter interpreter = InterpretProgram($"mul 32767 3");// uint16 max / 2

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 32765);
    }

    [Test]
    public void AndInstruction()
    {
        Interpreter interpreter = InterpretProgram("mov $named1 0b1111111111111111\nand $named1 0b1010101010101010");

        // mov
        interpreter.Step();

        // and
        AssertStep(interpreter);
        TestRegister(interpreter, "named1", 43690);
    }
}