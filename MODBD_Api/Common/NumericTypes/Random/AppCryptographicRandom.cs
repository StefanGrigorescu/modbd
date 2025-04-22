using System.Security.Cryptography;

namespace MODBD_Api.Common.NumericTypes.Random;

public partial class AppCryptographicRandom : ICryptographicRandom
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public string NextBase64String(int length)
    {
        byte[] randomNumber = new byte[length];
        _rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    //private static readonly System.Random _random = new();

    ///// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
    ///// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    //public double NextDouble() =>
    //    _random.NextDouble();

    ///// <summary>Returns a random integer that is within a specified range.</summary>
    ///// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    ///// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
    ///// <returns>
    ///// A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
    ///// but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
    ///// </returns>
    ///// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
    //public int Next(int minValue, int maxValue) =>
    //    _random.Next(minValue, maxValue);

    ///// <summary>
    ///// Randomly accesses an element in <paramref name="elements"/> collection and returns it.
    ///// </summary>
    ///// <typeparam name="T">Elements type.</typeparam>
    ///// <param name="elements">Collection.</param>
    ///// <returns>Element at random index in <paramref name="elements"/> collection.</returns>
    //public AppResponse<T> NextElement<T>(IReadOnlyList<T> elements)
    //{
    //    if (elements.Count == 0)
    //    {
    //        return AppResponse<T>.Failed("Could not retrieve random element from empty collection.");
    //    }
    //    int randomIdx = _random.Next(elements.Count);
    //    return AppResponse<T>.Succeeded(elements[randomIdx]);
    //}

    //public Guid NextGuid()
    //     => Guid.NewGuid();
}


public interface ICryptographicRandom
{
    string NextBase64String(int length);

    ///// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
    ///// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    //double NextDouble();

    ///// <summary>Returns a random integer that is within a specified range.</summary>
    ///// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    ///// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
    ///// <returns>
    ///// A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
    ///// but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
    ///// </returns>
    ///// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
    //int Next(int minValue, int maxValue);

    ///// <summary>
    ///// Randomly accesses an element in <paramref name="elements"/> collection and returns it.
    ///// </summary>
    ///// <typeparam name="T">Elements type.</typeparam>
    ///// <param name="elements">Collection.</param>
    ///// <returns>Element at random index in <paramref name="elements"/> collection.</returns>
    //AppResponse<T> NextElement<T>(IReadOnlyList<T> elements);

    //Guid NextGuid();
}
