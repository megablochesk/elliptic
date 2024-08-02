using System.Security.Cryptography;
using ELO.PointOperations;
using ELO.Points;

namespace ELO.ECDH;

public class ECDH(IPointOperations operations) : IECDH
{
    public BigInteger GeneratePrivateKey()
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

    public AffinePoint GeneratePublicKey(BigInteger privateKey)
    {
        return operations.MultiplyPoint(privateKey, Curve.G);
    }

    public AffinePoint DeriveSharedSecret(BigInteger privateKey, AffinePoint publicKey)
    {
        return operations.MultiplyPoint(privateKey, publicKey);
    }
}