namespace ELO.PointOperations;

public static class MathUtilities
{
    public static int GetHighestBit(BigInteger k) => (int)(k.GetBitLength() - 1);

    public static bool IsBitSet(BigInteger k, int i) => (k & BigInteger.One << i) != 0;

    public static BigInteger ModInverse(BigInteger a, BigInteger p)
    {
        if (p == 1)
            return 0;

        if (BigInteger.GreatestCommonDivisor(a, p) != 1)
            throw new InvalidOperationException("Inverse does not exist - a and p must be coprime.");

        return BigInteger.ModPow(a, p - 2, p);
    }
}