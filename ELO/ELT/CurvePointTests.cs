using ELO.Points;
using System.Numerics;

namespace ELT;

[TestFixture]
public class CurvePointTests
{
    private static readonly AffinePoint PointOnCurve = new(
        X: BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286"),
        Y: BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109"));

    private static readonly AffinePoint PointOffCurve = new(
        X: BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635280"),
        Y: BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405100"));

    [Test]
    public void AffinePointShouldBeOnCurve()
    {
        AffinePoint point = PointOnCurve;

        Assert.IsTrue(point.IsPointOnCurve());
    }

    [Test]
    public void AffinePointShouldNotBeOnCurve()
    {
        AffinePoint point = PointOffCurve;

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
        AffinePoint point = PointOnCurve;

        Assert.IsFalse(point.IsAtInfinity);
    }

    [Test]
    public void JacobianPointShouldBeOnCurve()
    {
        JacobianPoint point = new(PointOnCurve.X, PointOnCurve.Y, Z: 1);

        Assert.IsTrue(point.IsPointOnCurve());
    }

    [Test]
    public void JacobianPointShouldNotBeOnCurve()
    {
        JacobianPoint point = new(PointOffCurve.X, PointOffCurve.Y, 1);

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
        var point = new JacobianPoint(PointOnCurve.X, PointOnCurve.Y, 1);

        Assert.IsFalse(point.IsAtInfinity);
    }
}