using ELO.Points;

namespace ELO;

public static class AnalysisOutputs
{
    public static void VerifySharedSecrets(AffinePoint sharedSecretA, AffinePoint sharedSecretB)
    {
        sharedSecretA.EnsureOnCurve();
        sharedSecretB.EnsureOnCurve();

        Console.WriteLine("Alice's shared secret: " + sharedSecretA);
        Console.WriteLine("Bob's shared secret: " + sharedSecretB);
        Console.WriteLine("Shared secrets are equal: " + AffinePoint.ArePointsEqual(sharedSecretA, sharedSecretB));
    }
}