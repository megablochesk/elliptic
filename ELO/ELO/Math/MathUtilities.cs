namespace ELO.StandardMath;

public static class MathUtilities
{
    private static readonly int twoToW = 1 << Curve.WindowSize;

    public static ushort GetHighestBit(BigInteger number) => (ushort)(number.GetBitLength() - 1);

    public static bool IsBitSet(BigInteger number, int bit) => (number & BigInteger.One << bit) != 0;

    public static bool IsOdd(BigInteger number) => (number & BigInteger.One) == 1;

    public static List<int> ComputeNAF(BigInteger number)
    {
        List<int> naf = [];

        while (number > 0)
        {
            if (IsOdd(number))
            {
                var z = 2 - (int)(number % 4);
                naf.Add(z);
                number -= z;
            }
            else
            {
                naf.Add(0);
            }

            number >>= 1;
        }

        return naf;
    }

    public static List<int> GenerateWidthWNAF(BigInteger number)
    {
        var result = new List<int>();

        while (number > 0)
        {
            if (IsOdd(number))
            {
                var ki = SignedModulo(number);
                result.Insert(0, ki);

                number -= ki;
            }
            else result.Insert(0, 0);

            number >>= 1;
        }

        return result;
    }

    private static int SignedModulo(BigInteger number)
    {
        var twoToWMinusOne = twoToW >> 1;  // 2^(w-1)

        var modResult = (int)(number % twoToW);  // d mod 2^w

        return modResult >= twoToWMinusOne ? modResult - twoToW : modResult;
    }

    public static BigInteger ModInverse(BigInteger number, BigInteger modulo)
    {
        if (modulo == 1)
            return BigInteger.Zero;

        if (BigInteger.GreatestCommonDivisor(number, modulo) != 1)
            throw new InvalidOperationException(ExceptionMessages.NoInverseModulo);

        return BigInteger.ModPow(number, modulo - 2, modulo);
    }
}