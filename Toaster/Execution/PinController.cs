using System;

namespace Toaster.Execution;

/// <summary>
/// Controls reading and setting input and output pins.
/// </summary>
public class PinController
{
    private readonly bool[] _savedPinValues;

    public PinController(int pinCount)
    {
        _savedPinValues = new bool[pinCount];
    }

    /// <summary>
    /// Sets the values stored in the indexed pins.
    /// </summary>
    /// <param name="pinValues">The new values to store.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pinValues"/> does not match the number of indexed pins.</exception>
    public void SetIndexedPinValues(bool[] pinValues)
    {
        if (pinValues.Length != _savedPinValues.Length)
            throw new ArgumentOutOfRangeException(nameof(pinValues), _savedPinValues.Length, "argument array does not match pin count");

        Array.Copy(pinValues, _savedPinValues, pinValues.Length);
    }

    /// <summary>
    /// Gets a copy of the values stored in the indexed pins.
    /// </summary>
    /// <returns>The values stored in the pins.</returns>
    public bool[] GetIndexedPinValues()
    {
        // create result array and copy values to it
        bool[] resultArray = new bool[_savedPinValues.Length];
        Array.Copy(_savedPinValues, resultArray, _savedPinValues.Length);

        return resultArray;
    }

    /// <summary>
    /// Sets the value stored at the specific pin index.
    /// </summary>
    /// <param name="pinIndex">The index of the pin to set.</param>
    /// <param name="value">The value to set the pin to.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was too large or too small.</exception>
    public void SetIndexedPinValue(int pinIndex, bool value)
    {
        if (pinIndex >= _savedPinValues.Length)
            throw new ArgumentOutOfRangeException(nameof(pinIndex), _savedPinValues.Length, "target pin index is above the range of pin indices");

        if (pinIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pinIndex), 0, "target pin index is below the range of pin indices");

        _savedPinValues[pinIndex] = value;
    }

    /// <summary>
    /// Gets the value stored at the specific pin index.
    /// </summary>
    /// <param name="pinIndex">The index of the pin to set.</param>
    /// <returns>The value stored in the pin.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index was too large or too small.</exception>
    public bool GetIndexedPinValue(int pinIndex)
    {
        if (pinIndex >= _savedPinValues.Length)
            throw new ArgumentOutOfRangeException(nameof(pinIndex), _savedPinValues.Length, "target pin index is above the range of pin indices");

        if (pinIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pinIndex), 0, "target pin index is below the range of pin indices");

        return _savedPinValues[pinIndex];
    }
}