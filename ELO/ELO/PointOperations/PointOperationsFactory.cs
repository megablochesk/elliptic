using ELO.ECDH;
using ELO.Points;

namespace ELO.PointOperations;

public static class PointOperationFactory
{
    public static IPointOperations<JacobianPoint> GetJacobianOperations(AlgorithmType type)
    {
        return new JacobianOperations(type);
    }

    public static IPointOperations<AffinePoint> GetAffineOperations(AlgorithmType type)
    {
        return new AffineOperations(type);
    }
}