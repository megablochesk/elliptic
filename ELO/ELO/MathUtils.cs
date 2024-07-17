using System.Numerics;

namespace ELO;

public static class MathUtils
{
    public static int GetHighestBit(BigInteger k) => (int)(k.GetBitLength() - 1);

    public static bool IsBitSet(BigInteger k, int i) => (k & (BigInteger.One << i)) != 0;

    public static BigInteger ModInverse(BigInteger a, BigInteger mod)
    {
        BigInteger m0 = mod, t, q;
        BigInteger x0 = 0, x1 = 1;

        if (mod == 1) return 0;

        while (a > 1)
        {
            q = a / mod;

            t = mod;

            mod = a % mod; a = t;

            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }

        if (x1 < 0)
            x1 += m0;

        return x1;
    }
}