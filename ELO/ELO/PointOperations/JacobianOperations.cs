using ELO.Points;

namespace ELO.PointOperations;

public class JacobianOperations : IPointOperations<JacobianPoint>
{
    public JacobianPoint AddPoints(JacobianPoint p1, AffinePoint p2)
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

    public JacobianPoint DoublePoint(JacobianPoint p)
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

    public AffinePoint MultiplyPoint(BigInteger k, AffinePoint p)
    {
        if (k == BigInteger.Zero) return AffinePoint.AtInfinity;

        p.EnsureOnCurve();

        return MultiplyJacobianPointLeftToRight(k, p)
            .ToAffine();
    }

    private JacobianPoint MultiplyJacobianPointLeftToRight(BigInteger k, AffinePoint p)
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
}