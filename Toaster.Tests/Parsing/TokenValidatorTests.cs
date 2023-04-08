using System;
using NUnit.Framework;
using Toaster.Parsing;
using Toaster.Parsing.TokenValidators;

namespace Toaster.Tests.Parsing;

[TestFixture]
public class TokenValidatorTests
{
    [Test]
    public void LabelTokenValidator_Valid()
    {
        Token token1 = TestHelpers.GetSingleToken(":label1");
        Token token2 = TestHelpers.GetSingleToken(":label2");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        LabelTokenValidator validator = new LabelTokenValidator();
        validator.Validate(token1, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk);

        validator = new LabelTokenValidator();
        validator.Validate(token2, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk);
    }

    [Test]
    public void LabelTokenValidator_Invalid()
    {
        Token token1 = TestHelpers.GetSingleToken(":label1");
        Token token2 = TestHelpers.GetSingleToken(":label1");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        LabelTokenValidator validator = new LabelTokenValidator();
        validator.Validate(token1, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk);

        validator = new LabelTokenValidator();
        validator.Validate(token2, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void RegisterTokenValidator_Valid()
    {
        Token token = TestHelpers.GetSingleToken("$found");

        TokenValidationContext context = new TokenValidationContext(new[]
        {
            "found",
        });

        RegisterTokenValidator validator = new RegisterTokenValidator();
        validator.Validate(token, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk);
    }

    [Test]
    public void RegisterTokenValidator_Invalid()
    {
        Token token = TestHelpers.GetSingleToken("$notfound");

        TokenValidationContext context = new TokenValidationContext(new[]
        {
            "found",
        });

        RegisterTokenValidator validator = new RegisterTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void LabelArgumentTokenValidator_Invalid()
    {
        // whitespace before token stops it from becoming instruction token
        Token token = TestHelpers.GetSingleToken(" name");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        LabelArgumentTokenValidator validator = new LabelArgumentTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void LabelArgumentTokenValidator_Valid()
    {
        Token token = TestHelpers.GetSingleToken(" name");
        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());
        context.Labels.Add("name");

        LabelArgumentTokenValidator validator = new LabelArgumentTokenValidator();
        validator.Validate(token, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk);
    }

    [Test]
    public void MultiplePinTokenValidator_Invalid_LowerEnd()
    {
        // don't need to test the multiple types as that's done in ValueExtractorTests
        Token token = TestHelpers.GetSingleToken(".p8..p0");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        MultiPinTokenValidator validator = new MultiPinTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void MultiplePinTokenValidator_Invalid_PinCount()
    {
        // don't need to test the multiple types as that's done in ValueExtractorTests
        Token token = TestHelpers.GetSingleToken(".p0:8");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>())
        {
            PinCount = 4,
        };

        MultiPinTokenValidator validator = new MultiPinTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void MultiplePinTokenValidator_Invalid_TooHigh()
    {
        // don't need to test the multiple types as that's done in ValueExtractorTests
        Token token = TestHelpers.GetSingleToken(".p0:9999999999999999999999");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        MultiPinTokenValidator validator = new MultiPinTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void MultiplePinTokenValidator_Valid()
    {
        // don't need to test the multiple types as that's done in ValueExtractorTests
        Token token = TestHelpers.GetSingleToken(".p0..p7");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>())
        {
            PinCount = 8,
        };

        MultiPinTokenValidator validator = new MultiPinTokenValidator();
        validator.Validate(token, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void SinglePinTokenValidator_Invalid_TooHigh()
    {
        Token token = TestHelpers.GetSingleToken(".p99999999999999999999");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        SinglePinTokenValidator validator = new SinglePinTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void SinglePinTokenValidator_Invalid_PinCount()
    {
        Token token = TestHelpers.GetSingleToken(".p4");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>())
        {
            PinCount = 4,
        };

        SinglePinTokenValidator validator = new SinglePinTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void SinglePinTokenValidator_Valid()
    {
        Token token = TestHelpers.GetSingleToken(".p3");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>())
        {
            PinCount = 8,
        };

        SinglePinTokenValidator validator = new SinglePinTokenValidator();
        validator.Validate(token, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void ConstantTokenValidator_Invalid()
    {
        Token token = TestHelpers.GetSingleToken("9999999999999999999");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        ConstantTokenValidator validator = new ConstantTokenValidator();
        validator.Validate(token, context);

        Assert.IsFalse(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }

    [Test]
    public void ConstantTokenValidator_Valid()
    {
        Token token = TestHelpers.GetSingleToken("3");

        TokenValidationContext context = new TokenValidationContext(Array.Empty<string>());

        ConstantTokenValidator validator = new ConstantTokenValidator();
        validator.Validate(token, context);

        Assert.IsTrue(validator.ErrorCollection.IsOk, validator.ErrorCollection.ToString());
    }
}