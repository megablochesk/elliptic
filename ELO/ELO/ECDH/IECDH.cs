using ELO.Points;

namespace ELO.ECDH;

public interface IECDH
{
    BigInteger GeneratePrivateKey();
    AffinePoint GeneratePublicKey(BigInteger privateKey);
    AffinePoint DeriveSharedSecret(BigInteger privateKey, AffinePoint publicKey);
}