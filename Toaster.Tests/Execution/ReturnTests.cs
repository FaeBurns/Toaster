using System;
using System.Linq;
using NUnit.Framework;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Tests.Execution;

[TestFixture]
public class ReturnTests
{
    [Test]
    public void TestReturnIssue_NameME()
    {
        string[] programLines = new[]
        {
            "mov $acc 5",
            ":loop", 
            "jsr getpins", 
            "mov $r0 $rv",
            "jsr setpins",
            "add 1",
            "jmp loop",
            ":getpins",
            "mov $s0 $acc",
            "ret $s0",
            ":setpins",
            "mov $r1 $r0",
            "ret",
        };

        string program = String.Join("\n", programLines);

        TokenProgram tokenProgram = TestHelpers.GetProgram(program);
        Interpreter interpreter = new Interpreter(TestHelpers.GetGenericConfig(), tokenProgram);
        
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        
        Assert.AreEqual(5, interpreter.GetRegisterValue("rv"));
        
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        
        Assert.AreEqual(6, interpreter.GetRegisterValue("rv"));
        
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
        interpreter.Step();
    }
}