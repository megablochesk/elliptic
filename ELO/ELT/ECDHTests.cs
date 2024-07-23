using ELO.Points;
using ELO;
using System.Numerics;
using ELO.ECDH;

namespace ELT;

[TestFixture]
public class ECDHTests
{
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
        BigInteger alicePrivateKey = BigInteger.Parse("93083067474008655841404248258697333634583388142843875066892035839243010313215");
        BigInteger bobPrivateKey = BigInteger.Parse("26502975831688994212300543431059499816538530198313673272593496574021759095939");

        var ecdh = ECDHFactory.CreateECDH(algorithmType);


        var alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
        var bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

        Console.WriteLine($"{alicePublicKey.X} {alicePublicKey.Y}");
        Console.WriteLine($"{bobPublicKey.X} {bobPublicKey.Y}");

        var aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
        var bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

        var expectedX = BigInteger.Parse("704602078891827709968695104567010694948148510878878680802142883200395438103");
        var expectedY = BigInteger.Parse("92103446386463774353525877203731609590400293700811157530711463052594028852461");

        Assert.AreEqual(aliceSharedSecret.X, expectedX);
        Assert.AreEqual(aliceSharedSecret.Y, expectedY);

        Assert.That(AffinePoint.ArePointsEqual(aliceSharedSecret, bobSharedSecret));
    }
}