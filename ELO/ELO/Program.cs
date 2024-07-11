using System.Numerics;
using ELO;
using ELO.Points;


BigInteger alicePrivateKey = ECDH.GeneratePrivateKey();
BigInteger bobPrivateKey = ECDH.GeneratePrivateKey();

Console.WriteLine(alicePrivateKey);
Console.WriteLine(bobPrivateKey);

AffinePoint alicePublicKey = ECDH.GeneratePublicKey(alicePrivateKey);
AffinePoint bobPublicKey = ECDH.GeneratePublicKey(bobPrivateKey);

AffinePoint aliceSharedSecret = ECDH.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
AffinePoint bobSharedSecret = ECDH.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

ECDH.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);