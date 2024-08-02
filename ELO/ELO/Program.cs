using ELO.ECDH;

namespace ELO;

public class Program
{
    public static void Main()
    {
        var ecdh = ECDHFactory.CreateECDH(AlgorithmType.AffineLeftToRight);

        var alicePrivateKey = ecdh.GeneratePrivateKey();
        var bobPrivateKey = ecdh.GeneratePrivateKey();

        Console.WriteLine(alicePrivateKey);
        Console.WriteLine(bobPrivateKey);

        var alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
        var bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

        var aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
        var bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

        AnalysisOutputs.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);
    }
}