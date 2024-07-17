﻿namespace ELO.Points;

public abstract record Point
{
    public abstract bool IsAtInfinity { get; }

    public abstract Point AtInfinity { get; }

    public abstract bool IsPointOnCurve();

    public void EnsureOnCurve()
    {
        if (!IsPointOnCurve()) throw new ArgumentException("Point is not on curve");
    }
}
