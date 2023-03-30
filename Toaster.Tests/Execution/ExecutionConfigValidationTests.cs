using NUnit.Framework;
using Toaster.Execution;

namespace Toaster.Tests.Execution;

[TestFixture]
public class ExecutionConfigValidationTests
{
    [Test]
    public void Generic_Valid()
    {
        ExecutionConfig config = new ExecutionConfig()
        {
            NamedRegisters =
            {
                "posX",
                "posY",
            },

            BasicRegisterCount = 4,
            StackRegisterCount = 4,
            MaxStackDepth = 5,
            PinCount = 8,
        };

        ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
        validator.Validate();

        Assert.IsTrue(validator.IsOk);
    }

    [Test]
    public void Invalid_Names()
    {
        string[] invalidNames =
        {
            "acc", "t", "rv", "ra", "s0", "r10", "has_underline", "$invalid", "p2",
        };

        foreach (string invalidName in invalidNames)
        {
            ExecutionConfig config = new ExecutionConfig()
            {
                NamedRegisters =
                {
                    invalidName,
                },
            };

            ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
            validator.Validate();

            Assert.IsTrue(validator.HasErrors, $"register name: {invalidName} did not raise an error");
            Assert.AreEqual(1, validator.Errors.Count);
        }
    }

    [Test]
    public void Invalid_Counts()
    {
        ExecutionConfig config = new ExecutionConfig()
        {
            BasicRegisterCount = -1,
            MaxStackDepth = -1,
            PinCount = -1,
            StackRegisterCount = -1,
        };

        ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
        validator.Validate();

        Assert.IsTrue(validator.HasErrors);
        Assert.AreEqual(4, validator.Errors.Count);
    }

    [Test]
    public void Invalid_Duplicate_Names()
    {
        ExecutionConfig config = new ExecutionConfig()
        {
            NamedRegisters =
            {
                "name1",
                "name1",
                "name3",
            },
        };

        ExecutionConfigValidator validator = new ExecutionConfigValidator(config);
        validator.Validate();

        Assert.IsTrue(validator.HasErrors);
        Assert.AreEqual(1, validator.Errors.Count);
    }
}