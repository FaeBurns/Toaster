using System;

namespace Toaster.Execution;

/// <summary>
/// Handles getting and setting a set of indexed input and output pins.
/// </summary>
public class PinController
{
    private readonly bool[] _inputValues;
    private readonly bool[] _outputValues;

    /// <summary>
    /// Gets the amount of pins present on the controller.
    /// </summary>
    public int PinCount { get; }

    public PinController(int pinCount)
    {
        PinCount = pinCount;

        _inputValues = new bool[pinCount];
        _outputValues = new bool[pinCount];
    }

    /// <summary>
    /// <p>Gets the value stored in the specified pin. Result is taken from both input and output pins.</p>
    /// <p>This is likely the version you want to use instead of <see cref="GetInputPin"/> or <see cref="GetOutputPin"/>.</p>
    /// </summary>
    /// <param name="index">The index of the pin to get.</param>
    /// <returns>The value stored in the pin.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside the bounds of the pin set.</exception>
    public bool GetPinValue(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index cannot be less than zero");

        if (index >= PinCount)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index must be less than the pin count");

        // return result of or operation to get if it's set in either.
        return _inputValues[index] | _outputValues[index];
    }

    /// <summary>
    /// Sets the input pins to the specified values.
    /// </summary>
    /// <param name="values">The values to set the pins to.</param>
    /// <exception cref="ArgumentException">length of <paramref name="values"/> does not match <see cref="PinCount"/>.</exception>
    public void SetInputPins(bool[] values)
    {
        if (values.Length != PinCount)
            throw new ArgumentException($"length of values array must match {nameof(PinCount)}");

        Array.Copy(values, _inputValues, PinCount);
    }

    /// <summary>
    /// Sets the output pins to the specified values.
    /// </summary>
    /// <param name="values">The values to set the pins to.</param>
    /// <exception cref="ArgumentException">length of <paramref name="values"/> does not match <see cref="PinCount"/>.</exception>
    public void SetOutputPins(bool[] values)
    {
        if (values.Length != PinCount)
            throw new ArgumentException($"length of values array must match {nameof(PinCount)}");

        Array.Copy(values, _outputValues, PinCount);
    }

    /// <summary>
    /// Sets the specified input pin to the provided value.
    /// </summary>
    /// <param name="index">The index of the pin to set.</param>
    /// <param name="value">The value to set the pin to.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside the bounds of the pin set.</exception>
    public void SetInputPin(int index, bool value)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index cannot be less than zero");

        if (index >= PinCount)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index must be less than the pin count");

        _inputValues[index] = value;
    }

    /// <summary>
    /// Sets the specified output pin to the provided value.
    /// </summary>
    /// <param name="index">The index of the pin to set.</param>
    /// <param name="value">The value to set the pin to.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside the bounds of the pin set.</exception>
    public void SetOutputPin(int index, bool value)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index cannot be less than zero");

        if (index >= PinCount)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index must be less than the pin count");

        _outputValues[index] = value;
    }

    /// <summary>
    /// Gets the value of the specified input pin.
    /// </summary>
    /// <param name="index">The index of the pin to get.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside the bounds of the pin set.</exception>
    public bool GetInputPin(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index cannot be less than zero");

        if (index >= PinCount)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index must be less than the pin count");

        return _inputValues[index];
    }

    /// <summary>
    /// Gets the value of the specified output pin.
    /// </summary>
    /// <param name="index">The index of the pin to get.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> was outside the bounds of the pin set.</exception>
    public bool GetOutputPin(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index cannot be less than zero");

        if (index >= PinCount)
            throw new ArgumentOutOfRangeException(nameof(index), index, "index must be less than the pin count");

        return _outputValues[index];
    }
}