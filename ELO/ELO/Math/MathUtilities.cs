namespace ELO.StandardMath;

public static class MathUtilities
{
    private static readonly int twoToW = (int)Math.Pow(2, Curve.WindowSize);

    public static BigInteger GetHighestBit(BigInteger number) => number.GetBitLength() - 1;

    public static bool IsBitSet(BigInteger number, BigInteger bit)
    {
        BigInteger mask = BigInteger.Pow(2, (int)bit);
        return (number & mask) != BigInteger.Zero;
    }

    public static bool IsOdd(BigInteger number) => number % 2 != 0;

    public static List<BigInteger> ComputeNAF(BigInteger number)
    {
        List<BigInteger> naf = [];

        while (number > 0)
        {
            if (IsOdd(number))
            {
                var z = 2 - (number % 4);
                naf.Add(z);
                number -= z;
            }
            else
            {
                naf.Add(0);
            }

            number /= 2;
        }

        return naf;
    }

    public static List<BigInteger> GenerateWidthWNAF(BigInteger number)
    {
        var result = new List<BigInteger>();

        while (number > 0)
        {
            if (IsOdd(number))
            {
                var ki = SignedModulo(number);
                result.Insert(0, ki);

                number -= ki;
            }
            else result.Insert(0, 0);

            number /= 2;
        }

        return result;
    }

    private static BigInteger SignedModulo(BigInteger number)
    {
        var twoToWMinusOne = BigInteger.Pow(2, Curve.WindowSize - 1);  // 2^(w-1)

        var modResult = (number % twoToW);  // d mod 2^w

        return modResult >= twoToWMinusOne ? modResult - twoToW : modResult;
    }

    public static BigInteger ModInverse(BigInteger number, BigInteger modulo)
    {
        if (modulo == 1)
            return 0;

        if (BigInteger.GreatestCommonDivisor(number, modulo) != 1)
            throw new InvalidOperationException(ExceptionMessages.NoInverseModulo);

        return BigInteger.ModPow(number, modulo - 2, modulo);
    }
}