﻿namespace ELO.Points;

public record AffinePoint(BigInteger X, BigInteger Y) : Point
{
    public BigInteger X { get; } = X;
    public BigInteger Y { get; } = Y;

    public virtual bool IsAtInfinity => Y == 0 && X == 0;

    public override bool IsPointOnCurve()
    {
        if (IsAtInfinity) return false;

        return (BigInteger.Pow(X, 3) + Curve.A * X + Curve.B - BigInteger.Pow(Y, 2)) % Curve.P == 0;
    }

    public AffinePoint Negated => new(X, -Y % Curve.P);

    public JacobianPoint ToJacobian() => new(X, Y, Z: BigInteger.One);

    public static AffinePoint AtInfinity => new(X: 0, Y: 0);

    public static bool ArePointsEqual(AffinePoint first, AffinePoint second) =>
        (first.X - second.X) % Curve.P == 0
        && (first.Y - second.Y) % Curve.P == 0;
}