using System.Numerics;
using ELO;
using ELO.PointOperations;

namespace ELT;

public class MathUtilitiesTests
{
    public static readonly BigInteger NumberForModuloTesting =
        BigInteger.Parse("2346391098346120082933372396717514455605377270242392040502406036558391451327823432224103120108044879730919178283232490692426139919419451284040618676301796");

    public static readonly BigInteger ModuloDivisionRemainder =
        BigInteger.Parse("69187469364232031836548821531971153808731075654725806004116076052366752432012");

    [Test]
    public void FastModuloP256Test()
    {
        var input = NumberForModuloTesting;
        var expected = ModuloDivisionRemainder;
        var expectedStandard = input % Curve.P;

        var result = MathUtilities.FastModuloP256(input);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(result, Is.EqualTo(expectedStandard));
    }

    [Test]
    public void FastModuloP256WithBuilderTest()
    {
        var input = NumberForModuloTesting;
        var expected = ModuloDivisionRemainder;
        var expectedStandard = input % Curve.P;

        var result = MathUtilities.FastModuloP256WithBuilder(input);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(result, Is.EqualTo(expectedStandard));
    }

    [Test]
    public void NAFTest()
    {
        BigInteger input = 27;

        var result = MathUtilities.ComputeNAF(input);

        var expected = new List<int> { -1, 0, -1, 0, 0, 1 };

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestGenerateWidthWNAF()
    {
        BigInteger d = 75;

        var expected = new List<int> { 5, 0, 0, 0, -5 };

        var result = MathUtilities.GenerateWidthWNAF(d);

        Assert.That(result, Is.EqualTo(expected));
    }
}