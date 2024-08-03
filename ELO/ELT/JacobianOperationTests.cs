using ELO;
using ELO.PointOperations;
using ELO.Points;
using System.Numerics;

namespace ELT;

[TestFixture]
public class JacobianOperationTests
{
    private static readonly JacobianPoint PointOnCurve = new(
        X: BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286"),
        Y: BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109"),
        Z: BigInteger.One);

    [Test]
    public void DoublePoint_UsingGeneratorPoint_ShouldReturnDoubledJacobianPoint()
    {
        // Arrange
        var point = PointOnCurve;

        // Act
        JacobianPoint result = JacobianOperations.DoublePoint(point);

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void AddPoints_AddingPointOnCurveToItsDouble_ShouldYieldTriplePointInAffine()
    {
        // Arrange
        var point = PointOnCurve;
        var doubledPoint = JacobianOperations.DoublePoint(point);

        // Act
        JacobianPoint result = JacobianOperations.AddPoints(doubledPoint, point);


        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void DoublePoint_PointAtInfinity_ShouldRemainAtInfinity()
    {
        // Arrange
        var pointAtInfinity = JacobianPoint.AtInfinity;

        // Act
        JacobianPoint result = JacobianOperations.DoublePoint(pointAtInfinity);

        // Assert
        Assert.That(result, Is.EqualTo(JacobianPoint.AtInfinity));
    }

    [Test]
    public void AddPoints_PointAndItsNegation_ShouldReturnPointAtInfinity()
    {
        // Arrange
        var point = PointOnCurve;
        JacobianPoint negation = PointOnCurve.Negated;

        // Act
        JacobianPoint result = JacobianOperations.AddPoints(point, negation);

        // Assert
        Assert.That(result, Is.EqualTo(JacobianPoint.AtInfinity));
    }
}