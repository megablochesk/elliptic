using System.Text;

namespace ELO.PointOperations;

public static class MathUtilities
{
    private static readonly BigInteger P256 = Curve.P;
    private static readonly BigInteger P256X2 = P256 << 1;

    private static readonly int twoToW = 1 << Curve.WindowSize;

    public static ushort GetHighestBit(BigInteger k) => (ushort)(k.GetBitLength() - 1);

    public static bool IsBitSet(BigInteger k, int i) => (k & BigInteger.One << i) != 0;

    public static bool IsOdd(BigInteger number) => (number & BigInteger.One) == 1;

    public static List<int> ComputeNAF(BigInteger k)
    {
        List<int> naf = [];

        while (k > 0)
        {
            if (IsOdd(k))
            {
                var z = 2 - (int)(k % 4);
                naf.Add(z);
                k -= z;
            }
            else
            {
                naf.Add(0);
            }

            k >>= 1;
        }

        return naf;
    }

    public static List<int> GenerateWidthWNAF(BigInteger k)
    {
        var result = new List<int>();

        while (k > 0)
        {
            if (IsOdd(k))
            {
                var ki = SignedModulo(k);
                result.Insert(0,ki);

                k -= ki;
            }
            else result.Insert(0, 0);

            k >>= 1;
        }

        return result;
    }

    public static int SignedModulo(BigInteger k)
    {
        var twoToWMinusOne = twoToW >> 1;  // 2^(w-1)

        var modResult = (int)(k % twoToW);  // d mod 2^w

        return modResult >= twoToWMinusOne ? modResult - twoToW : modResult;
    }

    public static BigInteger ModInverse(BigInteger a, BigInteger p)
    {
        if (p == 1)
            return BigInteger.Zero;

        if (BigInteger.GreatestCommonDivisor(a, p) != 1)
            throw new InvalidOperationException("Inverse does not exist - a and p must be coprime.");

        return BigInteger.ModPow(a, p - 2, p);
    }

    public static BigInteger FastModuloP256(BigInteger a)
    {
        var aParts = new uint[16];
        for (var i = 0; i < 16; i++)
        {
            aParts[i] = (uint)((a >> (i * 32)) & 0xFFFFFFFF);
        }

        var hexParts = new string[16];
        for (var i = 0; i < 16; i++)
        {
            hexParts[i] = aParts[i].ToString("X8");
        }

        const string zeros = "00000000";

        var tString = "0" + hexParts[7] + hexParts[6] + hexParts[5] + hexParts[4] + hexParts[3] + hexParts[2] + hexParts[1] + hexParts[0];
        var s1String = hexParts[15] + hexParts[14] + hexParts[13] + hexParts[12] + hexParts[11] + zeros + zeros + zeros;
        var s2String = zeros + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[12] + zeros + zeros + zeros;
        var s3String = hexParts[15] + hexParts[14] + zeros + zeros + zeros + hexParts[10] + hexParts[9] + hexParts[8];
        var s4String = hexParts[8] + hexParts[13] + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[11] + hexParts[10] + hexParts[9];
        var d1String = hexParts[10] + hexParts[8] + zeros + zeros + zeros + hexParts[13] + hexParts[12] + hexParts[11];
        var d2String = "0" + hexParts[11] + hexParts[9] + zeros + zeros + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[12];
        var d3String = hexParts[12] + zeros + hexParts[10] + hexParts[9] + hexParts[8] + hexParts[15] + hexParts[14] + hexParts[13];
        var d4String = "0" + hexParts[13] + zeros + hexParts[11] + hexParts[10] + hexParts[9] + zeros + hexParts[15] + hexParts[14];

        var t = BigInteger.Parse(tString, System.Globalization.NumberStyles.HexNumber);
        var s1 = BigInteger.Parse(s1String, System.Globalization.NumberStyles.HexNumber);
        var s2 = BigInteger.Parse(s2String, System.Globalization.NumberStyles.HexNumber);
        var s3 = BigInteger.Parse(s3String, System.Globalization.NumberStyles.HexNumber);
        var s4 = BigInteger.Parse(s4String, System.Globalization.NumberStyles.HexNumber);
        var d1 = BigInteger.Parse(d1String, System.Globalization.NumberStyles.HexNumber);
        var d2 = BigInteger.Parse(d2String, System.Globalization.NumberStyles.HexNumber);
        var d3 = BigInteger.Parse(d3String, System.Globalization.NumberStyles.HexNumber);
        var d4 = BigInteger.Parse(d4String, System.Globalization.NumberStyles.HexNumber);

        d1 = P256X2 - d1;
        d2 = P256X2 - d2;
        d3 = P256 - d3;
        d4 = P256 - d4;

        var r = t + ((s1 + s2) << 1) + s3 + s4 + d1 + d2 + d3 + d4;

        while (r >= P256) r -= P256;

        return r;
    }

    public static BigInteger FastModuloP256WithBuilder(BigInteger a)
    {
        var aParts = new uint[16];
        for (var i = 0; i < 16; i++)
        {
            aParts[i] = (uint)((a >> (i * 32)) & 0xFFFFFFFF);
        }

        var zeros = "00000000";
        var hexParts = new string[16];
        for (var i = 0; i < 16; i++)
        {
            hexParts[i] = aParts[i].ToString("X8");
        }

        var sb = new StringBuilder();

        sb.Append("0");
        for (var i = 7; i >= 0; i--)
        {
            sb.Append(hexParts[i]);
        }
        var tString = sb.ToString();

        sb.Clear();
        for (var i = 15; i >= 11; i--)
        {
            sb.Append(hexParts[i]);
        }
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(zeros);
        var s1String = sb.ToString();

        sb.Clear();
        sb.Append(zeros);
        for (var i = 15; i >= 12; i--)
        {
            sb.Append(hexParts[i]);
        }
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(zeros);
        var s2String = sb.ToString();

        sb.Clear();
        sb.Append(hexParts[15]);
        sb.Append(hexParts[14]);
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(hexParts[10]);
        sb.Append(hexParts[9]);
        sb.Append(hexParts[8]);
        var s3String = sb.ToString();

        sb.Clear();
        sb.Append(hexParts[8]);
        sb.Append(hexParts[13]);
        sb.Append(hexParts[15]);
        sb.Append(hexParts[14]);
        sb.Append(hexParts[13]);
        sb.Append(hexParts[11]);
        sb.Append(hexParts[10]);
        sb.Append(hexParts[9]);
        var s4String = sb.ToString();

        sb.Clear();
        sb.Append(hexParts[10]);
        sb.Append(hexParts[8]);
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(hexParts[13]);
        sb.Append(hexParts[12]);
        sb.Append(hexParts[11]);
        var d1String = sb.ToString();

        sb.Clear();
        sb.Append("0");
        sb.Append(hexParts[11]);
        sb.Append(hexParts[9]);
        sb.Append(zeros);
        sb.Append(zeros);
        sb.Append(hexParts[15]);
        sb.Append(hexParts[14]);
        sb.Append(hexParts[13]);
        sb.Append(hexParts[12]);
        var d2String = sb.ToString();

        sb.Clear();
        sb.Append(hexParts[12]);
        sb.Append(zeros);
        sb.Append(hexParts[10]);
        sb.Append(hexParts[9]);
        sb.Append(hexParts[8]);
        sb.Append(hexParts[15]);
        sb.Append(hexParts[14]);
        sb.Append(hexParts[13]);
        var d3String = sb.ToString();

        sb.Clear();
        sb.Append("0");
        sb.Append(hexParts[13]);
        sb.Append(zeros);
        sb.Append(hexParts[11]);
        sb.Append(hexParts[10]);
        sb.Append(hexParts[9]);
        sb.Append(zeros);
        sb.Append(hexParts[15]);
        sb.Append(hexParts[14]);
        var d4String = sb.ToString();

        var t = BigInteger.Parse(tString, System.Globalization.NumberStyles.HexNumber);
        var s1 = BigInteger.Parse(s1String, System.Globalization.NumberStyles.HexNumber);
        var s2 = BigInteger.Parse(s2String, System.Globalization.NumberStyles.HexNumber);
        var s3 = BigInteger.Parse(s3String, System.Globalization.NumberStyles.HexNumber);
        var s4 = BigInteger.Parse(s4String, System.Globalization.NumberStyles.HexNumber);
        var d1 = BigInteger.Parse(d1String, System.Globalization.NumberStyles.HexNumber);
        var d2 = BigInteger.Parse(d2String, System.Globalization.NumberStyles.HexNumber);
        var d3 = BigInteger.Parse(d3String, System.Globalization.NumberStyles.HexNumber);
        var d4 = BigInteger.Parse(d4String, System.Globalization.NumberStyles.HexNumber);

        d1 = P256X2 - d1;
        d2 = P256X2 - d2;
        d3 = P256 - d3;
        d4 = P256 - d4;

        var r = t + ((s1 + s2) << 1) + s3 + s4 + d1 + d2 + d3 + d4;

        while (r >= P256) r -= P256;

        return r;
    }
}