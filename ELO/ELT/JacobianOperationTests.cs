using ELO;
using ELO.PointOperations;
using ELO.Points;
using System.Numerics;

namespace ELT;

[TestFixture]
public class JacobianOperationTests
{
    [Test]
    public void AddPoints_PointAtInfinityWithAnotherPoint_ShouldReturnOtherPoint()
    {
        // Arrange
        var p1 = JacobianPoint.AtInfinity;
        var p2 = new AffinePoint(new BigInteger(13), BigInteger.Parse("53404144414778303508799263379260966483386805595332806637100379275867514529459"));

        // Act
        JacobianPoint result = JacobianOperations.AddPoints(p1, p2);

        // Assert
        Assert.That(result, Is.EqualTo(p2.ToJacobian()));
    }

    [Test]
    public void AddTwoGPoints_ShouldReturn2G()
    {
        // Arrange
        var point = Curve.G.ToJacobian();

        // Act
        JacobianPoint result = JacobianOperations.DoublePoint(point);

        Console.WriteLine($"{result.X} {result.Y} {result.Z}");

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }

    [Test]
    public void AddGTo2G_ShouldReturn3G()
    {
        // Arrange
        var g = Curve.G;
        var g2 = JacobianOperations.DoublePoint(g.ToJacobian());

        // Act
        AffinePoint result = JacobianOperations.AddPoints(g2, g).ToAffine();


        Console.WriteLine($"{result.X} {result.Y}");

        // Assert
        Assert.IsTrue(result.IsPointOnCurve());
    }


}