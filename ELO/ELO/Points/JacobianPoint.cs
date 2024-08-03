namespace ELO.Points;

public record JacobianPoint(BigInteger X, BigInteger Y, BigInteger Z) : Point
{
    public BigInteger X { get; } = X;
    public BigInteger Y { get; } = Y;
    public BigInteger Z { get; } = Z;

    public virtual bool IsAtInfinity => Z == 0;

    public override bool IsPointOnCurve()
    {
        if (IsAtInfinity) return false;

        return (BigInteger.Pow(X, 3) 
             + (Curve.A * X + Curve.B * Z * Z) 
             * BigInteger.Pow(Z, 4) 
             - BigInteger.Pow(Y, 2)) % Curve.P == 0;
    }

    public JacobianPoint Negated => new(X, -Y % Curve.P, Z);

    public AffinePoint ToAffine()
    {
        if (IsAtInfinity) return AffinePoint.AtInfinity;

        var zSquared = Z * Z % Curve.P;
        var zCubed = zSquared * Z % Curve.P;

        var xAffine = X * BigInteger.ModPow(zSquared, Curve.P - 2, Curve.P) % Curve.P;
        var yAffine = Y * BigInteger.ModPow(zCubed, Curve.P - 2, Curve.P) % Curve.P;

        return new AffinePoint(xAffine, yAffine);
    }

    public static JacobianPoint AtInfinity 
        => new(X: BigInteger.One, Y: BigInteger.One, Z: BigInteger.Zero);
}