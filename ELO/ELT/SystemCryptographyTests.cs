using System.Security.Cryptography;

namespace ELT;

[TestFixture]
public class SystemCryptographyTests
{
    [Test]
    public static void ECDiffieHellman_ShouldGenerateIdenticalSharedSecrets_WhenExchangingPublicKeys()
    {
        using var alice = new ECDiffieHellmanCng(CngKey.Create(CngAlgorithm.ECDiffieHellmanP256));
        using var bob = new ECDiffieHellmanCng(CngKey.Create(CngAlgorithm.ECDiffieHellmanP256));

        alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        alice.HashAlgorithm =  CngAlgorithm.Sha256;

        bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        bob.HashAlgorithm = CngAlgorithm.Sha256;

        var alicePublicKey = alice.PublicKey.ToByteArray();
        var bobPublicKey = bob.PublicKey.ToByteArray();

        var aliceSharedSecret = alice.DeriveKeyMaterial(CngKey.Import(bobPublicKey, CngKeyBlobFormat.EccPublicBlob));
        var bobSharedSecret = bob.DeriveKeyMaterial(CngKey.Import(alicePublicKey, CngKeyBlobFormat.EccPublicBlob));

        Assert.That(bobSharedSecret, Is.EqualTo(aliceSharedSecret));
    }
}