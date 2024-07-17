using ELO.Points;
using ELO;
using System.Numerics;

namespace ELT;

[TestFixture]
public class ECDHTests
{
    [Test]
    public void TestEDHD()
    {
        BigInteger alicePrivateKey = BigInteger.Parse("93083067474008655841404248258697333634583388142843875066892035839243010313215");
        BigInteger bobPrivateKey = BigInteger.Parse("26502975831688994212300543431059499816538530198313673272593496574021759095939");


        AffinePoint alicePublicKey = ECDH.GeneratePublicKey(alicePrivateKey);
        AffinePoint bobPublicKey = ECDH.GeneratePublicKey(bobPrivateKey);

        AffinePoint aliceSharedSecret = ECDH.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
        AffinePoint bobSharedSecret = ECDH.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

        ECDH.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);

        Assert.IsTrue(aliceSharedSecret.IsPointOnCurve());
        Assert.IsTrue(bobSharedSecret.IsPointOnCurve());

        Assert.That(AffinePoint.ArePointsEqual(aliceSharedSecret, bobSharedSecret));
    }
}