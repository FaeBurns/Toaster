using NUnit.Framework;
using Toaster.Definition;
using Toaster.Instructions;
using Toaster.Instructions.Math;
using Toaster.Parsing;

namespace Toaster.Tests.Definition;

[TestFixture]
public class InstructionManagerTests
{
    private void AssertFetches<T>(string name, params TokenId[] args) where T : Instruction
    {
        Instruction result = InstructionManager.TryFetchInstructionBySignature(name, args);

        Assert.NotNull(result);

        Assert.AreEqual(typeof(T), result!.GetType());
    }

    [Test]
    public void TryFetch_UnknownName()
    {
        Assert.Throws<AssertionException>(() => AssertFetches<AddInstruction>("yes"));
    }

    [Test]
    public void TryFetch_UnknownOverload()
    {
        AssertFetches<AddInstruction>("add", TokenId.REGISTER, TokenId.LABEL_ARG);
    }

    [Test]
    public void TryFetch_Overload()
    {
        AssertFetches<AddInstruction>("add", TokenId.REGISTER, TokenId.HEX);
    }

    [Test]
    public void TryFetch()
    {
        AssertFetches<AddInstruction>("add", TokenId.REGISTER);
    }
}