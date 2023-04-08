using System;
using NUnit.Framework;
using Toaster.Execution;

namespace Toaster.Tests.Execution;

[TestFixture]
public class PinControllerTests
{
    [Test]
    public void GetPinValue()
    {
        PinController controller = new PinController(4);

        controller.SetInputPins(new[]
        {
            true, false, false, false,
        });

        controller.SetOutputPins(new[]
        {
            false, true, false, false,
        });

        Assert.IsTrue(controller.GetPinValue(0));
        Assert.IsTrue(controller.GetPinValue(1));
        Assert.IsFalse(controller.GetPinValue(2));
        Assert.IsFalse(controller.GetPinValue(3));
    }

    [Test]
    public void GetOutputValue()
    {
        PinController controller = new PinController(4);

        controller.SetInputPins(new[]
        {
            true, false, false, false,
        });

        controller.SetOutputPins(new[]
        {
            false, true, false, false,
        });

        Assert.IsFalse(controller.GetOutputPin(0));
        Assert.IsTrue(controller.GetOutputPin(1));
        Assert.IsFalse(controller.GetOutputPin(2));
        Assert.IsFalse(controller.GetOutputPin(3));
    }

    [Test]
    public void GetInputValue()
    {
        PinController controller = new PinController(4);

        controller.SetInputPins(new[]
        {
            true, false, false, false,
        });

        controller.SetOutputPins(new[]
        {
            false, true, false, false,
        });

        Assert.IsTrue(controller.GetInputPin(0));
        Assert.IsFalse(controller.GetInputPin(1));
        Assert.IsFalse(controller.GetInputPin(2));
        Assert.IsFalse(controller.GetInputPin(3));
    }

    [Test]
    public void SetInputValues()
    {
        PinController controller = new PinController(4);

        controller.SetInputPins(new[]
        {
            false, false, false, false,
        });

        Assert.IsFalse(controller.GetInputPin(0));
        Assert.IsFalse(controller.GetInputPin(1));
        Assert.IsFalse(controller.GetInputPin(2));
        Assert.IsFalse(controller.GetInputPin(3));

        controller.SetInputPins(new[]
        {
            false, true, true, false,
        });

        Assert.IsFalse(controller.GetInputPin(0));
        Assert.IsTrue(controller.GetInputPin(1));
        Assert.IsTrue(controller.GetInputPin(2));
        Assert.IsFalse(controller.GetInputPin(3));
    }

    [Test]
    public void SetOutputValues()
    {
        PinController controller = new PinController(4);

        controller.SetOutputPins(new[]
        {
            false, false, false, false,
        });

        Assert.IsFalse(controller.GetOutputPin(0));
        Assert.IsFalse(controller.GetOutputPin(1));
        Assert.IsFalse(controller.GetOutputPin(2));
        Assert.IsFalse(controller.GetOutputPin(3));

        controller.SetOutputPins(new[]
        {
            false, true, true, false,
        });

        Assert.IsFalse(controller.GetOutputPin(0));
        Assert.IsTrue(controller.GetOutputPin(1));
        Assert.IsTrue(controller.GetOutputPin(2));
        Assert.IsFalse(controller.GetOutputPin(3));
    }

    [Test]
    public void InputDoesNotModify()
    {
        PinController controller = new PinController(4);

        bool[] values = new[]
        {
            false, false, false, false,
        };

        controller.SetOutputPins(values);

        Assert.IsFalse(controller.GetOutputPin(0));
        Assert.IsFalse(controller.GetOutputPin(1));
        Assert.IsFalse(controller.GetOutputPin(2));
        Assert.IsFalse(controller.GetOutputPin(3));

        values[0] = true;
        values[1] = true;
        values[2] = true;
        values[3] = true;

        Assert.IsFalse(controller.GetOutputPin(0));
        Assert.IsFalse(controller.GetOutputPin(1));
        Assert.IsFalse(controller.GetOutputPin(2));
        Assert.IsFalse(controller.GetOutputPin(3));
    }

    [Test]
    public void SetInputValue()
    {
        PinController controller = new PinController(4);

        controller.SetInputPin(3, true);

        Assert.IsFalse(controller.GetPinValue(0));
        Assert.IsFalse(controller.GetPinValue(1));
        Assert.IsFalse(controller.GetPinValue(2));
        Assert.IsTrue(controller.GetPinValue(3));
    }

    [Test]
    public void SetOutputValue()
    {
        PinController controller = new PinController(4);

        controller.SetOutputPin(3, true);

        Assert.IsFalse(controller.GetPinValue(0));
        Assert.IsFalse(controller.GetPinValue(1));
        Assert.IsFalse(controller.GetPinValue(2));
        Assert.IsTrue(controller.GetPinValue(3));
    }

    [Test]
    public void ThrowOnArguments()
    {
        PinController controller = new PinController(4);

        Assert.Throws<ArgumentException>(() => controller.SetInputPins(new bool[2]));
        Assert.Throws<ArgumentException>(() => controller.SetOutputPins(new bool[2]));

        Assert.Throws<ArgumentOutOfRangeException>(() => controller.SetInputPin(-1, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.SetOutputPin(-1, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.SetInputPin(4, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.SetOutputPin(4, false));

        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetPinValue(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetInputPin(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetOutputPin(-1));

        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetPinValue(4));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetInputPin(4));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetOutputPin(4));
    }
}