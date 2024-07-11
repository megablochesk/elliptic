using ELO.Points;
using System.Numerics;

namespace ELT;

[TestFixture]
public class CurvePointTests
{
    [Test]
    public void AffinePointShouldBeOnCurve()
    {
        var x = BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286");
        var y = BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109");
        AffinePoint point = new(x, y);

        Assert.IsTrue(point.IsPointOnCurve());
    }

    [Test]
    public void AffinePointShouldNotBeOnCurve()
    {
        var x = BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286");
        var y = BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405108");
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
        var x = BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286");
        var y = BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109");
        JacobianPoint point = new(x, y, BigInteger.One);

        Assert.IsTrue(point.IsPointOnCurve());
    }

    [Test]
    public void JacobianPointShouldNotBeOnCurve()
    {
        var x = BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286");
        var y = BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405108");
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
        BigInteger x = new BigInteger(3);
        BigInteger y = new BigInteger(4);
        JacobianPoint point = new JacobianPoint(x, y, BigInteger.One);

        Assert.IsFalse(point.IsAtInfinity);
    }
}