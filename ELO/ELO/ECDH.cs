using System.Numerics;
using System.Security.Cryptography;
using ELO.PointOperations;
using ELO.Points;

namespace ELO;

public static class ECDH
{
    public static BigInteger GeneratePrivateKey()
    {
        var bytes = new byte[32];
        BigInteger privateKey;

        do
        {
            RandomNumberGenerator.Fill(bytes);
            privateKey = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
        } while (privateKey >= Curve.N);

        return privateKey;
    }

    public static AffinePoint GeneratePublicKey(BigInteger privateKey)
    {
        return Operations.MultiplyPoint(privateKey, Curve.G)
                    .ToAffine();
    }

    public static AffinePoint DeriveSharedSecret(BigInteger privateKey, AffinePoint publicKey)
    {
        return Operations.MultiplyPoint(privateKey, publicKey)
                    .ToAffine();
    }

    public static AffinePoint GeneratePublicKeyA(BigInteger privateKey)
    {
        return Operations.MultiplyAffinePointLeftToRight(privateKey, Curve.G);
    }

    public static AffinePoint DeriveSharedSecretA(BigInteger privateKey, AffinePoint publicKey)
    {
        return Operations.MultiplyAffinePointLeftToRight(privateKey, publicKey);
    }

    public static void VerifySharedSecrets(AffinePoint aliceSharedSecret, AffinePoint bobSharedSecret)
    {
        aliceSharedSecret.EnsureOnCurve();
        bobSharedSecret.EnsureOnCurve();

        Console.WriteLine("Alice's shared secret: " + aliceSharedSecret);
        Console.WriteLine("Bob's shared secret: " + bobSharedSecret);
        Console.WriteLine("Shared secrets are equal: " + AffinePoint.ArePointsEqual(aliceSharedSecret, bobSharedSecret));
    }
}