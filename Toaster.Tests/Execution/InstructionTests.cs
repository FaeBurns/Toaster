using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Tests.Execution;

[TestFixture]
public class InstructionTests
{
    private void ExecuteAll(Interpreter interpreter)
    {
        while (!interpreter.Finished)
        {
            AssertStep(interpreter);
        }
    }

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

    private void TestPin(Interpreter interpreter, int pinIndex, bool pinValue)
    {
        Assert.AreEqual(pinValue, interpreter.PinController.GetOutputPin(pinIndex), $"pin at index {pinIndex} did not have expected value of {pinValue}");
    }

    private void TestPins(Interpreter interpreter, int startIndex, params bool[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            bool value = interpreter.PinController.GetOutputPin(i + startIndex);
            Assert.AreEqual(values[i], value, $"value at pin {i + startIndex} is incorrect");
        }
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

    [Test]
    public void MoveShiftLeftInstruction()
    {
        Interpreter interpreter = InterpretProgram("msl $named0 0b00001111 2");

        AssertStep(interpreter);
        TestRegister(interpreter, "named0", 60);
    }

    [Test]
    public void MoveShiftRightInstruction_Truncated()
    {
        Interpreter interpreter = InterpretProgram("msr $named0 0b00001111 2");

        AssertStep(interpreter);
        TestRegister(interpreter, "named0", 3);
    }

    [Test]
    public void MoveShiftRightInstruction()
    {
        Interpreter interpreter = InterpretProgram("msr $named0 0b00001100 2");

        AssertStep(interpreter);
        TestRegister(interpreter, "named0", 3);
    }

    [Test]
    public void NotInstruction()
    {
        Interpreter interpreter = InterpretProgram("mov $r0 0b0000111100001111\nnot $r0");

        // mov
        interpreter.Step();

        // not
        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 61680);
    }

    [Test]
    public void OrInstruction()
    {
        Interpreter interpreter = InterpretProgram("mov $r0 0b1001_1001_1001_1001\nior $r0 0b0010_0010_0010_0010");

        interpreter.Step();

        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 0b1011_1011_1011_1011);
    }

    [Test]
    public void XorInstruction()
    {
        Interpreter interpreter = InterpretProgram("mov $r0 0b1001_1001_1001_1001\nxor $r0 0b0011_0011_0011_0011");

        interpreter.Step();

        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 0b1010_1010_1010_1010);
    }

    [Test]
    public void TestAreEqualInstruction()
    {
        Interpreter interpreter = InterpretProgram("teq 0b1100 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 1);
    }

    [Test]
    public void TestAreEqualInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("teq 0b1101 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 0);
    }

    [Test]
    public void TestNotEqualInstruction()
    {
        Interpreter interpreter = InterpretProgram("tne 0b1101 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 1);
    }

    [Test]
    public void TestNotEqualInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("tne 0b1100 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 0);
    }

    [Test]
    public void TestGreaterThanInstruction()
    {
        Interpreter interpreter = InterpretProgram("tgt 0b1101 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 1);
    }

    [Test]
    public void TestGreaterThanInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("tgt 0b1100 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 0);
    }

    [Test]
    public void TestLessThanInstruction()
    {
        Interpreter interpreter = InterpretProgram("tlt 0b1011 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 1);
    }

    [Test]
    public void TestLessThanInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("tgt 0b1100 0x0C");

        AssertStep(interpreter);
        TestRegister(interpreter, "t", 0);
    }

    [Test]
    public void SetPinsInstruction_Single()
    {
        Interpreter interpreter = InterpretProgram("stp .p0 1\nstp .p0 0\nstp .p0 1");

        AssertStep(interpreter);
        TestPin(interpreter, 0, true);

        AssertStep(interpreter);
        TestPin(interpreter, 0, false);

        AssertStep(interpreter);
        TestPin(interpreter, 0, true);
    }

    [Test]
    public void SetPinsInstruction_Multiple_SmallerInput()
    {
        Interpreter interpreter = InterpretProgram("stp .p0..p7 0b1011\nstp .p2..p7 0xF0\nstp .p0:8 0");

        AssertStep(interpreter);
        TestPins(interpreter, 0, false, false, false, false, true, false, true, true);

        AssertStep(interpreter);
        TestPins(interpreter, 0, false, false, true, true, false, false, false, false);

        AssertStep(interpreter);
        TestPins(interpreter, 0, false, false, false, false, false, false, false, false);
    }

    [Test]
    public void SetPinsInstruction_Multiple_BiggerInput()
    {
        Interpreter interpreter = InterpretProgram("stp .p0:8 0b1100_0110_0011\nstp .p0:8 0b1_0011_0011");

        AssertStep(interpreter);
        TestPins(interpreter, 0, false, true, true, false, false, false, true, true);

        AssertStep(interpreter);
        TestPins(interpreter, 0, false, false, true, true, false, false, true, true);
    }

    [Test]
    public void GetPinsInstruction_Single()
    {
        Interpreter interpreter = InterpretProgram("ldp $r0 .p0");

        interpreter.PinController.SetInputPin(0, true);

        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 1);
    }

    [Test]
    public void GetPinsInstruction_Multiple()
    {
        Interpreter interpreter = InterpretProgram("ldp $r0 .p0:8");

        interpreter.PinController.SetInputPin(0, false);
        interpreter.PinController.SetInputPin(1, true);
        interpreter.PinController.SetInputPin(2, true);
        interpreter.PinController.SetInputPin(3, false);
        interpreter.PinController.SetInputPin(4, false);
        interpreter.PinController.SetInputPin(5, true);
        interpreter.PinController.SetInputPin(6, true);
        interpreter.PinController.SetInputPin(7, false);

        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 102);
    }

    [Test]
    public void GetPinsInstruction_InternallySet()
    {
        Interpreter interpreter = InterpretProgram("stp .p0:8 102\nldp $r0 .p0:8");

        interpreter.Step();

        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 102);
    }

    [Test]
    public void GetPinsInstruction_MergesValue()
    {
        Interpreter interpreter = InterpretProgram("stp .p0:8 102\nmov $r0 0b1111_0000_0000_0000\nldp $r0 .p0:8");

        interpreter.Step();
        interpreter.Step();

        AssertStep(interpreter);
        TestRegister(interpreter, "r0", 0b1111_0000_0110_0110);
    }

    [Test]
    public void ClearPinsInstruction_Single()
    {
        Interpreter interpreter = InterpretProgram("stp .p0:8 0xFF\nclr .p0");

        interpreter.Step();

        AssertStep(interpreter);
        TestPin(interpreter, 0, false);
    }

    [Test]
    public void ClearPinsInstruction_Multiple()
    {
        Interpreter interpreter = InterpretProgram("stp .p0:8 0xFF\nclr .p0:8");

        interpreter.Step();
        AssertStep(interpreter);
        TestPins(interpreter, 0, false, false, false, false, false, false, false, false);
    }

    [Test]
    public void JumpInstruction_PreviousLabel()
    {
        string program = ":label\nadd 1\njmp label";
        Interpreter interpreter = InterpretProgram(program);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 1);
        Assert.AreEqual(1, interpreter.CurrentLineIndex);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 1);
        Assert.AreEqual(2, interpreter.CurrentLineIndex);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 2);
        Assert.AreEqual(1, interpreter.CurrentLineIndex);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 2);
        Assert.AreEqual(2, interpreter.CurrentLineIndex);

        AssertStep(interpreter);
        TestRegister(interpreter, "acc", 3);
        Assert.AreEqual(1, interpreter.CurrentLineIndex);
    }

    [Test]
    public void JumpInstruction_Bounce()
    {
        // :label1
        // jmp label2
        // mov $r0 1 ? should never occur
        // :label2
        // jmp label1
        // mov $r0 2
        Interpreter interpreter = InterpretProgram(":label1\njmp label2\nmov $r0 100\n:label2\njmp label1");

        // loop it 5 times to make sure it keeps in the loop
        for (int i = 0; i < 5; i++)
        {
            AssertStep(interpreter);
            Assert.AreEqual(interpreter.CurrentLineIndex, 1);
            TestRegister(interpreter, "r0", 0);

            AssertStep(interpreter);
            Assert.AreEqual(interpreter.CurrentLineIndex, 4);
            TestRegister(interpreter, "r0", 0);
        }
    }

    [Test]
    public void JumpInstruction_Bounce_Index()
    {
        // mov $r1 1
        // jmp 3
        // mov $r0 1 ? should never occur
        // jmp $r1
        // mov $r0 2
        Interpreter interpreter = InterpretProgram("mov $r1 1\njmp 3\nmov $r0 1\njmp $r1\nmov $r0 1");

        // mov
        interpreter.Step();

        // loop it 5 times to make sure it keeps in the loop
        for (int i = 0; i < 5; i++)
        {
            AssertStep(interpreter);
            Assert.AreEqual(interpreter.CurrentLineIndex, 1);
            TestRegister(interpreter, "r0", 0);

            AssertStep(interpreter);
            Assert.AreEqual(interpreter.CurrentLineIndex, 3);
            TestRegister(interpreter, "r0", 0);
        }
    }

    [Test]
    public void JumpSetReturnInstruction()
    {
        // mov is for padding
        Interpreter interpreter = InterpretProgram(":label\nmov $acc $acc\njsr label");

        Assert.AreEqual(0, interpreter.StackDepth);

        // loop to ensure
        for (int i = 0; i < 5; i++)
        {
            // mov
            interpreter.Step();

            AssertStep(interpreter);
            Assert.AreEqual(i + 1, interpreter.StackDepth);
            TestRegister(interpreter, "ra", 3);
        }
    }

    [Test]
    public void JumpSetReturnInstruction_Overflow()
    {
        Interpreter interpreter = InterpretProgram(":label\njsr label");

        Assert.AreEqual(0, interpreter.StackDepth);

        // loop to ensure
        // run 5 times to reach depth limit
        for (int i = 0; i < 5; i++)
        {
            AssertStep(interpreter);
            Assert.AreEqual(i + 1, interpreter.StackDepth);
        }

        interpreter.Step();
        Assert.IsTrue(interpreter.HasErrors, interpreter.InstructionErrorCollection.ToString());

        Console.WriteLine(interpreter.InstructionErrorCollection.ToString());
    }

    [Test]
    public void ReturnInstruction()
    {
        Interpreter interpreter = InterpretProgram("mov $s0 1\njsr label\n:label\nmov $s0 2\n ret");

        // mov
        interpreter.Step();
        TestRegister(interpreter, "s0", 1);

        // jsr
        interpreter.Step();
        TestRegister(interpreter, "s0", 0);

        // mov
        interpreter.Step();
        Assert.AreEqual(3, interpreter.CurrentLineIndex);
        TestRegister(interpreter, "s0", 2);

        // ret
        AssertStep(interpreter);
        // test that $s0 has reverted to the previous pre-jump value
        TestRegister(interpreter, "s0", 1);
    }

    [Test]
    public void ReturnIfTrueInstruction_Recursive()
    {
        string[] programLines = new[]
        {
            "mov $r0 4",
            "jsr loop",
            "jmp exit",
            ":loop",
            "teq $acc $r0",
            "rit",
            "add 1",
            "jsr loop",
            "ret",
            ":exit",
        };

        Interpreter interpreter = InterpretProgram(string.Join("\n", programLines));

        ExecuteAll(interpreter);

        Assert.AreEqual(0, interpreter.StackDepth);
        TestRegister(interpreter, "acc", 4);
    }

    [Test]
    public void ReturnInstruction_ReturnValue()
    {
        Interpreter interpreter = InterpretProgram("jsr func\njmp exit\n:func\nret 5\n:exit");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "rv", 5);
    }

    [Test]
    public void BranchAreEqualInstruction_True()
    {
        Interpreter interpreter = InterpretProgram("beq label 1 1\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 0);
    }

    [Test]
    public void BranchAreEqualInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("beq label 1 5\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 1);
    }

    [Test]
    public void BranchGreaterThanInstruction_True()
    {
        Interpreter interpreter = InterpretProgram("bgt label 2 1\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 0);
    }

    [Test]
    public void BranchGreaterThanInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("bgt label 1 1\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 1);
    }

    [Test]
    public void BranchLessThanInstruction_True()
    {
        Interpreter interpreter = InterpretProgram("blt label 1 2\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 0);
    }

    [Test]
    public void BranchLessThanInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("blt label 1 1\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 1);
    }

    [Test]
    public void BranchNotEqualInstruction_True()
    {
        Interpreter interpreter = InterpretProgram("bne label 2 1\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 0);
    }

    [Test]
    public void BranchNotEqualInstruction_False()
    {
        Interpreter interpreter = InterpretProgram("bne label 1 1\nmov $r0 1\n:label");

        ExecuteAll(interpreter);

        TestRegister(interpreter, "r0", 1);
    }
}