using System.Numerics;
using ELO;
using ELO.PointOperations;

namespace ELT;

public class MathUtilitiesTests
{
    [Test]
    public void FastModuloP256Test()
    {
        BigInteger input = BigInteger.Parse("2346391098346120082933372396717514455605377270242392040502406036558391451327823432224103120108044879730919178283232490692426139919419451284040618676301796");
        var expected = BigInteger.Parse("69187469364232031836548821531971153808731075654725806004116076052366752432012");
        var expectedStandard = input % Curve.P;

        var result = MathUtilities.FastModuloP256(input);

        Assert.AreEqual(expected, result);
        Assert.AreEqual(expectedStandard, result);
    }
}