using ELO.ECDH;
using ELO.Points;

namespace ELO;

public class Program
{
    public static void Main(string[] args)
    {
        var ecdh = ECDHFactory.CreateECDH(PointType.Affine);

        BigInteger alicePrivateKey = ecdh.GeneratePrivateKey();
        BigInteger bobPrivateKey = ecdh.GeneratePrivateKey();

        Console.WriteLine(alicePrivateKey);
        Console.WriteLine(bobPrivateKey);

        var alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
        var bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

        var aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
        var bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

        AnalysisOutputs.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);
    }
}