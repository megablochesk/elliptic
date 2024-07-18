using System.Numerics;
using ELO;
using ELO.Points;


BigInteger alicePrivateKey = ECDH.GeneratePrivateKey();
BigInteger bobPrivateKey = ECDH.GeneratePrivateKey();

Console.WriteLine(alicePrivateKey);
Console.WriteLine(bobPrivateKey);

AffinePoint alicePublicKey = ECDH.GeneratePublicKeyA(alicePrivateKey);
AffinePoint bobPublicKey = ECDH.GeneratePublicKeyA(bobPrivateKey);

AffinePoint aliceSharedSecret = ECDH.DeriveSharedSecretA(alicePrivateKey, bobPublicKey);
AffinePoint bobSharedSecret = ECDH.DeriveSharedSecretA(bobPrivateKey, alicePublicKey);

ECDH.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);