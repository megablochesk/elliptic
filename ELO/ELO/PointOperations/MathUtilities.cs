using System.Numerics;

namespace ELO.PointOperations;

public static class MathUtilities
{
    public static int GetHighestBit(BigInteger k) => (int)(k.GetBitLength() - 1);

    public static bool IsBitSet(BigInteger k, int i) => (k & BigInteger.One << i) != 0;

    public static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m;
        BigInteger y = 0, x = 1;

        if (m == 1)
            return 0;

        while (a > 1)
        {
            BigInteger q = a / m;
            BigInteger t = m;

            m = a % m;
            a = t;
            t = y;

            y = x - q * y;
            x = t;
        }

        if (x < 0)
            x += m0;

        return x;
    }
}