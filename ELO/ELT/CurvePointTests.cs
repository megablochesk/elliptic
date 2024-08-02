using ELO.Points;
using System.Numerics;

namespace ELT;

[TestFixture]
public class CurvePointTests
{
    private static readonly BigInteger XOfPoint =
        BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286");
    private static readonly BigInteger YOfPoint =
        BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109");

    [Test]
    public void AffinePointShouldBeOnCurve()
    {
        var x = XOfPoint;
        var y = YOfPoint;
        AffinePoint point = new(x, y);

        Assert.IsTrue(point.IsPointOnCurve());
    }

    [Test]
    public void AffinePointShouldNotBeOnCurve()
    {
        var x = XOfPoint;
        var y = YOfPoint - BigInteger.One;
        AffinePoint point = new(x, y);

        Assert.IsFalse(point.IsPointOnCurve());
    }

    [Test]
    public void AffinePointAtInfinity_ShouldBeAtInfinity()
    {
        var pointAtInfinity = AffinePoint.AtInfinity;

        Assert.IsTrue(pointAtInfinity.IsAtInfinity);
    }

    [Test]
    public void AffinePointWithNonZeroCoordinates_ShouldNotBeAtInfinity()
    {
        BigInteger x = new BigInteger(3);
        BigInteger y = new BigInteger(4);
        AffinePoint point = new AffinePoint(x, y);

        Assert.IsFalse(point.IsAtInfinity);
    }

    [Test]
    public void JacobianPointShouldBeOnCurve()
    {
        var x = XOfPoint;
        var y = YOfPoint;
        JacobianPoint point = new(x, y, BigInteger.One);

        Assert.IsTrue(point.IsPointOnCurve());
    }

    [Test]
    public void JacobianPointShouldNotBeOnCurve()
    {
        var x = XOfPoint;
        var y = YOfPoint - BigInteger.One;
        JacobianPoint point = new(x, y, BigInteger.One);

        Assert.IsFalse(point.IsPointOnCurve());
    }

    [Test]
    public void JacobianPointAtInfinity_ShouldBeAtInfinity()
    {
        var pointAtInfinity = JacobianPoint.AtInfinity;

        Assert.IsTrue(pointAtInfinity.IsAtInfinity);
    }

    [Test]
    public void JacobianPointWithNonZeroCoordinates_ShouldNotBeAtInfinity()
    {
        var x = new BigInteger(3);
        var y = new BigInteger(4);
        var point = new JacobianPoint(x, y, BigInteger.One);

        Assert.IsFalse(point.IsAtInfinity);
    }
}