using ELO.PointOperations;
using ELO.Points;

namespace ELO.ECDH;

public static class ECDHFactory
{
    public static IECDH CreateECDH(AlgorithmType algorithmType)
    {
        return algorithmType switch
        {
            AlgorithmType.AffineLeftToRight =>        new ECDH<AffinePoint>(PointOperationFactory.GetAffineOperations(algorithmType)),
            AlgorithmType.AffineMontgomeryLadder =>   new ECDH<AffinePoint>(PointOperationFactory.GetAffineOperations(algorithmType)),
            AlgorithmType.AffineWithNAF =>            new ECDH<AffinePoint>(PointOperationFactory.GetAffineOperations(algorithmType)),
            AlgorithmType.AffineWindowedMethod =>     new ECDH<AffinePoint>(PointOperationFactory.GetAffineOperations(algorithmType)),

            AlgorithmType.JacobianLeftToRight =>      new ECDH<JacobianPoint>(PointOperationFactory.GetJacobianOperations(algorithmType)),
            AlgorithmType.JacobianMontgomeryLadder => new ECDH<JacobianPoint>(PointOperationFactory.GetJacobianOperations(algorithmType)),
            AlgorithmType.JacobianWithNAF =>          new ECDH<JacobianPoint>(PointOperationFactory.GetJacobianOperations(algorithmType)),
            AlgorithmType.JacobianWindowedMethod =>   new ECDH<JacobianPoint>(PointOperationFactory.GetJacobianOperations(algorithmType)),

            _ => throw new ArgumentException("Unsupported point type")
        };
    }
}