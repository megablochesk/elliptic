using ELO.ECDH;
using ELO.Points;

namespace ELO.PointOperations;

public class JacobianOperations(AlgorithmType algorithmType) : IPointOperations<JacobianPoint>
{
    public JacobianPoint AddPoints(JacobianPoint p1, AffinePoint p2)
    {
        if (p1.IsAtInfinity) return p2.ToJacobian();
        if (p2.IsAtInfinity) return p1;

        return AddMixedPoints(p1, p2);
    }

    public JacobianPoint AddPoints(JacobianPoint p1, JacobianPoint p2)
    {
        if (p1.IsAtInfinity) return p2;
        if (p2.IsAtInfinity) return p1;

        return AddJacobianPoints(p1, p2);
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

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;
        if (z3 < 0) z3 += Curve.P;

        return new JacobianPoint(x3, y3, z3);
    }

    private static JacobianPoint AddJacobianPoints(JacobianPoint p1, JacobianPoint p2)
    {
        BigInteger Z1Z1 = p1.Z * p1.Z % Curve.P;
        BigInteger Z2Z2 = p2.Z * p2.Z % Curve.P;

        BigInteger u1 = p1.X * Z2Z2 % Curve.P;
        BigInteger u2 = p2.X * Z1Z1 % Curve.P;

        BigInteger Y1Z2 = p1.Y * p2.Z % Curve.P;
        BigInteger Y2Z1 = p2.Y * p1.Z % Curve.P;

        BigInteger s1 = Y1Z2 * Z2Z2 % Curve.P;
        BigInteger s2 = Y2Z1 * Z1Z1 % Curve.P;

        if (u1 == u2)
        {
            if (s1 == s2)
                return DoubleJacobianPoint(p1);
            
            return JacobianPoint.AtInfinity;
        }

        BigInteger h = u2 - u1;
        BigInteger I = BigInteger.ModPow(h << 1, 2, Curve.P);
        BigInteger j = h * I % Curve.P;
        BigInteger r = 2 * (s2 - s1) % Curve.P;
        BigInteger v = u1 * I % Curve.P;

        BigInteger x3 = (r * r - j - 2 * v) % Curve.P;
        BigInteger y3 = (r * (v - x3) - 2 * s1 * j) % Curve.P;
        BigInteger z3 = (BigInteger.ModPow(p1.Z + p2.Z,2, Curve.P) - Z1Z1 - Z2Z2) * h % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;
        if (z3 < 0) z3 += Curve.P;

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

    private static JacobianPoint PointDoubleRepeat(JacobianPoint point, int times)
    {
        JacobianPoint result = point;

        for (int i = 0; i < times; i++)
        {
            result = DoubleJacobianPoint(result);
        }
        return result;
    }

    public AffinePoint MultiplyPoint(BigInteger k, AffinePoint p)
    {
        if (k == BigInteger.Zero) return AffinePoint.AtInfinity;

        p.EnsureOnCurve();

        var result = algorithmType switch
        {
            AlgorithmType.JacobianLeftToRight      => MultiplyPointLeftToRight(k, p),
            AlgorithmType.JacobianMontgomeryLadder => MultiplyPointMontgomeryLadder(k, p),
            AlgorithmType.JacobianWithNAF          => MultiplyPointWithNAF(k, p),
            AlgorithmType.JacobianWindowedMethod   => MultiplyPointWindowedMethod(k, p),
            _ => throw new InvalidOperationException("Unsupported algorithm type.")
        };

        return result.ToAffine();
    }

    private JacobianPoint MultiplyPointLeftToRight(BigInteger k, AffinePoint p)
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

    private JacobianPoint MultiplyPointMontgomeryLadder(BigInteger k, AffinePoint p)
    {
        var r0 = JacobianPoint.AtInfinity;
        var r1 = p.ToJacobian();

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            if (MathUtilities.IsBitSet(k, i))
            {
                r0 = AddPoints(r1, r0);
                r1 = DoublePoint(r1);
            }
            else
            {
                r1 = AddPoints(r0, r1);
                r0 = DoublePoint(r0);
            }
        }

        return r0;
    }

    private JacobianPoint MultiplyPointWithNAF(BigInteger k, AffinePoint p)
    {
        var naf = MathUtilities.ComputeNAF(k);

        var result = JacobianPoint.AtInfinity;

        for (int i = naf.Count - 1; i >= 0; i--)
        {
            result = DoublePoint(result);

            if (naf[i] == -1) result = AddPoints(result, p.Negated);
            else if (naf[i] == 1) result = AddPoints(result, p);
        }

        return result;
    }

    public JacobianPoint MultiplyPointWindowedMethod(BigInteger k, AffinePoint p)
    {
        var precomputedPoints = PrecomputePoints(p);

        string kBinary = k.ToBinaryString();
        int m = kBinary.Length / Curve.WindowSize;

        var Q = JacobianPoint.AtInfinity;

        for (int i = m; i >= 0; i--)
        {
            Q = PointDoubleRepeat(Q, Curve.WindowSize);

            int startIdx = i * Curve.WindowSize;
            int endIdx = Math.Min(startIdx + Curve.WindowSize, kBinary.Length);

            if (startIdx < kBinary.Length)
            {
                string windowBinary = kBinary.Substring(startIdx, endIdx - startIdx);
                int d = Convert.ToInt32(windowBinary, 2);

                if (d > 0)
                {
                    Q = AddPoints(Q, precomputedPoints[d]);
                }
            }
        }

        return Q;
    }

    private static JacobianPoint[] PrecomputePoints(AffinePoint p)
    {
        int numPrecomputedPoints = 1 << Curve.WindowSize;

        JacobianPoint[] precomputedPoints = new JacobianPoint[numPrecomputedPoints];

        precomputedPoints[0] = JacobianPoint.AtInfinity;
        for (int i = 1; i < numPrecomputedPoints; i++)
        {
            precomputedPoints[i] = AddMixedPoints(precomputedPoints[i - 1], p);
        }

        return precomputedPoints;
    }
}