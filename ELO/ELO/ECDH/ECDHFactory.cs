using ELO.PointOperations;
using ELO.Points;

namespace ELO.ECDH;

public static class ECDHFactory
{
    public static IECDH CreateECDH(PointType pointType)
    {
        switch (pointType)
        {
            case PointType.Affine:
                return new ECDH<AffinePoint>(PointOperationFactory.GetAffineOperations());
            case PointType.Jacobian:
                return new ECDH<JacobianPoint>(PointOperationFactory.GetJacobianOperations());
            default:
                throw new ArgumentException("Unsupported point type");
        }
    }
}