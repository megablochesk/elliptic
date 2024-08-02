using ELO.ECDH;

namespace ELO.PointOperations;

public static class PointOperationFactory
{
    public static IPointOperations GetJacobianOperations(AlgorithmType type) 
        => new JacobianOperations(type);

    public static IPointOperations GetAffineOperations(AlgorithmType type) 
        => new AffineOperations(type);
}