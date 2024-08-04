using ELO.Points;

namespace ELO.ECDH;

public interface IECDH
{
    AffinePoint GeneratePublicKey(BigInteger privateKey);
    AffinePoint DeriveSharedSecret(BigInteger privateKey, AffinePoint publicKey);
}