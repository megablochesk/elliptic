using ELO.PointOperations;

namespace ELO.ECDH;

public static class ECDHFactory
{
    public static IECDH CreateECDH(AlgorithmType algorithmType)
    {
        return algorithmType switch
        {
            AlgorithmType.AffineLeftToRight      or 
            AlgorithmType.AffineMontgomeryLadder or
            AlgorithmType.AffineWithNAF          or
            AlgorithmType.AffineWindowedMethod =>     new ECDH(PointOperationFactory.GetAffineOperations(algorithmType)),

            AlgorithmType.JacobianLeftToRight      or 
            AlgorithmType.JacobianMontgomeryLadder or
            AlgorithmType.JacobianWithNAF          or
            AlgorithmType.JacobianWindowedMethod =>   new ECDH(PointOperationFactory.GetJacobianOperations(algorithmType)),

            _ => throw new ArgumentException(ExceptionMessages.UnsupportedAlgorithmType)
        };
    }
}