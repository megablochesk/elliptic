using ELO.ECDH;
using ELO.PointOperations;
using ELO.Points;
using ELO;
using System.Numerics;

namespace ELT;

[TestFixture]
public class AffineOperationsTests
{
    readonly AffineOperations _affineOperations = new(AlgorithmType.AffineWindowedMethod);

    [Test]
    public void AddPoints_PointAtInfinityWithAnotherPoint_ShouldReturnOtherPoint()
    {
        // Arrange
        var p1 = AffinePoint.AtInfinity;
        var p2 = new AffinePoint(new BigInteger(13), BigInteger.Parse("53404144414778303508799263379260966483386805595332806637100379275867514529459"));

        // Act
        AffinePoint result = _affineOperations.AddPoints(p1, p2);

        // Assert
        Assert.AreEqual(p2, result);
    }

    [Test]
    public void AddTwoGPoints_ShouldReturn2G()
    {
        // Arrange
        var point = Curve.G;

        // Act
        AffinePoint result = _affineOperations.DoublePoint(point);

        Console.WriteLine($"{result.X} {result.Y}");

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void AddGTo2G_ShouldReturn3G()
    {
        // Arrange
        var g = Curve.G;
        var g2 = _affineOperations.DoublePoint(g);

        // Act
        var result = _affineOperations.AddPoints(g2, g);


        Console.WriteLine($"{result.X} {result.Y}");

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void Subtract2GAndG_ShouldReturnG()
    {
        // Arrange
        var g = Curve.G;
        var negativeG = g.Negated;
        var g2 = _affineOperations.DoublePoint(g);

        // Act
        var result = _affineOperations.AddPoints(g2, negativeG);


        Console.WriteLine($"{result.X} {result.Y}");

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
        Assert.IsTrue(AffinePoint.ArePointsEqual(result, g));
    }
}