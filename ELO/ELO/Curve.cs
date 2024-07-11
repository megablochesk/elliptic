using System.Numerics;
using ELO.Points;

namespace ELO;

public static class Curve
{
    public static readonly BigInteger P = BigInteger.Parse("115792089210356248762697446949407573530086143415290314195533631308867097853951", System.Globalization.NumberStyles.Integer);
    public static readonly BigInteger A = -3;
    public static readonly BigInteger B = BigInteger.Parse("41058363725152142129326129780047268409114441015993725554835256314039467401291", System.Globalization.NumberStyles.Integer);
    public static readonly BigInteger N = BigInteger.Parse("115792089210356248762697446949407573529996955224135760342422259061068512044369", System.Globalization.NumberStyles.Integer);

    public static readonly BigInteger Gx = BigInteger.Parse("6B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", System.Globalization.NumberStyles.HexNumber);
    public static readonly BigInteger Gy = BigInteger.Parse("4FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", System.Globalization.NumberStyles.HexNumber);
    public static readonly AffinePoint G = new(Gx, Gy);

    public static JacobianPoint AddPoints(JacobianPoint p1, AffinePoint p2) => AddMixedPoints(p1, p2);
    private static JacobianPoint AddMixedPoints(JacobianPoint p1, AffinePoint p2)
    {
        if (p1.IsAtInfinity) return p2.ToJacobianCoordinates();
        if (p2.IsAtInfinity) return p1;

        var a = p2.X * BigInteger.Pow(p1.Z, 2) % P;
        var b = p2.Y * BigInteger.Pow(p1.Z, 3) % P;

        var c = (a - p1.X) % P;
        var d = (b - p1.Y) % P;

        var c2 = BigInteger.Pow(c, 2) % P;
        var c3 = BigInteger.Multiply(c2, c) % P;

        var x3 = (d * d - (c3 + 2 * p1.X * c2)) % P;
        var y3 = (d * (p1.X * c2 - x3) - p1.Y * c3) % P;
        var z3 = p1.Z * c % P;

        return new JacobianPoint(x3, y3, z3);
    }


    public static JacobianPoint DoublePoint(JacobianPoint p) => DoubleJacobianPoint(p);
    private static JacobianPoint DoubleJacobianPoint(JacobianPoint p)
    {
        if (p.IsAtInfinity) return JacobianPoint.AtInfinity;

        var a = 4 * p.X * p.Y * p.Y % P;
        var b = 8 * BigInteger.Pow(p.Y, 4) % P;

        var z2 = BigInteger.Pow(p.Z, 2) % P;

        var c = 3 * (p.X - z2) * (p.X + z2) % P;
        var d = (-2 * a + c * c) % P;    // equal to x3

        var y3 = (c * (a - d) - b) % P;
        var z3 = 2 * p.Y * p.Z % P;

        return new JacobianPoint(d, y3, z3);
    }

    public static JacobianPoint MultiplyPoint(BigInteger k, AffinePoint p) => MultiplyPointLeftToRight(k, p);
    private static JacobianPoint MultiplyPointLeftToRight(BigInteger k, AffinePoint p)
    {
        if (k == BigInteger.Zero) return JacobianPoint.AtInfinity;
        
        p.EnsureOnCurve();

        JacobianPoint result = JacobianPoint.AtInfinity;
        
        for (int i = GetHighestBit(k); i >= 0; i--)
        {
            result = DoublePoint(result);

            if (IsBitSet(k, i))
            {
                result = AddPoints(result, p);
            }
        }

        return result;
    }

    private static int GetHighestBit(BigInteger k) => (int)(k.GetBitLength() - 1);

    private static bool IsBitSet(BigInteger k, int i) => (k & (BigInteger.One << i)) != 0;
}