namespace ELO.Points;

public abstract class Point
{
    public abstract bool IsPointOnCurve();

    public void EnsureOnCurve()
    {
        if (!IsPointOnCurve()) throw new ArgumentException(ExceptionMessages.PointIsNotOnCurve);
    }
}
