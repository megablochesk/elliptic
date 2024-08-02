using ELO.Points;
using System.Numerics;
using ELO.ECDH;

namespace ELT;

[TestFixture]
public class ECDHTests
{
    private static readonly BigInteger AlicePrivateKey = 
        BigInteger.Parse("93083067474008655841404248258697333634583388142843875066892035839243010313215");
    private static readonly BigInteger BobPrivateKey = 
        BigInteger.Parse("26502975831688994212300543431059499816538530198313673272593496574021759095939");

    private static readonly BigInteger ExpectedX =
        BigInteger.Parse("704602078891827709968695104567010694948148510878878680802142883200395438103");
    private static readonly BigInteger ExpectedY =
        BigInteger.Parse("92103446386463774353525877203731609590400293700811157530711463052594028852461");

    [Test]
    [TestCase(AlgorithmType.AffineLeftToRight)]
    [TestCase(AlgorithmType.AffineMontgomeryLadder)]
    [TestCase(AlgorithmType.AffineWithNAF)]
    [TestCase(AlgorithmType.AffineWindowedMethod)]
    [TestCase(AlgorithmType.JacobianLeftToRight)]
    [TestCase(AlgorithmType.JacobianMontgomeryLadder)]
    [TestCase(AlgorithmType.JacobianWithNAF)]
    [TestCase(AlgorithmType.JacobianWindowedMethod)]
    public void TestEDHD(AlgorithmType algorithmType)
    {
        var alicePrivateKey = AlicePrivateKey;
        var bobPrivateKey = BobPrivateKey;

        var ecdh = ECDHFactory.CreateECDH(algorithmType);

        var alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
        var bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

        var aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
        var bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

        var expectedX = ExpectedX;
        var expectedY = ExpectedY;

        Assert.That(expectedX, Is.EqualTo(aliceSharedSecret.X));
        Assert.That(expectedY, Is.EqualTo(aliceSharedSecret.Y));

        Assert.That(AffinePoint.ArePointsEqual(aliceSharedSecret, bobSharedSecret));
    }
}