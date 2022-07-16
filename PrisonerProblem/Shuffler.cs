using System.Security.Cryptography;

public static class Shuffler
{
    /// <summary>
    /// This shuffles given array of integers using the https://en.wikipedia.org/wiki/Fisher–Yates_shuffle.
    /// </summary>
    /// <param name="arr">Array of integers to shuffle.</param>
    public static void Shuffle(this int[] arr)
    {
        for (int elementNum = arr.Length; elementNum > 0; elementNum--)
        {
            var rng = RandomNumberGenerator.GetInt32(0, elementNum);
            (arr[elementNum - 1], arr[rng]) = (arr[rng], arr[elementNum - 1]);
        }
    }
}
