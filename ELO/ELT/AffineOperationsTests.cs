using ELO.PointOperations;
using ELO.Points;
using System.Numerics;

namespace ELT;

[TestFixture]
public class AffineOperationsTests
{
    private static readonly AffinePoint PointOnCurve = new(
        X: BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286"),
        Y: BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109"));

    [Test]
    public void AddPoint_WithPointAtInfinity_ShouldReturnNonInfinityPoint()
    {
        // Arrange
        var pointAtInfinity = AffinePoint.AtInfinity;
        var finitePoint = PointOnCurve;

        // Act
        AffinePoint result = AffineOperations.AddPoints(pointAtInfinity, finitePoint);

        // Assert
        Assert.That(result, Is.EqualTo(finitePoint));
    }

    [Test]
    public void AddPoints_TwoPointsAtInfinity_ShouldReturnPointAtInfinity()
    {
        // Arrange
        var infinity1 = AffinePoint.AtInfinity;
        var infinity2 = AffinePoint.AtInfinity;

        // Act
        AffinePoint result = AffineOperations.AddPoints(infinity1, infinity2);

        // Assert
        Assert.That(result, Is.EqualTo(AffinePoint.AtInfinity));
    }

    [Test]
    public void AddPoints_PointAndItsNegation_ShouldReturnPointAtInfinity()
    {
        // Arrange
        var point = PointOnCurve;
        var negationOfPoint = new AffinePoint(point.X, -point.Y);

        // Act
        AffinePoint result = AffineOperations.AddPoints(point, negationOfPoint);

        // Assert
        Assert.That(result, Is.EqualTo(AffinePoint.AtInfinity));
    }



    [Test]
    public void DoublePoint_StartingFromPointOnCurve_ShouldDoublePointOnCurve()
    {
        // Arrange
        var point = PointOnCurve;

        // Act
        AffinePoint result = AffineOperations.DoublePoint(point);

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void AddPoints_AddingPointOnCurveToItsDouble_ShouldResultInTriplePoint()
    {
        // Arrange
        var point = PointOnCurve;
        var doublePoint = AffineOperations.DoublePoint(point);

        // Act
        AffinePoint result = AffineOperations.AddPoints(doublePoint, point);

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void AddPoints_SubtractingPointOnCurveFromItsDouble_ShouldReturnPointOnCurve()
    {
        // Arrange
        var point = PointOnCurve;
        var pointNegated = point.Negated;
        var doublePoint = AffineOperations.DoublePoint(point);

        // Act
        AffinePoint result = AffineOperations.AddPoints(doublePoint, pointNegated);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsPointOnCurve());
            Assert.IsTrue(AffinePoint.ArePointsEqual(result, point));
        });
    }
}