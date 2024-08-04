using System.Text;

namespace ELO.StandardMath;

public static class FastModuloP256
{
    private static readonly BigInteger P256 = Curve.P;
    private static readonly BigInteger P256X2 = P256 << 1;

    private static readonly string Zeros = "00000000";

    private static readonly int[] TIndexes = [7, 6, 5, 4, 3, 2, 1, 0];
    private static readonly int[] S1Indexes = [15, 14, 13, 12, 11, 16, 16, 16];
    private static readonly int[] S2Indexes = [16, 15, 14, 13, 12, 16, 16, 16];
    private static readonly int[] S3Indexes = [15, 14, 16, 16, 16, 10, 9, 8];
    private static readonly int[] S4Indexes = [8, 13, 15, 14, 13, 11, 10, 9];
    private static readonly int[] D1Indexes = [10, 8, 16, 16, 16, 13, 12, 11];
    private static readonly int[] D2Indexes = [11, 9, 16, 16, 15, 14, 13, 12];
    private static readonly int[] D3Indexes = [12, 16, 10, 9, 8, 15, 14, 13];
    private static readonly int[] D4Indexes = [13, 16, 11, 10, 9, 16, 15, 14];

    private static readonly StringBuilder _stringBuilder = new();


    public static BigInteger NotOptimisedFastModulo(BigInteger number)
    {
        var hexParts = BigIntegerToHexString(number);

        var tString  = "0" + hexParts[7] + hexParts[6] + hexParts[5] + hexParts[4] + hexParts[3] + hexParts[2] + hexParts[1] + hexParts[0];
        var s1String = "0" + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[12] + hexParts[11] + Zeros + Zeros + Zeros;
        var s2String = "0" + Zeros + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[12] + Zeros + Zeros + Zeros;
        var s3String = "0" + hexParts[15] + hexParts[14] + Zeros + Zeros + Zeros + hexParts[10] + hexParts[9] + hexParts[8];
        var s4String = "0" + hexParts[8] + hexParts[13] + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[11] + hexParts[10] + hexParts[9];
        var d1String = "0" + hexParts[10] + hexParts[8] + Zeros + Zeros + Zeros + hexParts[13] + hexParts[12] + hexParts[11];
        var d2String = "0" + hexParts[11] + hexParts[9] + Zeros + Zeros + hexParts[15] + hexParts[14] + hexParts[13] + hexParts[12];
        var d3String = "0" + hexParts[12] + Zeros + hexParts[10] + hexParts[9] + hexParts[8] + hexParts[15] + hexParts[14] + hexParts[13];
        var d4String = "0" + hexParts[13] + Zeros + hexParts[11] + hexParts[10] + hexParts[9] + Zeros + hexParts[15] + hexParts[14];

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

        var r = t + (s1 + s2 << 1) + s3 + s4 + d1 + d2 + d3 + d4;

        while (r >= P256) r -= P256;

        return r;
    }

    public static BigInteger MixedModulo(this BigInteger number) => 
        number.GetBitLength() <= 512 && number >= BigInteger.Zero 
            ? number.FastModulo() 
            : number % P256;

    public static BigInteger FastModulo(this BigInteger number)
    {
        var hexParts = BigIntegerToHexString(number);

        var t =  CompileFromHexToBigInteger(TIndexes , hexParts);
        var s1 = CompileFromHexToBigInteger(S1Indexes, hexParts); 
        var s2 = CompileFromHexToBigInteger(S2Indexes, hexParts);
        var s3 = CompileFromHexToBigInteger(S3Indexes, hexParts);
        var s4 = CompileFromHexToBigInteger(S4Indexes, hexParts);
        var d1 = CompileFromHexToBigInteger(D1Indexes, hexParts);
        var d2 = CompileFromHexToBigInteger(D2Indexes, hexParts);
        var d3 = CompileFromHexToBigInteger(D3Indexes, hexParts);
        var d4 = CompileFromHexToBigInteger(D4Indexes, hexParts);

        d1 = P256X2 - d1;
        d2 = P256X2 - d2;
        d3 = P256 - d3;
        d4 = P256 - d4;

        var r = t + (s1 + s2 << 1) + s3 + s4 + d1 + d2 + d3 + d4;

        while (r >= P256) r -= P256;

        return r;
    }

    public static BigInteger CompileFromHexToBigInteger(int[] indexes, string[] hexParts)
    {
        _stringBuilder.Clear();

        foreach (var i in indexes)
        {
            _stringBuilder.Append(hexParts[i]);
        }

        AdjustStringForPositiveBigInt();

        var compiled = _stringBuilder.ToString();
        
        return BigInteger.Parse(compiled, System.Globalization.NumberStyles.HexNumber);
    }

    private static void AdjustStringForPositiveBigInt()
    {
        if (_stringBuilder.Length > 0 && _stringBuilder[0] >= '8')
        {
            _stringBuilder.Insert(0, '0');
        }
    }

    public static string[] BigIntegerToHexString(BigInteger number)
    {
        var hexParts = new string[17];
        for (var i = 0; i < 16; i++)
        {
            var part = (uint)(number >> i * 32 & 0xFFFFFFFF);
            hexParts[i] = part.ToString("X8");

        }

        hexParts[16] = Zeros;

        return hexParts;
    }
}