﻿using System.Numerics;

namespace ELO.Points;

public record JacobianPoint(BigInteger X, BigInteger Y, BigInteger Z) : Point
{
    public BigInteger X { get; } = X;
    public BigInteger Y { get; } = Y;
    public BigInteger Z { get; } = Z;

    public override bool IsAtInfinity => Z == 0;
    public override JacobianPoint AtInfinity => new(1, 1, 0);

    public override bool IsPointOnCurve()
    {
        if (IsAtInfinity) return false;

        return (BigInteger.Pow(X, 3) + (Curve.A * X + Curve.B * Z * Z) * BigInteger.Pow(Z, 4) - BigInteger.Pow(Y, 2)) % Curve.P == 0;
    }

    public AffinePoint ToAffine()
    {
        if (IsAtInfinity)
        {
            throw new InvalidOperationException("Cannot convert the point at infinity to affine coordinates.");
        }

        BigInteger zSquared = BigInteger.Pow(Z, 2);
        BigInteger zCubed = zSquared * Z;

        BigInteger xAffine = X * BigInteger.ModPow(zSquared, Curve.P - 2, Curve.P) % Curve.P;
        BigInteger yAffine = Y * BigInteger.ModPow(zCubed, Curve.P - 2, Curve.P) % Curve.P;

        return new AffinePoint(xAffine, yAffine);
    }
}