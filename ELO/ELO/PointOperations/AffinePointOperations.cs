using ELO.Points;
using System.Numerics;

namespace ELO.PointOperations;

public class AffinePointOperations : IPointOperations
{
    public Point AddPoints(Point p1, Point p2)
    {
        var ap1 = (AffinePoint)p1;
        var ap2 = (AffinePoint)p2;

        if (ap1.IsAtInfinity) return ap2;
        if (ap2.IsAtInfinity) return ap1;

        ap1.EnsureOnCurve();
        ap2.EnsureOnCurve();

        return AddAffinePoints(ap1, ap2);
    }

    public Point DoublePoint(Point p)
    {
        var ap = (AffinePoint)p;

        if (ap.IsAtInfinity) return ap;

        ap.EnsureOnCurve();

        return DoubleAffinePoint(ap);
    }

    private AffinePoint AddAffinePoints(AffinePoint p1, AffinePoint p2)
    {
        if (p1.X == p2.X)
        {
            if (p1.Y != p2.Y || p1.Y == 0)
                return p1.AtInfinity;
            
            return (AffinePoint)DoublePoint(p1);
        }

        BigInteger m = (p2.Y - p1.Y) * MathUtils.ModInverse(p2.X - p1.X, Curve.P) % Curve.P;
        if (m < 0) m += Curve.P;

        BigInteger x3 = (m * m - p1.X - p2.X) % Curve.P;
        if (x3 < 0) x3 += Curve.P;

        BigInteger y3 = (m * (p1.X - x3) - p1.Y) % Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }

    private static AffinePoint DoubleAffinePoint(AffinePoint p)
    {
        BigInteger m = (3 * p.X * p.X + Curve.A) * MathUtils.ModInverse(2 * p.Y, Curve.P) % Curve.P;
        if (m < 0) m += Curve.P;

        BigInteger x3 = (m * m - 2 * p.X) % Curve.P;
        if (x3 < 0) x3 += Curve.P;

        BigInteger y3 = (m * (p.X - x3) - p.Y) % Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }
}