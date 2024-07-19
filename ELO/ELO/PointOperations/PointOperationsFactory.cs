using ELO.Points;

namespace ELO.PointOperations;

public static class PointOperationFactory
{
    public static IPointOperations<JacobianPoint> GetJacobianOperations()
    {
        return new JacobianOperations();
    }

    public static IPointOperations<AffinePoint> GetAffineOperations()
    {
        return new AffineOperations();
    }
}