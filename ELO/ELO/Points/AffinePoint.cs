using System.Numerics;

namespace ELO.Points;

public record AffinePoint(BigInteger X, BigInteger Y) : Point
{
    public BigInteger X { get; } = X;
    public BigInteger Y { get; } = Y;

    public override bool IsAtInfinity => Y == 0 && X == 0;
    
    public override bool IsPointOnCurve()
    {
        if (IsAtInfinity) return false;

        return (BigInteger.Pow(X, 3) + Curve.A * X + Curve.B - BigInteger.Pow(Y, 2)) % Curve.P == 0;
    }

    public JacobianPoint ToJacobian()
    {
        return new JacobianPoint(X, Y, 1);
    }

    public static AffinePoint AtInfinity => new(0, 0);

    public static bool ArePointsEqual(AffinePoint first, AffinePoint second) =>
        (first.X - second.X) % Curve.P == 0
        && (first.Y - second.Y) % Curve.P == 0;
}