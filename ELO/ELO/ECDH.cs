using System.Numerics;
using System.Security.Cryptography;
using ELO.PointOperations;
using ELO.Points;

namespace ELO;

public class ECDH(PointMultiplication context)
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

    public Point GeneratePublicKey(BigInteger privateKey)
    {
        return context.MultiplyPoint<AffinePoint>(privateKey, Curve.G);
    }

    public AffinePoint DeriveSharedSecret(BigInteger privateKey, Point publicKey)
    {
        return context.MultiplyPoint<AffinePoint>(privateKey, publicKey);
    }

    public static void VerifySharedSecrets(AffinePoint aliceSharedSecret, AffinePoint bobSharedSecret)
    {
        Console.WriteLine("Alice's shared secret: " + aliceSharedSecret);
        Console.WriteLine("Bob's shared secret: " + bobSharedSecret);
        Console.WriteLine("Shared secrets are equal: " + ((aliceSharedSecret.X - bobSharedSecret.X ) % Curve.P == 0 
                                                          && (aliceSharedSecret.Y - bobSharedSecret.Y) % Curve.P == 0));
    }
}