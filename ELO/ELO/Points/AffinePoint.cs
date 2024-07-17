using System.Numerics;

namespace ELO.Points;

public record AffinePoint(BigInteger X, BigInteger Y) : Point
{
    public BigInteger X { get; } = X;
    public BigInteger Y { get; } = Y;

    public override bool IsAtInfinity => Y == 0 && X == 0;
    public override AffinePoint AtInfinity => new(0, 0);

    public override bool IsPointOnCurve()
    {
        if (IsAtInfinity) return false;

        return (BigInteger.Pow(X, 3) + Curve.A * X + Curve.B - BigInteger.Pow(Y, 2)) % Curve.P == 0;
    }

    public JacobianPoint ToJacobianCoordinates()
    {
        return new JacobianPoint(X, Y, 1);
    }
}