using System;
using NUnit.Framework;
using Toaster.Parsing;
using Toaster.Parsing.TokenReaders;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Tests.Parsing;

[TestFixture]
public class ValueExtractorTests
{
    [Test]
    public void Constant_OutOfRangeInteger()
    {
        Token token = TestHelpers.GetSingleToken(Int32.MaxValue.ToString());
        Assert.Throws<OverflowException>(() => new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void Constant_OutOfRangeBinary()
    {
        Token token = TestHelpers.GetSingleToken("0b010101010101010101");
        Assert.Throws<OverflowException>(() => new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void Constant_OutOfRangeHex()
    {
        Token token = TestHelpers.GetSingleToken("0xFFFF00");
        Assert.Throws<OverflowException>(() => new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void Constant_InRangeInteger()
    {
        Token token = TestHelpers.GetSingleToken(UInt16.MaxValue.ToString());
        Assert.AreEqual(UInt16.MaxValue, new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void Constant_InRangeBinary()
    {
        Token token = TestHelpers.GetSingleToken("0b01001");
        Assert.AreEqual(9, new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void Constant_InRangeHex()
    {
        Token token = TestHelpers.GetSingleToken("0xABCD");
        Assert.AreEqual(43981, new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void Constant_Binary_LeadingZeros()
    {
        Token token = TestHelpers.GetSingleToken("0b00000000000000001");
        Assert.AreEqual(1, new ConstantValueExtractor().ExtractValue(token));
    }

    [Test]
    public void String_Register()
    {
        Token token = TestHelpers.GetSingleToken("$register");
        Assert.AreEqual("register", new StringValueExtractor().ExtractValue(token));
    }

    [Test]
    public void String_Label()
    {
        Token token = TestHelpers.GetSingleToken(":label");
        Assert.AreEqual("label", new StringValueExtractor().ExtractValue(token));
    }

    [Test]
    public void SinglePin()
    {
        Token token = TestHelpers.GetSingleToken(".p100");
        Assert.AreEqual(100, new SinglePinValueExtractor().ExtractValue(token));
    }

    [Test]
    public void MultiplePin_Range()
    {
        Token token = TestHelpers.GetSingleToken(".p0..p199");
        Assert.AreEqual(new PinRange(0, 199, 200), new MultiplePinValueExtractor().ExtractValue(token));
    }

    [Test]
    public void MultiplePin_Length()
    {
        Token token = TestHelpers.GetSingleToken(".p5:200");
        Assert.AreEqual(new PinRange(5, 204, 200), new MultiplePinValueExtractor().ExtractValue(token));
    }

    [Test]
    public void MultiplePin_SinglePin()
    {
        Token token = TestHelpers.GetSingleToken(".p34");
        Assert.AreEqual(new PinRange(34, 34, 1), new MultiplePinValueExtractor().ExtractValue(token));
    }
}