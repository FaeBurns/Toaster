using System;
using NUnit.Framework;
using Toaster.Definition;
using Toaster.Parsing;
using Toaster.Parsing.TokenValidators;

namespace Toaster.Tests.Parsing;

[TestFixture]
public class ProgramValidationTests
{
    [Test]
    public void Valid_Mov()
    {
        TokenProgramValidator programValidator = new TokenProgramValidator();
        programValidator.Validate(TestHelpers.GetProgram("mov $named0 $named1"), TestHelpers.GetGenericConfig());

        Assert.IsTrue(programValidator.ErrorCollection.IsOk, programValidator.ErrorCollection.ToString());
    }

    [Test]
    public void Valid_Mov_Comment()
    {
        TokenProgramValidator programValidator = new TokenProgramValidator();
        programValidator.Validate(TestHelpers.GetProgram("mov $named0 $named1; comment"), TestHelpers.GetGenericConfig());

        Assert.IsTrue(programValidator.ErrorCollection.IsOk, programValidator.ErrorCollection.ToString());
    }

    [Test]
    public void Invalid_Mov()
    {
        TokenProgramValidator programValidator = new TokenProgramValidator();
        programValidator.Validate(TestHelpers.GetProgram("mov named0 $named"), TestHelpers.GetGenericConfig());

        Assert.AreEqual(3, programValidator.ErrorCollection.Errors.Count, programValidator.ErrorCollection.ToString());
        Assert.IsTrue(programValidator.ErrorCollection.Errors[0].Message.StartsWith("Could not find valid override for mov."), "Fail during message test");
    }

    [Test]
    public void Invalid_Name()
    {
        TokenProgramValidator programValidator = new TokenProgramValidator();
        programValidator.Validate(TestHelpers.GetProgram("invalid $acc $t"), TestHelpers.GetGenericConfig());

        Assert.IsFalse(programValidator.ErrorCollection.IsOk);
        Assert.IsTrue(programValidator.ErrorCollection.Errors[0].Message.StartsWith("Cannot find instruction with name"));
    }
}