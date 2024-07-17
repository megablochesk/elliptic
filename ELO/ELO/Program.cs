using System.Numerics;
using ELO;
using ELO.PointOperations;
using ELO.Points;


var pointOperations = new MixedPointOperations();
var pointContext = new PointContext(pointOperations);
var pointMultiplier = new PointMultiplication(pointContext);
var ecdh = new ECDH(pointMultiplier);


BigInteger alicePrivateKey = 26;
BigInteger bobPrivateKey = 27;

Console.WriteLine(alicePrivateKey);
Console.WriteLine(bobPrivateKey);

Point alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
Point bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

alicePublicKey.EnsureOnCurve();
bobPublicKey.EnsureOnCurve();

AffinePoint aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
AffinePoint bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);

aliceSharedSecret.EnsureOnCurve();
bobSharedSecret.EnsureOnCurve();

ECDH.VerifySharedSecrets(aliceSharedSecret, bobSharedSecret);