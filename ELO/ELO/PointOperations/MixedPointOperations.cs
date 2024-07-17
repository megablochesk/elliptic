using ELO.Points;
using System.Numerics;

namespace ELO.PointOperations;

public class MixedPointOperations : IPointOperations
{
    public Point AddPoints(Point p1, Point p2)
    {
        var jp = (JacobianPoint)p1;
        var ap = (AffinePoint)p2;

        if (jp.IsAtInfinity) return ap.ToJacobianCoordinates();
        if (ap.IsAtInfinity) return jp;

        jp.EnsureOnCurve();
        ap.EnsureOnCurve();

        return AddMixedPoints(jp, ap);
    }

    private static JacobianPoint AddMixedPoints(JacobianPoint p1, AffinePoint p2)
    {
        var a = p2.X * BigInteger.ModPow(p1.Z, 2, Curve.P);
        var b = p2.Y * BigInteger.ModPow(p1.Z, 3, Curve.P);

        var c = (a - p1.X + Curve.P) % Curve.P;
        var d = (b - p1.Y + Curve.P) % Curve.P;

        var c2 = BigInteger.ModPow(c, 2, Curve.P);
        var c3 = BigInteger.Multiply(c2, c) % Curve.P;

        var x3 = (d * d - (c3 + (p1.X * c2 << 1)) + Curve.P) % Curve.P;
        var y3 = (d * (p1.X * c2 - x3) - p1.Y * c3 + Curve.P) % Curve.P;
        var z3 = p1.Z * c % Curve.P;

        return new JacobianPoint(x3, y3, z3);
    }

    public Point DoublePoint(Point p)
    {
        JacobianPoint jp = p switch
        {
            AffinePoint affinePoint => affinePoint.ToJacobianCoordinates(),
            JacobianPoint point => point,
            _ => throw new InvalidOperationException("Unsupported point type.")
        };

        if (jp.IsAtInfinity) return jp;

        jp.EnsureOnCurve();

        return DoubleJacobianPoint(jp);
    }

    private static JacobianPoint DoubleJacobianPoint(JacobianPoint p)
    {
        var a = (p.X << 2) * BigInteger.ModPow(p.Y, 2, Curve.P) % Curve.P;
        var b = (BigInteger.ModPow(p.Y, 4, Curve.P) << 3) % Curve.P;

        var z2 = BigInteger.ModPow(p.Z, 2, Curve.P) % Curve.P;

        var c = 3 * (p.X - z2) * (p.X + z2) % Curve.P;
        var d = ((-a << 1) + c * c) % Curve.P;    // equal to x3

        var y3 = (c * (a - d) - b) % Curve.P;
        var z3 = ((p.Y * p.Z) << 1) % Curve.P;

        return new JacobianPoint(d, y3, z3);
    }
}