using System.Text;

namespace ELO.PointOperations;

public static class MathUtilities
{
    private static readonly BigInteger P256 = Curve.P;
    private static readonly BigInteger P256X2 = P256 * 2;
    private static readonly int[] Index = [0, 32, 64, 96, 128, 160, 192, 224];

    public static int GetHighestBit(BigInteger k) => (int)(k.GetBitLength() - 1);

    public static bool IsBitSet(BigInteger k, int i) => (k & BigInteger.One << i) != 0;

    public static int FindLargestNonZeroDigit(List<int> dw)
    {
        int c = dw.Count - 1;
        while (c >= 0 && dw[c] == 0)
        {
            c--;
        }
        return c;
    }

    public static List<int> ComputeNAF(BigInteger k)
    {
        List<int> naf = [];

        while (k > 0)
        {
            if (k % 2 == 0)
            {
                naf.Add(0);
            }
            else
            {
                int z = (int)(2 - (k % 4));
                naf.Add(z);
                k -= z;
            }
            k >>= 1;
        }

        return naf;
    }

    public static List<int> GenerateWidthWNAF(BigInteger k)
    {
        var n = k.GetBitLength();
        BigInteger twoPowerW = 1 << Curve.WindowSize;

        var dw = new List<int>(new int[n + 1]);

        for (int i = 0; i <= n; i++)
        {
            if ((k & 1) == 1)
            {
                dw[i] = (int)(k % twoPowerW);

                k -= dw[i];
            }
            else
            {
                dw[i] = 0;
            }

            k >>= 1;
        }

        return dw;
    }

    public static BigInteger ModInverse(BigInteger a, BigInteger p)
    {
        if (p == 1)
            return 0;

        if (BigInteger.GreatestCommonDivisor(a, p) != 1)
            throw new InvalidOperationException("Inverse does not exist - a and p must be coprime.");

        return BigInteger.ModPow(a, p - 2, p);
    }

    public static BigInteger FastModuloP256(BigInteger a)
    {
        uint[] aParts = new uint[16];
        for (int i = 0; i < 16; i++)
        {
            aParts[i] = (uint)((a >> (i * 32)) & 0xFFFFFFFF);
        }


        BigInteger t = ((BigInteger)aParts[7] << Index[7]) +
                       ((BigInteger)aParts[6] << Index[6]) +
                       ((BigInteger)aParts[5] << Index[5]) +
                       ((BigInteger)aParts[4] << Index[4]) +
                       ((BigInteger)aParts[3] << Index[3]) +
                       ((BigInteger)aParts[2] << Index[2]) +
                       ((BigInteger)aParts[1] << Index[1]) +
                                    aParts[0];

        BigInteger s1 = ((BigInteger)aParts[15] << Index[7]) +
                        ((BigInteger)aParts[14] << Index[6]) +
                        ((BigInteger)aParts[13] << Index[5]) +
                        ((BigInteger)aParts[12] << Index[4]) +
                        ((BigInteger)aParts[11] << Index[3]);


        BigInteger s2 = ((BigInteger)aParts[15] << Index[6]) +
                        ((BigInteger)aParts[14] << Index[5]) +
                        ((BigInteger)aParts[13] << Index[4]) +
                        ((BigInteger)aParts[12] << Index[3]);

        BigInteger s3 = ((BigInteger)aParts[15] << Index[7]) +
                        ((BigInteger)aParts[14] << Index[6]) +
                        ((BigInteger)aParts[10] << Index[2]) +
                        ((BigInteger)aParts[9] << Index[1]) +
                                     aParts[8];

        BigInteger s4 = ((BigInteger)aParts[8] << Index[7]) +
                        ((BigInteger)aParts[13] << Index[6]) +
                        ((BigInteger)aParts[15] << Index[5]) +
                        ((BigInteger)aParts[14] << Index[4]) +
                        ((BigInteger)aParts[13] << Index[3]) +
                        ((BigInteger)aParts[11] << Index[2]) +
                        ((BigInteger)aParts[10] << Index[1]) +
                                     aParts[9];

        BigInteger d1 = ((BigInteger)aParts[10] << Index[7]) +
                        ((BigInteger)aParts[8] << Index[6]) +
                        ((BigInteger)aParts[13] << Index[2]) +
                        ((BigInteger)aParts[12] << Index[1]) +
                                     aParts[11];

        BigInteger d2 = ((BigInteger)aParts[11] << Index[7]) +
                        ((BigInteger)aParts[9] << Index[6]) +
                        ((BigInteger)aParts[15] << Index[3]) +
                        ((BigInteger)aParts[14] << Index[2]) +
                        ((BigInteger)aParts[13] << Index[1]) +
                                     aParts[12];

        BigInteger d3 = ((BigInteger)aParts[12] << Index[7]) +
                        ((BigInteger)aParts[10] << Index[5]) +
                        ((BigInteger)aParts[9] << Index[4]) +
                        ((BigInteger)aParts[8] << Index[3]) +
                        ((BigInteger)aParts[15] << Index[2]) +
                        ((BigInteger)aParts[14] << Index[1]) +
                                     aParts[13];

        BigInteger d4 = ((BigInteger)aParts[13] << Index[7]) +
                        ((BigInteger)aParts[11] << Index[5]) +
                        ((BigInteger)aParts[10] << Index[4]) +
                        ((BigInteger)aParts[9] << Index[3]) +
                        ((BigInteger)aParts[15] << Index[1]) +
                                     aParts[14];

        d1 = P256X2 - d1;
        d2 = P256X2 - d2;
        d3 = P256 - d3;
        d4 = P256 - d4;

        BigInteger r = t + ((s1 + s2) << 1) + s3 + s4 + d1 + d2 + d3 + d4;

        while (r >= P256) r -= P256;

        return r;
    }
}

public static class BigIntegerExtensions
{
    public static string ToBinaryString(this BigInteger bigint)
    {
        var bytes = bigint.ToByteArray();
        var idx = bytes.Length - 1;
            
        var base2 = new StringBuilder(bytes.Length * 8);

        var binary = Convert.ToString(bytes[idx], 2);

        if (binary[0] != '0' && bigint.Sign == 1)
        {
            base2.Append('0');
        }

        base2.Append(binary);

        for (idx--; idx >= 0; idx--)
        {
            base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
        }

        return base2.ToString();
    }
}