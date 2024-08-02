namespace ELO.Points;

public abstract record Point
{
    public abstract bool IsPointOnCurve();

    public void EnsureOnCurve()
    {
        if (!IsPointOnCurve()) throw new ArgumentException(ExceptionMessages.PointIsNotOnCurve);
    }
}
