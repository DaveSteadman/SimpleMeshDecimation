// a 1D array of ints, useful in storing loop ranges


using System;
using System.Collections.Generic;


/// <summary>
/// Represents a 1D array of integers with utility methods for common operations.
/// Useful for storing loop ranges and performing aggregate operations.
/// </summary>
public class FssInt1DArray
{
    private List<int> _data;

    /// <summary>
    /// Gets the number of elements in the array.
    /// </summary>
    public int Length => _data.Count;

    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the FssInt1DArray class with a specified size, filled with zeros.
    /// </summary>
    /// <param name="size">The initial size of the array.</param>
    public FssInt1DArray(int size)
    {
        _data = new List<int>(size);
        for (int i = 0; i < size; i++)
        {
            _data.Add(0); // Initialize with default value
        }
    }

    /// <summary>
    /// Initializes a new instance of the FssInt1DArray class with zero elements.
    /// </summary>
    public FssInt1DArray()
    {
        _data = new List<int>();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: basic set/get
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the value at the specified index.
    /// </summary>
    /// <param name="index">The index of the value to get or set.</param>
    /// <returns>The integer value at the specified index.</returns>
    public int this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }

    /// <summary>
    /// Adds a value to the end of the array.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public void Add(int value)
    {
        _data.Add(value);
    }

    /// <summary>
    /// Removes all elements from the array.
    /// </summary>
    public void Clear()
    {
        _data.Clear();
    }

    /// <summary>
    /// Gets the value at the specified index, with bounds checking.
    /// </summary>
    /// <param name="index">The index of the value to retrieve.</param>
    /// <returns>The integer value at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
    public int GetValue(int index)
    {
        if (index < 0 || index >= _data.Count)
        {
            throw new IndexOutOfRangeException("Index is out of range.");
        }
        return _data[index];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Complex set /get
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Returns the sum of all elements in the array.
    /// </summary>
    /// <returns>The sum of the elements.</returns>
    public int Sum()
    {
        int sum = 0;
        foreach (var value in _data)
            sum += value;

        return sum;
    }

    /// <summary>
    /// Returns the minimum value in the array.
    /// </summary>
    /// <returns>The minimum integer value.</returns>
    public int Min()
    {
        if (_data.Count == 0) throw new InvalidOperationException("Array is empty.");
        int min = _data[0];
        foreach (var value in _data)
            if (value < min) min = value;
        return min;
    }

    /// <summary>
    /// Returns the maximum value in the array.
    /// </summary>
    /// <returns>The maximum integer value.</returns>
    public int Max()
    {
        if (_data.Count == 0) throw new InvalidOperationException("Array is empty.");
        int max = _data[0];
        foreach (var value in _data)
            if (value > max) max = value;
        return max;
    }

    /// <summary>
    /// Returns the average of all elements in the array.
    /// </summary>
    /// <returns>The average as a double.</returns>
    public double Average()
    {
        if (_data.Count == 0) throw new InvalidOperationException("Array is empty.");
        return (double)Sum() / _data.Count;
    }

    /// <summary>
    /// Returns a shallow copy of the internal data as a standard array.
    /// </summary>
    /// <returns>An array of integers.</returns>
    public int[] ToArray()
    {
        return _data.ToArray();
    }

    /// <summary>
    /// Determines whether the array contains a specific value.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool Contains(int value)
    {
        return _data.Contains(value);
    }

    /// <summary>
    /// Removes the first occurrence of a specific value from the array.
    /// </summary>
    /// <param name="value">The value to remove.</param>
    /// <returns>True if the value was removed; otherwise, false.</returns>
    public bool Remove(int value)
    {
        return _data.Remove(value);
    }

    /// <summary>
    /// Returns the index of the first occurrence of a value in the array, or -1 if not found.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>The index of the value, or -1 if not found.</returns>
    public int IndexOf(int value)
    {
        return _data.IndexOf(value);
    }

}