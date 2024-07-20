using ELO.Points;
using ELO;
using System.Numerics;
using ELO.ECDH;

namespace ELT;

[TestFixture]
public class ECDHTests
{
    [Test]
    [TestCase(PointType.Affine)]
    [TestCase(PointType.Jacobian)]
    public void TestEDHD(PointType pointType)
    {
        BigInteger alicePrivateKey = BigInteger.Parse("93083067474008655841404248258697333634583388142843875066892035839243010313215");
        BigInteger bobPrivateKey = BigInteger.Parse("26502975831688994212300543431059499816538530198313673272593496574021759095939");


        var ecdh = ECDHFactory.CreateECDH(pointType);


        var alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
        var bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

        var aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
        var bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

        AnalysisOutputs.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);

        Assert.IsTrue(aliceSharedSecret.IsPointOnCurve());
        Assert.IsTrue(bobSharedSecret.IsPointOnCurve());

        Assert.That(AffinePoint.ArePointsEqual(aliceSharedSecret, bobSharedSecret));
    }
}