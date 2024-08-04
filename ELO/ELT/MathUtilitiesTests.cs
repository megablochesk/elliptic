using System.Numerics;
using ELO;
using ELO.StandardMath;

namespace ELT;

public class MathUtilitiesTests
{
    public static readonly BigInteger NumberForModuloTesting =
        BigInteger.Parse(
            "2346391098346120082933372396717514455605377270242392040502406036558391451327823432224103120108044879730919178283232490692426139919419451284040618676301796");

    public static readonly BigInteger ModuloDivisionRemainder =
        BigInteger.Parse("69187469364232031836548821531971153808731075654725806004116076052366752432012");

    [Test]
    public void FastModuloP256_ShouldMatchStandardModulo_WhenUsingLargeNumber()
    {
        // Arrange
        var input = NumberForModuloTesting;
        var expected = ModuloDivisionRemainder;
        var expectedStandardModulo = input % Curve.P;

        // Act
        var result = FastModuloP256.NotOptimisedFastModulo(input);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result, Is.EqualTo(expectedStandardModulo));
        });

    }

    [Test]
    public void FastModuloP256WithBuilder_ShouldMatchStandardModulo_WhenUsingLargeNumber()
    {
        // Arrange
        var input = NumberForModuloTesting;
        var expected = ModuloDivisionRemainder;
        var expectedStandardModulo = input % Curve.P;

        // Act
        var result = FastModuloP256.FastModulo(input);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result, Is.EqualTo(expectedStandardModulo));
        });
    }

    [Test]
    public void ComputeNAF_ShouldReturnCorrectNonAdjacentForm_ForSmallInteger()
    {
        // Arrange
        BigInteger input = 27;

        var expected = new List<int> { -1, 0, -1, 0, 0, 1 };

        // Act
        var result = MathUtilities.ComputeNAF(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateWidthWNAF_ShouldReturnExpectedWidthWNAF_ForSmallInteger()
    {
        // Arrange
        BigInteger input = 75;

        var expected = new List<int> { 5, 0, 0, 0, -5 };

        // Act
        var result = MathUtilities.GenerateWidthWNAF(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}