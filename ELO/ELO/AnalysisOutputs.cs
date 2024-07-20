using ELO.Points;

namespace ELO;

public static class AnalysisOutputs
{
    public static void VerifySharedSecrets(AffinePoint aliceSharedSecret, AffinePoint bobSharedSecret)
    {
        aliceSharedSecret.EnsureOnCurve();
        bobSharedSecret.EnsureOnCurve();

        Console.WriteLine("Alice's shared secret: " + aliceSharedSecret);
        Console.WriteLine("Bob's shared secret: " + bobSharedSecret);
        Console.WriteLine("Shared secrets are equal: " + AffinePoint.ArePointsEqual(aliceSharedSecret, bobSharedSecret));
    }
}