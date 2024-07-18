using ELO.Points;
using System.Numerics;

namespace ELO.PointOperations;

public class Operations
{
    public static JacobianPoint AddPoints(JacobianPoint p1, AffinePoint p2)
    {
        if (p1.IsAtInfinity) return p2.ToJacobian();
        if (p2.IsAtInfinity) return p1;

        return AddMixedPoints(p1, p2);
    }

    private static JacobianPoint AddMixedPoints(JacobianPoint p1, AffinePoint p2)
    {
        var a = p2.X * BigInteger.Pow(p1.Z, 2) % Curve.P;
        var b = p2.Y * BigInteger.Pow(p1.Z, 3) % Curve.P;

        var c = (a - p1.X) % Curve.P;
        var d = (b - p1.Y) % Curve.P;

        var c2 = BigInteger.Pow(c, 2) % Curve.P;
        var c3 = BigInteger.Multiply(c2, c) % Curve.P;

        var x3 = (d * d - (c3 + (p1.X * c2 << 1))) % Curve.P;
        var y3 = (d * (p1.X * c2 - x3) - p1.Y * c3) % Curve.P;
        var z3 = p1.Z * c % Curve.P;

        return new JacobianPoint(x3, y3, z3);
    }


    public static JacobianPoint DoublePoint(JacobianPoint p)
    {
        if (p.IsAtInfinity) return JacobianPoint.AtInfinity;

        return DoubleJacobianPoint(p);
    }

    private static JacobianPoint DoubleJacobianPoint(JacobianPoint p)
    {
        var a = (p.X << 2) * BigInteger.Pow(p.Y, 2) % Curve.P;
        var b = (BigInteger.Pow(p.Y, 4) << 3) % Curve.P;


        var z2 = BigInteger.Pow(p.Z, 2) % Curve.P;

        var c = 3 * (p.X - z2) * (p.X + z2) % Curve.P;
        var d = ((-a << 1) + c * c) % Curve.P;    // equal to x3

        var y3 = (c * (a - d) - b) % Curve.P;
        var z3 = ((p.Y * p.Z) << 1) % Curve.P;

        return new JacobianPoint(d, y3, z3);
    }

    public static JacobianPoint MultiplyPoint(BigInteger k, AffinePoint p)
    {
        if (k == BigInteger.Zero) return JacobianPoint.AtInfinity;

        p.EnsureOnCurve();

        return MultiplyJacobianPointLeftToRight(k, p);
    }

    private static JacobianPoint MultiplyJacobianPointLeftToRight(BigInteger k, AffinePoint p)
    {
        JacobianPoint result = JacobianPoint.AtInfinity;

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            result = DoublePoint(result);

            if (MathUtilities.IsBitSet(k, i))
            {
                result = AddPoints(result, p);
            }
        }

        return result;
    }

    public static AffinePoint MultiplyAffinePointLeftToRight(BigInteger k, AffinePoint p)
    {
        AffinePoint result = AffinePoint.AtInfinity;

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            result = DoubleAffinePoint(result);

            if (MathUtilities.IsBitSet(k, i))
            {
                result = AddAffinePoints(result, p);
            }
        }

        result.EnsureOnCurve();

        return result;
    }

    public static AffinePoint AddAffinePoints(AffinePoint p1, AffinePoint p2)
    {
        if (p1.IsAtInfinity) return p2;
        if (p2.IsAtInfinity) return p1;

        if (p1.X == p2.X)
        {
            if (p1.Y == p2.Y)
                return DoubleAffinePoint(p1);
            
            return AffinePoint.AtInfinity;
        }

        BigInteger lambda = (p2.Y - p1.Y) * MathUtilities.ModInverse(p2.X - p1.X, Curve.P) % Curve.P;
        BigInteger x3 = (lambda * lambda - p1.X - p2.X) % Curve.P;
        BigInteger y3 = (lambda * (p1.X - x3) - p1.Y) % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }

    public static AffinePoint DoubleAffinePoint(AffinePoint point)
    {
        if (point.IsAtInfinity)
            return point;

        BigInteger lambda = (3 * point.X * point.X + Curve.A) * MathUtilities.ModInverse(2 * point.Y, Curve.P) % Curve.P;
        BigInteger x3 = (lambda * lambda - 2 * point.X) % Curve.P;
        BigInteger y3 = (lambda * (point.X - x3) - point.Y) % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }
}